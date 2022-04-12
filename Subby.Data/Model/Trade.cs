﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("Trade", Schema = "subbynetwork")]
    public partial class Trade
    {
        public Trade()
        {
            Job = new HashSet<Job>();
            JobTrade = new HashSet<JobTrade>();
            UserTrade = new HashSet<UserTrade>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string Slug { get; set; }
        public bool? Active { get; set; }

        [InverseProperty("TradeId1Navigation")]
        public virtual ICollection<Job> Job { get; set; }
        [InverseProperty("Trade")]
        public virtual ICollection<JobTrade> JobTrade { get; set; }
        [InverseProperty("Trade")]
        public virtual ICollection<UserTrade> UserTrade { get; set; }
    }
}