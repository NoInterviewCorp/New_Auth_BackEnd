using System;

using Microsoft.EntityFrameworkCore;




namespace JwtTokenSpace
{

    public class UserDatabase : DbContext

    {

        public DbSet<User> users { get; set; }





        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        {

            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=userDB;Trusted_Connection=True;");

        }


    }



}