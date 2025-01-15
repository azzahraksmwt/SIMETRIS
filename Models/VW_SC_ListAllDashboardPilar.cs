using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class VW_SC_ListAllDashboardPilar
    {
        public string kode_cabang { get; set; }
        public string nama_cabang { get; set; }
        public int jumlah_pendidikan { get; set; }
        public int jumlah_kesehatan { get; set; }
        public int jumlah_ekonomi { get; set; }
        public int jumlah_lingkungan { get; set; }
        public int jumlah_tanggapdarurat { get; set; }
        public int total_skor { get; set; }
        public string prioritas { get; set; }
    }
}