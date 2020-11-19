using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FSD_Helpderly.Models;
using FSD_Helpderly.DAL;
using Microsoft.AspNetCore.Http;

namespace FSD_Helpderly.Controllers
{
    public class AdminController : Controller
    {
        private FirestoreDAL fDal = new FirestoreDAL();
        public IActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        //register volunteer
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }

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
                    ModelState.Clear();
                    return View("../Register/Index");
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

        //GET: Admin/OrganizationRegister
        [HttpGet]
        public IActionResult OrganizationRegister()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Login", "Home");
            }
            return View("../Admin/OrganizationRegister");
        }

        //POST: Admin/OrganizationRegister
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrganizationRegister(OrganizationRegister register)
        {
            if (ModelState.IsValid)
            {
                string email = register.Email;
                string dbPassword = await fDal.GetOrgPassword(email);
                if (dbPassword == "")
                {
                    //Add volunteer record to database
                    fDal.AddOrg(register.Email, register.Address, register.OrganizationName, register.Password, register.TelNo);
                    TempData["Message2"] = "Your Account have been successfully created!";
                    ModelState.Clear();
                    return View("../Admin/OrganizationRegister");
                }
                else
                {
                    TempData["Message2"] = "Email already exist!";
                    return View("../Admin/OrganizationRegister", register);
                }
            }
            else
            {
                //Input validation fails, return to the register view to display error message
                return View("../Admin/OrganizationRegister", register);
            }
        }
        public IActionResult DeletePost(string formId)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }

            string email = HttpContext.Session.GetString("Email");
            fDal.DeleteForm(formId);
            return RedirectToAction("ViewAllPosts", "Home");
        }
    }
}