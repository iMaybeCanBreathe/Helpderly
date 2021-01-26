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
            return RedirectToAction("ViewAllPosts", "Home");
        }

        async public Task<IActionResult> ViewActivePosts()
        {
            if (HttpContext.Session.GetString("Role") != "Volunteer")
            {
                return RedirectToAction("Login", "Home");
            }

            string email = HttpContext.Session.GetString("Email");
            List<object> selectedFormIds = await fDal.GetVolunteerForms(email);
            List<ElderlyPost> selectedForms = new List<ElderlyPost>();
            foreach (string formid in selectedFormIds)
            {
                ElderlyPost form = await fDal.GetForm(formid);
                if (form.Status == "ongoing")
                {
                    selectedForms.Add(form);
                }
            }

            return View("../Volunteers/SelectedViewPost", selectedForms);
        }

        async public Task<IActionResult> ViewDonePosts()
        {
            if (HttpContext.Session.GetString("Role") != "Volunteer")
            {
                return RedirectToAction("Login", "Home");
            }

            string email = HttpContext.Session.GetString("Email");
            List<object> selectedFormIds = await fDal.GetVolunteerForms(email);
            List<ElderlyPost> selectedForms = new List<ElderlyPost>();
            foreach (string formid in selectedFormIds)
            {
                ElderlyPost form = await fDal.GetForm(formid);
                if (form.Status == "done")
                {
                    selectedForms.Add(form);
                }
            }

            return View("../Volunteers/ViewDonePost", selectedForms);
        }

        public IActionResult AcceptPost(string formId)
        {
            if (HttpContext.Session.GetString("Role") != "Volunteer")
            {
                return RedirectToAction("Login", "Home");
            }

            string email = HttpContext.Session.GetString("Email");
            fDal.VolunteerAcceptForm(email, formId);
            return RedirectToAction("ViewActivePosts");
            
        }

        public IActionResult CancelPost(string formId)
        {
            if (HttpContext.Session.GetString("Role") != "Volunteer")
            {
                return RedirectToAction("Login", "Home");
            }

            string email = HttpContext.Session.GetString("Email");
            fDal.VolunteerCancelForm(email, formId);
            return RedirectToAction("ViewActivePosts");
        }

        public IActionResult SetPostDone(string formId)
        {
            if (HttpContext.Session.GetString("Role") != "Volunteer")
            {
                return RedirectToAction("Login", "Home");
            }

            fDal.UpdateFormStatus(formId, true);
            return RedirectToAction("ViewActivePosts");
        }

        public IActionResult SetPostOngoing(string formId)
        {
            if (HttpContext.Session.GetString("Role") != "Volunteer")
            {
                return RedirectToAction("Login", "Home");
            }

            fDal.UpdateFormStatus(formId, false);
            return RedirectToAction("ViewDonePosts");
        }
    }
}