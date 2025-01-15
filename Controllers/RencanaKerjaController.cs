using Microsoft.AspNetCore.Mvc;

namespace CREDA_App.Controllers
{
    public class RencanaKerjaController: Controller
    {
        public IActionResult RencanaKerjaTahunan()
        {
            return View();
        }

        public IActionResult DetailRencanaKerja()
        {
            return View();
        }
    }
}