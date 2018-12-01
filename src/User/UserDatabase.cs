using System;

using Microsoft.EntityFrameworkCore;




namespace JwtTokenSpace
{

    public class UserDatabase : DbContext

    {

        public DbSet<User> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connstring1 = "Server=localhost;Database=MyUserDatabase;User=SA;Password=password123";
            	var connstring = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? connstring1;
            	optionsBuilder.UseSqlServer (connstring);

        }
    }
}
