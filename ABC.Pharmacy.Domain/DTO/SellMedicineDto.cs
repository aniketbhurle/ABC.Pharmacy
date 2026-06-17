namespace ABC.Pharmacy.Domain.DTO;

public sealed record SellMedicineDto
{
    public Guid MedicineId { get; set; }
    public int Quantity { get; set; }
}
