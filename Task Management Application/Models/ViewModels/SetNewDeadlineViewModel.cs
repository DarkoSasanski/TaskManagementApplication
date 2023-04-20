using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_Application.Models.ViewModels
{
    public class SetNewDeadlineViewModel
    {
        public int TaskId { get; set; }
        [Required]
        [Range(typeof(DateTime), "1/1/2020", "31/12/9999", ErrorMessage = "The deadline must be at least until 1/1/2020")]
        public DateTime Deadline { get; set; }
    }
}