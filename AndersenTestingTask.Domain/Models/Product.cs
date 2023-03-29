namespace AndersenTestingTask.Domain.Models;

public class Product
{
    public string Title { get; set; }

    public decimal Price { get; set; }

    public List<string> Sizes { get; set; }

    public string Description { get; set; }

    public string CustomDescription(List<string> highlights)
    {
        if (!highlights.Any())
            return Description;
        var descriptionByWords = Description.Split(" ").ToList();
        highlights.ForEach(f =>
        {
            var w = descriptionByWords.IndexOf(f);
            if (w >= 0)
            {
                descriptionByWords[w] = $"<em>{f}</em>";
            }
        });
        return string.Join(" ", descriptionByWords);
    }

}