using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class VW_TR_Ekonomi
    {
        public int id { get; set; }
        public string kode_kriteria { get; set; }
        public string kode_cabang { get; set; }
        public int? tahun_dokumen { get; set; }
        public int? total_isian { get; set; }
    }
}