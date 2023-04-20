using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management_Application.Models;
using Task_Management_Application.Models.ViewModels;
using System.Data.Entity;

namespace Task_Management_Application.Controllers
{
    [Authorize(Roles ="Worker")]
    public class WorkerController : Controller
    {
        // GET: Worker
        public ApplicationDbContext db;
        public WorkerController()
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
        public ActionResult Index()
        {
            return View(db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name))).ToList());
        }
        public ActionResult ShowAvailableTasks()
        {
            return View(db.WorkTasks.Where(z => !z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.Status.Equals(TaskStatus.WaitingForApps)).ToList());
        }
        public ActionResult ShowAppliedTasks()
        {
            return View(db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.Status.Equals(TaskStatus.WaitingForApps)).ToList());
        }
        public ActionResult ShowAssignedTasks()
        {
            return View(db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.Status.Equals(TaskStatus.Assigned)).ToList());
        }
        public ActionResult ShowClosedTasks()
        {
            return View(db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && !z.Status.Equals(TaskStatus.Assigned) && !z.Status.Equals(TaskStatus.WaitingForApps)).ToList());
        }
        public ActionResult FilterClosedTasks(string criteria)
        {
            List<WorkTask> tasks = new List<WorkTask>();
            if (criteria == "All")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && !z.Status.Equals(TaskStatus.Assigned) && !z.Status.Equals(TaskStatus.WaitingForApps)).ToList();
            }
            else if (criteria == "Fulfilled")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.Status.Equals(TaskStatus.Fulfilled)).ToList();
            }
            else if (criteria == "Unfulfilled")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.Status.Equals(TaskStatus.Unfulfilled)).ToList();
            }
            else if (criteria == "Cancelled")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.Status.Equals(TaskStatus.Cancelled)).ToList();
            }
            else if (criteria == "Forfeited")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.Status.Equals(TaskStatus.Forfeited)).ToList();
            }
            return PartialView(tasks);
        }
        public ActionResult FilterTasks(string criteria)
        {
            List<WorkTask> tasks = new List<WorkTask>();
            if(criteria == "All")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.Status.Equals(TaskStatus.Assigned)).ToList();
            }
            else if(criteria == "Individual")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.ApplicationUsers.Count == 1 && z.Status.Equals(TaskStatus.Assigned)).ToList(); ;
            }
            else if(criteria == "Group")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.ApplicationUsers.Count > 1 && z.Status.Equals(TaskStatus.Assigned)).ToList();
            }
            else if(criteria == "NoTimeup")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.Deadline>=DateTime.Now && z.Status.Equals(TaskStatus.Assigned)).ToList();
            }
            else if (criteria == "Timeup")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.Deadline < DateTime.Now && z.Status.Equals(TaskStatus.Assigned)).ToList();
            }
            else if (criteria == "Fulfilled")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.FinishedWorkers.Any(u => u.Username.Equals(User.Identity.Name)) && z.Status.Equals(TaskStatus.Assigned)).ToList();
            }
            else if (criteria == "Forfeited")
            {
                tasks = db.WorkTasks.Where(z => z.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)) && z.ForfeitedWorkers.Any(u => u.Username.Equals(User.Identity.Name)) && z.Status.Equals(TaskStatus.Assigned)).ToList();
            }
            return PartialView(tasks);
        }

        public ActionResult Apply(int id)
        {
            var task = db.WorkTasks.Find(id);
            var username = User.Identity.Name;
            var user = db.Users.Where(z => z.UserName == username).FirstOrDefault(); ;
            if (!task.Status.Equals(TaskStatus.WaitingForApps))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            if (task.Deadline < DateTime.Now)
            {
                TempData["shortMessage"] = "Invalid action, your time's up";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }

            if (user == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The user does not exist", role = "worker" });
            }
            if (task.ApplicationUsers.Any(p => p.UserName == user.UserName))
            {
                TempData["shortMessage"] = "You've already applied";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = task.Id });
            }
            TempData["shortMessage"] = "You've successfully applied";
            TempData["class"] = "text-success";
            task.ApplicationUsers.Add(user);
            db.SaveChanges();
            return RedirectToAction("ShowTask", new { id = task.Id });
        }
        public ActionResult WithdrewApplication(int id)
        {
            var task = db.WorkTasks.Find(id);
            var username = User.Identity.Name;
            var user = db.Users.Where(z => z.UserName == username).FirstOrDefault(); ;
            if (!task.Status.Equals(TaskStatus.WaitingForApps))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            if (task.Deadline < DateTime.Now)
            {
                TempData["shortMessage"] = "Invalid action, your time's up";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }

            if (user == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The user does not exist", role = "worker" });
            }
            if (!task.ApplicationUsers.Any(p => p.UserName == user.UserName))
            {
                TempData["shortMessage"] = "You've not applied yet";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = task.Id });
            }
            TempData["shortMessage"] = "You've successfully withdrew your application";
            TempData["class"] = "text-success";
            user.Tasks.Remove(task);
            task.ApplicationUsers.Remove(user);
            db.SaveChanges();
            return RedirectToAction("ShowTask", new { id = task.Id });
        }
        public ActionResult ShowTask(int id)
        {
            WorkTask workTask = db.WorkTasks.Find(id);
            if (workTask == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "worker" });
            }
            if (TempData["shortMessage"] != null)
            {
                ViewBag.Message = TempData["shortMessage"].ToString();
                ViewBag.Class = TempData["class"].ToString();
            }
            if (TempData["logMessage"] != null)
            {
                ViewBag.LogMessage = TempData["logMessage"].ToString();
            }
            if(!workTask.Status.Equals(TaskStatus.WaitingForApps) && !workTask.ApplicationUsers.Any(u => u.UserName.Equals(User.Identity.Name)))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "worker" });
            }
            return View(workTask);
        }
        public ActionResult Finish(int id)
        {
            var task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "worker" });

            }
            if (!task.ApplicationUsers.Any(z => z.UserName == User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "worker" });
            }
            if (!task.Status.Equals(TaskStatus.Assigned))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            if (task.Deadline < DateTime.Now)
            {
                TempData["shortMessage"] = "Invalid action, your time's up";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            if (task.ForfeitedWorkers.Any(z => z.Username == User.Identity.Name))
            {
                TempData["shortMessage"] = "You've already forfeited this task";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            if (task.FinishedWorkers.Any(z => z.Username == User.Identity.Name))
            {
                TempData["shortMessage"] = "You've already fullfilfed this task";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            task.Finished++;
            FinishedWorker worker = new FinishedWorker() { Username = User.Identity.Name, DateFinished = DateTime.Now };
            db.FinishedWorkers.Add(worker);
            task.FinishedWorkers.Add(worker);
            if (task.Finished == task.ApplicationUsers.Count)
            {
                task.Status = TaskStatus.Fulfilled;
            }
            TaskLog log = CreateClosingLog("I've marked the task as fulfilled at " + DateTime.Now, "Fulfill log");
            if(log == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The user does not exist", role = "worker" });
            }
            db.TaskLogs.Add(log);
            task.TaskLogs.Add(log);
            db.SaveChanges();
            TempData["shortMessage"] = "You've successfully marked the task as fulfilled";
            TempData["class"] = "text-success";
            return RedirectToAction("ShowTask", new { id = id });
        }
        public ActionResult Forfeit(int id)
        {
            var task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "worker" });

            }
            if (!task.ApplicationUsers.Any(z => z.UserName == User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "worker" });
            }
            if (!task.Status.Equals(TaskStatus.Assigned))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            if (task.Deadline < DateTime.Now)
            {
                TempData["shortMessage"] = "Invalid action, your time's up";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            if (task.FinishedWorkers.Any(z => z.Username == User.Identity.Name))
            {
                TempData["shortMessage"] = "You've already fullfilfed this task";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            if (task.ForfeitedWorkers.Any(z => z.Username == User.Identity.Name))
            {
                TempData["shortMessage"] = "You've already forfeited this task";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            task.Forfeited++;
            ForfeitedWorker worker = new ForfeitedWorker() { Username = User.Identity.Name, DateFinished = DateTime.Now };
            db.ForfeitedWorkers.Add(worker);
            task.ForfeitedWorkers.Add(worker);
            if (task.Forfeited == task.ApplicationUsers.Count)
            {
                task.Status = TaskStatus.Forfeited;
            }
            TaskLog log = CreateClosingLog("I've forfeited the task at " + DateTime.Now, "Forfeit log");
            if (log == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The user does not exist", role = "worker" });
            }
            db.TaskLogs.Add(log);
            task.TaskLogs.Add(log);
            db.SaveChanges();
            TempData["shortMessage"] = "You've successfully forfeited the task";
            TempData["class"] = "text-success";
            return RedirectToAction("ShowTask", new { id = id });
        }
        public TaskLog CreateClosingLog(string LogText, string LogTitle)
        {
            TaskLog log = new TaskLog();
            log.Text = LogText;
            log.Title = LogTitle;
            var a_username = User.Identity.Name;
            var user = db.Users.Where(z => z.UserName.Equals(a_username)).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            log.FirstName = user.FirstName;
            log.LastName = user.LastName;
            log.Username = user.UserName;
            log.Time_Created = DateTime.Now;
            return log;
        }
        public ActionResult AddALog(int id)
        {
            var task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "worker" });

            }
            if (!task.Status.Equals(TaskStatus.Assigned) || !(DateTime.Now < task.Deadline))
            {
                TempData["logMessage"] = "Invalid action";
                return RedirectToAction("ShowTask", new { id = task.Id });
            }
            if (!task.ApplicationUsers.Any(z => z.UserName == User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "worker" });
            }
            TaskLog model = new TaskLog();
            model.WorkTaskId = id;
            return View(model);
        }
        [HttpPost]
        public ActionResult AddALog(TaskLog model)
        {
            if (ModelState.IsValid)
            {
                var task = db.WorkTasks.Find(model.WorkTaskId);
                if (task == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "worker" });

                }
                var a_username = User.Identity.Name;
                var user = db.Users.Where(z => z.UserName.Equals(a_username)).FirstOrDefault();
                if (user == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The user does not exist", role = "worker" });
                }
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Username = user.UserName;
                model.Time_Created = DateTime.Now;
                db.TaskLogs.Add(model);
                task.TaskLogs.Add(model);
                db.SaveChanges();
                return RedirectToAction("ShowTask", new { id = task.Id });
            }
            return View(model);
        }
        public ActionResult ShowMyLogs(int id)
        {
            WorkTask task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "worker" });
            }
            if (task.Status.Equals(TaskStatus.Unassigned) || task.Status.Equals(TaskStatus.WaitingForApps))
            {
                TempData["logMessage"] = "Invalid action";
                return RedirectToAction("ShowTask", new { id = task.Id });
            }
            if (!task.ApplicationUsers.Any(z => z.UserName == User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "worker" });
            }
            List<TaskLog> taskLogs = new List<TaskLog>();
            foreach(var log in task.TaskLogs)
            {
                if(log.Username.Equals(User.Identity.Name))
                taskLogs.Add(log);
            }
            return PartialView(taskLogs);
        }
    }
}