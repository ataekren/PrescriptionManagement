namespace SharedKernel.Models.Prescriptions.DTOs;

public record CreatePrescriptionRequest(
    string PatientTc,
    List<CreatePrescriptionItemRequest> Items
);

public record CreatePrescriptionItemRequest(
    string MedicineBarcode,
    string MedicineName,
    int Quantity,
    string Usage
);

public record SubmitPrescriptionRequest(
    int PrescriptionId,
    List<string> MedicineBarcodes
);

public record PrescriptionResponse(
    int Id,
    string PatientTc,
    int DoctorId,
    DateTime CreatedDate,
    PrescriptionStatus Status,
    List<PrescriptionItemResponse> Items,
    List<PrescriptionSubmissionResponse> Submissions
);

public record PrescriptionItemResponse(
    int Id,
    string MedicineBarcode,
    string MedicineName,
    int Quantity,
    string Usage,
    bool IsSubmitted
);

public record PrescriptionSubmissionResponse(
    int Id,
    int PharmacyId,
    DateTime SubmissionDate,
    List<string> SubmittedMedicineBarcodes
); 