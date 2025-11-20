using System.Collections.Generic;
using System.Linq;

namespace LecturerClaimSystem.Models
{
	public class InMemoryClaimRepository : IClaimRepository
	{
		private readonly List<Claim> _claims = new List<Claim>();

		public IEnumerable<Claim> GetAll()
		{
			return _claims;
		}

		public Claim? GetById(int id)
		{
			return _claims.FirstOrDefault(c => c.Id == id);
		}

		public void Add(Claim claim)
		{
		
			if (claim.Id == 0)
			{
				claim.Id = _claims.Count + 1;
			}
			_claims.Add(claim);
		}

		public void Update(Claim claim)
		{
			var existing = GetById(claim.Id);
			if (existing != null)
			{
				_claims.Remove(existing);
				_claims.Add(claim);
			}
		}
	}
}