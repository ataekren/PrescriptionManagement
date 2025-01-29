using Microsoft.AspNetCore.Mvc;
using MedicineService.Services;
using MedicineService.Repositories;
using SharedKernel.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Shared.Models;

namespace MedicineService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MedicineController : ControllerBase
{
    private readonly ExcelService _excelService;
    private readonly IMedicineRepository _medicineRepository;
    private readonly ILogger<MedicineController> _logger;
    private readonly IDistributedCache _cache;

    public MedicineController(
        ExcelService excelService,
        IMedicineRepository medicineRepository,
        ILogger<MedicineController> logger,
        IDistributedCache cache)
    {
        _excelService = excelService;
        _medicineRepository = medicineRepository;
        _logger = logger;
        _cache = cache;
    }

    [HttpGet("sync")]
    public async Task<ActionResult<SyncResult>> SyncMedicines()
    {
        try
        {
            var excelMedicines = await _excelService.GetLatestMedicineDataAsync();
            var existingMedicines = await _medicineRepository.GetAllAsync();
            
            // Filter out medicines that already exist
            var newMedicines = excelMedicines.Where(excelMed => 
                !existingMedicines.Any(existingMed => 
                    existingMed.Name.Equals(excelMed.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            // Identify medicines to deactivate (exist in DB but not in Excel)
            var medicineNamesToDeactivate = existingMedicines
                .Where(existingMed => 
                    existingMed.IsActive && // Only consider currently active medicines
                    !excelMedicines.Any(excelMed => 
                        excelMed.Name.Equals(existingMed.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(m => m.Name)
                .ToList();

            // Identify medicines to reactivate (exist in both Excel and DB, but are inactive in DB)
            var medicineNamesToReactivate = existingMedicines
                .Where(existingMed => 
                    !existingMed.IsActive && // Only consider currently inactive medicines
                    excelMedicines.Any(excelMed => 
                        excelMed.Name.Equals(existingMed.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(m => m.Name)
                .ToList();

            // Add new medicines
            if (newMedicines.Any())
            {
                await _medicineRepository.CreateManyAsync(newMedicines);
            }

            // Deactivate medicines not in Excel
            if (medicineNamesToDeactivate.Any())
            {
                await _medicineRepository.DeactivateMedicinesByNamesAsync(medicineNamesToDeactivate);
            }

            // Reactivate medicines that exist in Excel
            if (medicineNamesToReactivate.Any())
            {
                await _medicineRepository.ReactivateMedicinesByNamesAsync(medicineNamesToReactivate);
            }

            return Ok(new SyncResult
            {
                NewMedicinesCount = newMedicines.Count,
                DeactivatedMedicinesCount = medicineNamesToDeactivate.Count,
                ReactivatedMedicinesCount = medicineNamesToReactivate.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing medicines");
            return StatusCode(500, "An error occurred while syncing medicines");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Medicine>>> GetAllMedicines()
    {
        try
        {
            var medicines = await _medicineRepository.GetAllAsync();
            return Ok(medicines);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting medicines");
            return StatusCode(500, "An error occurred while getting medicines");
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<PagedResponse<Medicine>>> GetActiveMedicines([FromQuery] PaginationRequest pagination)
    {
        try
        {
            var cacheKey = $"active_medicines_page_{pagination.PageNumber}_{pagination.PageSize}";
            var cachedMedicines = await _cache.GetStringAsync(cacheKey);

            if (cachedMedicines != null)
            {
                var cachedResponse = JsonSerializer.Deserialize<PagedResponse<Medicine>>(cachedMedicines);
                return Ok(cachedResponse);
            }

            var medicines = await _medicineRepository.GetAllAsync();
            var activeMedicines = medicines.Where(m => m.IsActive).ToList();
        
            var totalCount = activeMedicines.Count;
            var items = activeMedicines
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            var response = new PagedResponse<Medicine>(items, pagination.PageNumber, pagination.PageSize, totalCount);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active medicines");
            return StatusCode(500, "An error occurred while getting active medicines");
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchMedicines(string name)  
    {  
        var medicines = await _medicineRepository.GetAllAsync(); 
        var filteredMedicines = medicines.Where(m => m.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();  
        return Ok(filteredMedicines);  
    }

    public class SyncResult
    {
        public int NewMedicinesCount { get; set; }
        public int DeactivatedMedicinesCount { get; set; }
        public int ReactivatedMedicinesCount { get; set; }
    }
} 