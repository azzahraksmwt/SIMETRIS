using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class TR_BatasWaktuPengisian
    {
        [Key]
        public int id { get; set; }
        public int? tahun_dokumen { get; set; }
        public DateTime? tanggal_mulai { get; set; }
        public DateTime? tanggal_berakhir { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}