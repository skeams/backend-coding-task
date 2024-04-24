using Claims.Models;
using Microsoft.Azure.Cosmos;

namespace Claims.Proxy;

public class CoversProxy
{
    private readonly Container _container;

    public CoversProxy(CosmosClient cosmosClient)
    {
        _container = cosmosClient.GetContainer("ClaimDb", "Cover")
                     ?? throw new ArgumentNullException(nameof(cosmosClient));
    }

    public async Task<IEnumerable<Cover>> GetCovers()
    {
        var query = _container.GetItemQueryIterator<Cover>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<Cover>();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task<Cover> GetCover(string id)
    {
        var response = await _container.ReadItemAsync<Cover>(id, new PartitionKey(id));
        return response.Resource;
    }

    public async Task CreateCover(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        await _container.CreateItemAsync(cover, new PartitionKey(cover.Id));
    }

    public async Task DeleteCover(string id)
    {
        await _container.DeleteItemAsync<Cover>(id, new PartitionKey(id));
    }
}