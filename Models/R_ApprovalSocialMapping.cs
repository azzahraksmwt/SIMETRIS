using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class R_ApprovalSocialMapping
    {
        [Key]
        public int id { get; set; }
        public string? kode_cabang { get; set; }
        public int? tahun_dokumen { get; set; }
        public string? esro_leader_flag { get; set; }
        public DateTime? esro_leader_date_approved { get; set; }
        public string? esro_leader_keterangan { get; set; }
    }
}