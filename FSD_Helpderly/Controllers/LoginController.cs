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
//        public ActionResult Login(IFormCollection formData)
//        {
//            // Read inputs from textboxes             
//            // Email address converted to lowercase
//            string password = formData["Password"].ToString();
//            string email = formData["txtEmail"].ToString();

//            if (email == null)
//            {
//                // Store an error message in TempData for display at the index view     
//                TempData["Message"] = "Email not found";
//                // Redirect user back to the index view through an action 
//                return View("Login");
//            }

//            else
//            {
//                return View("VolunteerViewPost");
//            }
//        }
//    }
//}
