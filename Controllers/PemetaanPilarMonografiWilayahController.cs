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
    // [Authentication]
    public class PemetaanPilarMonografiWilayahController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly PilarService _pilarService;
        public PemetaanPilarMonografiWilayahController(ApplicationDbContext db, PilarService pilarService)
        {
            _db = db;
            _pilarService = pilarService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var vw_sc_listAllDashboardPilar = await _db.VW_SC_ListAllDashboardPilar
                                    .AsNoTracking()
                                    .ToListAsync();    

            return View(vw_sc_listAllDashboardPilar);
        }

        [HttpGet]
        public async Task<IActionResult> SkorPemetaanPilar(string kode_cabang)
        {
            if (string.IsNullOrEmpty(kode_cabang))
            {
                kode_cabang = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_KODE_CABANG);
            }

            ViewBag.vw_sc_allDashboardPilar = await _db.VW_SC_AllDashboardPilar
                                    .Where(v => v.kode_cabang == kode_cabang)
                                    .AsNoTracking()
                                    .ToListAsync();

            ViewBag.vw_sc_allDashboardPilarTotal = await _db.VW_SC_AllDashboardPilarTotal
                                    .Where(v => v.kode_cabang == kode_cabang)
                                    .AsNoTracking()
                                    .ToListAsync();    

            ViewBag.vw_ctr_allPilar = await _db.VW_CTR_AllPilar
                                    .Where(v => v.kode_cabang == kode_cabang)
                                    .AsNoTracking()
                                    .ToListAsync();                        

            ViewBag.ctr_monografiDesa = await _db.CTR_MonografiDesa
                                    .Where(v => v.kode_cabang == kode_cabang)
                                    .AsNoTracking()
                                    .ToListAsync();  
            
            ViewBag.m_cabang = await _db.M_Cabang
                                    .Where(v => v.kode_cabang == kode_cabang)
                                    .AsNoTracking()
                                    .ToListAsync();  

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MonografiWilayah(int tahun_dokumen, string kode_cabang)
        {
            // var kode_cabang = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_KODE_CABANG);

            int tahunDokumen = (tahun_dokumen > 0) ? tahun_dokumen : DateTime.Now.Year;

            var getApproval = _db.R_ApprovalSocialMapping
            .Where(x =>  x.tahun_dokumen == tahunDokumen)
            .FirstOrDefault();
            
            if (getApproval != null) {
                ViewBag.approvalFLag = true;
            } else {
                ViewBag.approvalFLag = false;
            }
            
            var m_monografiDesa = await _db.M_MonografiDesa
                .AsNoTracking()
                .ToListAsync();

            for(int i = 0; i < m_monografiDesa.Count; i++){
                var transaksiMonografiDesa = await _db.TR_MonografiDesa.Where(s => s.kode_kategori == m_monografiDesa[i].kode_kategori 
                    && s.kode_cabang == kode_cabang && s.tahun_dokumen == tahunDokumen).AsNoTracking().ToListAsync();
                if(transaksiMonografiDesa.Count != 0){
                    m_monografiDesa[i].DTO_isian = transaksiMonografiDesa.FirstOrDefault().isian;
                }
            }

            var tahunList = _db.TR_MonografiDesa
                .Select(x => x.tahun_dokumen)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();
                
            // var ctr_monografiDesa = await _db.CTR_MonografiDesa
            //     .Where(v => v.kode_cabang == kode_cabang && v.tahun_dokumen == tahunDokumen)
            //     .Select(x => new { x.total_isian, x.total_kategori })
            //     .AsNoTracking()
            //     .FirstOrDefaultAsync(); 

            // if (ctr_monografiDesa != null)
            // {
            //     ViewBag.TotalIsian = ctr_monografiDesa.total_isian;
            //     ViewBag.TotalKategori = ctr_monografiDesa.total_kategori;
            // }
            // else
            // {
            //     ViewBag.TotalIsian = 0;
            //     ViewBag.TotalKategori = 84;
            // }

            var combinedDataPilar = await _pilarService.GetCombinedDataPilarAsync(kode_cabang, tahunDokumen);
            ViewBag.ctrDataAllPilar = combinedDataPilar;
            
            ViewBag.Kode_Cabang = kode_cabang;
            ViewBag.TahunList = tahunList;
            ViewBag.SelectedYear = tahunDokumen;

            var result = await _pilarService.CekEditDokumenAsync(tahun_dokumen);
            ViewBag.CanEdit = result.canEdit;

            await _pilarService.CreateDocumentApproval(kode_cabang, tahunDokumen);

            return View(m_monografiDesa);
        }

        [HttpPost]
        public async Task<IActionResult> MonografiWilayah(int id, List<M_MonografiDesa> monografiDesa, int tahun_dokumen, string kode_cabang)
        {
            try 
            {
                var namaKaryawan = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("NamaKaryawan")).Value;
                var isEditable = "No";

                if (!ModelState.IsValid)
                {
                    for(var i = 0; i < monografiDesa.Count(); i ++){
                        var transaksiExist = _db.TR_MonografiDesa.Where(s => s.kode_kategori == monografiDesa[i].kode_kategori 
                        && s.kode_cabang == kode_cabang && s.tahun_dokumen == tahun_dokumen).AsNoTracking().FirstOrDefault();
                        if (id == 0 && transaksiExist == null) {
                            if(monografiDesa[i].DTO_isian != null){
                                var Monografi = new TR_MonografiDesa(){
                                    isian = monografiDesa[i].DTO_isian,
                                    kode_kategori = monografiDesa[i].kode_kategori,
                                    kategori = monografiDesa[i].kategori,
                                    created_at = DateTime.Now,
                                    kode_cabang = kode_cabang,
                                    tahun_dokumen = tahun_dokumen
                                };                            
                                _db.TR_MonografiDesa.Add(Monografi);
                                _db.SaveChanges();
                            }
                        } else {
                            var existingMonografi = _db.TR_MonografiDesa.FirstOrDefault(
                                s => s.kode_kategori == monografiDesa[i].kode_kategori && 
                                    s.kode_cabang == kode_cabang && s.tahun_dokumen == tahun_dokumen);

                            if (existingMonografi != null)
                            {
                                existingMonografi.isian = monografiDesa[i].DTO_isian;
                                existingMonografi.updated_at = DateTime.Now;
                                existingMonografi.updated_by = namaKaryawan;

                                _db.TR_MonografiDesa.Update(existingMonografi);
                                _db.SaveChanges();

                                isEditable = "yes";
                            }
                        }
                    }

                    if (isEditable == "yes")
                    {
                        var existingPeriodeDokumen = _db.TR_PeriodeDokumen.FirstOrDefault(
                            s => s.kode_cabang == kode_cabang && s.tahun_dokumen == tahun_dokumen);

                        existingPeriodeDokumen.status_disetujui = "Sudah Dikonfirmasi";
                        _db.TR_PeriodeDokumen.Update(existingPeriodeDokumen);
                        _db.SaveChanges();

                        var existingPersetujuanDokumenList = _db.TR_PersetujuanDokumen
                            .Where(s => s.periode_dokumen_id == existingPeriodeDokumen.id && 
                                        (s.disetujui_untuk == "BM/SM" || s.disetujui_untuk == "ESRO Leader"))
                            .ToList();

                        foreach (var existingPersetujuanDokumen in existingPersetujuanDokumenList)
                        {
                            if (existingPersetujuanDokumen.disetujui_flag == "Konfirmasi Ulang")
                            {
                                existingPersetujuanDokumen.disetujui_flag = "Menunggu Persetujuan Ulang";
                                _db.TR_PersetujuanDokumen.Update(existingPersetujuanDokumen);
                            }
                        }

                        _db.SaveChanges();

                        await _pilarService.CreateDocumentApproval(kode_cabang, tahun_dokumen);
                    }

                    return RedirectToAction(nameof(MonografiWilayah), new { tahun_dokumen, kode_cabang });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return View(new { tahun_dokumen });
        }

        public IActionResult Monografi_Topografi()
        {
            return View();
        }

        public IActionResult Monografi_Penduduk()
        {
            return View();
        }

        public IActionResult Monografi_Pendidikan()
        {
            return View();
        }

        public IActionResult Monografi_Pekerjaan()
        {
            return View();
        }

        public IActionResult Monografi_SaranaDanPrasarana()
        {
            return View();
        }
    }
}