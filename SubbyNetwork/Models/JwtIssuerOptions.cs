using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubbyNetwork.Models
{
    public class JwtIssuerOptions
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public Func<string> JtiGenerator =>
            () => Guid.NewGuid().ToString();
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(300);
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    }
}
