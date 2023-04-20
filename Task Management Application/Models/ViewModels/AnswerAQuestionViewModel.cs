using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_Application.Models.ViewModels
{
    public class AnswerAQuestionViewModel
    {
        public int Object_Id { get; set; }
        [Required]
        public string Answer { get; set; }
        public string Layout { get; set; }

    }
}