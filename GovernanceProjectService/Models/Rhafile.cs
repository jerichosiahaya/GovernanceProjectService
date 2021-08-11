using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace GovernanceProjectService.Models
{
    public partial class Rhafile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }
        [Required]
        public string SubKondisi { get; set; }
        [Required]
        public string Kondisi { get; set; }
        [Required]
        public string Rekomendasi { get; set; }
        public string TindakLanjut { get; set; }
        public DateTime? TargetDate { get; set; }
        [Required]
        public string Assign { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
