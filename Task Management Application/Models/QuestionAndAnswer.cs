using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_Application.Models
{
    public class QuestionAndAnswer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Asking_Time { get; set; }
        public string Question { get; set; }
        public int WorkTaskId { get; set; }
        public virtual List<Answer> Answers { get; set; }
        public QuestionAndAnswer()
        {
            Answers = new List<Answer>();
        }

    }
}