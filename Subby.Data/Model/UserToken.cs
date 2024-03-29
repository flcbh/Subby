﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("UserToken", Schema = "subbynetwork")]
    public partial class UserToken
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Token { get; set; }
        [Column("User_id")]
        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserToken")]
        public virtual User User { get; set; }
    }
}