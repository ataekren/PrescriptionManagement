using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedKernel.Models.Prescriptions;

public class Prescription
{
    public int Id { get; set; }
    public string PatientTc { get; set; } = null!;
    public int DoctorId { get; set; }
    public DateTime CreatedDate { get; set; }
    public PrescriptionStatus Status { get; set; }
    public List<PrescriptionItem> Items { get; set; } = new();
    public List<PrescriptionSubmission> Submissions { get; set; } = new();
}

public class PrescriptionItem
{
    public int Id { get; set; }
    public string MedicineBarcode { get; set; } = null!;
    public string MedicineName { get; set; } = null!;
    public int Quantity { get; set; }
    public string Usage { get; set; } = null!;
    public bool IsSubmitted { get; set; }
}

public class PrescriptionSubmission
{
    public int Id { get; set; }
    public int PharmacyId { get; set; }
    public DateTime SubmissionDate { get; set; }
    public List<SubmittedMedicineBarcode> SubmittedMedicineBarcodes { get; set; } = new();
}

public class SubmittedMedicineBarcode
{
    public int Id { get; set; }
    public string Barcode { get; set; } = null!;
}

public enum PrescriptionStatus
{
    Created = 0,
    PartiallySubmitted = 1,
    Completed = 2
} 