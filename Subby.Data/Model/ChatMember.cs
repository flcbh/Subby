﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("ChatMember", Schema = "subbynetwork")]
    public partial class ChatMember
    {
        [Key]
        public int Id { get; set; }
        [Column("Channel_id")]
        public int? ChannelId { get; set; }
        [Column("User_id")]
        public int? UserId { get; set; }
        public bool? IsSeller { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsLeft { get; set; }

        [ForeignKey(nameof(ChannelId))]
        [InverseProperty(nameof(ChatChannel.ChatMember))]
        public virtual ChatChannel Channel { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("ChatMember")]
        public virtual User User { get; set; }
    }
}