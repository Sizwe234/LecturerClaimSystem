using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LecturerClaimSystem.Models;

namespace LecturerClaimSystem.Services
{
	public class ReportService
	{
		public byte[] ExportClaimsToCsv(IEnumerable<Claim> claims)
		{
			var sb = new StringBuilder();
			sb.AppendLine("Id,LecturerName,LecturerEmail,HoursWorked,HourlyRate,Total,Status,SubmittedDate");

			foreach (var c in claims)
			{
				var line = string.Join(",",
					c.Id,
					Escape(c.LecturerName),
					Escape(c.LecturerEmail),
					c.HoursWorked,
					c.HourlyRate,
					c.CalculateEarnings(),
					c.Status,
					c.SubmittedDate.ToString("yyyy-MM-dd HH:mm")
				);
				sb.AppendLine(line);
			}

			return Encoding.UTF8.GetBytes(sb.ToString());
		}

		private string Escape(string? value)
		{
			if (string.IsNullOrEmpty(value)) return "";
			var needsQuotes = value.Contains(",") || value.Contains("\"") || value.Contains("\n");
			var escaped = value.Replace("\"", "\"\"");
			return needsQuotes ? $"\"{escaped}\"" : escaped;
		}
	}
}