using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.EntityFrameworkCore;
using RealG.Models;

namespace RealG.Data
{
    public class RealGContext : DbContext
    {
        public RealGContext (DbContextOptions<RealGContext> options)
            : base(options)
        {
        }

        public DbSet<RealG.Models.Movie> Movie { get; set; } = default!;
        public DbSet<RealG.Models.Föreställning> Föreställning { get; set; } = default!;
        public DbSet<RealG.Models.Salong> Salong { get; set; } = default!;
        public DbSet<RealG.Models.Bokning> Bokning { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            base.OnModelCreating(modelBuilder);

            // Configure the 'Price' property of 'Movie' entity
            modelBuilder.Entity<Movie>()
                .Property(m => m.Pris)
                .HasColumnType("decimal(18, 2)"); // or use .HasPrecision(18, 2)
            modelBuilder.Entity<Bokning>()
    .HasOne(b => b.Föreställning)
    .WithMany(f => f.Bokningar)
    .HasForeignKey(b => b.FöreställningId)
    .OnDelete(DeleteBehavior.Restrict);

        }

        public static class SeedData
        {
            public static void Initialize(IServiceProvider serviceProvider)
            {
                using (var context = new RealGContext(
                    serviceProvider.GetRequiredService<
                        DbContextOptions<RealGContext>>()))
                {
                    // Look for any movies.
                    if (context.Movie.Any())
                    {
                        return;   // DB has been seeded
                    }
                    context.Movie.AddRange(
                        new Movie
                        {
                            Titel = "Oppenheimer",
                            Beskrivning = "1989-2-12",
                            Genre = "Real Story",
                            Pris = 155,
                            ImagePath = "Oppenheimer.png"
                        },
                        new Movie
                        {
                            Titel = "Freelance ",
                            Beskrivning = "1989-2-12",
                            Genre = "Comedy",
                            Pris = 155,
                            ImagePath = "Freelance.png"
                        },
                        new Movie
                        {
                            Titel = " Leo",
                            Beskrivning = "1989-2-12",
                            Genre = "Comedy",
                            Pris = 155,
                            ImagePath = "Leo.png"
                        },
                        new Movie
                        {
                            Titel = "Barbie",
                            Beskrivning = "1989-2-12",
                            Genre = "Childish",
                            Pris = 155,
                            ImagePath = "Barbie.png"
                        }
                    );
                    context.SaveChanges();
                    if (context.Salong.Any())
                    {
                        return;   // DB has been seeded
                    }
                    context.Salong.AddRange(
                        new Salong
                        {
                            Namn = "A1",
                            AntalStolar = 20,
                            Row = 2,
                            Number = 10,
                            
                        },
                        new Salong
                        {
                            Namn = "A2",
                            AntalStolar = 25,
                             Row = 5,
                            Number = 5,
                        },
                        new Salong
                        {
                            Namn = "A3",
                            AntalStolar = 30,
                            Row = 3,
                            Number = 10,
                        },
                        new Salong
                        {
                            Namn = "A4",
                            AntalStolar = 40,
                            Row = 4,
                            Number = 10,
                        }
                    );
                    context.SaveChanges();
                    if (!context.Föreställning.Any())
                    {
                        var movies = context.Movie.Take(4).ToList(); // Ensure only 4 movies are selected
                        var salongs = context.Salong.Take(4).ToList(); // Ensure at least 4 Salong are selected

                        // Define the fixed times
                        var times = new List<TimeSpan> {
        new TimeSpan(12, 0, 0), // 12:00
        new TimeSpan(14, 0, 0), // 14:00
        new TimeSpan(16, 0, 0), // 16:00
        new TimeSpan(18, 0, 0)  // 18:00
    };

                        // Define your date range for seeding
                        DateTime startDate = new DateTime(2023, 12, 1);
                        DateTime endDate = new DateTime(2023, 12, 31);

                        foreach (var movie in movies)
                        {
                            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                            {
                                for (int i = 0; i < times.Count; i++)
                                {
                                    context.Föreställning.Add(new Föreställning
                                    {
                                        FilmId = movie.Id,
                                        SalongId = salongs[i].SalongId, // Assign a unique SalongId for each time slot
                                        Datum = date.Date, // Store only the date part
                                        Tider = times[i], // Assign the time slot
                                                          // Set other properties as needed
                                    });
                                }
                            }
                        }

                        context.SaveChanges();
                    }



                }
            }
        }

    }

}
