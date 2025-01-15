using System.ComponentModel.DataAnnotations;

namespace CREDA_App.Models
{
    public class User_Management
    {
        [Key]
        public int id { get; set; }
        public string? id_karyawan { get; set; }
        public string? kode_cabang { get; set; }
        public string? nama_karyawan { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? jenis_kelamin { get; set; }
        public string? alamat { get; set; }
        public string? no_telepon { get; set; }
        public string? status { get; set; }
        public string? role { get; set; }
        public string? divisi { get; set; }
        public string? nama_file_gambar { get; set; }
        public string? nama_tampilan_gambar { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; } = DateTime.Now;
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}