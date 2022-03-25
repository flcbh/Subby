using System.Collections.Generic;
using Subby.Core.Entities;

namespace Subby.Web.New.Models.AdminViewModels
{
    public class DashboardViewModel
    {
        public decimal Revenue { get; set; }
        public int CountSubscriptions { get; set; }
        public int CountJobs { get; set; }
        public int CountNewJobs { get; set; }
        public int CountTrades { get; set; }
        public List<User> Users { get; set; }
        public int Sponsors { get; set; }
        public int Configurations { get; set; }
        public int Benefits { get; set; }
        public int Adverts { get; set; }
        public int AdvertCategories { get; set; }
    }
}