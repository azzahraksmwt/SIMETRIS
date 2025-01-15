using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class VW_DashboardMap
    {
        public string? kode_cabang { get; set; }
        public string? nama_cabang { get; set; }
        public string pilar { get; set; }
        public int jumlah { get; set; }
        public string prioritas { get; set; }
        public int tahun_dokumen { get; set; }
        public string? latitude { get; set; }
        public string? longitude { get; set; }
    }
}