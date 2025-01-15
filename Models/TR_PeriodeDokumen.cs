using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class TR_PeriodeDokumen
    {
        [Key]
        public int id { get; set; }
        public string? kode_cabang { get; set; }
        public int? tahun_dokumen { get; set; }
        public string? status_disetujui { get; set; }
        public string? status_verifikasi { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}