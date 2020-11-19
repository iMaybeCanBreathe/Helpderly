using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FSD_Helpderly.DAL;
using FSD_Helpderly.Models;

namespace FSD_Helpderly.Controllers
{
    public class RegisterController : Controller
    {
        private FirestoreDAL fDal = new FirestoreDAL();
        // GET: Register/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: Register/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IndexAsync(Register register)
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
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Message"] = "Email already exist!";
                    return View(register);
                }
            }
            else
            {
                //Input validation fails, return to the register view to display error message
                return View(register);
            }
        }

        //GET: Register/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            //if ((HttpContext.Session.GetString("Role") == null) ||
            //    (HttpContext.Session.GetString("Role") != "Customer"))
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            ChangePassword changePassword = new ChangePassword();
            //changePassword.DatabasePassword = HttpContext.Session.GetString("password");
            return View(changePassword);
        }

        //POST: Register/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePasswordAsync(ChangePassword changePassword)
        {
            if (ModelState.IsValid)
            {
                string email = "BBean@yahoo.com";
                string dbPassword = await fDal.GetVolunteerPassword(email);
                if (dbPassword != changePassword.Password)
                {
                    TempData["Message1"] = "Current Password is incorrect!";
                    return View(changePassword);
                }
                else
                {
                    fDal.UpdateVolunteerPassword(changePassword.ConfirmPassword);
                    TempData["Message1"] = "Password have been successfully changed!";
                    return View(changePassword);
                }
            }
            else
            {
                return View(changePassword);
            }
        }
    }
}
