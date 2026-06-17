using ABC.Pharmacy.Application.Interfaces;
using ABC.Pharmacy.Domain.Entities;
using System.Text.Json;
namespace ABC.Pharmacy.Infrastructure.Repositories;

public class MedicineRepository : IMedicineRepository
{
    private readonly string _filePath;
    public MedicineRepository()
    {
        _filePath = "Data/medicine.json";

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public async Task<List<Medicine>> GetAllAsync()
    {
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);

            return JsonSerializer.Deserialize<List<Medicine>>(json) ?? new List<Medicine>();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to read medicine data", ex);
        }
    }

    public async Task AddAsync(Medicine medicine)
    {
        var medicines = await GetAllAsync();

        medicines.Add(medicine);

        await SaveAsync(medicines);
    }

    public async Task<Medicine?> GetByIdAsync(Guid id)
    {
        var medicines = await GetAllAsync();
        return medicines.FirstOrDefault(x => x.Id == id);
    }

    public async Task UpdateAsync(Medicine medicine)
    {
        try
        {
            var medicines = await GetAllAsync();
            var index = medicines.FindIndex(x => x.Id == medicine.Id);

            if (index == -1)
                throw new KeyNotFoundException($"Medicine {medicine.Id} not found");

            medicines[index] = medicine;
            await SaveAsync(medicines);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to update new data", ex);
        }
    }

    private async Task SaveAsync(List<Medicine> medicines)
    {
        try
        {
            var json = JsonSerializer.Serialize(medicines, new JsonSerializerOptions { WriteIndented = true });

            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to save new data to the file", ex);
        }
    }

}
