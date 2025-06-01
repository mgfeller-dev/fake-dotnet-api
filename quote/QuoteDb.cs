using Microsoft.EntityFrameworkCore;

internal class QuoteDb : DbContext
{
    public QuoteDb(DbContextOptions<QuoteDb> options)
        : base(options)
    {
    }

    public DbSet<Quote> Quotes => Set<Quote>();
}