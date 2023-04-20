using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_Application.Models.ViewModels
{
    public class AssignAGroupTaskViewModel
    {
        public int TaskId { get; set; }
        [Required]
        [Display(Name = "Worker 1 Username")]
        public string Worker1Username { get; set; }
        [Required]
        [Display(Name = "Worker 2 Username")]
        public string Worker2Username { get; set; }
        [Required]
        [Display(Name = "Worker 3 Username")]
        public string Worker3Username { get; set; }
    }
}