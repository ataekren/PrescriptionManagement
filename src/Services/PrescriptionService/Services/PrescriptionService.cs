using Microsoft.EntityFrameworkCore;
using PrescriptionService.Data;
using SharedKernel.Models.Prescriptions;
using SharedKernel.Models.Prescriptions.DTOs;

namespace PrescriptionService.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly PrescriptionDbContext _context;

    public PrescriptionService(PrescriptionDbContext context)
    {
        _context = context;
    }

    public async Task<PrescriptionResponse> CreatePrescriptionAsync(int doctorId, CreatePrescriptionRequest request)
    {
        var prescription = new Prescription
        {
            PatientTc = request.PatientTc,
            DoctorId = doctorId,
            CreatedDate = DateTime.UtcNow,
            Status = PrescriptionStatus.Created,
            Items = request.Items.Select(i => new PrescriptionItem
            {
                MedicineBarcode = i.MedicineBarcode,
                MedicineName = i.MedicineName,
                Quantity = i.Quantity,
                Usage = i.Usage,
                IsSubmitted = false
            }).ToList()
        };

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        return await GetPrescriptionAsync(prescription.Id);
    }

    public async Task<PrescriptionResponse> SubmitPrescriptionAsync(int pharmacyId, SubmitPrescriptionRequest request)
    {
        var prescription = await _context.Prescriptions
            .Include(p => p.Items)
            .Include(p => p.Submissions)
            .FirstOrDefaultAsync(p => p.Id == request.PrescriptionId);

        if (prescription == null)
            throw new KeyNotFoundException("Prescription not found");

        // Update submitted items
        foreach (var item in prescription.Items)
        {
            if (request.MedicineBarcodes.Contains(item.MedicineBarcode))
                item.IsSubmitted = true;
        }

        // Add submission record
        prescription.Submissions.Add(new PrescriptionSubmission
        {
            PharmacyId = pharmacyId,
            SubmissionDate = DateTime.UtcNow,
            SubmittedMedicineBarcodes = request.MedicineBarcodes
                .Select(barcode => new SubmittedMedicineBarcode { Barcode = barcode })
                .ToList()
        });

        // Update prescription status
        prescription.Status = prescription.Items.All(i => i.IsSubmitted) 
            ? PrescriptionStatus.Completed 
            : PrescriptionStatus.PartiallySubmitted;

        await _context.SaveChangesAsync();

        return await GetPrescriptionAsync(prescription.Id);
    }

    public async Task<PrescriptionResponse> GetPrescriptionAsync(int id)
    {
        var prescription = await _context.Prescriptions
            .Include(p => p.Items)
            .Include(p => p.Submissions)
                .ThenInclude(s => s.SubmittedMedicineBarcodes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prescription == null)
            throw new KeyNotFoundException("Prescription not found");

        return MapToResponse(prescription);
    }

    public async Task<List<PrescriptionResponse>> GetPrescriptionsAsync(int? doctorId = null, int? pharmacyId = null, PrescriptionStatus? status = null)
    {
        var query = _context.Prescriptions
            .Include(p => p.Items)
            .Include(p => p.Submissions)
                .ThenInclude(s => s.SubmittedMedicineBarcodes)
            .AsQueryable();

        if (doctorId.HasValue)
            query = query.Where(p => p.DoctorId == doctorId.Value);

        if (pharmacyId.HasValue)
            query = query.Where(p => p.Submissions.Any(s => s.PharmacyId == pharmacyId.Value));

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var prescriptions = await query.ToListAsync();
        return prescriptions.Select(MapToResponse).ToList();
    }

    public async Task<List<PrescriptionResponse>> GetPrescriptionsByPatientTcAsync(string patientTc)
    {
        var prescriptions = await _context.Prescriptions
            .Include(p => p.Items)
            .Include(p => p.Submissions)
                .ThenInclude(s => s.SubmittedMedicineBarcodes)
            .Where(p => p.PatientTc == patientTc)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();

        return prescriptions.Select(MapToResponse).ToList();
    }

    private static PrescriptionResponse MapToResponse(Prescription prescription)
    {
        return new PrescriptionResponse(
            prescription.Id,
            prescription.PatientTc,
            prescription.DoctorId,
            prescription.CreatedDate,
            prescription.Status,
            prescription.Items.Select(i => new PrescriptionItemResponse(
                i.Id,
                i.MedicineBarcode,
                i.MedicineName,
                i.Quantity,
                i.Usage,
                i.IsSubmitted
            )).ToList(),
            prescription.Submissions.Select(s => new PrescriptionSubmissionResponse(
                s.Id,
                s.PharmacyId,
                s.SubmissionDate,
                s.SubmittedMedicineBarcodes.Select(b => b.Barcode).ToList()
            )).ToList()
        );
    }
} 