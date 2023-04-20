using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_Application.Models
{
    public class Remark
    {
        public int Id { get; set; }
        public int WorkTaskId { get; set; }
        [Required]
        [Display(Name = "Remark's Text")]
        public string Text { get; set; }
        public string Layout { get; set; }
    }
}