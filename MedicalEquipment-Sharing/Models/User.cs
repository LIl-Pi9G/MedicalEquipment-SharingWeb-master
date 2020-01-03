using System.ComponentModel.DataAnnotations;

namespace MedicalEquipment_Sharing.Models
{
	public class User
	{
		[Key, Required]
		public string Email { get; set; }

		[Required, StringLength(20)]
		public string UserName { get; set; }

		[Required]
		public string Password { get; set; }

		[Required, StringLength(30)]
		public string TelePhone { get; set; }

		public decimal Income { get; set; }
	}

}
