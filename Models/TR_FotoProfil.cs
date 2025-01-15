using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CREDA_App.Models
{
    public class TR_FotoProfil
    {
        [Key]
        public int id { get; set; }
        public string? ActualName { get; set; }
        public string? DisplayName { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}