using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class M_MonografiDesa
    {
        [Key]
        public int id { get; set; }
        public string kode_monografi { get; set; }
        public string monografi { get; set; }
        public string kode_kategori { get; set; }
        public string kategori { get; set; }
        public string? DTO_isian { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}