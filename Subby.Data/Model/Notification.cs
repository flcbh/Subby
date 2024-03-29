﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("Notification", Schema = "subbynetwork")]
    public partial class Notification
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Type { get; set; }
        [StringLength(255)]
        public string Value { get; set; }
        public bool? Read { get; set; }
        public DateTime? CreatedAt { get; set; }
        [Column("User_id")]
        public int? UserId { get; set; }
        public DateTime? ReadAt { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("Notification")]
        public virtual User User { get; set; }
    }
}