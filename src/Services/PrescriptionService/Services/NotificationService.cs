using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharedKernel.Models.Prescriptions;

namespace PrescriptionService.Services
{
    public interface INotificationService
    {
        Task SendIncompleteNotificationsAsync();
    }

    public class NotificationService : INotificationService
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IPrescriptionService prescriptionService,
            ILogger<NotificationService> logger)
        {
            _prescriptionService = prescriptionService;
            _logger = logger;
        }

        public async Task SendIncompleteNotificationsAsync()
        {
            try
            {
                // Get incomplete prescriptions (status = 1)
                var prescriptions = await _prescriptionService.GetPrescriptionsAsync(status: PrescriptionStatus.PartiallySubmitted);

                foreach (var prescription in prescriptions)
                {
                    // Simulate sending email notification
                    _logger.LogInformation(
                        "EMAIL NOTIFICATION: Incomplete prescription found!\n" +
                        $"Prescription ID: {prescription.Id}\n" +
                        $"Patient TC: {prescription.PatientTc}\n" +
                        $"Created Date: {prescription.CreatedDate}\n" +
                        $"Number of Items: {prescription.Items.Count}"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending incomplete prescription notifications");
            }
        }
    }
}
