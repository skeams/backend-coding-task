using Claims.Models;
using Claims.Proxy;
using Claims.Queue;

namespace Claims.Services
{
    public class ClaimsService
    {
        private readonly ClaimsProxy _proxy;
        private readonly AuditingQueue _queue;

        public ClaimsService(ClaimsProxy proxy, AuditingQueue queue)
        {
            _proxy = proxy;
            _queue = queue;
        }

        public async Task<IEnumerable<Claim>> GetClaims()
        {
            return await _proxy.GetClaimsAsync();
        }

        public async Task CreateClaim(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            _queue.AddAuditAction(new QueueItem
            {
                Action = "POST",
                Id = claim.Id,
                IsClaim = true
            });
            await _proxy.AddItemAsync(claim);
        }

        public async Task DeleteClaim(string id)
        {
            _queue.AddAuditAction(new QueueItem
            {
                Action = "DELETE",
                Id = id,
                IsClaim = true
            });
            await _proxy.DeleteItemAsync(id);
        }

        public async Task<Claim> GetClaim(string id)
        {
            return await _proxy.GetClaimAsync(id);
        }
    }
}