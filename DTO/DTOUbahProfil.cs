namespace CREDA_App.DTO
{
    public class DTOUbahProfil
    {
        public int id_karyawan { get; set; }
        public string nama_karyawan { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string kode_cabang { get; set; }
        public IFormFile file { get; set; }
    }
}