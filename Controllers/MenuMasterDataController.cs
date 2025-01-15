using CREDA_App.Data;
using CREDA_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CREDA_App.Controllers
{
    public class MenuMasterDataController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MenuMasterDataController(ApplicationDbContext db){
            _db = db;
        }

        public IActionResult PW_DaftarArea()
        {
            var masterdata_DaftarAreas = _db.m_pw_daftararea.ToList();
            var masterdata_PemetaanAreas = _db.m_pw_pemetaanarea
                .Where(x => x.deleted_at == null)
                .Join(
                    _db.m_pw_daftararea,
                    pemetaanarea => pemetaanarea.id_daftar_area,
                    daftararea => daftararea.id,
                    (pemetaanarea, daftararea) => new {
                        pemetaanarea = pemetaanarea,
                        kode_cabang = daftararea.kode_cabang
                    }
                )
                .ToList();

            var pemetaanwilayah = new 
            { 
                masterdata_Daftarareas = masterdata_DaftarAreas, 
                masterdata_Pemetaanareas = masterdata_PemetaanAreas 
            };
            return View(pemetaanwilayah);
        }

        public IActionResult PW_DaftarAreaAddOrEdit(int id=0)
        {
            if (id == 0) {
                return View(new m_pw_daftararea());
            } else {
                var data = _db.m_pw_daftararea.Find(id);
                return View(data);
            }
        }

        [HttpPost]
        public IActionResult PW_DaftarAreaAddOrEdit(int id, m_pw_daftararea daftararea)
        {
            try 
            {
                var existingDaftarArea = _db.m_pw_daftararea.FirstOrDefault(a => 
                    a.kode_cabang.ToUpper() == daftararea.kode_cabang.ToUpper());

                if (existingDaftarArea != null && existingDaftarArea.id != daftararea.id)
                {
                    TempData["ErrorMessage"] = "Data dengan kode cabang tersebut sudah ada di database.";
                    return RedirectToAction(nameof(PW_DaftarArea));
                }

                if (ModelState.IsValid)
                {
                    if (id == 0) {
                        daftararea.created_at = DateTime.Now;
                        _db.m_pw_daftararea.Add(daftararea);
                        _db.SaveChanges();
                    } else {
                        daftararea.id = id;
                        _db.m_pw_daftararea.Update(daftararea);
                        _db.SaveChanges();
                    }
                    return RedirectToAction(nameof(PW_DaftarArea));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction(nameof(PW_DaftarArea));
        }

        [HttpPost]
        public async Task<IActionResult> PW_DaftarAreaDelete(int id)
        {
            var daftarArea = await _db.m_pw_daftararea.FindAsync(id);
            if (daftarArea == null)
            {
                return NotFound();
            }

            daftarArea.deleted_at = DateTime.Now;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(PW_DaftarArea));
        }

        public IActionResult PW_PemetaanArea()
        {
            var masterdata_DaftarAreas = _db.m_pw_daftararea.ToList();
            var masterdata_PemetaanAreas = _db.m_pw_pemetaanarea
                .Where(x => x.deleted_at == null)
                .Join(
                    _db.m_pw_daftararea,
                    pemetaanarea => pemetaanarea.id_daftar_area,
                    daftararea => daftararea.id,
                    (pemetaanarea, daftararea) => new {
                        pemetaanarea = pemetaanarea,
                        kode_cabang = daftararea.kode_cabang
                    }
                )
                .ToList();

            var pemetaanwilayah = new 
            { 
                masterdata_Daftarareas = masterdata_DaftarAreas, 
                masterdata_Pemetaanareas = masterdata_PemetaanAreas 
            };
            return View(pemetaanwilayah);
        }

        public IActionResult PW_PemetaanAreaAddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                ViewBag.DaftarAreas = _db.m_pw_daftararea.ToList();
                return View(new m_pw_pemetaanarea());
            }
            else
            {
                var data = _db.m_pw_pemetaanarea.Find(id);
                ViewBag.DaftarAreas = _db.m_pw_daftararea.ToList();
                return View(data);
            }
        }

        [HttpPost]
        public IActionResult PW_PemetaanAreaAddOrEdit(int id, m_pw_pemetaanarea pemetaanarea)
        {
            try 
            {
                if (ModelState.IsValid)
                {
                    if (id == 0) {
                        pemetaanarea.created_at = DateTime.Now;
                        _db.m_pw_pemetaanarea.Add(pemetaanarea);
                        _db.SaveChanges();
                    } else {
                        pemetaanarea.id = id;
                        _db.m_pw_pemetaanarea.Update(pemetaanarea);
                        _db.SaveChanges();
                    }
                    return RedirectToAction(nameof(PW_PemetaanArea));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction(nameof(PW_PemetaanArea));
        }

        [HttpPost]
        public async Task<IActionResult> PW_PemetaanAreaDelete(int id)
        {
            var pemetaanArea = await _db.m_pw_pemetaanarea.FindAsync(id);
            if (pemetaanArea == null)
            {
                return NotFound();
            }

            pemetaanArea.deleted_at = DateTime.Now;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(PW_PemetaanArea));
        }

        public IActionResult DaftarCabang()
        {
            var daftarCabang = _db.M_Cabang.ToList();

            return View(daftarCabang);
        }

        public IActionResult DaftarCabangAddOrEdit(int id=0)
        {
            if (id == 0) {
                return View(new M_Cabang());
            } else {
                var data = _db.M_Cabang.Find(id);
                return View(data);
            }
        }

        [HttpPost]
        public IActionResult DaftarCabangAddOrEdit(int id, M_Cabang m_Cabang)
        {
            try 
            {
                var existingCabang = _db.M_Cabang
                    .AsNoTracking()
                    .FirstOrDefault();

                if (existingCabang != null && existingCabang.id == m_Cabang.id)
                {
                    TempData["ErrorMessage"] = "Data sudah ada di database.";
                    return RedirectToAction(nameof(DaftarCabang));
                }

                if (ModelState.IsValid)
                {
                    if (id == 0) {
                        m_Cabang.created_at = DateTime.Now;
                        _db.M_Cabang.Add(m_Cabang);
                    } else {
                        m_Cabang.id = id;
                        m_Cabang.updated_at = DateTime.Now;
                        _db.M_Cabang.Update(m_Cabang);
                    }

                    _db.SaveChanges();
                    return RedirectToAction(nameof(DaftarCabang));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction(nameof(DaftarCabang));
        }

        [HttpPost]
        public async Task<IActionResult> DaftarCabangDelete(int id)
        {
            var m_Cabang = await _db.M_Cabang.FindAsync(id);
            if (m_Cabang == null)
            {
                return NotFound();
            }

            m_Cabang.deleted_at = DateTime.Now;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(DaftarCabang));
        }

        public IActionResult BatasWaktuPengisian()
        {
            var tr_BatasWaktuPengisian = _db.TR_BatasWaktuPengisian.ToList();

            return View(tr_BatasWaktuPengisian);
        }

        public IActionResult BatasWaktuPengisianAddOrEdit(int id=0)
        {
            if (id == 0) {
                return View(new TR_BatasWaktuPengisian());
            } else {
                var data = _db.TR_BatasWaktuPengisian.Find(id);
                return View(data);
            }
        }

        [HttpPost]
        public IActionResult BatasWaktuPengisianAddOrEdit(int id, TR_BatasWaktuPengisian tr_BatasWaktuPengisian)
        {
            try 
            {
                var existingBatasWaktuPengisian = _db.TR_BatasWaktuPengisian
                    .AsNoTracking()
                    .FirstOrDefault();

                // if (existingBatasWaktuPengisian != null && existingBatasWaktuPengisian.id == tr_BatasWaktuPengisian.id)
                // {
                //     TempData["ErrorMessage"] = "Data sudah ada di database.";
                //     return RedirectToAction(nameof(BatasWaktuPengisian));
                // }

                if (ModelState.IsValid)
                {
                    if (id == 0) {
                        tr_BatasWaktuPengisian.created_at = DateTime.Now;
                        _db.TR_BatasWaktuPengisian.Add(tr_BatasWaktuPengisian);
                    } else {
                        tr_BatasWaktuPengisian.id = id;
                        tr_BatasWaktuPengisian.updated_at = DateTime.Now;
                        _db.TR_BatasWaktuPengisian.Update(tr_BatasWaktuPengisian);
                    }

                    _db.SaveChanges();
                    return RedirectToAction(nameof(BatasWaktuPengisian));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction(nameof(BatasWaktuPengisian));
        }

        [HttpPost]
        public async Task<IActionResult> BatasWaktuPengisianDelete(int id)
        {
            var tr_BatasWaktuPengisian = await _db.TR_BatasWaktuPengisian.FindAsync(id);
            if (tr_BatasWaktuPengisian == null)
            {
                return NotFound();
            }

            tr_BatasWaktuPengisian.deleted_at = DateTime.Now;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(BatasWaktuPengisian));
        }

        public IActionResult KPI()
        {
            return View();
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
                        usermanagement.nama_file_gambar = usermanagement.nama_file_gambar;
                        usermanagement.nama_tampilan_gambar =  usermanagement.nama_tampilan_gambar;
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