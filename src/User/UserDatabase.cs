using System;

using Microsoft.EntityFrameworkCore;




namespace JwtTokenSpace
{

    public class UserDatabase : DbContext

    {

        public DbSet<User> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        {

            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=UserDatabase;Trusted_Connection=True;");

           // optionsBuilder.UseSqlServer(@"Server=db;Database=UserDataBase;User=SA;Password=password123;");

        }
    }
}