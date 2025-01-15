using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CREDA_App.Models;
using CREDA_App.Data;
using System.Security.Claims;
using CREDA_App.DTO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;
using System.Net.Http.Headers;

namespace CREDA_App.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public HomeController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public IActionResult Index()
    {
        ViewBag.M_Cabang = _db.M_Cabang.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(User_Management model)
    {
        try
        {
            var user = _db.User_Management.SingleOrDefault(u => u.id_karyawan == model.id_karyawan && u.password == model.password);

            if (user != null)
            {
                HttpContext.Session.SetString("User", user.ToString());

                var claims = new List<Claim>();

                if (user.id_karyawan != null)
                {
                    String id_karyawan = user.id_karyawan;
                    if (!String.IsNullOrEmpty(id_karyawan))
                    {
                        Claim idKaryawanClaim = new Claim(DTOApplication.ClaimConstant.CLAIM_ID_KARYAWAN, id_karyawan);
                        claims.Add(idKaryawanClaim);
                    }
                }

                if (user.kode_cabang != null)
                {
                    String kode_cabang = user.kode_cabang;
                    if (!String.IsNullOrEmpty(kode_cabang))
                    {
                        Claim kodeCabangClaim = new Claim(DTOApplication.ClaimConstant.CLAIM_KODE_CABANG, kode_cabang);
                        claims.Add(kodeCabangClaim);
                    }
                }

                if (user.nama_karyawan != null)
                {
                    String nama_karyawan = user.nama_karyawan;
                    if (!String.IsNullOrEmpty(nama_karyawan))
                    {
                        Claim namaKaryawanClaim = new Claim(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN, nama_karyawan);
                        claims.Add(namaKaryawanClaim);
                    }
                }

                if (user.email != null)
                {
                    String email = user.email;
                    if (!String.IsNullOrEmpty(email))
                    {
                        Claim emailClaim = new Claim(DTOApplication.ClaimConstant.CLAIM_EMAIL, email);
                        claims.Add(emailClaim);
                    }
                }

                if (user.password != null)
                {
                    String password = user.password;
                    if (!String.IsNullOrEmpty(password))
                    {
                        Claim passwordClaim = new Claim(DTOApplication.ClaimConstant.CLAIM_PASSWORD, password);
                        claims.Add(passwordClaim);
                    }
                }

                if (user.jenis_kelamin != null)
                {
                    String jenis_kelamin = user.jenis_kelamin;
                    if (!String.IsNullOrEmpty(jenis_kelamin))
                    {
                        Claim jenisKelaminClaim = new Claim(DTOApplication.ClaimConstant.CLAIM_JENIS_KELAMIN, jenis_kelamin);
                        claims.Add(jenisKelaminClaim);
                    }
                }

                if (user.alamat != null)
                {
                    String alamat = user.alamat;
                    if (!String.IsNullOrEmpty(alamat))
                    {
                        Claim alamatClaim = new Claim(DTOApplication.ClaimConstant.CLAIM_ALAMAT, alamat);
                        claims.Add(alamatClaim);
                    }
                }

                if (user.no_telepon != null)
                {
                    String no_telepon = user.no_telepon;
                    if (!String.IsNullOrEmpty(no_telepon))
                    {
                        Claim noTeleponClaim = new Claim(DTOApplication.ClaimConstant.CLAIM_NO_TELEPON, no_telepon);
                        claims.Add(noTeleponClaim);
                    }
                }

                if (user.status != null)
                {
                    String status = user.status;
                    if (!String.IsNullOrEmpty(status))
                    {
                        Claim statusClaim = new Claim(DTOApplication.ClaimConstant.CLAIM_STATUS, status);
                        claims.Add(statusClaim);
                    }
                }

                if (user.role != null)
                {
                    String role = user.role;
                    if (!String.IsNullOrEmpty(role))
                    {
                        Claim roleClaim = new Claim(DTOApplication.ClaimConstant.CLAIM_ROLE, role);
                        claims.Add(roleClaim);
                    }
                }

                if (user.divisi != null)
                {
                    String divisi = user.divisi;
                    if (!String.IsNullOrEmpty(divisi))
                    {
                        Claim divisiClaim = new Claim(DTOApplication.ClaimConstant.CLAIM_DIVISI, divisi);
                        claims.Add(divisiClaim);
                    }
                }

                var fotoProfil = user.nama_file_gambar;
                fotoProfil = string.IsNullOrEmpty(fotoProfil) ? "user-avatar.jpg" : fotoProfil;
                HttpContext.Session.SetString("Foto", fotoProfil);

                var claimsIdentity = new ClaimsIdentity(claims, "Login");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Beranda");
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public IActionResult Register(User_Management usermanagement)
    {
        if (ModelState.IsValid)
        {
            try
            {
                usermanagement.status = "Aktif";
                usermanagement.role = "User";
                usermanagement.created_at = DateTime.Now;
                _db.User_Management.Add(usermanagement);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
        return View("Index", usermanagement);
    }

    [Authorize]
    public IActionResult Beranda(int selectedYear, string pilarLine, string pilarMap, string selectedCabang)
    {
        var kode_cabang = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("KodeCabang"))?.Value ?? "-";
        var role = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_ROLE);

        int tahunDokumen = 0;
        string linePilar = string.IsNullOrEmpty(pilarLine) ? "Total Skor" : pilarLine;
        string mapPilar = string.IsNullOrEmpty(pilarMap) ? "Total Skor" : pilarMap;

        List<int> tahunList;
        if (role == "BM/SM" || role == "CSR Local" || role == "CSR HO") 
        {
            var exists = _db.VW_DashboardMap.Any(x => x.kode_cabang == kode_cabang);

            if (exists)
            {
            tahunList = _db.VW_DashboardMap
                .Where(x => kode_cabang == kode_cabang)
                .Select(x => x.tahun_dokumen)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();
            }
            else
            {
                tahunList = new List<int>();
            }
        } else {
            tahunList = _db.VW_DashboardMap
                .Select(x => x.tahun_dokumen)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();
        }

        if (tahunList.Count > 0) 
        {
            tahunDokumen = (selectedYear == 0) ? DateTime.Now.Year : selectedYear;
        }
        else
        {
            tahunDokumen = 0; 
        }

        var cabangList = _db.VW_DashboardMap
            .Select(x => x.kode_cabang)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        string cabangTerpilih = cabangList != null && cabangList.Any()
            ? string.IsNullOrEmpty(selectedCabang) ? "ADR" : selectedCabang
            : "ADR"; 

        string kodeCabang = role == "BM/SM" || role == "CSR Local" || role == "CSR HO" ? kode_cabang : cabangTerpilih;

        ViewBag.pilarList = _db.VW_DashboardMap
            .Select(x => x.pilar)
            .OrderBy(x => x)
            .Distinct()
            .ToArray();
        
        // MONOGRAFI WILAYAH
        var monografiData = new List<VW_TR_MonografiDesa>(); // Default empty list
        if (_db.VW_DashboardMap.Any(x => x.kode_cabang == kodeCabang && x.tahun_dokumen == tahunDokumen))
        {
            monografiData = _db.VW_TR_MonografiDesa
                .Where(x => x.kode_cabang == kodeCabang && x.tahun_dokumen == tahunDokumen)
                .ToList();
        }

        var kategoriDict = monografiData.ToDictionary(x => x.kategori, x => x.isian);

        ViewBag.luasWilayah = kategoriDict.TryGetValue("Luas Wilayah", out var luasWilayah) ? luasWilayah : "-";
        ViewBag.topografiDesa = kategoriDict.TryGetValue("Topografi Desa", out var topografiDesa) ? topografiDesa : "-";
        ViewBag.tingkatPerkembangan = kategoriDict.TryGetValue("Tingkat Perkembangan Desa", out var tingkatPerkembangan) ? tingkatPerkembangan : "-";
        ViewBag.jumlahPenduduk = kategoriDict.TryGetValue("Jumlah Penduduk", out var jumlahPenduduk) ? jumlahPenduduk : "-";
        ViewBag.jumlahPendudukMiskin = kategoriDict.TryGetValue("Jumlah Penduduk Miskin", out var jumlahPendudukMiskin) ? jumlahPendudukMiskin : "-";
        ViewBag.umrKabupatenKota = kategoriDict.TryGetValue("UMR Kabupaten/Kota", out var umrKabupatenKota) ? umrKabupatenKota : "-";

        bool isAdmin = User.IsInRole("Admin");

        List<dynamic> barDataResult;

        if (isAdmin)
        {
            // Query untuk Admin: tanpa filter kode_cabang dan melakukan sum per pilar dan tahun_dokumen, kemudian dibagi jumlah cabang
            var adminDashboard = _db.VW_DashboardMap
                .Where(x => x.tahun_dokumen == tahunDokumen && x.pilar != "Total Skor");

            int jumlahCabang = adminDashboard
                .Select(x => x.kode_cabang)
                .Distinct()
                .Count();

            barDataResult = adminDashboard
                .GroupBy(x => new { x.pilar, x.tahun_dokumen })
                .Select(g => new
                {
                    Pilar = g.Key.pilar,
                    TahunDokumen = g.Key.tahun_dokumen,
                    Jumlah = g.Sum(x => x.jumlah) / (double)jumlahCabang // Membagi sum dengan jumlah cabang
                })
                .ToList<dynamic>();
        }
        else
        {
            // Query untuk non-Admin: filter berdasarkan kode_cabang
            var barDashboard = _db.VW_DashboardMap
                .Where(x => x.kode_cabang == kodeCabang && x.tahun_dokumen == tahunDokumen && x.pilar != "Total Skor")
                .ToList();

            barDataResult = barDashboard
                .Select(x => new
                {
                    Pilar = x.pilar,
                    TahunDokumen = x.tahun_dokumen,
                    Jumlah = x.jumlah
                })
                .ToList<dynamic>();
        }

        // Menambahkan data default jika barDataResult kosong
        var defaultPilar = new List<string> { "Pendidikan", "Kesehatan", "Ekonomi", "Lingkungan", "Tanggap Darurat" };

        if (barDataResult == null || !barDataResult.Any())
        {
            barDataResult = defaultPilar
                .Select(pilar => new
                {
                    Pilar = pilar,
                    TahunDokumen = tahunDokumen,
                    Jumlah = 0
                })
                .ToList<dynamic>();
        }

        // Menyimpan data ke ViewBag
        ViewBag.barData = new
        {
            Labels = barDataResult.Select(x => x.Pilar).ToArray(),
            DataValues = barDataResult.Select(x => x.Jumlah).ToArray()
        };

        // GAUGE CHART
        var gaugeDashboard = _db.VW_DashboardMap
            .Where(x => x.kode_cabang == kodeCabang && x.tahun_dokumen == tahunDokumen 
            && x.pilar == "Total Skor")
            .ToList();

        ViewBag.gaugeData = new 
        {
            Labels = gaugeDashboard.Select(x => x.prioritas).FirstOrDefault(),
            DataValues = gaugeDashboard.Select(x => x.jumlah).FirstOrDefault()
        };

        // MAP
        ViewBag.dashboardMap = _db.VW_DashboardMap
            .Where(x => x.tahun_dokumen == tahunDokumen && x.pilar == mapPilar)
            .ToList();

        // LINE CHART
        var lineDashboard = _db.VW_DashboardMap
            .Where(x => x.kode_cabang == kodeCabang && x.pilar == linePilar)
            .OrderBy(x => x.tahun_dokumen)
            .ToList();

        int defaultCount = 5; // Jumlah data default yang diinginkan
        var defaultLabels = Enumerable.Repeat("", defaultCount).ToList(); // Labels default dalam format string
        var defaultValues = Enumerable.Repeat(0, defaultCount).ToList(); // Nilai default

        var lineData = new
        {
            Labels = lineDashboard.Any()
                ? lineDashboard.Select(x => x.tahun_dokumen.ToString()).ToArray() // Convert tahun_dokumen ke string
                : defaultLabels.ToArray(),
            DataValues = lineDashboard.Any()
                ? lineDashboard.Select(x => x.jumlah).ToArray()
                : defaultValues.ToArray()
        };

        ViewBag.lineData = lineData;

        ViewBag.selectedYear = tahunDokumen;
        ViewBag.PilarLine = linePilar;
        ViewBag.PilarMap = mapPilar;
        ViewBag.TahunList = tahunList;
        ViewBag.CabangList = cabangList;

        ViewBag.namaCabang = _db.M_Cabang
            .Where(c => c.kode_cabang == kodeCabang)
            .Select(c => c.nama_cabang)
            .FirstOrDefault();

        if (User.Identity.IsAuthenticated)
        {
            var idKaryawanClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("IdKaryawan"));

            if (idKaryawanClaim != null)
            {
                ViewBag.IdKaryawan = idKaryawanClaim.Value;
                ViewBag.NamaKaryawan = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("NamaKaryawan")).Value;
                ViewBag.Email = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("Email")).Value;
                ViewBag.KodeCabang = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("KodeCabang"))?.Value ?? "-";
                ViewBag.Role = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("Role")).Value;

                return View();
            }
        }
        ViewBag.ErrorMessage = "Anda tidak sepenuhnya terotentikasi. Silakan login untuk mengakses halaman ini.";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Profil()
    {
        var idKaryawan = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("IdKaryawan")).Value;
        var kodeCabang = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("KodeCabang"))?.Value ?? "-";

        var userData = await _db.User_Management
            .Where(s => s.id_karyawan == idKaryawan)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        ViewBag.Cabang = _db.M_Cabang
            .Where(c => c.kode_cabang == kodeCabang)
            .FirstOrDefault();

        return View(userData);
    }

    public async Task<IActionResult> UbahProfil()
    {
        var idKaryawan = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("IdKaryawan")).Value;
        var kodeCabang = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("KodeCabang"))?.Value ?? "-";

        var userData = await _db.User_Management
            .Where(s => s.id_karyawan == idKaryawan)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        ViewBag.Cabang = _db.M_Cabang
            .Where(c => c.kode_cabang == kodeCabang)
            .FirstOrDefault();

        return View(userData);
    }

    [HttpPost]
    public async Task<IActionResult> UbahProfil([FromForm] DTOApplication.GetUser getUser, int id)
    {
        string physicalPath;
        
        var fileContent = ContentDispositionHeaderValue.Parse(getUser.file.ContentDisposition);
        string webRootPath = _env.WebRootPath;

        var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
        var extension = Path.GetExtension(getUser.file.FileName);
        var actualFileName = $"{DateTime.Now.ToString("yyMMddmmss")}_{fileName}{extension}";
        physicalPath = Path.Combine(webRootPath, "FotoProfil", actualFileName);

        using (var fileStream = new FileStream(physicalPath, FileMode.Create))
        {
            await getUser.file.CopyToAsync(fileStream);
        }

        FileStream fs = new FileStream(physicalPath, FileMode.Open, FileAccess.Read);
        fs.Close();

        var dataProfil = _db.User_Management.FirstOrDefault(x => x.id == id);

        if (dataProfil != null)
        {
            dataProfil.nama_file_gambar = actualFileName;
            dataProfil.nama_tampilan_gambar = fileName;
            dataProfil.updated_at = DateTime.Now;
            dataProfil.updated_by = User.FindFirstValue(DTOApplication.ClaimConstant.CLAIM_NAMA_KARYAWAN);
            _db.User_Management.Update(dataProfil);  
            _db.SaveChanges(); 
        };
          
        return RedirectToAction("Profil", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        Response.Cookies.Delete(".AspNetCore.Session");
        Response.Cookies.Delete(".AspNetCore.Cookies");
        return RedirectToAction(nameof(Index));
    }
}
