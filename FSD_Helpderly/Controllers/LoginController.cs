//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using FSD_Helpderly.DAL;
//using FSD_Helpderly.Models;
//using Microsoft.AspNetCore.Http;

//namespace FSD_Helpderly.Controllers
//{
//    public class LoginController : Controller
//    {
//        private FirestoreDAL fDal = new FirestoreDAL();

//        public IActionResult Login()
//        {
//            return View();
//        }
    
//        // POST: Login
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        async public Task<ActionResult> Login(IFormCollection formData)
//        {
//            // Read inputs from textboxes             
//            // Email address converted to lowercase
//            string password = formData["Password"].ToString();
//            string email = formData["txtEmail"].ToString();

//            string dbPassword = await fDal.GetVolunteerPassword(email);
//            if (dbPassword == "")
//            {
//                dbPassword = await fDal.GetOrgPassword(email);
//                return RedirectToAction("VolunteerViewPost");
//            }
//            if (dbPassword == "")
//            {
//                TempData["Message"] = "Email not found";
//                return View("Login");
//            }
//        }
//    }
//}