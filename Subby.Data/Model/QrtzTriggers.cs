﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subby.Data
{
    [Table("QRTZ_TRIGGERS")]
    [Index(nameof(SchedName), nameof(CalendarName), Name = "IDX_QRTZ_T_C")]
    [Index(nameof(SchedName), nameof(TriggerGroup), Name = "IDX_QRTZ_T_G")]
    [Index(nameof(SchedName), nameof(JobName), nameof(JobGroup), Name = "IDX_QRTZ_T_J")]
    [Index(nameof(SchedName), nameof(JobGroup), Name = "IDX_QRTZ_T_JG")]
    [Index(nameof(SchedName), nameof(NextFireTime), Name = "IDX_QRTZ_T_NEXT_FIRE_TIME")]
    [Index(nameof(SchedName), nameof(MisfireInstr), nameof(NextFireTime), Name = "IDX_QRTZ_T_NFT_MISFIRE")]
    [Index(nameof(SchedName), nameof(TriggerState), nameof(NextFireTime), Name = "IDX_QRTZ_T_NFT_ST")]
    [Index(nameof(SchedName), nameof(MisfireInstr), nameof(NextFireTime), nameof(TriggerState), Name = "IDX_QRTZ_T_NFT_ST_MISFIRE")]
    [Index(nameof(SchedName), nameof(MisfireInstr), nameof(NextFireTime), nameof(TriggerGroup), nameof(TriggerState), Name = "IDX_QRTZ_T_NFT_ST_MISFIRE_GRP")]
    [Index(nameof(SchedName), nameof(TriggerGroup), nameof(TriggerState), Name = "IDX_QRTZ_T_N_G_STATE")]
    [Index(nameof(SchedName), nameof(TriggerName), nameof(TriggerGroup), nameof(TriggerState), Name = "IDX_QRTZ_T_N_STATE")]
    [Index(nameof(SchedName), nameof(TriggerState), Name = "IDX_QRTZ_T_STATE")]
    public partial class QrtzTriggers
    {
        [Key]
        [Column("SCHED_NAME")]
        [StringLength(120)]
        public string SchedName { get; set; }
        [Key]
        [Column("TRIGGER_NAME")]
        [StringLength(150)]
        public string TriggerName { get; set; }
        [Key]
        [Column("TRIGGER_GROUP")]
        [StringLength(150)]
        public string TriggerGroup { get; set; }
        [Required]
        [Column("JOB_NAME")]
        [StringLength(150)]
        public string JobName { get; set; }
        [Required]
        [Column("JOB_GROUP")]
        [StringLength(150)]
        public string JobGroup { get; set; }
        [Column("DESCRIPTION")]
        [StringLength(250)]
        public string Description { get; set; }
        [Column("NEXT_FIRE_TIME")]
        public long? NextFireTime { get; set; }
        [Column("PREV_FIRE_TIME")]
        public long? PrevFireTime { get; set; }
        [Column("PRIORITY")]
        public int? Priority { get; set; }
        [Required]
        [Column("TRIGGER_STATE")]
        [StringLength(16)]
        public string TriggerState { get; set; }
        [Required]
        [Column("TRIGGER_TYPE")]
        [StringLength(8)]
        public string TriggerType { get; set; }
        [Column("START_TIME")]
        public long StartTime { get; set; }
        [Column("END_TIME")]
        public long? EndTime { get; set; }
        [Column("CALENDAR_NAME")]
        [StringLength(200)]
        public string CalendarName { get; set; }
        [Column("MISFIRE_INSTR")]
        public int? MisfireInstr { get; set; }
        [Column("JOB_DATA")]
        public byte[] JobData { get; set; }

        [ForeignKey("SchedName,JobName,JobGroup")]
        [InverseProperty("QrtzTriggers")]
        public virtual QrtzJobDetails QrtzJobDetails { get; set; }
        [InverseProperty("QrtzTriggers")]
        public virtual QrtzCronTriggers QrtzCronTriggers { get; set; }
        [InverseProperty("QrtzTriggers")]
        public virtual QrtzSimpleTriggers QrtzSimpleTriggers { get; set; }
        [InverseProperty("QrtzTriggers")]
        public virtual QrtzSimpropTriggers QrtzSimpropTriggers { get; set; }
    }
}