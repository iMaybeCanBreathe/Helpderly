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

namespace FSD_Helpderly.Controllers
{
    public class OrganizationController : Controller
    {
        private FirestoreDAL fDal = new FirestoreDAL();
        public IActionResult Index()
        {
            return RedirectToAction("Form", "Organization");
        }
        public IActionResult Form()
        {
            if (HttpContext.Session.GetString("Role") != "Organization")
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(ElderlyPost elderlyPost)
        {
            if (ModelState.IsValid)
            {
                fDal.AddForm(elderlyPost);
                return View("FormTY");
            }

            else
            {
                return View("Form", elderlyPost);
            }
        }
    }
}
