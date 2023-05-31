using Microsoft.EntityFrameworkCore;
using MovieSystem.Data;

namespace MovieSystem
{
    public class Program 
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
            {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));

            builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseCors("corsapp");
            app.UseAuthorization();


            //Get all users
            app.MapGet("/Get/User", async (DataContext context) =>
            {
                var user = context.User;
                return await user.ToListAsync();
            })
           .WithName("GetUser");

            //Get genre for specific user
            app.MapGet("/Get/UserGenre/", async (int Id, DataContext context) =>
            {
                var userGenre = from x in context.UserGenre
                                select new
                                {
                                    x.User.Id,
                                    x.Genre.Title
                                };

                return await userGenre.Where(x => x.Id == Id).ToListAsync();
            })
            .WithName("/Get/GenrebyUserId");

            //Get movies for specific user
            app.MapGet("/Get/UserMovie/", async (int Id, DataContext context) =>
            {
                var userMovie = from x in context.UserGenre
                                select new
                                {
                                    x.User.Id,
                                    x.Movie
                                };

                return await userMovie.Where(x => x.Id == Id).ToListAsync();
            })
            .WithName("GetMoviebyUserId");

            //Get rating for specific user and movie
            app.MapGet("/Get/MoviesRating/", async (int Id, DataContext context) =>
            {
                var movieRating = from x in context.UserGenre
                                  select new
                                  {
                                      x.User.Id,
                                      x.Movie,
                                      x.Rating
                                  };

                return await movieRating.Where(x => x.Id == Id).ToListAsync();
            })
            .WithName("GetMovieRatingbyUserId");

            //Add/Update rating with userId and movie
            app.MapPost("/Post/AddRating/", async (DataContext context, int userId, int rating, string movie) =>
            {
                var updateRows = await context.UserGenre.Where(x => x.UserId == userId).Where(x => x.Movie == movie)
                .ExecuteUpdateAsync(updates =>
                updates.SetProperty(x => x.Rating, rating));

                return updateRows == 0 ? Results.NotFound() : Results.NoContent();
            })
                .WithName("PostRatingByUserIdAndMovieId");

            //Add movie to specific user and genre
            app.MapPost("/Post/AddMovie/", async (DataContext context, int userId, int genreId, string movie) =>
            {
                var respons = new Models.UserGenre
                {
                    UserId = userId,
                    Movie = movie,
                    GenreId = genreId
                };
                await context.UserGenre.AddAsync(respons);
                await context.SaveChangesAsync();
            })
                .WithName("PostMovieByUserIdAndMovieId");

            //Add genre to user
            app.MapPost("/Post/AddGenre/", async (DataContext context, int userId, int genreId) =>
            {

                var respons = new Models.UserGenre
                {
                    UserId = userId,
                    GenreId = genreId
                };
                await context.UserGenre.AddAsync(respons);
                await context.SaveChangesAsync();
                return respons;
            })
                .WithName("PostGenreAndUserbyUserIdAndGenreId");

            //Get Recomendations
            app.MapGet("/Get/Recommendations/", async (DataContext context, string genreTitle) =>
            {
                var genre = await context.Genre.FirstOrDefaultAsync(g => g.Title == genreTitle);

                var apiKey = "c57f1aec22d6876dc6f561c84046225c";
                var url = $"https://api.themoviedb.org/3/discover/movie?api_key={apiKey}&sort_by=popularity.desc&include_adult=true&include_video=false&with_genres={genre.Id}&with_watch_monetization_types=free";

                var client = new HttpClient();

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(content, contentType: "application/json");
            });

            app.Run();
        }
    }
}