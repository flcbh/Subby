﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("UserReview", Schema = "subbynetwork")]
    public partial class UserReview
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Review { get; set; }
        public DateTime? DateCompleted { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? Tidiness { get; set; }
        public int? Reliability { get; set; }
        public int? Courtesy { get; set; }
        [Column("User_id")]
        public int? UserId { get; set; }
        [Column("Job_id")]
        public int? JobId { get; set; }
        [Column("Reviewer_id")]
        public int? ReviewerId { get; set; }

        [ForeignKey(nameof(JobId))]
        [InverseProperty("UserReview")]
        public virtual Job Job { get; set; }
        [ForeignKey(nameof(ReviewerId))]
        [InverseProperty("UserReviewReviewer")]
        public virtual User Reviewer { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserReviewUser")]
        public virtual User User { get; set; }
    }
}