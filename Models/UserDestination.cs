using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class UserDestination
    {
        public string UserId { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;

        public int DestinationId { get; set; }
        public Destination Destination { get; set; } = null!;
    }
}
