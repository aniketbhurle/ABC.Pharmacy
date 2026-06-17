using System.ComponentModel.DataAnnotations;

namespace ABC.Pharmacy.Domain.Entities;

public class Medicine
{
    public Guid Id { get; set; }
    [Required]
    public required string FullName { get; set; }
    [Required]
    public required string Notes { get; set; }
    [Required]
    public DateTime ExpiryDate { get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public required string Brand { get; set; }
}
