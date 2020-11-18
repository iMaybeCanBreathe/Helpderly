using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FSD_Helpderly.DAL;
using FSD_Helpderly.Models;

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
            List<object> SelectedFormId = await fDal.GetVolunteerForms("BBean@yahoo.com");
            List<ElderlyPost> selectedform = new List<ElderlyPost>();
            foreach (string formid in SelectedFormId)
            {
                ElderlyPost form = await fDal.GetForm(formid);
                selectedform.Add(form);
            }

            return View("../Volunteers/SelectedViewPost", selectedform);
        }
    }
}