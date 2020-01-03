using MedicalEquipment_Sharing.Models.Enum;
using System;

namespace MedicalEquipment_Sharing.Models.ViewModels
{
	public class ShowOrderViewModel
	{
		public MedicalEquipment Equipment { get; set; }

		public User Owner { get; set; }

		public User Lessee { get; set; }

		public DateTime SharedDate { get; set; }

		public DateTime RevertedDate { get; set; }

		public OrderStatus State { get; set; }
	}
}
