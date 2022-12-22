using System.ComponentModel.DataAnnotations.Schema;

namespace Phlog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; } = "portrait";

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? ImageFileName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
