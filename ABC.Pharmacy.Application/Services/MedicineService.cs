using ABC.Pharmacy.Application.Interfaces;
using ABC.Pharmacy.Domain.DTO;
using ABC.Pharmacy.Domain.Entities;

namespace ABC.Pharmacy.Application.Services;

public class MedicineService : IMedicineService
{
    private readonly IMedicineRepository _repository;
    public MedicineService(IMedicineRepository medicineRepository)
    {
        _repository = medicineRepository;
    }

    public async Task<List<Medicine>> GetAllMedicinesAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task AddMedicineAsync(MedicineDto newMedicine)
    {
        var medicine = new Medicine
        {
            Id = Guid.NewGuid(),
            FullName = newMedicine.FullName,
            Notes = newMedicine.Notes,
            ExpiryDate = newMedicine.ExpiryDate,
            Quantity = newMedicine.Quantity,
            Price = newMedicine.Price,
            Brand = newMedicine.Brand
        };

        await _repository.AddAsync(medicine);
    }

    public async Task<List<Medicine>> SearchMedicinesAsync(string medName)
    {
        if (string.IsNullOrWhiteSpace(medName))
            throw new ArgumentNullException(nameof(medName), "Search term cannot be null or empty.");

        var medicines = await _repository.GetAllAsync();

        return medicines
            .Where(x => x.FullName.Contains(medName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task UpdateMedicineStockAsync(Guid medicineId, int quantityToDeduct)
    {
        if (quantityToDeduct < 0)
            throw new ArgumentException("Cannot deduct negative quantity.", nameof(quantityToDeduct));

        var medicine = await _repository.GetByIdAsync(medicineId);
        if (medicine == null)
            throw new KeyNotFoundException($"Medicine {medicineId} not found.");

        if (medicine.Quantity < quantityToDeduct)
            throw new InvalidOperationException($"Insufficient stock. Available: {medicine.Quantity}, requested: {quantityToDeduct}.");

        medicine.Quantity -= quantityToDeduct;

        await _repository.UpdateAsync(medicine);
    }
}
