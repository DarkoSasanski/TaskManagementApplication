using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Management_Application.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionAndAnswerId { get; set; }
        public string Text { get; set; }
        public DateTime TimeAnswered { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}