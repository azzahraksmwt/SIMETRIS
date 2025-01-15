namespace CREDA_App.DTO
{
    public class DTOApplication
    {
        public class ClaimConstant
        {
            public const String CLAIM_ID_KARYAWAN = "IdKaryawan";
            public const String CLAIM_KODE_CABANG = "KodeCabang";
            public const String CLAIM_NAMA_CABANG = "NamaCabang";
            public const String CLAIM_NAMA_KARYAWAN = "NamaKaryawan";
            public const String CLAIM_EMAIL = "Email";
            public const String CLAIM_PASSWORD = "Password";
            public const String CLAIM_JENIS_KELAMIN = "JenisKelamin";
            public const String CLAIM_ALAMAT = "Alamat";
            public const String CLAIM_NO_TELEPON = "NoTelepon";
            public const String CLAIM_STATUS = "Status";
            public const String CLAIM_ROLE = "Role";
            public const String CLAIM_DIVISI = "Divisi";
        }

        public class GetUser
        {
            public int id { get; set; }
            public string id_karyawan { get; set; }
            public string kode_cabang { get; set; }
            public string nama_cabang { get; set; }
            public string nama_karyawan { get; set; }
            public string password { get; set; }
            public string email { get; set; }
            public string jenis_kelamin { get; set; }
            public string alamat { get; set; }
            public string no_telepon { get; set; }
            public string role { get; set; }
            public string divisi { get; set; }
            public string status { get; set;}
            public IFormFile file { get; set; }
        }
    }
}