using System;

namespace SubbyNetwork.Extensions
{
    public static class RatingCalculation
    {
        public static double GetRating(int star1, int star2, int star3, int star4, int star5)
        {
            double rating = (double)(5 * star5 + 4 * star4 + 3 * star3 + 2 * star2 + 1 * star1) /
                            (star1 + star2 + star3 + star4 + star5);
            rating = Math.Round(rating, 1);
            return double.IsNaN(rating) ? 0 : rating;
        }
    }
}