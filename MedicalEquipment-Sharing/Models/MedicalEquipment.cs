using MedicalEquipment_Sharing.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalEquipment_Sharing.Models
{
	public class MedicalEquipment
	{
		[Key, Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Required, StringLength(255)]
		public string Name { get; set; }

		[Required]
		public decimal Price { get; set; }

		public string Describe { get; set; }

		public string ImageUrl { get; set; }

		[Required, ForeignKey(nameof(User.Email))]
		public string UserEmail { get; set; }

		[Required]
		public decimal SoldPrice { get; set; }

		[Required]
		public decimal OriginalPrice { get; set; }

		public EquipmentType Type { get; set; }

		public EquipmentManufacturer Manufacturer { get; set; }

		public MedicalEquipment()
		{
			Type = EquipmentType.Other;
			Manufacturer = EquipmentManufacturer.Other;
		}
	}
}
