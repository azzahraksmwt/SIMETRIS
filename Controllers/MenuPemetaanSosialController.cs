using System.Linq.Expressions;
using System.Security.Claims;
using CREDA_App.Data;
using CREDA_App.DTO;
using CREDA_App.Helper;
using CREDA_App.Models;
using CREDA_App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CREDA_App.Controllers
{
    public class MenuPemetaanSosialController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly PilarService _pilarService;

        public MenuPemetaanSosialController(ApplicationDbContext db, PilarService pilarService)
        {
            _db = db;
            _pilarService = pilarService;
        }

        [HttpGet]
        public async Task<IActionResult> DaftarPemetaanSosial(string tab)
        {
            var kode_cabang = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_KODE_CABANG);
            var currentUserRole = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_ROLE);
            ViewBag.KodeCabang = kode_cabang;

            IEnumerable<dynamic> daftarPemetaanSosial = new List<dynamic>();;

            if (currentUserRole == "BM/SM" || currentUserRole == "CSR Local" || currentUserRole == "CSR HO"){
                daftarPemetaanSosial = await _db.TR_PeriodeDokumen
                    .Where(p => p.kode_cabang == kode_cabang)
                    .Select(p => new
                    {
                        p.id,
                        p.tahun_dokumen,
                        p.kode_cabang,
                        // HasApproval = _db.TR_PersetujuanDokumen
                        //     .Any(a => a.periode_dokumen_id == p.id) 
                        p.status_disetujui,
                        p.status_verifikasi
                    })
                    .OrderByDescending(p => p.tahun_dokumen)
                    .ToListAsync();
            } 
            else if (currentUserRole == "Dept Head CSR")
            {
                var query = _db.TR_PeriodeDokumen.AsQueryable();

                if (string.IsNullOrEmpty(tab))
                {
                    tab = "CabangCMO";
                }

                if (tab == "CabangCMO")
                {
                    daftarPemetaanSosial = await query
                        .Where(p => p.kode_cabang == kode_cabang)  
                        .ToListAsync()
                        .ContinueWith(task => task.Result
                            .GroupBy(p => p.tahun_dokumen)
                            .Select(g => g.First())
                            .OrderByDescending(p => p.tahun_dokumen)
                            .ToList());

                    foreach (var item in daftarPemetaanSosial)
                    {
                        Console.WriteLine($"Tahun Dokumen: {item.tahun_dokumen}, Kode Cabang: {item.kode_cabang}, Status Disetujui: {item.status_disetujui}");
                    }
                }
                else if (tab == "SeluruhCabang")
                {
                    daftarPemetaanSosial = await query
                        .ToListAsync() 
                        .ContinueWith(task => task.Result
                        .GroupBy(p => p.tahun_dokumen)
                        .Select(g => g.First())
                        .OrderByDescending(p => p.tahun_dokumen)
                        .ToList());

                    foreach (var item in daftarPemetaanSosial)
                    {
                        Console.WriteLine($"Tahun Dokumen: {item.tahun_dokumen}, Kode Cabang: {item.kode_cabang}, Status Disetujui: {item.status_disetujui}");
                    }
                }

                // if (tabActive == "cabangCMO" && !string.IsNullOrEmpty(kode_cabang))
                // {
                //     // Filter berdasarkan kode_cabang jika tab "cabangCMO" aktif
                //     query = query.Where(p => p.kode_cabang == kode_cabang);
                // }
                // else if (tabActive == "cabangCMO")
                // {
                //     // Jika tab "cabangCMO" aktif, filter berdasarkan kode cabang yang diambil dari claims
                //     query = query.Where(p => p.kode_cabang == kode_cabang);
                // }
            }
            else
            {
                // Menampilkan data tanpa filter kode_cabang
                daftarPemetaanSosial = _db.TR_PeriodeDokumen
                    .AsEnumerable()
                    .GroupBy(p => p.tahun_dokumen)
                    .Select(g => g.First())
                    .OrderByDescending(p => p.tahun_dokumen)
                    .ToList();
            }

            return View(daftarPemetaanSosial);
        }

        [HttpGet]
        public async Task<IActionResult> DaftarPemetaanSosialSemuaCabang()
        {
            // var kode_cabang = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_KODE_CABANG);

            var daftarPemetaanSosial = await _db.TR_PeriodeDokumen
                .OrderByDescending(p => p.tahun_dokumen)
                .ToListAsync(); // Ambil data dari database ke memori

            var hasilGrup = daftarPemetaanSosial
                .GroupBy(p => p.tahun_dokumen)
                .Select(g => g.First())
                .ToList(); 

            return View(hasilGrup);
        }

        [HttpGet]
        public async Task<IActionResult> DaftarSemuaCabang(int tahun_dokumen)
        {
            var currentUserRole = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_ROLE);

            string[] disetujuiUntuk;
            if (currentUserRole == "ESRO Leader")
            {
                disetujuiUntuk = new[] { "BM/SM", "Dept Head CSR" };
            }
            else if (currentUserRole == "Dept Head CSR")
            {
                disetujuiUntuk = new[] { "ESRO Leader" };
            }
            else
            {
                return Forbid(); 
            }

            var daftarPemetaanSosial = await _db.TR_PeriodeDokumen
            .Where(p =>
                p.tahun_dokumen == tahun_dokumen &&
                _db.TR_PersetujuanDokumen
                    // .Any(a => a.periode_dokumen_id == p.id && (a.disetujui_untuk == disetujuiUntuk && a.disetujui_flag == "Disetujui") || (a.disetujui_untuk == disetujuiUntuk && a.disetujui_flag == "Konfirmasi Ulang" || a.disetujui_flag == "Menunggu Persetujuan Ulang") )
                    .Any(a => a.periode_dokumen_id == p.id && disetujuiUntuk.Contains(a.disetujui_untuk) && a.disetujui_flag == "Disetujui")
            )
            .Join(
                _db.M_Cabang,
                periode => periode.kode_cabang,
                cabang => cabang.kode_cabang,
                (periode, cabang) => new
                {
                    periode.id,
                    periode.kode_cabang,
                    cabang.nama_cabang,
                    periode.tahun_dokumen,
                    periode.status_verifikasi
                }
            )
            .ToListAsync();
            
            var total_cabang = await _db.M_Cabang.CountAsync();

            var total_persetujuan = await _db.TR_PeriodeDokumen
                .Where(p =>
                    p.tahun_dokumen == tahun_dokumen &&
                    _db.TR_PersetujuanDokumen
                        .Any(a => a.periode_dokumen_id == p.id &&
                                disetujuiUntuk.Contains(a.disetujui_untuk) &&
                                a.disetujui_flag == "Disetujui")
                )
                .CountAsync();

            ViewBag.tahunDokumen = tahun_dokumen;
            ViewBag.totalCabang = total_cabang;
            ViewBag.totalPersetujuan = total_persetujuan;
            ViewBag.verifikasiDokumen = daftarPemetaanSosial.Select(d => d.status_verifikasi).FirstOrDefault();
            
            // ViewBag.statusBefore = "BM/SM";

            // ViewBag.allApproved = total_persetujuan == total_cabang;

            return View(daftarPemetaanSosial);
        }

        [HttpGet]
        public IActionResult TambahDokumen()
        {
            var kodeCabang = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("KodeCabang"))?.Value ?? "-";
            ViewBag.namaCabang = _db.M_Cabang
                .Where(c => c.kode_cabang == kodeCabang)
                .FirstOrDefault();

            var today = DateTime.Now;
            ViewBag.tahunDokumen = _db.TR_BatasWaktuPengisian
                .Where(b => b.tanggal_mulai <= today && b.tanggal_berakhir >= today)
                .Select(b => b.tahun_dokumen)
                .ToList();
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TambahDokumen(TR_PeriodeDokumen tr_PeriodeDokumen)
        {
            var kodeCabang = tr_PeriodeDokumen.kode_cabang;
            var tahunDokumen = tr_PeriodeDokumen.tahun_dokumen;

            var existingDokumen = await _db.TR_PeriodeDokumen
                .FirstOrDefaultAsync(d => d.kode_cabang == kodeCabang && d.tahun_dokumen == tahunDokumen);

            if (existingDokumen != null)
            {
                TempData["ErrorMessage"] = "Dokumen dengan tahun tersebut sudah ada di database.";
                return RedirectToAction("DaftarPemetaanSosial", "MenuPemetaanSosial");
            }

            tr_PeriodeDokumen.created_at = DateTime.Now;
            tr_PeriodeDokumen.status_disetujui = "Draft";
            _db.TR_PeriodeDokumen.Add(tr_PeriodeDokumen);
            _db.SaveChanges();
            
            return RedirectToAction("DaftarPemetaanSosial", "MenuPemetaanSosial");
        }

        [HttpGet]
        public IActionResult VerifikasiDokumen(int tahun_dokumen)
        {
            ViewBag.tahunDokumen = _db.TR_PeriodeDokumen
                .Where(b => b.tahun_dokumen == tahun_dokumen)
                .Select(b => b.tahun_dokumen)
                .FirstOrDefault();
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifikasiDokumen(TR_PeriodeDokumen tr_PeriodeDokumen)
        {
            var verifikasiDokumenList = await _db.TR_PeriodeDokumen
                .Where(d => d.tahun_dokumen == tr_PeriodeDokumen.tahun_dokumen)
                .ToListAsync();

            foreach (var item in verifikasiDokumenList) {
                item.status_verifikasi = tr_PeriodeDokumen.status_verifikasi;
                item.updated_at = DateTime.Now;
                item.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);
                _db.TR_PeriodeDokumen.Update(item);
            }

            _db.SaveChangesAsync();

            return RedirectToAction("DaftarPemetaanSosial", "MenuPemetaanSosial");
        }

        [HttpGet]
        public async Task<IActionResult> StatusDokumen(int id, int tahunDokumen)
        {
            var currentUserRole = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_ROLE);
            ViewBag.userRole = currentUserRole;
            ViewBag.tahunDokumen = tahunDokumen;

            var daftarPemetaanSosial = await _db.TR_PeriodeDokumen
                .Where(p => p.id == id)
                .Select(p => new 
                {
                    PeriodeDokumen = p,
                    PersetujuanDokumen = _db.TR_PersetujuanDokumen
                        .Where(a => a.periode_dokumen_id == p.id)
                        .ToList()
                })
                .ToListAsync();

            return View(daftarPemetaanSosial);
        }

        [HttpPost]
        public async Task<IActionResult> StatusDokumen(int id, string keterangan_disetujui, int tahun_dokumen, string disetujui_flag)
        {
            var currentUserRole = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_ROLE);

            var tr_PeriodeDokumen = _db.TR_PeriodeDokumen.FirstOrDefault(x => x.id == id);

            if (tr_PeriodeDokumen != null && ((tr_PeriodeDokumen.status_disetujui == "Menunggu Persetujuan" || tr_PeriodeDokumen.status_disetujui == "Sudah Dikonfirmasi") && currentUserRole == "BM/SM" || currentUserRole == "Dept Head CSR") )
            {
                tr_PeriodeDokumen.status_disetujui = currentUserRole;
                tr_PeriodeDokumen.updated_at = DateTime.Now;
                tr_PeriodeDokumen.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);
                if (disetujui_flag == "Konfirmasi Ulang") {
                    tr_PeriodeDokumen.status_disetujui = disetujui_flag;
                }

                _db.TR_PeriodeDokumen.Update(tr_PeriodeDokumen);
                
                _db.SaveChanges(); 

                if (disetujui_flag == "Konfirmasi Ulang")
                {
                    await _pilarService.SendApprovalNotification("CSR Local", tr_PeriodeDokumen.kode_cabang, tahun_dokumen);
                    await _pilarService.SendApprovalNotification("CSR HO", tr_PeriodeDokumen.kode_cabang, tahun_dokumen);
                }

                var existingPersetujuanDokumen = _db.TR_PersetujuanDokumen.FirstOrDefault(
                    s => s.periode_dokumen_id == tr_PeriodeDokumen.id && s.disetujui_untuk == currentUserRole);

                if (existingPersetujuanDokumen == null)
                {
                    var tr_PersetujuanDokumen = new TR_PersetujuanDokumen
                    {
                        periode_dokumen_id = tr_PeriodeDokumen.id,
                        disetujui_untuk = currentUserRole,
                        keterangan_disetujui = keterangan_disetujui,
                        disetujui_oleh = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN),
                        disetujui_flag = disetujui_flag,
                        tanggal_disetujui = DateTime.Now,
                        created_at = DateTime.Now
                    };
                    _db.TR_PersetujuanDokumen.Add(tr_PersetujuanDokumen);
                }
                else
                {
                    existingPersetujuanDokumen.keterangan_disetujui = keterangan_disetujui;
                    existingPersetujuanDokumen.disetujui_oleh = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);
                    existingPersetujuanDokumen.disetujui_flag = disetujui_flag;
                    existingPersetujuanDokumen.tanggal_disetujui = DateTime.Now;
                    existingPersetujuanDokumen.created_at = DateTime.Now;
                    existingPersetujuanDokumen.updated_at = DateTime.Now;
                    existingPersetujuanDokumen.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);

                    _db.TR_PersetujuanDokumen.Update(existingPersetujuanDokumen);
                }

                _db.SaveChanges();  

                if (disetujui_flag == "Disetujui")
                {
                    var existingESROLeaderApproval = _db.TR_PersetujuanDokumen.FirstOrDefault(
                        s => s.periode_dokumen_id == tr_PeriodeDokumen.id && s.disetujui_untuk == "ESRO Leader");

                    if (existingESROLeaderApproval != null)
                    {
                        existingESROLeaderApproval.disetujui_flag = "Menunggu Persetujuan Ulang";
                        existingESROLeaderApproval.updated_at = DateTime.Now;
                        existingESROLeaderApproval.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);

                        _db.TR_PersetujuanDokumen.Update(existingESROLeaderApproval);
                        _db.SaveChanges();
                    }
                    await _pilarService.SendApprovalNotification("ESRO Leader", tr_PeriodeDokumen.kode_cabang, tahun_dokumen);
                }
            } else {
                var status_now = "";

                if (tr_PeriodeDokumen.status_disetujui == "BM/SM" || tr_PeriodeDokumen.status_disetujui == "Dept Head CSR" && currentUserRole == "ESRO Leader"){
                    status_now = "ESRO Leader";
                } else if (tr_PeriodeDokumen.status_disetujui == "ESRO Leader" && currentUserRole == "Dept Head CSR"){
                    status_now = "Dept Head CSR";
                }

                tr_PeriodeDokumen.status_disetujui = status_now;
                tr_PeriodeDokumen.updated_at = DateTime.Now;
                tr_PeriodeDokumen.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);
                if (disetujui_flag == "Konfirmasi Ulang") {
                    tr_PeriodeDokumen.status_disetujui = disetujui_flag;
                }

                _db.TR_PeriodeDokumen.Update(tr_PeriodeDokumen);
                
                _db.SaveChanges();

                if (disetujui_flag == "Konfirmasi Ulang")
                {
                    await _pilarService.SendApprovalNotification("CSR Local", tr_PeriodeDokumen.kode_cabang, tahun_dokumen);
                    await _pilarService.SendApprovalNotification("CSR HO", tr_PeriodeDokumen.kode_cabang, tahun_dokumen);
                }

                var existingPersetujuanDokumen = _db.TR_PersetujuanDokumen.FirstOrDefault(
                    s => s.periode_dokumen_id == tr_PeriodeDokumen.id && s.disetujui_untuk == currentUserRole);

                // var getData = _db.TR_PeriodeDokumen
                //     .Where(x=> x.tahun_dokumen == tahun_dokumen && x.status_disetujui == tr_PeriodeDokumen.status_disetujui)
                //     .ToList();

                // foreach (var item in getData) {
                //     item.status_disetujui = status_now;
                //     _db.TR_PeriodeDokumen.Update(item);

                //     var existingPersetujuanDokumen = _db.TR_PersetujuanDokumen.FirstOrDefault(
                //         s => s.periode_dokumen_id == item.id && s.disetujui_untuk == status_now);

                    if (existingPersetujuanDokumen == null)
                    {
                        var tr_PersetujuanDokumen = new TR_PersetujuanDokumen
                        {
                            periode_dokumen_id = tr_PeriodeDokumen.id,
                            disetujui_untuk = status_now,
                            keterangan_disetujui = keterangan_disetujui,
                            disetujui_oleh = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN),
                            disetujui_flag = disetujui_flag,
                            tanggal_disetujui = DateTime.Now,
                            created_at = DateTime.Now
                        };
                        _db.TR_PersetujuanDokumen.Add(tr_PersetujuanDokumen);
                    }
                    else
                    {
                        existingPersetujuanDokumen.keterangan_disetujui = keterangan_disetujui;
                        existingPersetujuanDokumen.disetujui_oleh = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);
                        existingPersetujuanDokumen.disetujui_flag = disetujui_flag;
                        existingPersetujuanDokumen.tanggal_disetujui = DateTime.Now;
                        existingPersetujuanDokumen.created_at = DateTime.Now;
                        existingPersetujuanDokumen.updated_at = DateTime.Now;
                        existingPersetujuanDokumen.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);

                        _db.TR_PersetujuanDokumen.Update(existingPersetujuanDokumen);
                    }

                    _db.SaveChanges();  

                    if (disetujui_flag == "Konfirmasi Ulang" && currentUserRole == "ESRO Leader")
                    {
                        var existingBMApproval = _db.TR_PersetujuanDokumen.FirstOrDefault(
                            s => s.periode_dokumen_id == tr_PeriodeDokumen.id && (s.disetujui_untuk == "BM/SM" || s.disetujui_untuk == "Dept Head CSR"));

                        if (existingBMApproval != null)
                        {
                            existingBMApproval.disetujui_flag = "Menunggu Persetujuan Ulang";
                            existingBMApproval.updated_at = DateTime.Now;
                            existingBMApproval.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);

                            _db.TR_PersetujuanDokumen.Update(existingBMApproval);
                            _db.SaveChanges();
                        }
                    }  

                    await _pilarService.SendApprovalNotification("Dept Head CSR", tr_PeriodeDokumen.kode_cabang, tahun_dokumen);
                // }
            }

            if (currentUserRole == "BM/SM") {
                return RedirectToAction("DaftarPemetaanSosial", "MenuPemetaanSosial");
            } else {
                return RedirectToAction("DaftarSemuaCabang", "MenuPemetaanSosial", new { tahun_dokumen = tahun_dokumen });
            }
        }

        // [HttpPost]
        // public async Task<IActionResult> StatusDokumen(int id, string keterangan_disetujui, int tahun_dokumen, string disetujui_flag)
        // {
        //     var currentUserRole = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_ROLE);

        //     var tr_PeriodeDokumen = _db.TR_PeriodeDokumen.FirstOrDefault(x => x.id == id);

        //     if (tr_PeriodeDokumen != null && ((tr_PeriodeDokumen.status_disetujui == "Menunggu Persetujuan" || tr_PeriodeDokumen.status_disetujui == "Sudah Dikonfirmasi") && currentUserRole == "BM/SM"))
        //     {
        //         tr_PeriodeDokumen.status_disetujui = currentUserRole;
        //         tr_PeriodeDokumen.updated_at = DateTime.Now;
        //         tr_PeriodeDokumen.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);
        //         if (disetujui_flag == "Konfirmasi Ulang")
        //         {
        //             tr_PeriodeDokumen.status_disetujui = disetujui_flag;
        //         }

        //         _db.TR_PeriodeDokumen.Update(tr_PeriodeDokumen);
        //         await _db.SaveChangesAsync();

        //         var existingPersetujuanDokumen = _db.TR_PersetujuanDokumen.FirstOrDefault(
        //             s => s.periode_dokumen_id == tr_PeriodeDokumen.id && s.disetujui_untuk == currentUserRole);

        //         if (existingPersetujuanDokumen == null)
        //         {
        //             var tr_PersetujuanDokumen = new TR_PersetujuanDokumen
        //             {
        //                 periode_dokumen_id = tr_PeriodeDokumen.id,
        //                 disetujui_untuk = currentUserRole,
        //                 keterangan_disetujui = keterangan_disetujui,
        //                 disetujui_oleh = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN),
        //                 disetujui_flag = disetujui_flag,
        //                 tanggal_disetujui = DateTime.Now,
        //                 created_at = DateTime.Now
        //             };
        //             _db.TR_PersetujuanDokumen.Add(tr_PersetujuanDokumen);
        //         }
        //         else
        //         {
        //             existingPersetujuanDokumen.keterangan_disetujui = keterangan_disetujui;
        //             existingPersetujuanDokumen.disetujui_oleh = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);
        //             existingPersetujuanDokumen.disetujui_flag = disetujui_flag;
        //             existingPersetujuanDokumen.tanggal_disetujui = DateTime.Now;
        //             existingPersetujuanDokumen.created_at = DateTime.Now;
        //             existingPersetujuanDokumen.updated_at = DateTime.Now;
        //             existingPersetujuanDokumen.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);

        //             _db.TR_PersetujuanDokumen.Update(existingPersetujuanDokumen);
        //         }

        //         await _db.SaveChangesAsync();

        //         if (disetujui_flag == "Disetujui")
        //         {
        //             var existingESROLeaderApproval = _db.TR_PersetujuanDokumen.FirstOrDefault(
        //                 s => s.periode_dokumen_id == tr_PeriodeDokumen.id && s.disetujui_untuk == "ESRO Leader");

        //             if (existingESROLeaderApproval != null)
        //             {
        //                 existingESROLeaderApproval.disetujui_flag = "Menunggu Persetujuan Ulang";
        //                 existingESROLeaderApproval.updated_at = DateTime.Now;
        //                 existingESROLeaderApproval.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);

        //                 _db.TR_PersetujuanDokumen.Update(existingESROLeaderApproval);
        //                 await _db.SaveChangesAsync();
        //             }
        //         }

        //         if (disetujui_flag == "Konfirmasi Ulang" && currentUserRole == "ESRO Leader")
        //         {
        //             var existingBMApproval = _db.TR_PersetujuanDokumen.FirstOrDefault(
        //                 s => s.periode_dokumen_id == tr_PeriodeDokumen.id && s.disetujui_untuk == "BM/SM");

        //             if (existingBMApproval != null)
        //             {
        //                 existingBMApproval.disetujui_flag = "Menunggu Persetujuan Ulang";
        //                 existingBMApproval.updated_at = DateTime.Now;
        //                 existingBMApproval.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);

        //                 _db.TR_PersetujuanDokumen.Update(existingBMApproval);
        //                 _db.SaveChanges();
        //             }
        //         }  
             

        //         // Send Approval Notification Email
        //         await _pilarService.SendApprovalNotification("ESRO Leader", tr_PeriodeDokumen.kode_cabang, tahun_dokumen);
        //     }
        //     else
        //     {
        //         var status_now = "";

        //         if (tr_PeriodeDokumen.status_disetujui == "BM/SM" && currentUserRole == "ESRO Leader")
        //         {
        //             status_now = "ESRO Leader";
        //         }
        //         else if (tr_PeriodeDokumen.status_disetujui == "ESRO Leader" && currentUserRole == "Dept Head CSR")
        //         {
        //             status_now = "Dept Head CSR";
        //         }

        //         tr_PeriodeDokumen.status_disetujui = status_now;
        //         tr_PeriodeDokumen.updated_at = DateTime.Now;
        //         tr_PeriodeDokumen.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);
        //         if (disetujui_flag == "Konfirmasi Ulang")
        //         {
        //             tr_PeriodeDokumen.status_disetujui = disetujui_flag;
        //         }

        //         _db.TR_PeriodeDokumen.Update(tr_PeriodeDokumen);
        //         await _db.SaveChangesAsync();

        //         var existingPersetujuanDokumen = _db.TR_PersetujuanDokumen.FirstOrDefault(
        //             s => s.periode_dokumen_id == tr_PeriodeDokumen.id && s.disetujui_untuk == currentUserRole);

        //         if (existingPersetujuanDokumen == null)
        //         {
        //             var tr_PersetujuanDokumen = new TR_PersetujuanDokumen
        //             {
        //                 periode_dokumen_id = tr_PeriodeDokumen.id,
        //                 disetujui_untuk = currentUserRole,
        //                 keterangan_disetujui = keterangan_disetujui,
        //                 disetujui_oleh = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN),
        //                 disetujui_flag = disetujui_flag,
        //                 tanggal_disetujui = DateTime.Now,
        //                 created_at = DateTime.Now
        //             };
        //             _db.TR_PersetujuanDokumen.Add(tr_PersetujuanDokumen);
        //         }
        //         else
        //         {
        //             existingPersetujuanDokumen.keterangan_disetujui = keterangan_disetujui;
        //             existingPersetujuanDokumen.disetujui_oleh = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);
        //             existingPersetujuanDokumen.disetujui_flag = disetujui_flag;
        //             existingPersetujuanDokumen.tanggal_disetujui = DateTime.Now;
        //             existingPersetujuanDokumen.created_at = DateTime.Now;
        //             existingPersetujuanDokumen.updated_at = DateTime.Now;
        //             existingPersetujuanDokumen.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);

        //             _db.TR_PersetujuanDokumen.Update(existingPersetujuanDokumen);
        //         }

        //         await _db.SaveChangesAsync();

        //         // Send Approval Notification Email
        //         await _pilarService.SendApprovalNotification("Dept Head CSR", tr_PeriodeDokumen.kode_cabang, tahun_dokumen);
        //     }

        //     if (currentUserRole == "BM/SM") {
        //         return RedirectToAction("DaftarPemetaanSosial", "MenuPemetaanSosial");
        //     } else {
        //         return RedirectToAction("DaftarSemuaCabang", "MenuPemetaanSosial", new { tahun_dokumen = tahun_dokumen });
        //     }
            
        // }

    }
}