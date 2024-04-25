using Claims.Models;
using Claims.Proxy;
using Claims.Queue;
using Claims.Utils;

namespace Claims.Services;

public class CoversService
{
    private readonly CoversProxy _proxy;
    private readonly AuditingQueue _queue;

    public CoversService(CoversProxy proxy, AuditingQueue queue)
    {
        _proxy = proxy;
        _queue = queue;
    }

    public decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return CoverUtils.ComputePremium(startDate, endDate, coverType);
    }

    public async Task<IEnumerable<Cover>> GetCovers()
    {
        return await _proxy.GetCovers();
    }

    public async Task<Cover> GetCover(string id)
    {
        return await _proxy.GetCover(id);
    }

    public async Task<Cover> CreateCover(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = CoverUtils.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

        _queue.AddAuditAction(new QueueItem
        {
            Action = "POST",
            Id = cover.Id,
            IsClaim = false
        });
        await _proxy.CreateCover(cover);

        return cover;
    }

    public async Task DeleteCover(string id)
    {
        _queue.AddAuditAction(new QueueItem
        {
            Action = "DELETE",
            Id = id,
            IsClaim = false
        });
        await _proxy.DeleteCover(id);
    }
}