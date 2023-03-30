using AndersenTestingTask.Domain.Models;
using AndersenTestingTask.Domain.Repositories.Interfaces;

namespace AndersenTestingTask.Domain.Repositories;

public class ProductCache : IProductCache
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
                minPrice = product.Price;
            if (maxPrice < product.Price)
                maxPrice = product.Price;
            foreach (var size in product.Sizes)
            {
                allSizes.Add(size);
            }

            var wordsInDesc = product.Description.Split(" ");
            foreach (var word in wordsInDesc)
            {
                if (commonWords.TryGetValue(word.Trim('.'), out var i))
                {
                    i++;
                    commonWords[word.Trim('.')] = i;
                }
                else
                {
                    commonWords.Add(word.Trim('.'), 1);
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