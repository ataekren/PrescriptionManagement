namespace SharedKernel.DTOs
{
    public class MedicineDTO
    {
        public string Name { get; set; } = default!;
        public string Barcode { get; set; } = default!;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
} 