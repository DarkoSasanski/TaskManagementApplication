using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_Application.Models.ViewModels
{
    public class AssignATaskViewModel
    {
        public int TaskId { get; set; }
        [Required]
        [Display(Name = "Worker Username")]
        public string WorkerUsername { get; set; }
    }

}