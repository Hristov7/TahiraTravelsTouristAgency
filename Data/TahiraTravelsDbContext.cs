using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class TahiraTravelsDbContext : IdentityDbContext
    {
        public TahiraTravelsDbContext(DbContextOptions options) : base(options)
        {
        }

        protected TahiraTravelsDbContext()
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Destination> Destinations { get; set; }
        public virtual DbSet<UserDestination> UsersDestinations { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<TourGuide> TourGuides { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserDestination>()
                .HasKey(ud => new { ud.UserId, ud.DestinationId });

            builder.Entity<UserDestination>()
                .HasOne(ur => ur.Destination)
                .WithMany(r => r.UsersDestinations)
                .HasForeignKey(ur => ur.DestinationId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Booking>()
                .HasOne(b=>b.Tour)
                .WithMany(t=>t.Bookings)
                .HasForeignKey(b=>b.TourId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Review>()
                .HasOne(b=>b.Tour)
                .WithMany()
                .HasForeignKey(b=>b.TourId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<TourGuide>()
                .HasOne(b => b.Tour)
                .WithMany(g => g.TourGuides)
                .HasForeignKey(b => b.TourId)
                .OnDelete(DeleteBehavior.NoAction);

            var defaultUser = new IdentityUser
            {
                Id = "df1c3a0f-1234-4cde-bb55-d5f15a6aabcd",
                UserName = "admin@tahiratravels.com",
                NormalizedUserName = "ADMIN@TAHIRATRAVELS.COM",
                Email = "admin@tahiratravels.com",
                NormalizedEmail = "ADMIN@TAHIRATRAVELS.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(
                    new IdentityUser { UserName = "admin@tahiratravels.com" },
                    "Admin123!")
            };
            builder.Entity<IdentityUser>().HasData(defaultUser);

            builder.Entity<Category>().HasData(
                new Category {Id = 1, Name = "Beach Escapes" },
                new Category {Id = 2, Name = "Mountain Adventures" },
                new Category {Id = 3, Name = "City Breaks" },
                new Category {Id = 4, Name = "Desert Safaris" },
                new Category {Id = 5, Name = "Island Retreats" },
                new Category {Id = 6, Name = "Countryside Retreats" }
            );

            builder.Entity<Destination>().HasData(
                new Destination
                {
                    Id = 1,
                    Name = "Sunny Beach",
                    Description = "A beautiful beach with golden sands and clear waters.",
                    ImageUrl = "https://sunnybeach-guide.com/wp-content/uploads/2024/03/sunny-beach-main-2.jpg",
                    AuthorId = "df1c3a0f-1234-4cde-bb55-d5f15a6aabcd",
                    CreatedOn = DateTime.Now,
                    CategoryId = 1
                },
                new Destination
                {
                    Id = 2,
                    Name = "Musala",
                    Description = "A breathtaking mountain peak with stunning views.",
                    ImageUrl = "https://www.thetraveler.bg/wp-content/uploads/2021/01/Musala-6.jpg",
                    AuthorId = "df1c3a0f-1234-4cde-bb55-d5f15a6aabcd",
                    CreatedOn = DateTime.Now,
                    CategoryId = 2
                },
                new Destination
                {
                    Id = 3,
                    Name = "Sofia City",
                    Description = "A vibrant city with a bustling nightlife. The Capital!",
                    ImageUrl = "https://endurotourssofia.com/wp-content/uploads/sofia.webp",
                    AuthorId = "df1c3a0f-1234-4cde-bb55-d5f15a6aabcd",
                    CreatedOn = DateTime.Now,
                    CategoryId = 3
                }
            );
        }
    }
}
