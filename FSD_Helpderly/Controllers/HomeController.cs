using System;
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

namespace FSD_Helpderly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private FirestoreDAL fDal = new FirestoreDAL();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        async public Task<IActionResult> VolunteerViewPost()
        {
            Dictionary<string, ElderlyPost> forms = await fDal.GetAllForms();
            List<ElderlyPost> elderlyPostList = new List<ElderlyPost>();
            foreach (KeyValuePair<string, ElderlyPost> form in forms)
            {
                System.Diagnostics.Debug.WriteLine(form.Key);
                elderlyPostList.Add(form.Value);
            }
            return View("../Volunteers/VolunteerViewPost", elderlyPostList);
        }
        public IActionResult Form()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(ElderlyPost elderlyPost)
        {
            if (ModelState.IsValid)
            {
                fDal.AddForm(elderlyPost.AdditionalInfo, elderlyPost.Description, elderlyPost.Email, elderlyPost.EndTime, elderlyPost.FirstName, elderlyPost.LastName, elderlyPost.Location, elderlyPost.MobileNumber, elderlyPost.StartTime);
                return View("FormTY");
            }

            else
            {
                return View("Form", elderlyPost);
            }
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
