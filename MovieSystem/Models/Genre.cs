using System.ComponentModel.DataAnnotations;

namespace MovieSystem.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }

        public List<UserGenre> UserGenres { get; set; }
    }
}
