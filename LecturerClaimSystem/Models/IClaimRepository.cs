using System.Collections.Generic;

namespace LecturerClaimSystem.Models
{
	public interface IClaimRepository
	{
		IEnumerable<Claim> GetAll();
		Claim? GetById(int id);
		void Update(Claim claim);
		void Add(Claim claim);
	}
}