using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
	[Table("users")]
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)] //Auto Increment
		[Column("userId")]
		public int Id { get; set; }

		[Required]
		[Column("userName")]
		public string Username { get; set; }

		[Required]
		[Column("email")]
		public string Email { get; set; }

		
		[Required]
		[Column("password")]
		public string PasswordHash { get; set; }

		[Required]
		[Column("failedAttempts")]
		public int FailedAttempts { get; set; }

		[Required]
		[Column("lockoutEnd")]
		public DateTime? LockoutEnd { get; set; }

		public ICollection<Order> Orders { get; set; }
	}
}
