using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_Application.Models.ViewModels
{
    public class AskAQuestionViewModel
    {
        public int Task_Id { get; set; }
        public string Layout { get; set; }
        [Required]
        public string Question { get; set; }
    }
}