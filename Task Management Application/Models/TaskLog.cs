using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_Application.Models
{
    public class TaskLog
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public DateTime Time_Created { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [Display(Name ="Log's text")]
        public string Text { get; set; }
        public int WorkTaskId { get; set; }
    }
}