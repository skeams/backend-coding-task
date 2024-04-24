using Claims.Auditing;
using Claims.Models;
using Claims.Proxy;
using Claims.Utils;

namespace Claims.Services;

public class CoversService
{
    private readonly CoversProxy _proxy;
    private readonly Auditer _auditer;

    public CoversService(CoversProxy proxy, AuditContext auditContext)
    {
        _proxy = proxy;
        _auditer = new Auditer(auditContext);
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

        _auditer.AuditCover(cover.Id, "POST");
        await _proxy.CreateCover(cover);

        return cover;
    }

    public async Task DeleteCover(string id)
    {
        _auditer.AuditCover(id, "DELETE");
        await _proxy.DeleteCover(id);
    }
}