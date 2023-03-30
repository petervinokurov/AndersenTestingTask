using AndersenTestingTask.Domain.Models;
using AndersenTestingTask.Domain.Services.Interfaces;

namespace AndersenTestingTask.Domain.Services;

public class FilterObjectProvider : IFilterObjectProvider
{
    public ResponseFilter GetFilterObject(IEnumerable<Product> products)
    {
        decimal minPrice = 0;
        decimal maxPrice = 0;
        HashSet<string> allSizes = new HashSet<string>();
        Dictionary<string, int> commonWords = new Dictionary<string, int>();
        
        foreach (var product in products)
        {
            if (minPrice > product.Price || minPrice == 0)
            {
                minPrice = product.Price;
            }

            if (maxPrice < product.Price)
            {
                maxPrice = product.Price;
            }
            
            allSizes.UnionWith(product.Sizes);

            var wordsInDesc = product.Description.Split(" ").Select(x => x.Trim('.'));
            foreach (var word in wordsInDesc)
            {
                if (commonWords.TryGetValue(word, out var i))
                {
                    i++;
                    commonWords[word] = i;
                }
                else
                {
                    commonWords.Add(word, 1);
                }
            }
        }
        
        return new ResponseFilter
        {
            Sizes = allSizes.ToArray(),
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Words = commonWords.OrderByDescending(x => x.Value)
                .Skip(5).Take(10).Select(x => x.Key).ToArray() 
        };
    }
}