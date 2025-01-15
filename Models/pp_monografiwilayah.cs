using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CREDA_App.Models
{
    public class pp_monografiwilayah
    {
        [Key]
        public int id { get; set; }
        public string? id_karyawan { get; set; }
        public string? divisi { get; set; }
        public string? tpr_topografidesa { get; set; }
        public string? tpr_perkembangandesa { get; set; }
        public string? tpr_luaswilayah { get; set; }
        public string? tpr_wilayahutara { get; set; }
        public string? tpr_wilayahselatan { get; set; }
        public string? tpr_wilayahbarat { get; set; }
        public string? tpr_wilayahtimur { get; set; }
        public string? tpr_kecamatan { get; set; }
        public string? tpr_kabupaten { get; set; }
        public string? tpr_provinsi { get; set; }
        public string? pendu_usia014 { get; set; }
        public string? pendu_usia1565 { get; set; }
        public string? pendu_usia65 { get; set; }
        public string? pendu_islam { get; set; }
        public string? pendu_kristen { get; set; }
        public string? pendu_katolik { get; set; }
        public string? pendu_budha { get; set; }
        public string? pendu_konghucu { get; set; }
        public string? pendu_hindu { get; set; }
        public string? pendu_lainnya { get; set; }
        public string? pendu_pendudukmiskin { get; set; }
        public string? pendu_umr { get; set; }
        public string? pendi_tidaksekolah { get; set; }
        public string? pendi_sd { get; set; }
        public string? pendi_smp { get; set; }
        public string? pendi_smadansmk { get; set; }
        public string? pendi_diploma1 { get; set; }
        public string? pendi_diploma2 { get; set; }
        public string? pendi_diploma3 { get; set; }
        public string? pendi_diploma4ataustrata1 { get; set; }
        public string? pendi_strata2 { get; set; }
        public string? pendi_strata3 { get; set; }
        public string? peker_pns { get; set; }
        public string? peker_karyawanswasta { get; set; }
        public string? peker_karyawanbumn { get; set; }
        public string? peker_tni { get; set; }
        public string? peker_kepolisian { get; set; }
        public string? peker_wiraswasta { get; set; }
        public string? peker_pensiunan { get; set; }
        public string? peker_pelajarataumahasiswa { get; set; }
        public string? peker_buruh { get; set; }
        public string? peker_irt { get; set; }
        public string? peker_dosen { get; set; }
        public string? peker_guru { get; set; }
        public string? peker_lainlain { get; set; }
        public string? peker_tidakbekerja { get; set; }
        public string? sarana_administrasi_kantordesa { get; set; }
        public string? sarana_kesehatan_puskemas { get; set; }
        public string? sarana_kesehatan_pustu { get; set; }
        public string? sarana_kesehatan_poskesdes { get; set; }
        public string? sarana_kesehatan_ukbm { get; set; }
        public string? sarana_kesehatan_apotek { get; set; }
        public string? sarana_pendidikan_perpusdes { get; set; }
        public string? sarana_pendidikan_paud { get; set; }
        public string? sarana_pendidikan_tk { get; set; }
        public string? sarana_pendidikan_sd { get; set; }
        public string? sarana_pendidikan_smp { get; set; }
        public string? sarana_pendidikan_sma { get; set; }
        public string? sarana_ibadah_masjid { get; set; }
        public string? sarana_ibadah_mushola { get; set; }
        public string? sarana_ibadah_gereja { get; set; }
        public string? sarana_ibadah_pura { get; set; }
        public string? sarana_ibadah_wihara { get; set; }
        public string? sarana_ibadah_klenteng { get; set; }
        public string? sarana_umum_tempatolahraga { get; set; }
        public string? sarana_umum_tempatkesenian { get; set; }
        public string? sarana_umum_balaipertemuan { get; set; }
        public string? sarana_umum_mckumum { get; set; }
        public string? sarana_umum_pasar { get; set; }
        public string? sarana_umum_wificorner { get; set; }
        public string? sarana_keuangan_bank { get; set; }
        public string? sarana_keuangan_koperasisimpanpinjam { get; set; }
        public string? sarana_keuangan_lembagakeuangannonbank { get; set; }
        public string? sarana_keuangan_atm { get; set; }
        public string? sarana_keuangan_minimarket { get; set; }
        public string? sarana_keuangan_tokokelontong { get; set; }
        public string? sarana_keuangan_umkmdesa { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
   }
}