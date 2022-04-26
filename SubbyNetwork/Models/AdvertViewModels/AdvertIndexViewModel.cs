using LastContent.Utilities.Pagination;
using Subby.Data;
using System.Linq;
using System.Security.Claims;

namespace SubbyNetwork.Models.AdvertViewModels
{
    public class AdvertIndexViewModel<Advert>
    {
        SubbynetworkContext db;

        public AdvertIndexViewModel()
        {
            db = new SubbynetworkContext();
        }

        public ICollection<AdvertCategory> Categories { get; set; }
        public int CategoryId { get; set; }
        public IList<Advert> Items { get; set; }
        public bool IsFree { get; set; }
        public bool IsSold { get; set; }
        public int Messages { get; set; }
        public int Miles { get; set; }
        public List<Media> MediaCollection { get; set; }


        public int CountItems()
        {
            return Items.Count;
        }

        public AdvertIndexViewModel<Advert> ReturnData(AdvertIndexViewModel<Advert> model)
        {
            //var user = db.User.FirstOrDefault(x => x.Email == System.Security.Claims.ClaimsPrincipal.Current.Identity.Name);
            model.Items = (IList<Advert>)db.Advert.Where(a => a.IsSold == model.IsSold && a.IsFree == a.IsFree).ToList();

            if (model.Miles > 0)
            {
                //var postcode = user.TradePostcode;
                ////var longitude = Convert.ToDouble(user.Longitude);
                ////var latitude = Convert.ToDouble(user.Latitude);

                //if (Convert.ToInt32(longitude) != 0 && Convert.ToInt32(latitude) != 0)
                //{
                //    model.Items = (IList<Advert>)db.Advert.Where(x =>
                //        3958.75 * Math.Acos(Math.Sin((double)latitude / 57.2958) *
                //        Math.Sin((double)x.Latitude / 57.2958) +
                //        Math.Cos((double)latitude / 57.2958) *
                //        Math.Cos((double)x.Latitude / 57.2958) *
                //        Math.Cos((double)x.Longitude / 57.2958 - (double)longitude / 57.2958)) <= model.Miles
                //    );
                //}
                //else
                //{
                //    model.Items = (IList<Advert>)Enumerable.Empty<Advert>().AsQueryable();
                //}
            }

            return model;
        }
    }
}