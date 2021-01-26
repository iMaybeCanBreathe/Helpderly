using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSD_Helpderly.Models;
using Microsoft.AspNetCore.Mvc;
using FSD_Helpderly.DAL;
using Microsoft.AspNetCore.Http;

namespace FSD_Helpderly.Controllers
{
    public class ElderlyController : Controller
    {
        private FirestoreDAL fDal = new FirestoreDAL();

        public IActionResult Index()
        {
            return RedirectToAction("Form", "Elderly");
        }

        async public Task<IActionResult> Form()
        {
            if (HttpContext.Session.GetString("Role") != "Elderly")
            {
                return RedirectToAction("Login", "Home");
            }

            //get elderly data
            string email = HttpContext.Session.GetString("Email");
            ElderlyPost elderlyPost = await fDal.GetElderlyDetails(email);

            return View("../Home/Form", elderlyPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        async public Task<ActionResult> Form(ElderlyPost elderlyPost)
        {
            elderlyPost.Email = HttpContext.Session.GetString("Email");

            if (ModelState.IsValid)
            {
                string formId = await fDal.AddForm(elderlyPost);
                fDal.UpdateElderlyDetails(elderlyPost);
                fDal.AddFormToElderly(elderlyPost.Email, formId);
                return View("../Home/FormTY");
            }

            else
            {
                return View("../Home/Form", elderlyPost);
            }
        }

        async public Task<IActionResult> ViewActivePosts()
        {
            if (HttpContext.Session.GetString("Role") != "Elderly")
            {
                return RedirectToAction("Login", "Home");
            }

            string email = HttpContext.Session.GetString("Email");
            List<object> selectedFormIds = await fDal.GetElderlyForms(email);
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

        public IActionResult CancelPost(string formId)
        {
            if (HttpContext.Session.GetString("Role") != "Volunteer")
            {
                return RedirectToAction("Login", "Home");
            }

            string email = HttpContext.Session.GetString("Email");
            fDal.RemoveFormFromElderly(email, formId);
            fDal.DeleteForm(formId);
            return RedirectToAction("SelectedViewPost");
        }
    }
}