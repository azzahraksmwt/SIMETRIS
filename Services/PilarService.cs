using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CREDA_App.Data;
using Microsoft.EntityFrameworkCore;
using CREDA_App.Models;
using Microsoft.AspNetCore.Mvc;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;
using Task = System.Threading.Tasks.Task;

namespace CREDA_App.Services
{
    public class PilarService
    {
        private readonly ApplicationDbContext _db;

        public PilarService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<object>> GetCombinedDataPilarAsync(string kode_cabang, int tahunDokumen)
        {
            // Ambil data kriteria dari M_IndikatorAllPilar
            var pilarKriteria = await _db.M_IndikatorAllPilar
                .GroupBy(x => x.pilar)
                .Select(g => new 
                {
                    Pilar = g.Key.Replace(" ", ""),
                    TotalKriteria = g.Count(),
                })
                .ToListAsync();

            var monografiDesaCount = await _db.M_MonografiDesa.CountAsync();

            // Ambil total isian dari berbagai tabel berdasarkan pilar
            var totalEkonomi = await _db.CTR_Ekonomi
                .Where(x => x.kode_cabang == kode_cabang && x.tahun_dokumen == tahunDokumen)
                .SumAsync(x => x.total_isian);

            var totalPendidikan = await _db.CTR_Pendidikan
                .Where(x => x.kode_cabang == kode_cabang && x.tahun_dokumen == tahunDokumen)
                .SumAsync(x => x.total_isian);

            var totalKesehatan = await _db.CTR_Kesehatan
                .Where(x => x.kode_cabang == kode_cabang && x.tahun_dokumen == tahunDokumen)
                .SumAsync(x => x.total_isian);

            var totalLingkungan = await _db.CTR_Lingkungan
                .Where(x => x.kode_cabang == kode_cabang && x.tahun_dokumen == tahunDokumen)
                .SumAsync(x => x.total_isian);

            var totalTanggapDarurat = await _db.CTR_TanggapDarurat
                .Where(x => x.kode_cabang == kode_cabang && x.tahun_dokumen == tahunDokumen)
                .SumAsync(x => x.total_isian);

            // Ambil data Monografi Wilayah
            var monografiWilayah = await _db.CTR_MonografiDesa
                .Where(v => v.kode_cabang == kode_cabang && v.tahun_dokumen == tahunDokumen)
                .Select(x => new 
                {
                    Pilar = "MonografiWilayah",
                    TotalMonografiIsian = x.total_isian,
                    TotalMonografiKategori = x.total_kategori
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            // Gabungkan semua data menjadi satu
            var combinedDataPilar = pilarKriteria.Select(k => new
            {
                Pilar = k.Pilar,
                TotalKriteria = k.TotalKriteria,
                TotalIsian = (k.Pilar == "Ekonomi" ? totalEkonomi : 0) +
                             (k.Pilar == "Pendidikan" ? totalPendidikan : 0) +
                             (k.Pilar == "Kesehatan" ? totalKesehatan : 0) +
                             (k.Pilar == "Lingkungan" ? totalLingkungan : 0) +
                             (k.Pilar == "TanggapDarurat" ? totalTanggapDarurat : 0)
            }).ToList();

            // Jika data Monografi Wilayah ditemukan, tambahkan ke hasil
            if (monografiWilayah != null)
            {
                combinedDataPilar.Add(new 
                {
                    Pilar = monografiWilayah.Pilar,
                    TotalKriteria = monografiWilayah.TotalMonografiKategori, 
                    TotalIsian = monografiWilayah.TotalMonografiIsian
                });
            }
            else
            {
                combinedDataPilar.Add(new
                {
                    Pilar = "MonografiWilayah",
                    TotalKriteria = monografiDesaCount,
                    TotalIsian = 0
                });
            }

            // Urutkan berdasarkan kode_pilar
            var urutanPilar = new[] { "MonografiWilayah", "Pendidikan", "Kesehatan", "Ekonomi", "Lingkungan", "TanggapDarurat" };
            combinedDataPilar = combinedDataPilar
                .OrderBy(k => Array.IndexOf(urutanPilar, k.Pilar))
                .ToList();

            // Kembalikan hasil sebagai List<object>
            return combinedDataPilar.Cast<object>().ToList();
        }

        public async Task CreateDocumentApproval(string kode_cabang, int tahunDokumen)
        {
            try {
            var periodeDokumen = await _db.TR_PeriodeDokumen
                .Where(pd => pd.kode_cabang == kode_cabang && pd.tahun_dokumen == tahunDokumen)
                .FirstOrDefaultAsync();

            if (periodeDokumen == null)
            {
                return;
            }

            var dataPilar = await GetCombinedDataPilarAsync(kode_cabang, tahunDokumen);
            
            bool isCompleted = dataPilar.All(item => 
            {
                dynamic pilar = item;
                return pilar.TotalIsian == pilar.TotalKriteria;
            });
            
            //    bool isExist = await _db.TR_PeriodeDokumen
            //         .Join(_db.TR_PersetujuanDokumen,
            //             periode => periode.id,
            //             persetujuan => persetujuan.periode_dokumen_id,
            //             (periode, persetujuan) => new { periode, persetujuan })
            //         .AnyAsync(x => x.periode.kode_cabang == kode_cabang);
        
            // await SendApprovalNotification("BM/SM", kode_cabang, tahunDokumen);

            if (isCompleted && periodeDokumen.status_disetujui == "Draft" || periodeDokumen.status_disetujui == "Sudah Dikonfirmasi")
            {
                var approvalRoles = new[] { "BM/SM", "ESRO Leader", "Dept Head CSR" };
                // var approvalEntries = approvalRoles.Select(role => new TR_PersetujuanDokumen
                // {
                //     periode_dokumen_id = periodeDokumen.id,
                //     disetujui_untuk = role,
                //     disetujui_flag = "Menunggu Persetujuan",
                //     created_at = DateTime.Now
                // }).ToList();

                // _db.TR_PersetujuanDokumen.AddRange(approvalEntries);

                if (periodeDokumen.status_disetujui == "Draft") {
                    periodeDokumen.status_disetujui = "Menunggu Persetujuan";
                    periodeDokumen.status_verifikasi = "Dokumen Belum Diverifikasi";
                    _db.TR_PeriodeDokumen.Update(periodeDokumen);
                }

                var initialRole = approvalRoles.First();
                await SendApprovalNotification(initialRole, kode_cabang, tahunDokumen);

                await _db.SaveChangesAsync();       
            }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                if(ex.InnerException != null){
                    Console.WriteLine(ex.InnerException.Message);
                }
            }
        }

        // public async Task SendApprovalNotification(string role, string kode_cabang, int tahunDokumen)
        // {
        //     // Dapatkan informasi pengguna berdasarkan peran
        //     var userEmails = await _db.User_Management
        //         .Where(u => u.role == role && u.kode_cabang == kode_cabang)
        //         .Select(u => u.email)
        //         .ToListAsync();

        //     // Konfigurasi pengiriman email
        //     // var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
        //     // request.Headers.Add("api-key", "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");
        //     // Configuration.Default.ApiKey.Add("api-key", "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");
        //     var configuration = new Configuration { ApiKey = { ["api-key"] = "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" } };
        //     var apiInstance = new TransactionalEmailsApi(configuration);
        //     string senderName = "CREDA";
        //     string senderEmail = "azzahta7070@gmail.com";

        //     // Ganti URL dasar ke localhost
        //     string baseUrl = "http://localhost:7147/MenuPemetaanSosial/DaftarPemetaanSosial"; // Sesuaikan port sesuai aplikasi Anda

        //     foreach (var email in userEmails)
        //     {
        //         try
        //         {
        //             // Buat URL dengan query parameter
        //             string approvalLink = $"{baseUrl}?kode_cabang={kode_cabang}&tahun={tahunDokumen}";

        //             // Konten email
        //             string subject = $"Persetujuan Dokumen Tahun {tahunDokumen} untuk {role}";
        //             string message = $@"
        //                 <p>Dokumen tahun {tahunDokumen} untuk cabang {kode_cabang} telah selesai.</p>
        //                 <p>Mohon untuk melakukan persetujuan melalui tautan berikut:</p>
        //                 <p><a href='{approvalLink}'>Klik di sini untuk melakukan persetujuan</a></p>
        //                 <p>Anda akan diminta untuk login sebelum mengakses halaman persetujuan.</p>";

        //             var emailSender = new SendSmtpEmailSender(senderName, senderEmail);
        //             var recipient = new SendSmtpEmailTo(email, role);
        //             var sendSmtpEmail = new SendSmtpEmail(emailSender, new List<SendSmtpEmailTo> { recipient }, null, null, message, null, subject);

        //             await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
        //         }
        //         catch (Exception ex)
        //         {
        //             Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
        //         }
        //     }
        // }

        public async Task SendApprovalNotification(string role, string kode_cabang, int tahunDokumen)
        {
            try
            {
                var users = new List<dynamic>();
                var cabang = await _db.M_Cabang
                    .Where(c => c.kode_cabang == kode_cabang)
                    .Select(c => c.nama_cabang)
                    .FirstOrDefaultAsync();

                if (role == "BM/SM")
                {
                    users = await _db.User_Management
                        .Where(u => u.role == role && u.kode_cabang == kode_cabang)
                        .Join(_db.M_Cabang,
                            u => u.kode_cabang,
                            c => c.kode_cabang,
                            (u, c) => new
                            {
                                u.email,
                                u.nama_karyawan,
                                nama_cabang = c.nama_cabang
                            })
                        .Cast<dynamic>()
                        .ToListAsync();

                    if (users.Count == 0)
                    {
                        Console.WriteLine($"Tidak ada email yang ditemukan untuk role {role} di cabang {kode_cabang}");
                        return;
                    }

                    string baseUrl = "http://localhost:7147/MenuPemetaanSosial/DaftarPemetaanSosial";
                    string loginUrl = "https://localhost:7147/";
                    
                    // foreach (var email in userEmails)
                    // {
                    //     string approvalLink = $"{baseUrl}?kode_cabang={kode_cabang}&tahun={tahunDokumen}";
                    //     string subject = $"Persetujuan Dokumen Tahun {tahunDokumen} untuk {role}";
                    //     string message = $@"
                    //         <p>Dokumen tahun {tahunDokumen} untuk cabang {kode_cabang} telah selesai.</p>
                    //         <p>Mohon untuk melakukan persetujuan melalui tautan berikut:</p>
                    //         <p><a href='{approvalLink}'>Klik di sini untuk melakukan persetujuan</a></p>
                    //         <p>Anda akan diminta untuk login sebelum mengakses halaman persetujuan.</p>";

                    //     await SendEmailAsync(email, role, subject, message);
                    // }
                
                    foreach (var user in users)
                    {
                        var sendSmtpEmail = new SendSmtpEmail()
                        {
                            To = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(user.email) },
                            TemplateId = 2,
                            Params = new Dictionary<string, string>
                            {
                                { "Nama", user.nama_karyawan },
                                { "Cabang", user.nama_cabang },
                                { "TahunDokumen", tahunDokumen.ToString() },
                                { "loginUrl", loginUrl } 
                            }
                        };

                        await SendEmailAsync(sendSmtpEmail);
                    }
                }
                else if (role == "ESRO Leader")
                {
                    users = await _db.User_Management
                        .Where(u => u.role == role)
                        .Select(u => new
                            {
                                u.email,
                                u.nama_karyawan
                            })
                        .Cast<dynamic>()
                        .ToListAsync();

                    if (users.Count == 0)
                    {
                        Console.WriteLine($"Tidak ada email yang ditemukan untuk role {role} di cabang {kode_cabang}");
                        return;
                    }

                    string loginUrl = "https://localhost:7147/";
                
                    foreach (var user in users)
                    {
                        var sendSmtpEmail = new SendSmtpEmail()
                        {
                            To = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(user.email) },
                            TemplateId = 4,
                            Params = new Dictionary<string, string>
                            {
                                { "Nama", user.nama_karyawan },
                                { "Cabang", cabang },
                                { "TahunDokumen", tahunDokumen.ToString() },
                                { "loginUrl", loginUrl } 
                            }
                        };

                        await SendEmailAsync(sendSmtpEmail);
                    }
                }
                else if (role == "Dept Head CSR")
                {
                    // Hitung jumlah cabang
                    var totalCabang = await _db.M_Cabang.CountAsync();

                    // Hitung jumlah dokumen yang sudah disetujui oleh ESRO Leader
                    var approvedByEsroLeader = await _db.TR_PeriodeDokumen
                        .Where(pd => pd.status_disetujui == "ESRO Leader" && pd.tahun_dokumen == tahunDokumen)
                        .CountAsync();

                    if (approvedByEsroLeader < totalCabang)
                    {
                        Console.WriteLine("Belum semua dokumen disetujui oleh ESRO Leader.");
                        return;
                    }

                    users = await _db.User_Management
                        .Where(u => u.role == role)
                        .Select(u => new
                            {
                                u.email,
                                u.nama_karyawan
                            })
                        .Cast<dynamic>()
                        .ToListAsync();

                    if (users.Count == 0)
                    {
                        Console.WriteLine($"Tidak ada email yang ditemukan untuk role {role} di cabang {kode_cabang}");
                        return;
                    }

                    string loginUrl = "https://localhost:7147/";
                
                    foreach (var user in users)
                    {
                        var sendSmtpEmail = new SendSmtpEmail()
                        {
                            To = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(user.email) },
                            TemplateId = 5,
                            Params = new Dictionary<string, string>
                            {
                                { "Nama", user.nama_karyawan },
                                { "TahunDokumen", tahunDokumen.ToString() },
                                { "loginUrl", loginUrl } 
                            }
                        };

                        await SendEmailAsync(sendSmtpEmail);
                    }
                }
                // Role CSR Local
                else if (role == "CSR Local")
                {
                    users = await _db.User_Management
                        .Where(u => u.role == "CSR Local" && u.kode_cabang == kode_cabang)
                        .Select(u => new
                            {
                                u.email,
                                u.nama_karyawan
                            })
                        .Cast<dynamic>()
                        .ToListAsync();

                    if (users.Count == 0)
                    {
                        Console.WriteLine($"Tidak ada email yang ditemukan untuk role {role} di cabang {kode_cabang}");
                        return;
                    }

                    string loginUrl = "https://localhost:7147/";

                    foreach (var user in users)
                    {
                        var sendSmtpEmail = new SendSmtpEmail()
                        {
                            To = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(user.email) },
                            TemplateId = 6,  // Gunakan template ID 6 untuk CSR Local
                            Params = new Dictionary<string, string>
                            {
                                { "Nama", user.nama_karyawan },
                                { "Cabang", cabang },
                                { "TahunDokumen", tahunDokumen.ToString() },
                                { "loginUrl", loginUrl }
                            }
                        };

                        await SendEmailAsync(sendSmtpEmail);
                    }
                }
                // Role CSR HO
                else if (role == "CSR HO")
                {
                    users = await _db.User_Management
                        .Where(u => u.role == "CSR HO" && u.kode_cabang == kode_cabang)
                        .Select(u => new
                            {
                                u.email,
                                u.nama_karyawan
                            })
                        .Cast<dynamic>()
                        .ToListAsync();

                    if (users.Count == 0)
                    {
                        Console.WriteLine($"Tidak ada email yang ditemukan untuk role {role}");
                        return;
                    }

                    string loginUrl = "https://localhost:7147/";

                    foreach (var user in users)
                    {
                        var sendSmtpEmail = new SendSmtpEmail()
                        {
                            To = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(user.email) },
                            TemplateId = 6,  // Gunakan template ID 7 untuk CSR HO
                            Params = new Dictionary<string, string>
                            {
                                { "Nama", user.nama_karyawan },
                                { "Cabang", cabang },
                                { "TahunDokumen", tahunDokumen.ToString() },
                                { "loginUrl", loginUrl }
                            }
                        };

                        await SendEmailAsync(sendSmtpEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendApprovalNotification for role {role}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
            }
        }

