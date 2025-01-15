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
    public class MenuPemetaanSosialControllercopy : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly PilarService _pilarService;

        public MenuPemetaanSosialControllercopy(ApplicationDbContext db, PilarService pilarService)
        {
            _db = db;
            _pilarService = pilarService;
        }

        [HttpGet]
        public async Task<IActionResult> DaftarPemetaanSosial()
        {
            var kode_cabang = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_KODE_CABANG);

            var daftarPemetaanSosial = await _db.TR_PeriodeDokumen
                .Where(p => p.kode_cabang == kode_cabang)
                .Select(p => new
                {
                    p.id,
                    p.tahun_dokumen,
                    HasApproval = _db.TR_PersetujuanDokumen
                        .Any(a => a.periode_dokumen_id == p.id) 
                })
                .OrderByDescending(p => p.tahun_dokumen)
                .ToListAsync();

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
            var daftarPemetaanSosial = await _db.TR_PeriodeDokumen
            .Where(p =>
                p.tahun_dokumen == tahun_dokumen &&
                _db.TR_PersetujuanDokumen
                    .Any(a => a.periode_dokumen_id == p.id && (a.disetujui_untuk == "BM/SM"))
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
                    periode.tahun_dokumen
                }
            )
            .ToListAsync();

            var total_cabang = await _db.M_Cabang.CountAsync();

            var total_persetujuan = await _db.TR_PeriodeDokumen
                .Where(p =>
                    p.tahun_dokumen == tahun_dokumen &&
                    _db.TR_PersetujuanDokumen
                        .Any(a => a.periode_dokumen_id == p.id &&
                                a.disetujui_untuk == "BM/SM" &&
                                a.disetujui_flag == "Disetujui")
                )
                .CountAsync();

            ViewBag.tahunDokumen = tahun_dokumen;
            ViewBag.totalCabang = total_cabang;
            ViewBag.totalPersetujuan = total_persetujuan;
            ViewBag.statusBefore = "BM/SM";

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
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TambahDokumen(TR_PeriodeDokumen tr_PeriodeDokumen)
        {
            tr_PeriodeDokumen.created_at = DateTime.Now;
            tr_PeriodeDokumen.status_disetujui = "Draft";
            _db.TR_PeriodeDokumen.Add(tr_PeriodeDokumen);
            _db.SaveChanges();

            // if(tr_PeriodeDokumen.tahun_dokumen != null){
            //     await _pilarService.CreateDocumentApproval(tr_PeriodeDokumen.kode_cabang, 2027);
            // }

            
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
        public async Task<IActionResult> StatusDokumen(int id, string keterangan_disetujui, int tahun_dokumen, string status_before)
        {
            var currentUserRole = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_ROLE);

            if (id != 0) {
                var tr_PeriodeDokumen = _db.TR_PeriodeDokumen.FirstOrDefault(x => x.id == id);

                if (tr_PeriodeDokumen != null && (tr_PeriodeDokumen.status_disetujui == "New" && currentUserRole == "BM/SM") )
                {
                    
                    tr_PeriodeDokumen.status_disetujui = "BM/SM";
                    _db.TR_PeriodeDokumen.Update(tr_PeriodeDokumen);
                    
                    var tr_PersetujuanDokumen = new TR_PersetujuanDokumen(){
                        periode_dokumen_id = id,
                        disetujui_untuk = "BM/SM",
                        keterangan_disetujui = keterangan_disetujui,
                        disetujui_oleh = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN),
                        disetujui_flag = "Disetujui",
                        tanggal_disetujui = DateTime.Now
                    };
                    _db.TR_PersetujuanDokumen.Add(tr_PersetujuanDokumen);    

                    _db.SaveChanges();           
                } else {
                    var status_now = "";

                    if (tr_PeriodeDokumen.status_disetujui == "BM/SM" && currentUserRole == "ESRO Leader"){
                        status_now = "ESRO Leader";
                    } else if (tr_PeriodeDokumen.status_disetujui == "ESRO Leader" && currentUserRole == "Dept Head CSR"){
                        status_now = "Dept Head CSR";
                    }

                    var getData = _db.TR_PeriodeDokumen
                        .Where(x=> x.tahun_dokumen == tahun_dokumen && x.status_disetujui == tr_PeriodeDokumen.status_disetujui)
                        .ToList();

                    foreach (var item in getData) {
                        item.status_disetujui = status_now;
                        _db.TR_PeriodeDokumen.Update(item);
                        
                        var tr_PersetujuanDokumen = new TR_PersetujuanDokumen(){
                            periode_dokumen_id = item.id,
                            disetujui_untuk = status_now,
                            keterangan_disetujui = keterangan_disetujui,
                            disetujui_oleh = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN),
                            disetujui_flag = "Disetujui",
                            tanggal_disetujui = DateTime.Now
                        };
                        _db.TR_PersetujuanDokumen.Add(tr_PersetujuanDokumen); 
                        _db.SaveChanges();    
                    }
                }
            } else {
                var status_now = "";

                if (status_before == "BM/SM" && currentUserRole == "ESRO Leader"){
                    status_now = "ESRO Leader";
                } else if (status_before == "ESRO Leader" && currentUserRole == "Dept Head CSR"){
                    status_now = "Dept Head CSR";
                }

                var tr_PeriodeDokumen = _db.TR_PeriodeDokumen.Where(x => x.tahun_dokumen == tahun_dokumen && x.status_disetujui == status_before).ToList();
                foreach (var item in tr_PeriodeDokumen) {
                    item.status_disetujui = status_now;
                    _db.TR_PeriodeDokumen.Update(item);
                    
                    var tr_PersetujuanDokumen = new TR_PersetujuanDokumen(){
                        periode_dokumen_id = item.id,
                        disetujui_untuk = status_now,
                        keterangan_disetujui = keterangan_disetujui,
                        disetujui_oleh = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN),
                        disetujui_flag = "Disetujui",
                        tanggal_disetujui = DateTime.Now
                    };
                    _db.TR_PersetujuanDokumen.Add(tr_PersetujuanDokumen); 
                    _db.SaveChanges();    
                }
            }

            return RedirectToAction("DaftarPemetaanSosial", "MenuPemetaanSosial");
        }
    }
}