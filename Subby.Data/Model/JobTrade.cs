﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("JobTrade", Schema = "subbynetwork")]
    public partial class JobTrade
    {
        [Key]
        public int Id { get; set; }
        [Column("Job_id")]
        public int? JobId { get; set; }
        [Column("Trade_id")]
        public int? TradeId { get; set; }

        [ForeignKey(nameof(JobId))]
        [InverseProperty("JobTrade")]
        public virtual Job Job { get; set; }
        [ForeignKey(nameof(TradeId))]
        [InverseProperty("JobTrade")]
        public virtual Trade Trade { get; set; }
    }
}