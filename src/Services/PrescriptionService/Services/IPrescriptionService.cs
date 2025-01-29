using SharedKernel.Models.Prescriptions;
using SharedKernel.Models.Prescriptions.DTOs;

namespace PrescriptionService.Services;

public interface IPrescriptionService
{
    Task<PrescriptionResponse> CreatePrescriptionAsync(int doctorId, CreatePrescriptionRequest request);
    Task<PrescriptionResponse> SubmitPrescriptionAsync(int pharmacyId, SubmitPrescriptionRequest request);
    Task<PrescriptionResponse> GetPrescriptionAsync(int id);
    Task<List<PrescriptionResponse>> GetPrescriptionsAsync(int? doctorId = null, int? pharmacyId = null, PrescriptionStatus? status = null);
    Task<List<PrescriptionResponse>> GetPrescriptionsByPatientTcAsync(string patientTc);
} 