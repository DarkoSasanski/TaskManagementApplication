using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Task_Management_Application.Models;
using Task_Management_Application.Models.ViewModels;

namespace Task_Management_Application.Controllers
{
    [Authorize(Roles ="Administrator, Supervisor, Worker")]
    public class WorkTasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WorkTasks
        /*
        public ActionResult Index()
        {
            return View(db.WorkTasks.ToList());
        }

        // GET: WorkTasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkTask workTask = db.WorkTasks.Find(id);
            if (workTask == null)
            {
                return HttpNotFound();
            }
            return View(workTask);
        }

        // GET: WorkTasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WorkTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Status,Title,Description,Add_Remarks,Deadline,CreatorUserName")] WorkTask workTask)
        {
            if (ModelState.IsValid)
            {
                db.WorkTasks.Add(workTask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(workTask);
        }

        // GET: WorkTasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkTask workTask = db.WorkTasks.Find(id);
            if (workTask == null)
            {
                return HttpNotFound();
            }
            return View(workTask);
        }

        // POST: WorkTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Status,Title,Description,Add_Remarks,Deadline,CreatorUserName")] WorkTask workTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(workTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(workTask);
        }

        // GET: WorkTasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkTask workTask = db.WorkTasks.Find(id);
            if (workTask == null)
            {
                return HttpNotFound();
            }
            return View(workTask);
        }

        // POST: WorkTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WorkTask workTask = db.WorkTasks.Find(id);
            db.WorkTasks.Remove(workTask);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        */

        private static string GetRole(ApplicationUser user)
        {
            var con = new WorkTasksController();
            var roleid = user.Roles.ElementAt(0).RoleId;
            return con.db.Roles.Find(roleid).Name;
        }
        public ActionResult AnswerAQuestion(int id, string layout)
        {
            var mod = db.QuestionAndAnswers.Find(id);
            if(mod == null)
            {
                var username = User.Identity.Name;
                var user = db.Users.Where(z => z.UserName == username).FirstOrDefault();
                var role = GetRole(user);
                return RedirectToAction("NotFound", "Error", new { message = "An error has occured, the question does not exist", role = role });

            }
            AnswerAQuestionViewModel model = new AnswerAQuestionViewModel();
            model.Object_Id = id;
            model.Layout = layout;
            return View(model);
        }
        [HttpPost]
        public ActionResult AnswerAQuestion(AnswerAQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mod = db.QuestionAndAnswers.Find(model.Object_Id);
                var a_username = User.Identity.Name;
                var user = db.Users.Where(z => z.UserName.Equals(a_username)).FirstOrDefault();
                var role = GetRole(user);
                if (mod == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "An error has occured, the question does not exist", role = role });

                }
                Answer answer = new Answer();
                answer.FirstName = user.FirstName;
                answer.LastName = user.LastName;
                answer.TimeAnswered = DateTime.Now;
                answer.Text = model.Answer;
                mod.Answers.Add(answer);
                db.SaveChanges();
                return RedirectToAction("ShowTask", role, new { id = mod.WorkTaskId });

            }
            return View(model);
        }
        public ActionResult AskAQuestion(int id, string layout)
        {
            AskAQuestionViewModel model = new AskAQuestionViewModel();
            model.Task_Id = id;
            model.Layout = layout;
            return View(model);
        }
        [HttpPost]
        public ActionResult AskAQuestion(AskAQuestionViewModel model)
        {
            if(ModelState.IsValid)
            {
                var a_username = User.Identity.Name;
                var user = db.Users.Where(z => z.UserName.Equals(a_username)).FirstOrDefault();
                var role = GetRole(user);
                var task = db.WorkTasks.Find(model.Task_Id);
                if (task == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = role });
                }
                QuestionAndAnswer mod = new QuestionAndAnswer();
                mod.Question = model.Question;
                mod.FirstName = user.FirstName;
                mod.LastName = user.LastName;
                mod.Asking_Time = DateTime.Now;
                db.QuestionAndAnswers.Add(mod);
                task.QuestionAndAnswers.Add(mod);
                db.SaveChanges();
                return RedirectToAction("ShowTask", role, new { id = task.Id });

            }
            return View(model);

        }
        [Authorize(Roles ="Administrator, Supervisor")]
        public ActionResult AddRemark(int id, string layout)
        {
            if(User.IsInRole("Supervisor"))
            {
                var task = db.WorkTasks.Find(id);
                if (task == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = "supervisor" });
                }
                if(!task.SupervisorUserName.Equals(User.Identity.Name))
                {
                    return RedirectToAction("Forbidden", "Error", new { message = "Forbidden action", role = "supervisor" });
                }
            }
            Remark model = new Remark();
            model.WorkTaskId = id;
            model.Layout = layout;
            return View(model);
        }
        [HttpPost]
        public ActionResult AddRemark(Remark model)
        {
            if(ModelState.IsValid)
            {
                var a_username = User.Identity.Name;
                var user = db.Users.Where(z => z.UserName.Equals(a_username)).FirstOrDefault();
                var role = GetRole(user);
                var task = db.WorkTasks.Find(model.WorkTaskId);
                if (task == null)
                {
                    return RedirectToAction("NotFound", "Error", new { message = "The task couldn't be found", role = role });
                }
                db.Remarks.Add(model);
                task.Add_Remarks.Add(model);
                db.SaveChanges();
                return RedirectToAction("ShowTask", role, new { id = task.Id });
            }
            return View(model);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
