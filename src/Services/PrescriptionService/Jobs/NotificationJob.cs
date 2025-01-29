using Quartz;
using PrescriptionService.Services;

namespace PrescriptionService.Jobs
{
    public class NotificationJob : IJob
    {
        private readonly INotificationService _notificationService;

        public NotificationJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _notificationService.SendIncompleteNotificationsAsync();
        }
    }
}
