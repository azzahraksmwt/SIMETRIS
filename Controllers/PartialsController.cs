using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CREDA_App.Data;
using CREDA_App.DTO;
using CREDA_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CREDA_App.Controllers
{
    public class PartialsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PartialsController(ApplicationDbContext db){
            _db = db;
        }

        [HttpGet]
        public IActionResult UbahStatus(int id)
        {
            ViewBag.Year = id;
            
            return View(new R_ApprovalSocialMapping());
        }

        [HttpPost]
        public IActionResult UbahStatus(R_ApprovalSocialMapping rApprovalSocialMapping)
        {
            rApprovalSocialMapping.kode_cabang = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_KODE_CABANG);
            rApprovalSocialMapping.esro_leader_date_approved = DateTime.Now;
            _db.R_ApprovalSocialMapping.Add(rApprovalSocialMapping);
            _db.SaveChanges();
            return RedirectToAction("MonografiWilayah", "PemetaanPilarMonografiWilayah");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}