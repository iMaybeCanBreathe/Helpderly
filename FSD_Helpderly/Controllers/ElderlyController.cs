using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FSD_Helpderly.Controllers
{
    public class ElderlyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}