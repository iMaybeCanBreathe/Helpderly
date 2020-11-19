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
    }
}