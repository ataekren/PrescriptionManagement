using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrescriptionService.Services;
using SharedKernel.Models.Prescriptions;
using SharedKernel.Models.Prescriptions.DTOs;
using System.Security.Claims;

namespace PrescriptionService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrescriptionController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;
    private readonly ILogger<PrescriptionController> _logger;

    public PrescriptionController(
        IPrescriptionService prescriptionService,
        ILogger<PrescriptionController> logger)
    {
        _prescriptionService = prescriptionService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<ActionResult<PrescriptionResponse>> CreatePrescription(
        [FromBody] CreatePrescriptionRequest request)
    {
        try
        {
            var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (doctorId == 0)
            {
                _logger.LogWarning("User ID not found in token");
                return Unauthorized(new { message = "Invalid user identification" });
            }

            var prescription = await _prescriptionService.CreatePrescriptionAsync(doctorId, request);
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating prescription");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("submit")]
    [Authorize(Roles = "Pharmacy")]
    public async Task<ActionResult<PrescriptionResponse>> SubmitPrescription(
        [FromBody] SubmitPrescriptionRequest request)
    {
        try
        {
            var pharmacyId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (pharmacyId == 0)
            {
                _logger.LogWarning("User ID not found in token");
                return Unauthorized(new { message = "Invalid user identification" });
            }

            var prescription = await _prescriptionService.SubmitPrescriptionAsync(pharmacyId, request);
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting prescription");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PrescriptionResponse>> GetPrescription(int id)
    {
        try
        {
            var prescription = await _prescriptionService.GetPrescriptionAsync(id);
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving prescription {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<PrescriptionResponse>>> GetPrescriptions(
        [FromQuery] SharedKernel.Models.Prescriptions.PrescriptionStatus? status = null)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == 0 || string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("User ID or role not found in token");
                return Unauthorized(new { message = "Invalid user identification" });
            }

            var prescriptions = await _prescriptionService.GetPrescriptionsAsync(
                role == "Doctor" ? userId : null,
                role == "Pharmacy" ? userId : null,
                status
            );
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving prescriptions");
            return BadRequest(new { message = ex.Message });
        }
    }
} 