using SharedKernel.Models;

namespace MedicineService.Repositories;

public interface IMedicineRepository
{
    Task<IEnumerable<Medicine>> GetAllAsync();
    Task<Medicine?> GetByIdAsync(string id);
    Task<Medicine> CreateAsync(Medicine medicine);
    Task<bool> UpdateAsync(Medicine medicine);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<Medicine>> CreateManyAsync(IEnumerable<Medicine> medicines);
    Task DeactivateMedicinesByNamesAsync(IEnumerable<string> medicineNames);
    Task ReactivateMedicinesByNamesAsync(IEnumerable<string> medicineNames);
}