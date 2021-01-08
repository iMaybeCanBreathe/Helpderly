using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FSD_Helpderly.Controllers
{
    public class AboutController : Controller
    {
        [HttpGet]
        // GET: About Us
        public IActionResult About()
        {
            return View();
        }
    }
}
