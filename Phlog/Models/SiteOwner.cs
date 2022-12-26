using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Phlog.Models
{
    public class SiteOwner : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? AboutMe { get; set; }

        public string? AvatarFileName { get; set; } = "default.png";

        [NotMapped]
        public IFormFile? AvatarImageFile { get; set; }

        public string? InstagramId { get; set; }
        public string? TwitterId { get; set; }
        public string? FaceBookId { get; set; }

    }
}
