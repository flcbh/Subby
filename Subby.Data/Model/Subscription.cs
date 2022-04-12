﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("Subscription", Schema = "subbynetwork")]
    public partial class Subscription
    {
        public Subscription()
        {
            TransactionNavigation = new HashSet<Transaction>();
        }

        [Key]
        public int Id { get; set; }
        [Column(TypeName = "decimal(19, 5)")]
        public decimal? Amount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        [Column("User_id")]
        public int? UserId { get; set; }
        [Column("Transaction_id")]
        public int? TransactionId { get; set; }
        [StringLength(255)]
        public string Plan { get; set; }

        [ForeignKey(nameof(TransactionId))]
        [InverseProperty("Subscription")]
        public virtual Transaction Transaction { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("Subscription")]
        public virtual User User { get; set; }
        [InverseProperty("SubscriptionNavigation")]
        public virtual ICollection<Transaction> TransactionNavigation { get; set; }
    }
}