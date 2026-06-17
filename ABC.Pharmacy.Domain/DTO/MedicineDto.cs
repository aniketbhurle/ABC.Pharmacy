namespace ABC.Pharmacy.Domain.DTO;

public sealed record MedicineDto
{
    public required string FullName { get; set; }
    public required string Notes { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public required string Brand { get; set; }
}
