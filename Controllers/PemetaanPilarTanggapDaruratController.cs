using System.Security.Claims;
using CREDA_App.Data;
using CREDA_App.DTO;
using CREDA_App.Models;
using CREDA_App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CREDA_App.Controllers
{
    public class PemetaanPilarTanggapDaruratController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly PilarService _pilarService;
        public PemetaanPilarTanggapDaruratController(ApplicationDbContext db, PilarService pilarService)
        {
            _db = db;
            _pilarService = pilarService;
        }
        
        [HttpGet]
        public async Task<IActionResult> PilarTanggapDarurat(int tahun_dokumen, string kode_cabang)
        {
            int tahunDokumen = (tahun_dokumen > 0) ? tahun_dokumen : DateTime.Now.Year;

            var getApproval = _db.R_ApprovalSocialMapping
            .Where(x =>  x.tahun_dokumen == tahunDokumen)
            .FirstOrDefault();
            
            if (getApproval != null) {
                ViewBag.approvalFLag = true;
            } else {
                ViewBag.approvalFLag = false;
            }

            var tahunList = _db.TR_TanggapDarurat
                .Select(x => x.tahun_dokumen)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();

            var ctrData = await _db.CTR_TanggapDarurat
                .Where(x => x.kode_cabang == kode_cabang && x.tahun_dokumen == tahunDokumen)
                .Select(x => new { x.kode_kriteria, x.total_isian, x.total_kriteria, x.kriteria })
                .ToListAsync();

            var vwData = await _db.VW_KriteriaTanggapDarurat
                .Select(x => new { x.kode_kriteria, x.total_kriteria, x.kriteria })
                .ToListAsync();

            var combinedData = vwData.Select(vwItem =>
            {
                var ctrItem = ctrData.FirstOrDefault(c => c.kode_kriteria == vwItem.kode_kriteria);
                return new 
                {
                    KodeKriteria = vwItem.kode_kriteria,
                    Kriteria = ctrItem?.kriteria ?? vwItem.kriteria,
                    TotalIsian = ctrItem?.total_isian ?? 0,
                    TotalKriteria = ctrItem?.total_kriteria ?? vwItem.total_kriteria
                };
            }).ToList();

            ViewBag.TanggapDaruratData = combinedData;

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
            ViewBag.SelectedYear = tahunDokumen;
            ViewBag.TahunList = tahunList;

            var result = await _pilarService.CekEditDokumenAsync(tahun_dokumen);
            ViewBag.CanEdit = result.canEdit;

            await _pilarService.CreateDocumentApproval(kode_cabang, tahunDokumen);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DetailPilarTanggapDarurat(string kriteria, string kode_cabang, int tahun_dokumen)
        {
            string _kode_kriteria = kriteria.Trim();

            var indikator = await _db.VW_M_IndikatorAllPilar
                                    .Where(s => s.kode_kriteria == _kode_kriteria)
                                    .AsNoTracking()
                                    .ToListAsync();

            for(int i = 0; i < indikator.Count; i++){
                var transaksiTanggapDarurat = await _db.TR_TanggapDarurat.Where(s => s.no_indikator == indikator[i].no_indikator 
                && s.kode_cabang == kode_cabang && s.tahun_dokumen == tahun_dokumen).AsNoTracking().ToListAsync();
                if(transaksiTanggapDarurat.Count != 0){
                    indikator[i].DTO_isian = transaksiTanggapDarurat.FirstOrDefault().isian;
                    indikator[i].DTO_tahun_data = transaksiTanggapDarurat.FirstOrDefault().tahun_data;
                    indikator[i].DTO_sumber_data = transaksiTanggapDarurat.FirstOrDefault().sumber_data;
                }
            }

            var result = await _pilarService.CekEditDokumenAsync(tahun_dokumen);
            ViewBag.CanEdit = result.canEdit;
            ViewBag.SelectedYear = tahun_dokumen; 
            ViewBag.KodeCabang = kode_cabang;

            return View(indikator);
        }

        [HttpPost]
        public IActionResult DetailPilarTanggapDarurat(int id, List<VW_M_IndikatorAllPilar> Indikator, int tahun_dokumen, string kode_cabang)
        {
            try
            {
                var namaKaryawan = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("NamaKaryawan")).Value;
                // var kodeCabang = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("KodeCabang")).Value;
                var isEditable = "No";
                
                if (!ModelState.IsValid)
                {
                    for(var i = 0; i < Indikator.Count(); i++){
                        var transaksiExist = _db.TR_TanggapDarurat
                                    .Where(s => s.no_indikator == Indikator[i].no_indikator 
                                    && s.kode_cabang == kode_cabang && s.tahun_dokumen == tahun_dokumen)
                                    .AsNoTracking()
                                    .FirstOrDefault();

                        if (id == 0 && transaksiExist == null) {
                            if(Indikator[i].DTO_isian !=null){
                                var Pilar = new TR_TanggapDarurat(){
                                    isian = Indikator[i].DTO_isian,
                                    tahun_data = Indikator[i].DTO_tahun_data,
                                    sumber_data = Indikator[i].DTO_sumber_data,
                                    indikator = Indikator[i].indikator,
                                    kode_kriteria = Indikator[i].kode_kriteria,
                                    no_indikator = Indikator[i].no_indikator,
                                    created_at = DateTime.Now,
                                    kode_cabang = kode_cabang,
                                    tahun_dokumen = tahun_dokumen
                                };
                                _db.TR_TanggapDarurat.Add(Pilar);
                                _db.SaveChanges();
                            }
                        } else {
                            var existingPilar = _db.TR_TanggapDarurat.FirstOrDefault(
                                s => s.no_indikator == Indikator[i].no_indikator &&
                                    s.kode_cabang == kode_cabang && s.tahun_dokumen == tahun_dokumen);
                            
                            if (existingPilar != null)
                            {
                                existingPilar.isian = Indikator[i].DTO_isian;
                                existingPilar.tahun_data = Indikator[i].DTO_tahun_data;
                                existingPilar.sumber_data = Indikator[i].DTO_sumber_data;
                                existingPilar.updated_at = DateTime.Now;
                                existingPilar.updated_by = namaKaryawan;

                                _db.TR_TanggapDarurat.Update(existingPilar);
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

                        var existingPersetujuanDokumen = _db.TR_PersetujuanDokumen.FirstOrDefault(
                            s => s.periode_dokumen_id == existingPeriodeDokumen.id && s.disetujui_untuk == "BM/SM");

                        if (existingPersetujuanDokumen.disetujui_flag == "Konfirmasi Ulang") {
                            existingPersetujuanDokumen.disetujui_flag = "Menunggu Persetujuan Ulang";
                            _db.TR_PersetujuanDokumen.Update(existingPersetujuanDokumen);
                            _db.SaveChanges();
                        }
                    }

                    return RedirectToAction(nameof(PilarTanggapDarurat), new { tahun_dokumen, kode_cabang });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction(nameof(PilarTanggapDarurat), new { tahun_dokumen });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}