        private async Task SendEmailAsync( SendSmtpEmail sendSmtpEmail)
        {
            try
            {
                var configuration = new Configuration { ApiKey = { ["api-key"] = "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" } };
                var apiInstance = new TransactionalEmailsApi(configuration);

                var response = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);

                // Console.WriteLine($"Email sent to {email} with response: {response.Message}");
                // string senderName = "Program CSR";
                // string senderEmail = "azzahta7070@gmail.com";

                // var emailSender = new SendSmtpEmailSender(senderName, senderEmail);
                // var recipient = new SendSmtpEmailTo(email, role);
                // var sendSmtpEmail = new SendSmtpEmail(emailSender, new List<SendSmtpEmailTo> { recipient }, null, null, message, null, subject);

                // await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to");
            }
        }

        public async Task<dynamic> CekEditDokumenAsync(int tahun_dokumen)
        {
            var batasWaktu = await _db.TR_BatasWaktuPengisian
                .FirstOrDefaultAsync(b => b.tahun_dokumen == tahun_dokumen);

            var currentDate = DateTime.Now;

            if (currentDate >= batasWaktu.tanggal_mulai && currentDate <= batasWaktu.tanggal_berakhir)
            {
                return new { canEdit = true, message = "Dokumen dapat diedit." };
            }
            else
            {
                return new { canEdit = false, message = "Dokumen tidak dapat diedit karena melewati batas waktu." };
            }
        }
    }
}
