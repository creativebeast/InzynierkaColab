using Inzynierka.DAL;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Inzynierka.Models.ViewModels;

namespace Inzynierka.Models
{
    public class Worker
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public string WorkerId { get; set; }
        public string WorkerName { get; set; }
        public int Privilages { get; set; }
        public int CompanyID { get; set; }
        public DateTime? ModDate { get; set; }
        public DateTime? CreationDate { get; set; }

        public static List<Worker>? GetWorkersByCompanyID(ProjectContext context, int companyId)
        {
            List<Worker>? companyWorkers = context.Workers.Where(w => w.CompanyID == companyId)?.ToList();
            return companyWorkers;
        }

        public static List<WorkerUsername>? GetWorkersUsernames(ProjectContext context, List<Worker> workers)
        {
            List<WorkerUsername> expandedWorkers = new List<WorkerUsername>();
            foreach(Worker worker in workers)
            {
                string? workerName = context.Users.FirstOrDefault(u => u.ID == worker.UserID)?.Username;
                if (workerName != null)
                    expandedWorkers.Add(new WorkerUsername() { Worker = worker, Username = workerName } );
            }
            return expandedWorkers;
        }

        public static bool UpdateCompanyWorker(ProjectContext context, Worker workerChanges, int companyId, out bool dataChanged)
        {
            dataChanged = false;

            Worker? targetWorker = context.Workers.FirstOrDefault(w => w.ID == workerChanges.ID && w.CompanyID == companyId);
            if (targetWorker == null) 
                return false; //No worker to update found

            if (!String.IsNullOrEmpty(workerChanges.WorkerId) && workerChanges.WorkerId != targetWorker.WorkerId)
            {
                targetWorker.WorkerId = workerChanges.WorkerId;
                dataChanged = true;
            }

            if(workerChanges.Privilages > 0 && workerChanges.Privilages < 3 && workerChanges.Privilages != targetWorker.Privilages)
            {
                targetWorker.Privilages = workerChanges.Privilages;
                dataChanged = true;
            }

            if(dataChanged == true)
            {
                context.Update(targetWorker);
                if(context.SaveChanges() != 1) 
                    return false; //Couldn't update target worker
            } 
            else
                return true; //No changes Made

            return true; //Made changes with success
        }

        public static bool DeleteWorkerFromCompany(ProjectContext context, int companyId, int workerId)
        {
            Worker? workerToDelete = context.Workers.FirstOrDefault(w => w.UserID == workerId && w.CompanyID == companyId);
            if (workerToDelete == null)
                return false;

            context.Remove(workerToDelete);
            if (context.SaveChanges() != 1)
                return false;

            return true;
        }
    }
}
