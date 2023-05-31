using System.ComponentModel.DataAnnotations;

namespace MovieSystem.Models
{
    public class UserGenre
    {
        [Key]
        public int Id { get; set; }

        public string? Movie { get; set; }

        public int? Rating { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        //public List<UserGenreMovie> UserGenresMovies { get; set; }
    }
}
