using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management_Application.Models;

namespace Task_Management_Application.Controllers
{
    [Authorize(Roles = "Administrator, Supervisor, Worker")]
    public class UsersController : Controller
    {
        // GET: Users
        public ApplicationDbContext db;
        public UsersController()
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
        private static string GetRole(ApplicationUser user)
        {
            var con = new UsersController();
            var roleid = user.Roles.ElementAt(0).RoleId;
            return con.db.Roles.Find(roleid).Name;
        }
        public ActionResult ShowWorker(string id, string layout)
        {
            var a_username = User.Identity.Name;
            var a_user = db.Users.Where(z => z.UserName.Equals(a_username)).FirstOrDefault();
            var role = GetRole(a_user);
            ApplicationUser worker = db.Users.Find(id);
            if (worker == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The worker does not exist", role = role });
            }
            if(worker.Roles.Count==0)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The worker does not exist", role = role });
            }
            var workerrole = GetRole(worker);
            if (workerrole != "Worker")
            {
                return RedirectToAction("NotFound", "Error", new { message = "The worker does not exist", role = role });
            }//samo za workers statistika da e dostapna
            if(User.IsInRole("Worker"))
            {
                if(!a_username.Equals(worker.UserName))
                {
                    return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access other worker's record", role = role });
                }
            }//sekoj worker da moze svojot record samo
            if (TempData["shortMessage"] != null)
            {
                ViewBag.Message = TempData["shortMessage"].ToString();
            }
            ViewBag.Layout = layout;
            ViewBag.Role = role;
            return View(worker);
        }
        [Authorize(Roles = "Administrator, Supervisor")]
        public ActionResult FilterTasks(string criteria, string id, string role)
        {
            ViewBag.Role = role;
            List<WorkTask> tasks = new List<WorkTask>();
            ApplicationUser worker = db.Users.Find(id);
            if (worker == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The worker does not exist", role = role });
            }
            if (criteria == "All")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u=>u.Id.Equals(id))).ToList();
            }
            else if (criteria == "Assigned")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.Id.Equals(id)) && z.Status.Equals(TaskStatus.Assigned)).ToList();
            }
            else if (criteria == "WaitingForApps")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.Id.Equals(id)) && z.Status.Equals(TaskStatus.WaitingForApps)).ToList();
            }
            else if (criteria == "Cancelled")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.Id.Equals(id)) && z.Status.Equals(TaskStatus.Cancelled)).ToList();
            }
            else if (criteria == "Forfeited")
            {
                tasks = db.WorkTasks.Where(z => ((z.Status.Equals(TaskStatus.Assigned) || z.Status.Equals(TaskStatus.Fulfilled)) && z.ForfeitedWorkers.Any(w => w.Username.Equals(worker.UserName))) || (z.Status.Equals(TaskStatus.Forfeited) && z.ApplicationUsers.Any(u=>u.Id.Equals(id)))).ToList();
            }
            else if (criteria == "Unfulfilled")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.Id.Equals(id)) && z.Status.Equals(TaskStatus.Unfulfilled)).ToList();
            }
            else if (criteria == "Fulfilled")
            {
                tasks = db.WorkTasks.Where(z => (z.Status.Equals(TaskStatus.Assigned) && z.FinishedWorkers.Any(w => w.Username.Equals(worker.UserName))) || (z.Status.Equals(TaskStatus.Fulfilled) && z.ApplicationUsers.Any(u => u.Id.Equals(id)) && !z.ForfeitedWorkers.Any(u => u.Username.Equals(worker.UserName)))).ToList();
            }
            else if (criteria == "Individual")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.Id.Equals(id)) && z.ApplicationUsers.Count == 1).ToList();
            }
            else if (criteria == "Group")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.Id.Equals(id)) && z.ApplicationUsers.Count > 1).ToList();
            }
            return PartialView(tasks);
        }
        [Authorize(Roles ="Administrator")]
        public ActionResult ShowSupervisor(string id)
        {
            ApplicationUser supervisor = db.Users.Find(id);
            if (supervisor == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The supervisor does not exist", role = "Administrator" });
            }
            if (supervisor.Roles.Count == 0)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The supervisor does not exist", role = "Administrator" });
            }
            var supervisorrole = GetRole(supervisor);
            if (supervisorrole != "Supervisor")
            {
                return RedirectToAction("NotFound", "Error", new { message = "The supervisor does not exist", role = "Administrator" });
            }
            if (TempData["shortMessage"] != null)
            {
                ViewBag.Message = TempData["shortMessage"].ToString();
            }
            ViewBag.Layout = "~/Views/Shared/_AdministratorLayout.cshtml";
            ViewBag.Role = "Administrator";
            return View(supervisor);
        }
        [Authorize(Roles ="Administrator")]
        public ActionResult FilterSuperTasks(string criteria, string superusername, string role)
        {
            ViewBag.Role = role;
            List<WorkTask> tasks = new List<WorkTask>();
            if(criteria == "All")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(superusername)).ToList();
            }
            else if (criteria == "Unassigned")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(superusername) && z.Status.Equals(TaskStatus.Unassigned)).ToList();
            }
            else if (criteria == "Fulfilled")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(superusername) && z.Status.Equals(TaskStatus.Fulfilled)).ToList();
            }
            else if (criteria == "Assigned")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(superusername) && z.Status.Equals(TaskStatus.Assigned)).ToList();
            }
            else if (criteria == "WaitingForApps")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(superusername) && z.Status.Equals(TaskStatus.WaitingForApps)).ToList();
            }
            else if (criteria == "Cancelled")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(superusername) && z.Status.Equals(TaskStatus.Cancelled)).ToList();
            }
            else if (criteria == "Forfeited")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(superusername) && z.Status.Equals(TaskStatus.Forfeited)).ToList();
            }
            else if (criteria == "Unfulfilled")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(superusername) && z.Status.Equals(TaskStatus.Unfulfilled)).ToList();
            }
            return PartialView(tasks);
        }


    }
}