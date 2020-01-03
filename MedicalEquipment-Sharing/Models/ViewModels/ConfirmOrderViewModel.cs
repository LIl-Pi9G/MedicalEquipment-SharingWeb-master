using MedicalEquipment_Sharing.Models.Enum;

namespace MedicalEquipment_Sharing.Models.ViewModels
{
	public class ConfirmOrderViewModel
	{
		public PayOrderType Type { get; set; }

		public long EquipmentId { get; set; }

		public double TotalAmount { get; set; }

		public string Subject { get; set; }

		public string Body { get; set; }
	}
}
