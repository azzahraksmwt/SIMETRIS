using CREDA_App.Models;
using Microsoft.EntityFrameworkCore;

namespace CREDA_App.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<pp_monografiwilayah> pp_monografiwilayah { get; set; }
        public DbSet<m_pw_daftararea> m_pw_daftararea { get; set; }
        public DbSet<m_pw_pemetaanarea> m_pw_pemetaanarea { get; set; }
        public DbSet<User_Management> User_Management { get; set; }
        public DbSet<M_Cabang>? M_Cabang { get; set; }
        public DbSet<TR_FotoProfil> TR_FotoProfil { get; set; }
        public DbSet<TR_MonografiDesa> TR_MonografiDesa { get; set; }
        public DbSet<TR_Ekonomi> TR_Ekonomi { get; set; }
        public DbSet<TR_Kesehatan> TR_Kesehatan { get; set; }
        public DbSet<TR_Lingkungan> TR_Lingkungan { get; set; }
        public DbSet<TR_Pendidikan> TR_Pendidikan { get; set; }
        public DbSet<TR_TanggapDarurat> TR_TanggapDarurat { get; set; }
        public DbSet<TR_StatusDokumen> TR_StatusDokumen { get; set; }
        public DbSet<TR_PeriodeDokumen> TR_PeriodeDokumen { get; set; }
        public DbSet<TR_PersetujuanDokumen> TR_PersetujuanDokumen { get; set; }
        public DbSet<TR_BatasWaktuPengisian> TR_BatasWaktuPengisian { get; set; }
        public DbSet<R_ApprovalSocialMapping> R_ApprovalSocialMapping { get; set; }
        public DbSet<M_IndikatorAllPilar>? M_IndikatorAllPilar { get; set; }
        public DbSet<M_MonografiDesa>? M_MonografiDesa { get; set; }
        public DbSet<M_SC_Ekonomi> M_SC_Ekonomi { get; set; }
        public DbSet<M_SC_Kesehatan> M_SC_Kesehatan { get; set; }
        public DbSet<M_SC_Lingkungan> M_SC_Lingkungan { get; set; }
        public DbSet<M_SC_Pendidikan> M_SC_Pendidikan { get; set; }
        public DbSet<M_SC_TanggapDarurat> M_SC_TanggapDarurat { get; set; }
        public DbSet<CTR_Ekonomi> CTR_Ekonomi { get; set; }
        public DbSet<CTR_Kesehatan> CTR_Kesehatan { get; set; }
        public DbSet<CTR_Pendidikan> CTR_Pendidikan { get; set; }
        public DbSet<CTR_Lingkungan> CTR_Lingkungan { get; set; }
        public DbSet<CTR_TanggapDarurat> CTR_TanggapDarurat { get; set; }
        public DbSet<VW_KriteriaEkonomi> VW_KriteriaEkonomi { get; set; }
        public DbSet<VW_KriteriaLingkungan> VW_KriteriaLingkungan { get; set; }
        public DbSet<VW_KriteriaPendidikan> VW_KriteriaPendidikan { get; set; }
        public DbSet<VW_KriteriaKesehatan> VW_KriteriaKesehatan { get; set; }
        public DbSet<VW_KriteriaTanggapDarurat> VW_KriteriaTanggapDarurat { get; set; }
        public DbSet<CTR_MonografiDesa> CTR_MonografiDesa { get; set; }
        public DbSet<CTR_KriteriaPendidikan> CTR_KriteriaPendidikan { get; set; }
        public DbSet<CTR_KriteriaKesehatan> CTR_KriteriaKesehatan { get; set; }
        public DbSet<CTR_KriteriaEkonomi> CTR_KriteriaEkonomi { get; set; }
        public DbSet<CTR_KriteriaLingkungan> CTR_KriteriaLingkungan { get; set; }
        public DbSet<CTR_KriteriaTanggapDarurat> CTR_KriteriaTanggapDarurat { get; set; }
        public DbSet<VW_CTR_KriteriaPendidikan> VW_CTR_KriteriaPendidikan { get; set; }
        public DbSet<VW_CTR_KriteriaKesehatan> VW_CTR_KriteriaKesehatan { get; set; }
        public DbSet<VW_CTR_KriteriaEkonomi> VW_CTR_KriteriaEkonomi { get; set; }
        public DbSet<VW_CTR_KriteriaLingkungan> VW_CTR_KriteriaLingkungan { get; set; }
        public DbSet<VW_CTR_KriteriaTanggapDarurat> VW_CTR_KriteriaTanggapDarurat { get; set; }
        public DbSet<VW_M_IndikatorAllPilar> VW_M_IndikatorAllPilar { get; set; }
        public DbSet<VW_DashboardMap> VW_DashboardMap { get; set; }
        public DbSet<VW_SC_AllDashboardPilar> VW_SC_AllDashboardPilar { get; set; }
        public DbSet<VW_SC_AllDashboardPilarTotal> VW_SC_AllDashboardPilarTotal { get; set; }
        public DbSet<VW_SC_ListAllDashboardPilar> VW_SC_ListAllDashboardPilar { get; set; }
        public DbSet<VW_CTR_AllPilar> VW_CTR_AllPilar { get; set; }
        public DbSet<VW_TR_MonografiDesa> VW_TR_MonografiDesa { get; set; }
        public DbSet<VW_TR_Pendidikan> VW_TR_Pendidikan { get; set; }
        public DbSet<VW_TR_Kesehatan> VW_TR_Kesehatan { get; set; }
        public DbSet<VW_TR_Ekonomi> VW_TR_Ekonomi { get; set; }
        public DbSet<VW_TR_Lingkungan> VW_TR_Lingkungan { get; set; }
        public DbSet<VW_TR_TanggapDarurat> VW_TR_TanggapDarurat { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                modelBuilder.Entity<VW_CTR_KriteriaPendidikan>(c=> {
                    c.HasNoKey();
                    c.ToView("VW_CTR_KriteriaPendidikan");
                });

                modelBuilder.Entity<VW_CTR_KriteriaKesehatan>(c=> {
                    c.HasNoKey();
                    c.ToView("VW_CTR_KriteriaKesehatan");
                });

                modelBuilder.Entity<VW_CTR_KriteriaEkonomi>(c=> {
                    c.HasNoKey();
                    c.ToView("VW_CTR_KriteriaEkonomi");
                });

                modelBuilder.Entity<VW_CTR_KriteriaLingkungan>(c=> {
                    c.HasNoKey();
                    c.ToView("VW_CTR_KriteriaLingkungan");
                });

                modelBuilder.Entity<VW_CTR_KriteriaTanggapDarurat>(c=> {
                    c.HasNoKey();
                    c.ToView("VW_CTR_KriteriaTanggapDarurat");
                });

                modelBuilder.Entity<VW_M_IndikatorAllPilar>(c=> {
                    c.HasNoKey();
                    c.ToView("VW_M_IndikatorAllPilar");
                });

                modelBuilder.Entity<M_Cabang>(c=> {
                    c.ToTable("M_Cabang");
                });

                modelBuilder.Entity<VW_DashboardMap>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_DashboardMap");
                });

                modelBuilder.Entity<VW_SC_AllDashboardPilar>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_SC_AllDashboardPilar");
                });

                modelBuilder.Entity<VW_SC_AllDashboardPilarTotal>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_SC_AllDashboardPilarTotal");
                });

                modelBuilder.Entity<VW_SC_ListAllDashboardPilar>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_SC_ListAllDashboardPilar");
                });

                modelBuilder.Entity<VW_CTR_AllPilar>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_CTR_AllPilar");
                });

                modelBuilder.Entity<VW_KriteriaEkonomi>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_KriteriaEkonomi");
                });

                modelBuilder.Entity<VW_KriteriaLingkungan>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_KriteriaLingkungan");
                });

                modelBuilder.Entity<VW_KriteriaKesehatan>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_KriteriaKesehatan");
                });

                modelBuilder.Entity<VW_KriteriaPendidikan>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_KriteriaPendidikan");
                });

                modelBuilder.Entity<VW_KriteriaTanggapDarurat>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_KriteriaTanggapDarurat");
                });
                
                modelBuilder.Entity<CTR_Ekonomi>(c=> {
                    c.HasNoKey();
                    c.ToTable("CTR_Ekonomi");
                });

                modelBuilder.Entity<CTR_Kesehatan>(c=> {
                    c.HasNoKey();
                    c.ToTable("CTR_Kesehatan");
                });

                modelBuilder.Entity<CTR_Pendidikan>(c=> {
                    c.HasNoKey();
                    c.ToTable("CTR_Pendidikan");
                });

                modelBuilder.Entity<CTR_Lingkungan>(c=> {
                    c.HasNoKey();
                    c.ToTable("CTR_Lingkungan");
                });

                modelBuilder.Entity<CTR_TanggapDarurat>(c=> {
                    c.HasNoKey();
                    c.ToTable("CTR_TanggapDarurat");
                });
                
                modelBuilder.Entity<CTR_MonografiDesa>(c=> {
                    c.HasNoKey();
                    c.ToTable("CTR_MonografiDesa");
                });

                modelBuilder.Entity<CTR_KriteriaPendidikan>(c=> {
                    c.HasNoKey();
                    c.ToTable("CTR_KriteriaPendidikan");
                });

                modelBuilder.Entity<CTR_KriteriaKesehatan>(c=> {
                    c.HasNoKey();
                    c.ToTable("CTR_KriteriaKesehatan");
                });

                modelBuilder.Entity<CTR_KriteriaEkonomi>(c=> {
                    c.HasNoKey();
                    c.ToTable("CTR_KriteriaEkonomi");
                });

                modelBuilder.Entity<CTR_KriteriaLingkungan>(c=> {
                    c.HasNoKey();
                    c.ToTable("CTR_KriteriaLingkungan");
                });

                modelBuilder.Entity<CTR_KriteriaTanggapDarurat>(c=> {
                    c.HasNoKey();
                    c.ToTable("CTR_KriteriaTanggapDarurat");
                });

                modelBuilder.Entity<VW_TR_MonografiDesa>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_TR_MonografiDesa");
                });
                modelBuilder.Entity<VW_TR_Ekonomi>(c=> {
                    c.HasNoKey();
                    c.ToTable("VW_TR_Ekonomi");
                });
        }
    }
}
