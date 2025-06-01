using System.Collections.Immutable;

public class Quote
{
    private static readonly ImmutableList<Quote> _quotes =
    [
        Create("Edsger W. Dijkstra",
            "Simplicity is prerequisite for reliability."
        ),
        Create("Edsger W. Dijkstra",
            "Computer science is no more about computers than astronomy is about telescopes."
        ),
        Create("Edsger W. Dijkstra",
            "Elegance is not a dispensable luxury but a factor that decides between success and failure."
        ),
        Create("Edsger W. Dijkstra",
            "There should be no such thing as boring mathematics."
        ),
        Create("Edsger W. Dijkstra",
            "Aim for brevity while avoiding jargon."
        ),
        Create("Edsger W. Dijkstra",
            "The lurking suspicion that something could be simplified is the world's richest source of rewarding challenges."
        ),
        Create("Edsger W. Dijkstra",
            "Program testing can be used to show the presence of bugs, but never to show their absence!"
        ),
        Create("Confucius",
            "Our greatest glory is not in never falling, but in rising every time we fall."
        )
    ];

    public Quote(string source, string text)
    {
        Id = Guid.NewGuid().ToString();
        Source = source;
        Text = text;
    }

    public string? Id { get; set; }
    public string Source { get; set; }
    public string Text { get; set; }

    public static Quote Create(string source, string text)
    {
        return new Quote(source, text);
    }

    public static Quote GetRandomQuote()
    {
        var rnd = new Random();

        return _quotes[rnd.Next(_quotes.Count)];
    }
}