using Microsoft.EntityFrameworkCore;
using SharedKernel.Models.Prescriptions;

namespace PrescriptionService.Data;

public class PrescriptionDbContext : DbContext
{
    public PrescriptionDbContext(DbContextOptions<PrescriptionDbContext> options) : base(options)
    {
    }

    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
    public DbSet<PrescriptionSubmission> PrescriptionSubmissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PatientTc).HasMaxLength(11).IsRequired();
            
            entity.HasMany(p => p.Items)
                .WithOne()
                .HasForeignKey("PrescriptionId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Submissions)
                .WithOne()
                .HasForeignKey("PrescriptionId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PrescriptionItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MedicineBarcode).HasMaxLength(13).IsRequired();
            entity.Property(e => e.MedicineName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Usage).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<PrescriptionSubmission>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasMany(e => e.SubmittedMedicineBarcodes)
                .WithOne()
                .HasForeignKey("PrescriptionSubmissionId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubmittedMedicineBarcode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Barcode).HasMaxLength(13).IsRequired();
        });
    }
} 