using MedicalEquipment_Sharing.Models.Enum;

namespace MedicalEquipment_Sharing.Models.ViewModels
{
	public class SellEquipmentViewModel
	{
		public long EquipmentId { get; set; }
		public OrderStatus State { get; set; } 
	}
}
