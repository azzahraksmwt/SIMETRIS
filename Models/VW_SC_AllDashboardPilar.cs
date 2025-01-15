using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class VW_SC_AllDashboardPilar
    {
        public int no_pilar { get; set; }
        public string pilar { get; set; }
        public string kode_cabang { get; set; }
        public int jumlah { get; set; }
        public string prioritas { get; set; }
        public int? tahun_dokumen { get; set; }
    }
}