using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Task_Management_Application.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public virtual List<WorkTask> Tasks { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            
        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<WorkTask> WorkTasks { get; set; }
        public DbSet<TaskLog> TaskLogs { get; set; }
        public DbSet<QuestionAndAnswer> QuestionAndAnswers { get; set; }
        public DbSet<FinishedWorker> FinishedWorkers { get; set; }
        public DbSet<ForfeitedWorker> ForfeitedWorkers { get; set; }
        public DbSet<Remark> Remarks { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)   
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

    }
}