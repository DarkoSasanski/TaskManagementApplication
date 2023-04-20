using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Task_Management_Application.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult NotFound(string message, string role)
        {
            ViewBag.Message = message;
            ViewBag.Role = role;
            return View();
        }
        public ActionResult Forbidden(string message, string role)
        {
            ViewBag.Message = message;
            ViewBag.Role = role;
            return View();
        }
    }
}