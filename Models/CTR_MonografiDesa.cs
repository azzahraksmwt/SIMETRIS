using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class CTR_MonografiDesa
    {
        public string? kode_cabang { get; set; }
        public int tahun_dokumen { get; set; }
        public int total_kategori { get; set; }
        public int total_isian { get; set; }
    }
}