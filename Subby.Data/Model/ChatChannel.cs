﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("ChatChannel", Schema = "subbynetwork")]
    public partial class ChatChannel
    {
        public ChatChannel()
        {
            Chat = new HashSet<Chat>();
            ChatMember = new HashSet<ChatMember>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Title { get; set; }
        [Column("Advert_id")]
        public int? AdvertId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Active { get; set; }

        [ForeignKey(nameof(AdvertId))]
        [InverseProperty("ChatChannel")]
        public virtual Advert Advert { get; set; }
        [InverseProperty("Channel")]
        public virtual ICollection<Chat> Chat { get; set; }
        [InverseProperty("Channel")]
        public virtual ICollection<ChatMember> ChatMember { get; set; }
    }
}