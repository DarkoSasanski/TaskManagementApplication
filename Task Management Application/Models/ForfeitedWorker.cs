using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Management_Application.Models
{
    public class ForfeitedWorker
    {
        public int Id { get; set; }
        public int WorkTaskId { get; set; }
        public string Username { get; set; }
        public DateTime DateFinished { get; set; }
    }
}