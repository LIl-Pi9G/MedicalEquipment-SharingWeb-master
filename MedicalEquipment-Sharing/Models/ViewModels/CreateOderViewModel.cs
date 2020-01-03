using MedicalEquipment_Sharing.Models.Enum;
using System;

namespace MedicalEquipment_Sharing.Models.ViewModels
{
	public class CreateOderViewModel
	{
		public string SellerEmail { get; set; }

		public string BuyerEmail { get; set; }

		public long EquipmentId { get; set; }

		public DateTime SoldDate { get; set; }

		public DateTime RevertedDate { get; set; }

		public OrderStatus State { get; set; }

		public string PayOrderNum { get; set; }

		//public string TransferOrderNum { get; set; }

		public string DepositOrderNum { get; set; }

		public string RetDepositOrderNum { get; set; }
	}
}
