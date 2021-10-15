using Microsoft.AspNetCore.Identity;

namespace API.Data.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; }
    }
}
