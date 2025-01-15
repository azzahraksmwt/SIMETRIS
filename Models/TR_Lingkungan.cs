using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class TR_Lingkungan
    {
        [Key]
        public int id { get; set; }
        public string kode_cabang { get; set; }
        public string kode_kriteria { get; set; }
        public int no_indikator { get; set; }
        public string indikator { get; set; }
        public double? isian { get; set; }
        public int? tahun_data { get; set; }
        public string? sumber_data { get; set; }
        public int? tahun_dokumen { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}