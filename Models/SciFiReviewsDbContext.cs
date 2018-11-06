using Microsoft.EntityFrameworkCore;
using SciFiReviewsApi.Models.EntityModels;

namespace SciFiReviewsApi.Models
{
    public class SciFiReviewsDbContext : DbContext
    {
        public SciFiReviewsDbContext(DbContextOptions<SciFiReviewsDbContext> options) :
            base(options)
        {

        }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>()
                .Property<string>("PeopleWhoLikedReviewDb")
                .HasField("_peopleWhoLikedReview");

            modelBuilder.Entity<Reviewer>()
                .Property<string>("LikedReviewsDb")
                .HasField("_likedReviews");
        }
    }
}
