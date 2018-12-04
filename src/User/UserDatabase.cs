using System;

using Microsoft.EntityFrameworkCore;




namespace JwtTokenSpace
{

    public class UserDatabase : DbContext

    {

        public DbSet<User> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connstring1 = "Server=localhost;Database=MyUserDatabase1;User=SA;Password=AlquidA@9826";
            	var connstring = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? connstring1;
            	optionsBuilder.UseSqlServer (connstring);
        // optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=MyUserDatabase1;Trusted_Connection=True;");

        }
    }
}
