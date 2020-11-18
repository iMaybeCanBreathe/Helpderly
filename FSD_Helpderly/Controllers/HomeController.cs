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
using Microsoft.AspNetCore.Http;

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
            List<ElderlyPost> elderlyPostList = await fDal.GetAllForms();
            return View("../Volunteers/VolunteerViewPost", elderlyPostList);
        }
       async public Task<IActionResult> Accept(string id)
        {
            ElderlyPost selectedpost = await fDal.GetForm(id);
            return View("../Volunteers/Accept", selectedpost);            
            
            //ElderlyPost elderMap = formDetailMaptoAccept(elderpost);
            //return View(elderMap);


        }

        //public ElderlyPost formDetailMaptoAccept(ElderlyPost elderpost)
        //{
        //    ElderlyPost formDetailMaptoAccept = new ElderlyPost
        //    {
        //        AdditionalInfo = elderpost.AdditionalInfo,
        //        Description = elderpost.Description,
        //        Email = elderpost.Email,
        //        EndTime = elderpost.EndTime,
        //        Location = elderpost.Location,
        //        MobileNumber = elderpost.MobileNumber,
        //        Name = elderpost.Name,
        //        QuantityVolunteer = elderpost.QuantityVolunteer,
        //        Region = elderpost.Region,
        //        StartTime = elderpost.StartTime,

        //    };
        //    return formDetailMaptoAccept;
        //}

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
                System.Diagnostics.Debug.WriteLine(elderlyPost.StartTime.ToString());
                System.Diagnostics.Debug.WriteLine(elderlyPost.EndTime.ToString());
                fDal.AddForm(elderlyPost);
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
