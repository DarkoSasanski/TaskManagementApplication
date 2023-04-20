using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Management_Application.Models
{
    public static class TaskStatus
    {
        public const string Unassigned = "Unassigned";
        public const string WaitingForApps = "Waiting For Applications";
        public const string Assigned = "Assigned";
        public const string Fulfilled = "Fulfilled";
        public const string Unfulfilled = "Unfulfilled";
        public const string Forfeited = "Forfeited";
        public const string Cancelled = "Cancelled";
    }
}