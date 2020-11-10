using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FSD_Helpderly.Models;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;

namespace FSD_Helpderly.Controllers
{
    public class RegisterController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "Ww0yJFX2CoMWcN7Bf8HqDQwTyICVYzVrL4W83Dm3",
            BasePath = "https://helpderly.firebaseio.com/"

        };
        IFirebaseClient client;
        // GET: Register/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: Register/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Register register)
        {
            if (ModelState.IsValid)
            {
                //Add volunteer record to database
                AddVolunteerToFirebase(register);
                TempData["Message"] = "Your Account have been successfully created!";
                return RedirectToAction("Index");
            }
            else
            {
                //Input validation fails, return to the register view to display error message
                return View(register);
            }
        }
        private void AddVolunteerToFirebase(Register register)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = register;
            PushResponse response = client.Push("Volunteers/", data);
            data.VolunteerID = response.Result.name;
            SetResponse setResponse = client.Set("Volunteers/" + data.VolunteerID, data);
        }
    }
}
