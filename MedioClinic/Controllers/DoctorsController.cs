using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MedioClinic.Controllers
{
    public class DoctorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(string nodeGuid, string? urlSlug)
        {
            return View();
        }
    }
}
