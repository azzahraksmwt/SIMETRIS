using CREDA_App.Data;
using CREDA_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace CREDA_App.Controllers
{
    public class MenuUserManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MenuUserManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult UserManagement()
        {
            if (HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("Role")).Value == "Admin")
            {
               var usermanagements = _db.User_Management.ToList();
                return View(usermanagements); 
            }
            return RedirectToAction("Beranda", "Home"); 
        }

        public IActionResult UserManagementAddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new User_Management());
            }
            else
            {
                var data = _db.User_Management.Find(id);
                return View(data);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserManagementAddOrEdit(int id, User_Management usermanagement)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var myName = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("NamaKaryawan")).Value;
                    if (id == 0)
                    {
                        usermanagement.updated_by = myName;
                        usermanagement.created_at = DateTime.Now;
                        _db.User_Management.Add(usermanagement);
                    }
                    else
                    {
                        usermanagement.updated_by = myName;
                        _db.User_Management.Update(usermanagement);
                    }
                    _db.SaveChanges();
                    return RedirectToAction(nameof(UserManagement));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Terjadi kesalahan saat menyimpan data: " + ex.Message);
                }
            }
            return View(usermanagement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserManagement(int id)
        {
            var userManagement = await _db.User_Management.FindAsync(id);
            if (userManagement != null)
            {
                userManagement.deleted_at = DateTime.Now;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(UserManagement));
        }

    }
}