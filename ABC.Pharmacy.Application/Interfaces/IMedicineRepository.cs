using ABC.Pharmacy.Domain.Entities;

namespace ABC.Pharmacy.Application.Interfaces;

public interface IMedicineRepository
{
    Task<List<Medicine>> GetAllAsync(); //GetAllMedicines
    Task AddAsync(Medicine medicine);   //AddNewMedicine
    Task UpdateAsync(Medicine medicine); //UpdateMedicine
    Task<Medicine?> GetByIdAsync(Guid medicineId); 
}
