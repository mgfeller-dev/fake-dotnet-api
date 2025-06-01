using Microsoft.EntityFrameworkCore;

public static class QuoteEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/quotes", async (QuoteDb db) => await db.Quotes.ToListAsync());

        app.MapGet("/quotes/{id}", async (int id, QuoteDb db) =>
            await db.Quotes.FindAsync(id)
                is Quote quote
                ? Results.Ok(quote)
                : Results.NotFound());

        app.MapPost("/quotes", async (Quote quote, QuoteDb db) =>
        {
            if (string.IsNullOrEmpty(quote.Id)) quote.Id = Guid.NewGuid().ToString();
            db.Quotes.Add(quote);
            await db.SaveChangesAsync();

            return Results.Created($"/quotes/{quote.Id}", quote);
        });

        app.MapPost("/quotes/rand", async (QuoteDb db) =>
        {
            var quote = Quote.GetRandomQuote();
            db.Quotes.Add(quote);
            await db.SaveChangesAsync();

            return Results.Created($"/quotes/{quote.Id}", quote);
        });

        app.MapPut("/quotes/{id}", async (int id, Quote inputQuote, QuoteDb db) =>
        {
            var quote = await db.Quotes.FindAsync(id);

            if (quote is null) return Results.NotFound();

            quote.Source = inputQuote.Source;
            quote.Text = inputQuote.Text;

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        app.MapDelete("/quotes/{id}", async (int id, QuoteDb db) =>
        {
            if (await db.Quotes.FindAsync(id) is Quote quote)
            {
                db.Quotes.Remove(quote);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }

            return Results.NotFound();
        });
    }
}