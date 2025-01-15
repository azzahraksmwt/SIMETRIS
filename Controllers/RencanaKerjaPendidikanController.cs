using Microsoft.AspNetCore.Mvc;

namespace CREDA_App.Controllers
{
    public class RencanaKerjaPendidikanController: Controller
    {
        public IActionResult RenjaPendidikan()
        {
            return View();
        }

        public IActionResult DetailRenjaPendidikan()
        {
            return View();
        }

        public IActionResult RenjaPendidikanAddOrEdit()
        {
            return View();
        }

        public IActionResult KPIProgramAddOrEdit()
        {
            return View();
        }
    }
}