using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FSD_Helpderly.Models;
using FSD_Helpderly.DAL;
using Google.Type;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Firestore;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Routing;

namespace FSD_Helpderly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private FirestoreDAL fDal = new FirestoreDAL();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return RedirectToAction("About", "Home");
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult ElderlyGetOTP()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ElderlyGetOTP(ElderlyGetOTPViewModel login)
        {
            string email = login.EmailAddress;

            fDal.GenerateElderlyOTP(email);

            return RedirectToAction("ElderlyCheckOTP", "Home", new { email = email });
        }

        public IActionResult ElderlyCheckOTP(string email)
        {
            LoginViewModel loginView = new LoginViewModel {EmailAddress = email, Password = "" };
            return View(loginView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        async public Task<ActionResult> ElderlyCheckOTP(LoginViewModel login)
        {
            string otp = login.Password;
            string email = login.EmailAddress;

            string elderlyOTP = await fDal.GetElderlyOTP(email);
            if (otp == elderlyOTP)
            {
                //Store user role "Elderly" as a string in session with the key "Role"
                HttpContext.Session.SetString("Role", "Elderly");

                //Store user email string in session with the key "Email"
                HttpContext.Session.SetString("Email", email);

                return RedirectToAction("Form", "Elderly");
            }
            else
            {
                ModelState.AddModelError("CustomError", "Incorrect OTP!");
                return View(login);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        async public Task<ActionResult> Login(LoginViewModel login)
        {
            // Read inputs from textboxes             
            // Email address converted to lowercase
            string password = login.Password;
            string email = login.EmailAddress;

            string VolunteerPassword = await fDal.GetVolunteerPassword(email);

            if (VolunteerPassword != "")
            {
                if (VolunteerPassword == password)
                {
                    //login to vclunteer

                    //Store user role "Volunteer" as a string in session with the key "Role"
                    HttpContext.Session.SetString("Role", "Volunteer");

                    //Store user email string in session with the key "Email"
                    HttpContext.Session.SetString("Email", email);

                    return RedirectToAction("ViewAllPosts");
                }
                else
                {
                    ModelState.AddModelError("CustomError", "Incorrect password");
                    return View(login);
                }
            }
            else
            {
                //if email not found in volunteer, check in org
                string OrgPassword = await fDal.GetOrgPassword(email);
                if (OrgPassword != "")
                {
                    if (OrgPassword == password)
                    {
                        //login to organisation

                        //StoreLocation user role "Organization" as a string in session with the key "Role"
                        HttpContext.Session.SetString("Role", "Organization");

                        //Store user email string in session with the key "Email"
                        HttpContext.Session.SetString("Email", email);

                        return RedirectToAction("ViewAllPosts");
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "Incorrect password");
                        return View(login);
                    }
                }
                else
                {
                    //if email not found in volunteer and organisation, check in admin
                    string AdminPassword = await fDal.GetAdminPassword(email);
                    if (AdminPassword != "")
                    {
                        if (AdminPassword == password)
                        {
                            //login to Admin

                            //StoreLocation user role "Organization" as a string in session with the key "Role"
                            HttpContext.Session.SetString("Role", "Admin");
                            
                            //Store user email string in session with the key "Email"
                            HttpContext.Session.SetString("Email", email);

                            return RedirectToAction("ViewAllPosts");
                        }

                        else
                        {
                            ModelState.AddModelError("AdminError", "Incorrect password");
                            return View(login);
                        }
                    }

                    else
                    {
                        ModelState.AddModelError("CustomError", "Email not found");
                        return View(login);
                    }
                }
            }
        }

        public ActionResult LogOut()
        {
            // Clear all key-values pairs stored in session state
            HttpContext.Session.Clear();
            // Call the Index action of Home controller
            return RedirectToAction("Login");
        }

        //GET: Register/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetString("Role") != "Volunteer")
            {
            return RedirectToAction("Login", "Home");
            }
            ChangePassword changePassword = new ChangePassword();
            changePassword.Email = HttpContext.Session.GetString("Email");
            return View("../Register/ChangePassword", changePassword);
        }

        //POST: Register/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePasswordAsync(ChangePassword changePassword)
        {
            if (ModelState.IsValid)
            {
                string email = HttpContext.Session.GetString("Email");
                string dbPassword = await fDal.GetVolunteerPassword(email);
                if (dbPassword != changePassword.Password)
                {
                    TempData["Message1"] = "Current Password is incorrect!";
                    return View("../Register/ChangePassword", changePassword);
                }
                else
                {
                    fDal.UpdateVolunteerPassword(changePassword.ConfirmPassword, changePassword.Email);
                    TempData["Message1"] = "Password have been successfully changed!";
                    return View("../Register/ChangePassword", changePassword);
                }
            }
            return View("../Register/ChangePassword", changePassword);
        }

        async public Task<IActionResult> ViewAllPosts()
        {
            List<ElderlyPost> elderlyPostList = await fDal.GetAllForms();
            return View("../Volunteers/VolunteerViewPost", elderlyPostList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        async public Task<IActionResult> ViewFilteredPosts(IFormCollection formData)
        {
            System.DateTime startTime = System.DateTime.Parse(formData["startTime"]);
            System.DateTime endTime = System.DateTime.Parse(formData["endTime"]);
            List<ElderlyPost> elderlyPostList = await fDal.GetFormsByDate(startTime, endTime);
            return View("../Volunteers/VolunteerViewPost", elderlyPostList);
        }
        async public Task<IActionResult> ViewPostDetails(string id)
        {
            ElderlyPost selectedpost = await fDal.GetForm(id);
            return View("../Volunteers/ViewPostDetails", selectedpost);            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
