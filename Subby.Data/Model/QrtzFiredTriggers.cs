﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("QRTZ_FIRED_TRIGGERS")]
    [Index(nameof(SchedName), nameof(InstanceName), nameof(RequestsRecovery), Name = "IDX_QRTZ_FT_INST_JOB_REQ_RCVRY")]
    [Index(nameof(SchedName), nameof(JobGroup), Name = "IDX_QRTZ_FT_JG")]
    [Index(nameof(SchedName), nameof(JobName), nameof(JobGroup), Name = "IDX_QRTZ_FT_J_G")]
    [Index(nameof(SchedName), nameof(TriggerGroup), Name = "IDX_QRTZ_FT_TG")]
    [Index(nameof(SchedName), nameof(InstanceName), Name = "IDX_QRTZ_FT_TRIG_INST_NAME")]
    [Index(nameof(SchedName), nameof(TriggerName), nameof(TriggerGroup), Name = "IDX_QRTZ_FT_T_G")]
    public partial class QrtzFiredTriggers
    {
        [Key]
        [Column("SCHED_NAME")]
        [StringLength(120)]
        public string SchedName { get; set; }
        [Key]
        [Column("ENTRY_ID")]
        [StringLength(140)]
        public string EntryId { get; set; }
        [Required]
        [Column("TRIGGER_NAME")]
        [StringLength(150)]
        public string TriggerName { get; set; }
        [Required]
        [Column("TRIGGER_GROUP")]
        [StringLength(150)]
        public string TriggerGroup { get; set; }
        [Required]
        [Column("INSTANCE_NAME")]
        [StringLength(200)]
        public string InstanceName { get; set; }
        [Column("FIRED_TIME")]
        public long FiredTime { get; set; }
        [Column("SCHED_TIME")]
        public long SchedTime { get; set; }
        [Column("PRIORITY")]
        public int Priority { get; set; }
        [Required]
        [Column("STATE")]
        [StringLength(16)]
        public string State { get; set; }
        [Column("JOB_NAME")]
        [StringLength(150)]
        public string JobName { get; set; }
        [Column("JOB_GROUP")]
        [StringLength(150)]
        public string JobGroup { get; set; }
        [Column("IS_NONCONCURRENT")]
        public bool? IsNonconcurrent { get; set; }
        [Column("REQUESTS_RECOVERY")]
        public bool? RequestsRecovery { get; set; }
    }
}