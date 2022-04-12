﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("JobApplication", Schema = "subbynetwork")]
    public partial class JobApplication
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        [Column(TypeName = "decimal(19, 5)")]
        public decimal? Cost { get; set; }
        public bool? Accepted { get; set; }
        public bool? Read { get; set; }
        public bool? IsQualify { get; set; }
        public bool? Approved { get; set; }
        [StringLength(255)]
        public string RejectionReason { get; set; }
        [StringLength(255)]
        public string File { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [Column("Job_id")]
        public int? JobId { get; set; }
        [Column("User_id")]
        public int? UserId { get; set; }
        [Column("Applicant_id")]
        public int? ApplicantId { get; set; }
        public bool? EligibleToWorkInUk { get; set; }
        public bool? EligibleToWorkInEu { get; set; }
        [StringLength(255)]
        public string Firstname { get; set; }
        [StringLength(255)]
        public string Lastname { get; set; }
        [StringLength(255)]
        public string Phone { get; set; }
        [StringLength(255)]
        public string Email { get; set; }
        public bool? Rejected { get; set; }
        [StringLength(255)]
        public string Quote { get; set; }
        [StringLength(255)]
        public string Estimation { get; set; }

        [ForeignKey(nameof(ApplicantId))]
        [InverseProperty("JobApplicationApplicant")]
        public virtual User Applicant { get; set; }
        [ForeignKey(nameof(JobId))]
        [InverseProperty("JobApplication")]
        public virtual Job Job { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("JobApplicationUser")]
        public virtual User User { get; set; }
    }
}