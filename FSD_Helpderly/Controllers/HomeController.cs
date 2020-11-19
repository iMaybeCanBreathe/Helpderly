﻿using System;
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
using System.ComponentModel.DataAnnotations;

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
            return RedirectToAction("Form", "Home");
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
                    ModelState.AddModelError("CustomError", "Email not found");
                    return View(login);
                }
            }
        }

        //Logout Current Session
        public ActionResult LogOut()
        {
            // Clear all key-values pairs stored in session state
            HttpContext.Session.Clear();

            // Redirect logout user to the home page
            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            return View("../Register/Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Register register)
        {
            if (ModelState.IsValid)
            {
                string email = register.Email;
                string dbPassword = await fDal.GetVolunteerPassword(email);
                if (dbPassword == "")
                {
                    //Add volunteer record to database
                    fDal.AddVolunteer(register.Email, register.Nationality, register.Password, register.TelNo, register.VolunteerName);
                    TempData["Message"] = "Your Account have been successfully created!";
                    return View("../Register/Index", register);
                }
                else
                {
                    TempData["Message"] = "Email already exist!";
                    return View("../Register/Index", register);
                }
            }
            else
            {
                //Input validation fails, return to the register view to display error message
                return View("../Register/Index", register);
            }
        }

        //GET: Register/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Volunteer"))
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
            else
            {
                return View("../Register/ChangePassword", changePassword);
            }
        }

        async public Task<IActionResult> ViewAllPosts()
        {
            List<ElderlyPost> elderlyPostList = await fDal.GetAllForms();
            return View("../Volunteers/VolunteerViewPost", elderlyPostList);
        }
       async public Task<IActionResult> ViewPostDetails(string id)
        {
            ElderlyPost selectedpost = await fDal.GetForm(id);
            System.Diagnostics.Debug.WriteLine(selectedpost.QuantityVolunteer);
            return View("../Volunteers/ViewPostDetails", selectedpost);            
        }

        async public Task<IActionResult> SelectedViewPost()
        {
            string email = HttpContext.Session.GetString("Email");
            List<object> SelectedFormId = await fDal.GetVolunteerForms(email);
            List<ElderlyPost> selectedform = new List<ElderlyPost>();
            foreach (string formid in SelectedFormId)
            {
              ElderlyPost form = await fDal.GetForm(formid);
                selectedform.Add(form);
            }
            return View("../Volunteers/SelectedViewPost",selectedform);
        }

        public IActionResult Form()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(ElderlyPost elderlyPost)
        {
            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine(elderlyPost.StartTime.ToString());
                System.Diagnostics.Debug.WriteLine(elderlyPost.EndTime.ToString());
                fDal.AddForm(elderlyPost);
                return View("FormTY");
            }

            else
            {
                return View("Form", elderlyPost);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
