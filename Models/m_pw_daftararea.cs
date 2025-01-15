using System.ComponentModel.DataAnnotations;

namespace CREDA_App.Models
{
    public class m_pw_daftararea
    {
        [Key]
        public int id { get; set; }
        public string kode_cabang { get; set; }
        public string detail_lokasi { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }

        public m_pw_daftararea()
        {
            if (kode_cabang == null)
            {
                kode_cabang = "";
            }
            if (detail_lokasi == null)
            {
                detail_lokasi = "";
            }
        }
    }
}