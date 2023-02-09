using System;
using Microsoft.EntityFrameworkCore;

namespace SQLApp
{
    public class MovieDataContext : DbContext
    {
        public MovieDataContext()
        {
        }

        // Pass configuration to the context
        public MovieDataContext(DbContextOptions<MovieDataContext> options) : base(options)
        {

        }

        // Columns increment themselves
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Actor> Actors { get; set; }
    }
}

