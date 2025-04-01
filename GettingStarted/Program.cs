using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HotChocolate.Data;

// Required for WebApplication builder
var builder = WebApplication.CreateBuilder(args);

// Register HTTP client
builder.Services.AddHttpClient();

// Register EF Core with SQLite
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlite("Data Source=payments.db"));

// Register GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

// Create DB scope for initialization
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.Payments.Any())
    {
        dbContext.Payments.AddRange(new List<Payment>
        {
            new Payment { UserId = 1, Amount = 100.0m, Date = DateTime.UtcNow.AddDays(-10) },
            new Payment { UserId = 1, Amount = 150.0m, Date = DateTime.UtcNow.AddDays(-5) }
        });
        await dbContext.SaveChangesAsync();
    }

    Console.WriteLine("Payments in the database:");
    foreach (var payment in dbContext.Payments.ToList())
    {
        Console.WriteLine($"PaymentId: {payment.PaymentId}, UserId: {payment.UserId}, Amount: {payment.Amount}, Date: {payment.Date}");
    }
}

app.MapGraphQL();

Console.WriteLine("GraphQL server running at http://localhost:5000/graphql");
app.Run();


// ---------- Models Below ----------

public class Payment
{
    public int PaymentId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public List<Payment> Payments { get; set; } = new();
}

public class UserProfile
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

public class Query
{
    public async Task<User> GetUserAsync(
        int id,
        [Service] IHttpClientFactory httpClientFactory,
        [Service] PaymentDbContext dbContext)
    {
        var httpClient = httpClientFactory.CreateClient();
        var profile = await httpClient.GetFromJsonAsync<UserProfile>($"https://jsonplaceholder.typicode.com/users/{id}");

        var payments = await dbContext.Payments
            .Where(p => p.UserId == id)
            .ToListAsync();

        return new User
        {
            Id = id,
            Name = profile?.Name ?? "Unknown",
            Email = profile?.Email ?? "",
            Payments = payments
        };
    }
}

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }
    public DbSet<Payment> Payments { get; set; }
}
