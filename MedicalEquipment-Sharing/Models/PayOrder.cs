using MedicalEquipment_Sharing.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace MedicalEquipment_Sharing.Models
{
	public class PayOrder
	{
		[Key, Required]
		public string OrderNum { get; set; }

		public PayOrderType OrderType { get; set; }

		public bool PaySuccess { get; set; }

		[Required]
		public decimal Amount { get; set; }

		[Required]
		public string Account{ get; set; }

		public PayOrder()
		{
			PaySuccess = false;
		}
	}
}
