using ABC.Pharmacy.Domain.DTO;
using ABC.Pharmacy.Domain.Entities;

namespace ABC.Pharmacy.Application.Interfaces;

public interface IMedicineService
{
    Task<List<Medicine>> GetAllMedicinesAsync();
    Task AddMedicineAsync(MedicineDto medicine);
    Task UpdateMedicineStockAsync(Guid medicineId, int quantity);
    Task<List<Medicine>> SearchMedicineAsync(string searchTerm);
}
