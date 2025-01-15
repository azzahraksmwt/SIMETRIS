using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class M_SC_Pendidikan
    {
        [Key]
        public int id { get; set; }
        public string keterangan_nilai { get; set; }
        public int total_indikator { get; set; }
        public int no_indikator_1 { get; set; }
        public int no_indikator_2 { get; set; }
        public int no_indikator_3 { get; set; }
        public int formula_group { get; set; }
        public string formula_excel { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime deleted_at { get; set; }
    }
}