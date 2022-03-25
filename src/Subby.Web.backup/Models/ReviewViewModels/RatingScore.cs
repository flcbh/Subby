namespace Subby.Web.Models.ReviewViewModels
{
    public class RatingScore
    {
        public double Tidiness { get; set; }
        public double Courtesy { get; set; }
        public double Reliability { get; set; }
        public double Overall { get; set; }
        public int TotalReviews { get; set; }
    }
}