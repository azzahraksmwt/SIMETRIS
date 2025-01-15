using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class M_IndikatorAllPilar
    {
        [Key]
        public int no_indikator { get; set; }
        public int no_program { get; set; }
        public string program { get; set; }
        public string kode_pilar { get; set; }
        public string pilar { get; set; }
        public int no_kriteria { get; set; }
        public string kode_kriteria { get; set; }
        public string kriteria { get; set; }
        public string indikator { get; set; }
        public string jenis_isian_indikator { get; set; }
        public string? deskripsi { get; set; }
        public string? DTO_kode_cabang { get; set; }
        public double? DTO_isian { get; set; }
        public int DTO_tahun_data { get; set; }
        public string? DTO_sumber_data { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime deleted_at { get; set; }
    }
}