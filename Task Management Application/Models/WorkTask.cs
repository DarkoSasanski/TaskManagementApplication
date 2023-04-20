using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_Application.Models
{
    public class WorkTask
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Additional Remarks")]
        public virtual List<Remark> Add_Remarks { get; set; }
        [Required]
        [Range(typeof(DateTime), "1/1/2020", "31/12/9999", ErrorMessage="The deadline must be at least until 1/1/2020")]
        public DateTime Deadline { get; set; }
        [Display(Name ="Supervisor")]
        public string SupervisorUserName { get; set; }
        public int Finished { get; set; }
        public int Forfeited { get; set; }
        public virtual List<FinishedWorker> FinishedWorkers { get; set; }
        public virtual List<ForfeitedWorker> ForfeitedWorkers { get; set; }
        public virtual List<QuestionAndAnswer> QuestionAndAnswers { get; set; }
        public virtual List<TaskLog> TaskLogs { get; set; }
        public virtual List<ApplicationUser> ApplicationUsers { get; set; }
        public WorkTask()
        {
            ApplicationUsers = new List<ApplicationUser>();
            QuestionAndAnswers = new List<QuestionAndAnswer>();
            TaskLogs = new List<TaskLog>();
            Add_Remarks = new List<Remark>();
            Finished = 0;
            Forfeited = 0;
            FinishedWorkers = new List<FinishedWorker>();
            ForfeitedWorkers = new List<ForfeitedWorker>();
        }
    }
}