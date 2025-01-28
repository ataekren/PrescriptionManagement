using MedicineService.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SharedKernel.Models;

namespace MedicineService.Repositories;

public class MedicineRepository : IMedicineRepository
{
    private readonly IMongoCollection<Medicine> _medicineCollection;

    public MedicineRepository(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _medicineCollection = mongoDatabase.GetCollection<Medicine>(mongoDbSettings.Value.CollectionName);
    }

    public async Task<IEnumerable<Medicine>> GetAllAsync()
    {
        return await _medicineCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Medicine?> GetByIdAsync(string id)
    {
        return await _medicineCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Medicine> CreateAsync(Medicine medicine)
    {
        await _medicineCollection.InsertOneAsync(medicine);
        return medicine;
    }

    public async Task<IEnumerable<Medicine>> CreateManyAsync(IEnumerable<Medicine> medicines)
    {
        await _medicineCollection.InsertManyAsync(medicines);
        return medicines;
    }

    public async Task<bool> UpdateAsync(Medicine medicine)
    {
        var result = await _medicineCollection.ReplaceOneAsync(x => x.Id == medicine.Id, medicine);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _medicineCollection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task DeactivateMedicinesByNamesAsync(IEnumerable<string> medicineNames)
    {
        var filter = Builders<Medicine>.Filter.In(m => m.Name, medicineNames);
        var update = Builders<Medicine>.Update.Set(m => m.IsActive, false);
        
        await _medicineCollection.UpdateManyAsync(filter, update);
    }

    public async Task ReactivateMedicinesByNamesAsync(IEnumerable<string> medicineNames)
    {
        var filter = Builders<Medicine>.Filter.In(m => m.Name, medicineNames);
        var update = Builders<Medicine>.Update.Set(m => m.IsActive, true);
        
        await _medicineCollection.UpdateManyAsync(filter, update);
    }
}