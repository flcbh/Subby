using System;

namespace Subby.Core.Models.Job
{
    public class JobModel
    {
        public string Avatar { get; set; }
        public string FirstName { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string ContractType { get; set; }
        public int Reviews { get; set; }
        public string Budget { get; set; }
        public int Id { get; set; }
        public int ViewCount { get; set; }
        public int Applications { get; set; }
        public bool IsFilled { get; set; }
    }
}