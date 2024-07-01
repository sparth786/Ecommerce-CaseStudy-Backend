using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public static class DbContextHelper
{
    public static MyDbContext GetDbContext(IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:SQLConnectionString"];

        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>()
            .UseSqlServer(connectionString);

        return new MyDbContext(optionsBuilder.Options);
    }
}
