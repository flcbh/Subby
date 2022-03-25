using System;

namespace Subby.Core.Models.Auth
{
    public class AuthResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string Scheme { get; set; }
        public bool IsSandbox { get; set; }
        public DateTime? Expires { get; set; }
    }
}