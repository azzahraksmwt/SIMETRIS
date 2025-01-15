using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class TR_StatusDokumen
    {
        [Key]
        public int id { get; set; }
        public string? kode_cabang { get; set; }
        public int? tahun_dokumen { get; set; }
        public string? bm_sm_flag { get; set; }
        public DateTime? bm_sm_tanggal_disetujui { get; set; }
        public string? bm_sm_keterangan { get; set; }
        public string? esro_leader_flag { get; set; }
        public DateTime? esro_leader_tanggal_disetujui { get; set; }
        public string? esro_leader_keterangan { get; set; }
        public string? dept_head_csr_flag { get; set; }
        public DateTime? dept_head_csr_tanggal_disetujui { get; set; }
        public string? dept_head_csr_keterangan { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}