using System;
using System.ComponentModel.DataAnnotations;
using Subby.Core.Models.Auth;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class JobApplication : BaseEntity
    {
        public virtual Job Job { get; set; }
        public virtual User Applicant { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string Lastname { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Email { get; set; }
        
        public virtual string Quote { get; set; }
        public virtual string Estimation { get; set; }
        public virtual bool Accepted { get; set; }
        public virtual bool Read { get; set; }
        public virtual bool Rejected { get; set; }
        public virtual string RejectionReason { get; set; }
        public virtual string File { get; set; }
        [Display(Name = "Eligible to work in the UK")]
        public virtual bool EligibleToWorkInUk { get; set; }
        [Display(Name = "Eligible to work in the EU")]
        public virtual bool EligibleToWorkInEu { get; set; }
        public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}