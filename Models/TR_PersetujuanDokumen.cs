using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class TR_PersetujuanDokumen
    {
        [Key]
        public int id { get; set; }
        public int? periode_dokumen_id { get; set; }
        public string? disetujui_untuk { get; set; }
        public string? disetujui_flag { get; set; }
        public string? disetujui_oleh { get; set; }
        public DateTime? tanggal_disetujui { get; set; }
        public string? keterangan_disetujui { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}