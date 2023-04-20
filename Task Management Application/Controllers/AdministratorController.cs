using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management_Application.Models;
using Task_Management_Application.Models.ViewModels;

namespace Task_Management_Application.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class AdministratorController : Controller
    {
        // GET: Administrator
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }
        public ApplicationDbContext db;
        public AdministratorController()
        {
            db = new ApplicationDbContext();
        }
        protected override void Dispose(bool dispossing)
        {
            base.Dispose(dispossing);
            if (dispossing)
            {
                db.Dispose();
            }
        }
        public ActionResult ShowTasks()
        {
            return View(db.WorkTasks.ToList());
        }
        public ActionResult FilterTasks(string criteria)
        {
            List<WorkTask> tasks = new List<WorkTask>();
            if (criteria == "All")
            {
                tasks = db.WorkTasks.ToList();
            }
            else if(criteria == "Unassigned")
            {
                tasks = db.WorkTasks.Where(z => z.Status.Equals(TaskStatus.Unassigned)).ToList();
            }
            else if (criteria == "Assigned")
            {
                tasks = db.WorkTasks.Where(z => z.Status.Equals(TaskStatus.Assigned)).ToList();
            }
            else if (criteria == "WaitingForApps")
            {
                tasks = db.WorkTasks.Where(z => z.Status.Equals(TaskStatus.WaitingForApps)).ToList();
            }
            else if (criteria == "Cancelled")
            {
                tasks = db.WorkTasks.Where(z => z.Status.Equals(TaskStatus.Cancelled)).ToList();
            }
            else if (criteria == "Forfeited")
            {
                tasks = db.WorkTasks.Where(z => z.Status.Equals(TaskStatus.Forfeited)).ToList();
            }
            else if (criteria == "Unfulfilled")
            {
                tasks = db.WorkTasks.Where(z => z.Status.Equals(TaskStatus.Unfulfilled)).ToList();
            }
            else if (criteria == "Fulfilled")
            {
                tasks = db.WorkTasks.Where(z => z.Status.Equals(TaskStatus.Fulfilled)).ToList();
            }
            else if (criteria == "Individual")
            {
                tasks = db.WorkTasks.Where(z => (z.ApplicationUsers.Count == 1) && !(z.Status.Equals(TaskStatus.Unassigned) || z.Status.Equals(TaskStatus.WaitingForApps))).ToList();
            }
            else if (criteria == "Group")
            {
                tasks = db.WorkTasks.Where(z => (z.ApplicationUsers.Count > 1) && !(z.Status.Equals(TaskStatus.Unassigned) || z.Status.Equals(TaskStatus.WaitingForApps))).ToList();
            }
            return PartialView(tasks);
        }
        public ActionResult ShowTask(int id)
        {
            WorkTask workTask = db.WorkTasks.Find(id);
            if (workTask == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "administrator" });
            }
            if (TempData["shortMessage"] != null)
            {
                ViewBag.Message = TempData["shortMessage"].ToString();
                ViewBag.Class = TempData["class"].ToString();
            }
            return View(workTask);
        }

        public ActionResult Cancel(int id)
        {
            var task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "administrator" });
            }
            if (task.Status.Equals(TaskStatus.WaitingForApps) || task.Status.Equals(TaskStatus.Unassigned))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            if (task.Status.Equals(TaskStatus.Cancelled))
            {
                TempData["shortMessage"] = "Invalid action, the task is already cancelled";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            task.Status = TaskStatus.Cancelled;
            db.SaveChanges();
            TempData["shortMessage"] = "You've successfully cancelled the task";
            TempData["class"] = "text-success";
            return RedirectToAction("ShowTask", new { id = task.Id });
        }
        public ActionResult ShowWorkers()
        {
            List<ApplicationUser> users = db.Users.ToList();
            return View(users.FindAll(z =>
            {
                if(z.Roles.Count==0)
                {
                    return false;
                }
                var role = GetRole(z);
                return role.Equals("Worker");
            }));
        }
        public ActionResult ShowSupervisors()
        {
            List<ApplicationUser> users = db.Users.ToList();
            return View(users.FindAll(z =>
            {
                if (z.Roles.Count == 0)
                {
                    return false;
                }
                var role = GetRole(z);
                return role.Equals("Supervisor");
            }));
        }
        private static string GetRole(ApplicationUser user)
        {
            var con = new AdministratorController();
            var roleid = user.Roles.ElementAt(0).RoleId;
            return con.db.Roles.Find(roleid).Name;
        }
        public ActionResult DeleteTask(int id)
        {
            WorkTask workTask = db.WorkTasks.Find(id);
            if (workTask == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "administrator" });
            }
            db.WorkTasks.Remove(workTask);
            db.SaveChanges();
            return RedirectToAction("ShowTasks");
        }
        public ActionResult DeleteUser(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The user does not exist", role = "administrator" });
            }
            if(user.Roles.Count!=0 && GetRole(user).Equals("Administrator"))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't delete administrators", role = "administrator" });
            }
            if (user.Roles.Count != 0 && GetRole(user).Equals("Worker"))
            {
                if (user.Tasks.Count != 0)
                {
                    TempData["shortMessage"] = "Invalid action, there are still tasks associted tasks with this user";
                    return RedirectToAction("ShowWorker", "Users", new { id = id, layout= "~/Views/Shared/_AdministratorLayout.cshtml" });
                }
            }
            if (user.Roles.Count != 0 && GetRole(user).Equals("Supervisor"))
            {
                if(db.WorkTasks.Where(z => z.SupervisorUserName.Equals(user.UserName)).ToList().Count!=0)
                {
                    TempData["shortMessage"] = "Invalid action, there are still tasks associted tasks with this user";
                    return RedirectToAction("ShowSupervisor", "Users", new { id = id });
                }
            }
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}