using System.ComponentModel.DataAnnotations;

namespace CREDA_App.Models
{
    public class m_pw_pemetaanarea
    {
        [Key]
        public int id { get; set; }
        public int id_daftar_area { get; set; }
        public string sub_area { get; set; }
        public string detail_lokasi { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }

        public m_pw_pemetaanarea()
        {
            if (sub_area == null)
            {
                sub_area = "";
            }
            if (detail_lokasi == null)
            {
                detail_lokasi = "";
            }
        }
    }
}