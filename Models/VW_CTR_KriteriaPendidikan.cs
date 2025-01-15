using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class VW_CTR_KriteriaPendidikan
    {
        public string? kode_cabang { get; set; }
        public string? kode_kriteria { get; set; }
        public string? kriteria { get; set; }
        public int tahun_dokumen { get; set; }
        public int total_isian { get; set; }
        public int total_kriteria { get; set; }
    }
}