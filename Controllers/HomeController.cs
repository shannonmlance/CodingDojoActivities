using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CodingDojoActivities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Activities.Controllers
{
    public class HomeController : Controller
    {
        private CodingDojoActivitiesContext dbContext;
        public HomeController(CodingDojoActivitiesContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        [HttpGet("register")]
        [HttpGet("login")]
        public ViewResult Register()
        {
            return View();
        }

        [HttpPost("register/process")]
        public IActionResult ProcessRegister(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View("Register");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    dbContext.Add(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    return RedirectToAction("Home");
                }
            }
            else
            {
                return View("Register");
            }
        }

        [HttpPost("login/process")]
        public IActionResult ProcessLogin(LoginUser lUser)
        {
            if(ModelState.IsValid)
            {
                User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == lUser.LoginEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid email/password.");
                    return View("Register");
                }
                else
                {
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(lUser, userInDb.Password, lUser.LoginPassword);
                    if(result == 0)
                    {
                        ModelState.AddModelError("LoginEmail", "Invalid email/password.");
                        return View("Register");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                        return RedirectToAction("Home");
                    }
                }
            }
            else
            {
                return View("Register");
            }
        }

        [HttpGet("home")]
        public ViewResult Home()
        {
            int? loggedin = HttpContext.Session.GetInt32("UserId");
            if(loggedin == null)
            {
                return View("Register");
            }
            List<cdActivity> ActivityList = dbContext.cdActivities.ToList();
            foreach(cdActivity a in ActivityList)
            {
                TimeSpan DateGap = a.Date - DateTime.Today;
                if(DateGap.Days < 0)
                {
                    dbContext.Remove(a);
                    dbContext.SaveChanges();
                }
            }
            List<cdActivity> AllActivities = dbContext.cdActivities.OrderBy(a => a.Date)
                .Include(a => a.Participations)
                    .ThenInclude(p => p.User)
                .Include(a => a.Creator)
                .ToList();
            User OneUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)loggedin);
            ViewBag.CurrentUserName = OneUser.FirstName;
            return View(AllActivities);
        }

        [HttpGet("new")]
        public ViewResult New()
        {
            int? loggedin = HttpContext.Session.GetInt32("UserId");
            if(loggedin == null)
            {
                return View("Register");
            }
            return View();
        }

        [HttpPost("activity/process")]
        public IActionResult ProcessActivity(cdActivity cdactivity)
        {
            if(ModelState.IsValid)
            {
                TimeSpan time = new TimeSpan(0,0,0,0);
                if(cdactivity.DurationType == "days")
                {
                    TimeSpan ts = new TimeSpan(cdactivity.Duration,0,0,0);
                    time = time.Add(ts);
                }
                if(cdactivity.DurationType == "hours")
                {
                    TimeSpan ts = new TimeSpan(0,cdactivity.Duration,0,0);
                    time = time.Add(ts);
                }
                if(cdactivity.DurationType == "minutes")
                {
                    TimeSpan ts = new TimeSpan(0,0,cdactivity.Duration,0);
                    time = time.Add(ts);
                }
                cdActivity ThisActivity = new cdActivity
                {
                    Title = cdactivity.Title,
                    Date = cdactivity.Date,
                    Time = cdactivity.Time,
                    Duration = cdactivity.Duration,
                    DurationType = cdactivity.DurationType,
                    DurationTimespan = time,
                    Description = cdactivity.Description,
                    UserId = (int)HttpContext.Session.GetInt32("UserId")
                };
                dbContext.Add(ThisActivity);
                dbContext.SaveChanges();
                return RedirectToAction("Activity", new {id = ThisActivity.cdActivityId});
            }
            else
            {
                return View("New");
            }
        }

        [HttpGet("activity/{id}")]
        public ViewResult Activity(int id)
        {
            int? loggedin = HttpContext.Session.GetInt32("UserId");
            if(loggedin == null)
            {
                return View("Register");
            }
            cdActivity ThisActivity = dbContext.cdActivities
                .Include(a => a.Participations)
                    .ThenInclude(p => p.User)
                .Include(a => a.Creator)
                .FirstOrDefault(a => a.cdActivityId == id);
            return View(ThisActivity);
        }

        [HttpGet("join/{id}")]
        public IActionResult Join(int id)
        {
            int? loggedin = HttpContext.Session.GetInt32("UserId");
            if(loggedin == null)
            {
                return View("Register");
            }
            else
            {
                cdActivity ThisActivity = dbContext.cdActivities.FirstOrDefault(a => a.cdActivityId == id);
                User ThisUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)loggedin);
                List<cdActivity> UsersActivities = dbContext.cdActivities
                    .Where(a => a.Participations
                    .Any(p => p.User.UserId == (int)loggedin))
                    .ToList();
                DateTime starttime = ThisActivity.Date.Add(ThisActivity.Time.TimeOfDay);
                DateTime endtime = starttime.Add(ThisActivity.DurationTimespan);
                int boolCounter = 0;
                foreach(cdActivity a in UsersActivities)
                {
                    if(a.DurationTimespan > ThisActivity.DurationTimespan)
                    {
                        DateTime newstarttime = a.Date.Add(a.Time.TimeOfDay);
                        DateTime newendtime = newstarttime.Add(a.DurationTimespan);
                        bool isTimeBetween = starttime >= newstarttime && starttime <= newendtime;
                        if(isTimeBetween == true)
                        {
                            boolCounter++;
                        }
                    }
                    else
                    {
                        DateTime newstarttime = a.Date.Add(a.Time.TimeOfDay);
                        DateTime newendtime = newstarttime.Add(a.DurationTimespan);
                        bool isTimeBetween = newstarttime >= starttime && newstarttime <= endtime;
                        if(isTimeBetween == true)
                        {
                            boolCounter++;
                        }
                    }
                }
                foreach(cdActivity a in UsersActivities)
                {
                    if(a.DurationTimespan < ThisActivity.DurationTimespan)
                    {
                        DateTime newstarttime = a.Date.Add(a.Time.TimeOfDay);
                        DateTime newendtime = newstarttime.Add(a.DurationTimespan);
                        bool isTimeBetween = newendtime >= starttime && newendtime <= endtime;
                        if(isTimeBetween == true)
                        {
                            boolCounter++;
                        }
                    }
                    else
                    {
                        DateTime newstarttime = a.Date.Add(a.Time.TimeOfDay);
                        DateTime newendtime = newstarttime.Add(a.DurationTimespan);
                        bool isTimeBetween = endtime >= newstarttime && endtime <= newendtime;
                        if(isTimeBetween == true)
                        {
                            boolCounter++;
                        }
                    }
                }
                if(boolCounter > 0)
                {
                    string error = "You already have an activity scheduled during this time.";
                    ViewBag.Error = error;
                    List<cdActivity> ActivityList = dbContext.cdActivities.ToList();
                    foreach(cdActivity a in ActivityList)
                    {
                        TimeSpan DateGap = a.Date - DateTime.Today;
                        if(DateGap.Days < 0)
                        {
                            dbContext.Remove(a);
                            dbContext.SaveChanges();
                        }
                    }
                    List<cdActivity> AllActivities = dbContext.cdActivities.OrderBy(a => a.Date)
                        .Include(a => a.Participations)
                            .ThenInclude(p => p.User)
                        .Include(a => a.Creator)
                        .ToList();
                    User OneUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)loggedin);
                    ViewBag.CurrentUserName = OneUser.FirstName;
                    return View("Home", AllActivities);
                }
                else
                {
                    Participation ThisParticipation = new Participation
                    {
                        UserId = ThisUser.UserId,
                        cdActivityId = ThisActivity.cdActivityId
                    };
                    dbContext.Add(ThisParticipation);
                    dbContext.SaveChanges();
                    return RedirectToAction("Home");
                }
            }
        }

        [HttpGet("unjoin/{id}")]
        public IActionResult UnJoin(int id)
        {
            int? loggedin = HttpContext.Session.GetInt32("UserId");
            if(loggedin == null)
            {
                return View("Register");
            }
            Participation ThisParticipation = dbContext.Participations
                .FirstOrDefault(p => p.cdActivityId == id && p.UserId == (int)loggedin);
            dbContext.Remove(ThisParticipation);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            int? loggedin = HttpContext.Session.GetInt32("UserId");
            if(loggedin == null)
            {
                return View("Register");
            }
            cdActivity ThisActivity = dbContext.cdActivities.FirstOrDefault(a => a.cdActivityId == id);
            dbContext.Remove(ThisActivity);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Register");
        }
    }
}
