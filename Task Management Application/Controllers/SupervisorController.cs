using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management_Application.Models;
using Task_Management_Application.Models.ViewModels;

namespace Task_Management_Application.Controllers
{
    [Authorize(Roles ="Supervisor")]
    public class SupervisorController : Controller
    {
        // GET: Supervisor
        public ApplicationDbContext db;
        public SupervisorController()
        {
            db = new ApplicationDbContext();
        }
        protected override void Dispose(bool dispossing)
        {
            base.Dispose(dispossing);
            if(dispossing)
            {
                db.Dispose();
            }
        }
        public ActionResult Index()
        {
            return View(db.WorkTasks.Where(z=>z.SupervisorUserName.Equals(User.Identity.Name)).ToList());
        }
        public ActionResult CreateNewTask()
        {
            WorkTask Model = new WorkTask();
            Model.SupervisorUserName = User.Identity.Name;
            Model.Status = TaskStatus.Unassigned;
            ViewBag.Types = new List<string>() { "Unassigned", "Waiting For Applications", "Individual", "Group" };
            return View(Model);
        }
        [HttpPost]
        public ActionResult CreateNewTask(WorkTask workTask)
        {
            if (ModelState.IsValid)
            {
                if(workTask.Deadline<DateTime.Now)
                {
                    ViewBag.Message = "Invalid deadline, the deadline must be at least until " + DateTime.Now;
                    ViewBag.Types = new List<string>() { "Unassigned", "Waiting For Applications", "Individual", "Group" };
                    return View(workTask);
                }
                db.WorkTasks.Add(workTask);
                db.SaveChanges();
                if (workTask.Type.Equals("Unassigned"))
                {
                    TempData["shortMessage"] = "The task was successfully created";
                    TempData["class"] = "text-success";
                    return RedirectToAction("ShowTask", new { id = workTask.Id });
                }
                else if (workTask.Type.Equals("Waiting For Applications"))
                {
                    TempData["shortMessage"] = "The task is now accepting applications";
                    TempData["class"] = "text-success";
                    workTask.Status = TaskStatus.WaitingForApps;
                    db.SaveChanges();
                    return RedirectToAction("ShowTask", new { id = workTask.Id });
                }
                else if(workTask.Type.Equals("Individual"))
                {
                    return RedirectToAction("AssignATask", new { id = workTask.Id });
                }
                else
                {
                    return RedirectToAction("AssignAGroupTask", new { id = workTask.Id });
                }
            }
            ViewBag.Types = new List<string>() { "Unassigned", "Waiting For Applications", "Individual", "Group" };
            return View(workTask);
        }
        public ActionResult AssignForApplications(int id)
        {
            var task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            if (!task.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "supervisor" });
            }
            if (!task.Status.Equals(TaskStatus.Unassigned))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            task.Status = TaskStatus.WaitingForApps;
            db.SaveChanges();
            TempData["shortMessage"] = "The task is now accepting applications";
            TempData["class"] = "text-success";
            return RedirectToAction("ShowTask", new { id = task.Id });

        }
        public ActionResult AssignATask(int id)
        {
            var task = db.WorkTasks.Find(id);
            if(task == null )
            {
                return RedirectToAction("NotFound", "Error", new {message="The task couldn't be found", role="supervisor"});
            }
            if (!task.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't assign this task", role = "supervisor" });
            }
            if (!(task.Status.Equals(TaskStatus.Unassigned) || task.Status.Equals(TaskStatus.WaitingForApps)))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            AssignATaskViewModel model = new AssignATaskViewModel();
            model.TaskId = id;
            return View(model);
        }
        [HttpPost]
        public ActionResult AssignATask(AssignATaskViewModel model)
        {
            if(ModelState.IsValid)
            {
                var task = db.WorkTasks.Find(model.TaskId);
                var user = db.Users.Where(z => z.UserName == model.WorkerUsername).FirstOrDefault(); ;
                if (task == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
                }
                if (user == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The worker does not exist", role = "supervisor" });
                }
                if(user.Roles.Count==0 || !GetRole(user).Equals("Worker"))
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The worker does not exist", role = "supervisor" });
                }
                List<ApplicationUser> users = task.ApplicationUsers;
                task.ApplicationUsers = new List<ApplicationUser>();
                foreach (var u in users)
                {
                    u.Tasks.Remove(task);
                }
                db.SaveChanges();
                task.ApplicationUsers.Add(user);
                task.Status = TaskStatus.Assigned;
                db.SaveChanges();
                TempData["shortMessage"] = "You've successfully assigned the task";
                TempData["class"] = "text-success";
                return RedirectToAction("ShowTask", new { id = task.Id });
            }
            return View(model);
        }
        public ActionResult AssignToUser(int id, string username)
        {
            var task = db.WorkTasks.Find(id);
            var user = db.Users.Where(z => z.UserName == username).FirstOrDefault(); ;
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            if (!task.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't assign this task", role = "supervisor" });
            }
            if (!task.Status.Equals(TaskStatus.WaitingForApps))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            if (user == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The worker does not exist", role = "supervisor" });
            }
            if (user.Roles.Count == 0 || !GetRole(user).Equals("Worker"))
            {
                return RedirectToAction("NotFound", "Error", new { message = "The worker does not exist", role = "supervisor" });
            }
            List<ApplicationUser> users = task.ApplicationUsers;
            task.ApplicationUsers = new List<ApplicationUser>();
            foreach(var u in users)
            {
                u.Tasks.Remove(task);
            }
            db.SaveChanges();
            task.ApplicationUsers.Add(user);
            task.Status = TaskStatus.Assigned;
            db.SaveChanges();
            TempData["shortMessage"] = "You've successfully assigned the task";
            TempData["class"] = "text-success";
            return RedirectToAction("SetNewDeadline", new { id = task.Id });
        }
        public ActionResult SetNewDeadline(int id)
        {
            SetNewDeadlineViewModel model = new SetNewDeadlineViewModel();
            var task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            if (!task.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "supervisor" });
            }
            if (!(task.Status.Equals(TaskStatus.Assigned) || task.Status.Equals(TaskStatus.Unassigned) || task.Status.Equals(TaskStatus.WaitingForApps)))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            model.TaskId = id;
            model.Deadline = task.Deadline;
            return View(model);
        }
        [HttpPost]
        public ActionResult SetNewDeadline(SetNewDeadlineViewModel model)
        {
            if(ModelState.IsValid)
            {
                if (model.Deadline < DateTime.Now)
                {
                    ViewBag.Message = "Invalid deadline, the deadline must be at least until " + DateTime.Now;
                    return View(model);
                }
                var task = db.WorkTasks.Find(model.TaskId);
                if (task == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
                }
                task.Deadline = model.Deadline;
                db.SaveChanges();
                if(TempData["shortMessage"]!=null)
                {
                    TempData["shortMessage"] = TempData["shortMessage"] + " and set a new deadline";

                }
                else
                {
                    TempData["shortMessage"] = "You've successfully set a new deadline";

                }
                TempData["class"] = "text-success";
                return RedirectToAction("ShowTask", new { id = task.Id });
            }
            return View(model);
        }
        public ActionResult AssignAGroupTask(int id)
        {
            var task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            if (!task.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't assign this task", role = "supervisor" });
            }
            if (!(task.Status.Equals(TaskStatus.Unassigned) || task.Status.Equals(TaskStatus.WaitingForApps)))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            AssignAGroupTaskViewModel model = new AssignAGroupTaskViewModel();
            model.TaskId = id;
            return View(model);
        }
        [HttpPost]
        public ActionResult AssignAGroupTask(AssignAGroupTaskViewModel model)
        {
            if(ModelState.IsValid)
            {
                var task = db.WorkTasks.Find(model.TaskId);
                var user1 = db.Users.Where(z => z.UserName == model.Worker1Username).FirstOrDefault();
                var user2 = db.Users.Where(z => z.UserName == model.Worker2Username).FirstOrDefault();
                var user3 = db.Users.Where(z => z.UserName == model.Worker3Username).FirstOrDefault();
                if (task == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
                }

                if (user1== null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "Worker 1 does not exist", role = "supervisor" });
                }

                if (user2 == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "Worker 2 does not exist", role = "supervisor" });
                }
                if (user3 == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "Worker 3 does not exist", role = "supervisor" });
                }
                if (user1.Roles.Count == 0 || !GetRole(user1).Equals("Worker"))
                {
                    return RedirectToAction("NotFound", "Error", new { message = "Worker 1 does not exist", role = "supervisor" });
                }
                if (user2.Roles.Count == 0 || !GetRole(user2).Equals("Worker"))
                {
                    return RedirectToAction("NotFound", "Error", new { message = "Worker 2 does not exist", role = "supervisor" });
                }
                if (user3.Roles.Count == 0 || !GetRole(user3).Equals("Worker"))
                {
                    return RedirectToAction("NotFound", "Error", new { message = "Worker 3 does not exist", role = "supervisor" });
                }
                List<ApplicationUser> users = task.ApplicationUsers;
                task.ApplicationUsers = new List<ApplicationUser>();
                foreach (var u in users)
                {
                    u.Tasks.Remove(task);
                }
                db.SaveChanges();
                task.ApplicationUsers.Add(user1);
                task.ApplicationUsers.Add(user2);
                task.ApplicationUsers.Add(user3);
                task.Status = TaskStatus.Assigned;
                db.SaveChanges();
                TempData["shortMessage"] = "You've successfully assigned the task";
                TempData["class"] = "text-success";
                return RedirectToAction("ShowTask", new { id = task.Id });
            }
            return View(model);
        }
        public ActionResult AddAWorker(int id)
        {
            var task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            if (!task.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't assign this task", role = "supervisor" });
            }
            if (!(task.Status.Equals(TaskStatus.Assigned) && task.Deadline > DateTime.Now))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            AssignATaskViewModel model = new AssignATaskViewModel();
            model.TaskId = id;
            return View(model);
        }
        [HttpPost]
        public ActionResult AddAWorker(AssignATaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                var task = db.WorkTasks.Find(model.TaskId);
                var user = db.Users.Where(z => z.UserName == model.WorkerUsername).FirstOrDefault(); ;
                if (task == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
                }
                if (user == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The worker does not exist", role = "supervisor" });
                }
                if (user.Roles.Count == 0 || !GetRole(user).Equals("Worker"))
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The worker does not exist", role = "supervisor" });
                }
                task.ApplicationUsers.Add(user);
                Remark rem = new Remark() { Text = "Additional worker was added at " + DateTime.Now };
                db.Remarks.Add(rem);
                task.Add_Remarks.Add(rem);
                db.SaveChanges();
                TempData["shortMessage"] = "You've successfully added a worker to the task";
                TempData["class"] = "text-success";
                return RedirectToAction("ShowTask", new { id = task.Id });
            }
            return View(model);
        }
        public ActionResult ShowTask(int id)
        {
            WorkTask workTask = db.WorkTasks.Find(id);
            if (workTask == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            if (TempData["shortMessage"] != null)
            {
                ViewBag.Message = TempData["shortMessage"].ToString();
                ViewBag.Class = TempData["class"].ToString();
            }
            if (!workTask.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "supervisor" });
            }
            return View(workTask);
        }
        public ActionResult Unfulfill(int id)
        {
            var task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            if (!task.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "supervisor" });
            }
            if (!(task.Status.Equals(TaskStatus.Assigned) && task.Deadline < DateTime.Now) && !task.Status.Equals(TaskStatus.Fulfilled))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            task.Status = TaskStatus.Unfulfilled;
            db.SaveChanges();
            TempData["shortMessage"] = "You've successfully marked the task as unfulfilled";
            TempData["class"] = "text-success";
            return RedirectToAction("ShowTask", new { id = task.Id });
        }
        public ActionResult Fulfill(int id)
        {
            var task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            if (!task.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "supervisor" });
            }
            if (!(task.Status.Equals(TaskStatus.Assigned) && task.Deadline < DateTime.Now) && !task.Status.Equals(TaskStatus.Unfulfilled))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            task.Status = TaskStatus.Fulfilled;
            db.SaveChanges();
            TempData["shortMessage"] = "You've successfully marked the task as fulfilled";
            TempData["class"] = "text-success";
            return RedirectToAction("ShowTask", new { id = task.Id });
        }
        public ActionResult RecreateTask(int id)
        {
            WorkTask task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            if (!task.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "supervisor" });
            }
            if (!(task.Status.Equals(TaskStatus.Unfulfilled) || task.Status.Equals(TaskStatus.Forfeited) || task.Status.Equals(TaskStatus.Cancelled)))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            Remark rem = new Remark();
            rem.Text = "The task was recreated at " + DateTime.Now;
            db.Remarks.Add(rem);
            task.Add_Remarks.Add(rem);
            db.SaveChanges();
            ViewBag.Types = new List<string>() { "Unassigned", "Waiting For Applications", "Individual", "Group" };
            WorkTask newTask = new WorkTask() { Title = task.Title, Description = task.Description, Deadline = task.Deadline, Type = task.Type, SupervisorUserName = task.SupervisorUserName, Status = TaskStatus.Unassigned };
            return View(newTask);
        }
        [HttpPost]
        public ActionResult RecreateTask(WorkTask workTask)
        {
            if (ModelState.IsValid)
            {
                if (workTask.Deadline < DateTime.Now)
                {
                    ViewBag.Message = "Invalid deadline, the deadline must be at least until " + DateTime.Now;
                    ViewBag.Types = new List<string>() { "Unassigned", "Waiting For Applications", "Individual", "Group" };
                    return View(workTask);
                }
                db.WorkTasks.Add(workTask);
                db.SaveChanges();
                if (workTask.Type.Equals("Unassigned"))
                {
                    TempData["shortMessage"] = "The task was successfully created";
                    TempData["class"] = "text-success";
                    return RedirectToAction("ShowTask", new { id = workTask.Id });
                }
                else if (workTask.Type.Equals("Waiting For Applications"))
                {
                    TempData["shortMessage"] = "The task is now accepting applications";
                    TempData["class"] = "text-success";
                    workTask.Status = TaskStatus.WaitingForApps;
                    db.SaveChanges();
                    return RedirectToAction("ShowTask", new { id = workTask.Id });
                }
                else if (workTask.Type.Equals("Individual"))
                {
                    return RedirectToAction("AssignATask", new { id = workTask.Id });
                }
                else
                {
                    return RedirectToAction("AssignAGroupTask", new { id = workTask.Id });
                }
            }
            ViewBag.Types = new List<string>() { "Unassigned", "Waiting For Applications", "Individual", "Group" };
            return View(workTask);
        }
        public ActionResult ShowUnassignedTasks()
        {
            List<WorkTask> tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.Unassigned)).ToList();
            return View(tasks);
        }
        public ActionResult ShowAvailableTasks()
        {
            List<WorkTask> tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.WaitingForApps)).ToList();
            return View(tasks);
        }
        public ActionResult ShowAssignedTasks()
        {
            List<WorkTask> tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.Assigned)).ToList();
            return View(tasks);
        }
        public ActionResult FilterAssignedTasks(string criteria)
        {
            List<WorkTask> tasks = new List<WorkTask>();
            if (criteria == "All")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.Assigned)).ToList();
            }
            else if (criteria == "Individual")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.Assigned) && z.ApplicationUsers.Count == 1).ToList();
            }
            else if (criteria == "Group")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.Assigned) && z.ApplicationUsers.Count > 1).ToList();
            }
            else if (criteria == "NoTimeup")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.Assigned) && z.Deadline >= DateTime.Now).ToList();
            }
            else if (criteria == "Timeup")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.Assigned) && z.Deadline < DateTime.Now).ToList();
            }
            return PartialView(tasks);
        }
        public ActionResult ShowClosedTasks()
        {
            List<WorkTask> tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && (z.Status.Equals(TaskStatus.Fulfilled) || z.Status.Equals(TaskStatus.Unfulfilled) || z.Status.Equals(TaskStatus.Cancelled) || z.Status.Equals(TaskStatus.Forfeited))).ToList();
            return View(tasks);
        }
        public ActionResult FilterClosedTasks(string criteria)
        {
            List<WorkTask> tasks = new List<WorkTask>();
            if (criteria == "All")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && (z.Status.Equals(TaskStatus.Fulfilled) || z.Status.Equals(TaskStatus.Unfulfilled) || z.Status.Equals(TaskStatus.Cancelled) || z.Status.Equals(TaskStatus.Forfeited))).ToList();
            }
            else if (criteria == "Fulfilled")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.Fulfilled)).ToList();
            }
            else if (criteria == "Unfulfilled")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.Unfulfilled)).ToList();
            }
            else if (criteria == "Cancelled")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.Cancelled)).ToList();
            }
            else if (criteria == "Forfeited")
            {
                tasks = db.WorkTasks.Where(z => z.SupervisorUserName.Equals(User.Identity.Name) && z.Status.Equals(TaskStatus.Forfeited)).ToList();
            }
            return PartialView(tasks);
        }
        public ActionResult ShowWorkers()
        {
            List<ApplicationUser> users = db.Users.ToList();
            return View(users.FindAll(z =>
            {
                if (z.Roles.Count == 0)
                {
                    return false;
                }
                var role = GetRole(z);
                return role.Equals("Worker");
            }));
        }
        private static string GetRole(ApplicationUser user)
        {
            var con = new SupervisorController();
            var roleid = user.Roles.ElementAt(0).RoleId;
            return con.db.Roles.Find(roleid).Name;
        }
        public ActionResult ShowWorkerLogs(int id, string username)
        {
            WorkTask task = db.WorkTasks.Find(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            if (!task.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "supervisor" });
            }
            if (task.Status.Equals(TaskStatus.Unassigned) || task.Status.Equals(TaskStatus.WaitingForApps))
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            List<TaskLog> taskLogs = new List<TaskLog>();
            if(username == "all")
            {
                return PartialView(task.TaskLogs);
            }
            foreach (var log in task.TaskLogs)
            {
                if (log.Username.Equals(username))
                    taskLogs.Add(log);
            }
            return PartialView(taskLogs);
        }
        public ActionResult EditTask(int id)
        {
            WorkTask workTask = db.WorkTasks.Find(id);
            if (workTask == null)
            {
                return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
            }
            if (!workTask.SupervisorUserName.Equals(User.Identity.Name))
            {
                return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action, you can't access this task", role = "supervisor" });
            }
            if (!(workTask.Status.Equals(TaskStatus.Assigned) || workTask.Status.Equals(TaskStatus.Unassigned) || workTask.Status.Equals(TaskStatus.WaitingForApps)))
            {
                TempData["shortMessage"] = "Invalid action";
                TempData["class"] = "text-danger";
                return RedirectToAction("ShowTask", new { id = id });
            }
            return View(workTask);
        }
        [HttpPost]
        public ActionResult EditTask(WorkTask workTask)
        {
            if (ModelState.IsValid)
            {
                if (workTask.Deadline < DateTime.Now)
                {
                    ViewBag.Message = "Invalid deadline, the deadline must be at least until " + DateTime.Now;
                    return View(workTask);
                }
                db.Entry(workTask).State = EntityState.Modified;
                Remark rem = new Remark();
                rem.Text = "The task was edited at " + DateTime.Now;
                db.Remarks.Add(rem);
                workTask.Add_Remarks.Add(rem);
                db.SaveChanges();
                TempData["shortMessage"] = "You've successfully edited the task";
                TempData["class"] = "text-success";
                return RedirectToAction("ShowTask", new { id = workTask.Id });
            }
            return View(workTask);

        }

    }
}