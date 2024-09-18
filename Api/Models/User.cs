using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
	[Table("users")]
	public class User
	{
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)] //Auto Increment
		[Column("ID")]
		public int Id { get; set; }

		[Required]
		[Column("name")]
		public string Username { get; set; }

		[Required]
		[Column("email")]
		public string Email { get; set; }

		
		[Required]
		[Column("password")]
		public string PasswordHash { get; set; }

	}
}
