namespace DemoApi.Models;

public class Document
{
    public static readonly List<Document> Items = [];

    static Document()
    {
        // seed documents
        var rnd = new Random();
        var list = new List<Document>(10);

        for (var i = 0; i < 10; i++)
        {
            list.Add(
                new Document
                {
                    Id = i,
                    Name = $"Document {i}",
                    AccessLevel = (AccessLevel)rnd.Next(0, 2),
                });
        }

        Items.AddRange(list);
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public AccessLevel AccessLevel { get; set; }

    public DateTime CreatedDate { get; } = DateTime.UtcNow;
}
