namespace CREDA_App.DTO
{
    public class DTOTransaction
    {
        public List<VW_CTR_KriteriaEkonomi> dto_ctr_ekonomi { get; set; }
        public class VW_CTR_KriteriaEkonomi
        {
             public string? kode_cabang { get; set; }
            public string? kode_kriteria { get; set; }
            public string? kriteria { get; set; }
            public int tahun_dokumen { get; set; }
            public int total_isian { get; set; }
            public int total_kriteria { get; set; }
        }
    }
}