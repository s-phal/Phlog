using System.ComponentModel.DataAnnotations.Schema;

namespace Phlog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Category { get; set; } = "portrait";
        public string? InstagramUsername { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? ImageFileName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
