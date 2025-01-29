using Quartz;
using MedicineService.Controllers;

namespace MedicineService.Jobs
{
    public class MedicineSyncJob : IJob
    {
        private readonly MedicineController _medicineController;

        public MedicineSyncJob(MedicineController medicineController)
        {
            _medicineController = medicineController;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _medicineController.SyncMedicines();
        }
    }
}
