using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FSD_Helpderly.DAL;
using FSD_Helpderly.Models;
using Google.Api;
using Microsoft.AspNetCore.Http;

namespace FSD_Helpderly.Controllers
{
    public class VolunteerController : Controller
    {
        private FirestoreDAL fDal = new FirestoreDAL();
        public IActionResult Index()
        {
            return View();
        }

        async public Task<IActionResult> SelectedViewPost()
        {
            if (HttpContext.Session.GetString("Role") != "Volunteer")
            {
                return RedirectToAction("Index", "Home");
            }

            List<object> selectedFormIds = await fDal.GetVolunteerForms("BBean@yahoo.com");
            List<ElderlyPost> selectedForms = new List<ElderlyPost>();
            foreach (string formid in selectedFormIds)
            {
                ElderlyPost form = await fDal.GetForm(formid);
                selectedForms.Add(form);
            }

            return View("../Volunteers/SelectedViewPost", selectedForms);
        }

        public IActionResult AcceptPost(string formId)
        {
            string email = HttpContext.Session.GetString("Email");
            fDal.VolunteerAcceptForm(email, formId);
            return RedirectToAction("SelectedViewPost");
        }


        public IActionResult CancelPost(string formId)
        {
            string email = HttpContext.Session.GetString("Email");
            fDal.VolunteerCancelForm(email, formId);
            return RedirectToAction("SelectedViewPost");
        }
    }
}