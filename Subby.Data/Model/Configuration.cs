﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("Configuration", Schema = "subbynetwork")]
    public partial class Configuration
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Key { get; set; }
        [StringLength(255)]
        public string Value { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
    }
}