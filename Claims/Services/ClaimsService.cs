using Claims.Auditing;
using Claims.Models;
using Claims.Proxy;

namespace Claims.Services
{
    public class ClaimsService
    {
        private readonly ClaimsProxy _proxy;
        private readonly Auditer _auditer;

        public ClaimsService(ClaimsProxy proxy, AuditContext auditContext)
        {
            _proxy = proxy;
            _auditer = new Auditer(auditContext);
        }

        public async Task<IEnumerable<Claim>> GetClaims()
        {
            return await _proxy.GetClaimsAsync();
        }

        public async Task CreateClaim(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            _auditer.AuditClaim(claim.Id, "POST");
            await _proxy.AddItemAsync(claim);
        }

        public async Task DeleteClaim(string id)
        {
            _auditer.AuditClaim(id, "DELETE");
            await _proxy.DeleteItemAsync(id);
        }

        public async Task<Claim> GetClaim(string id)
        {
            return await _proxy.GetClaimAsync(id);
        }
    }
}