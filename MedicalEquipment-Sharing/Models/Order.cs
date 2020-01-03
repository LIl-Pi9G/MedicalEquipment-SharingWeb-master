using MedicalEquipment_Sharing.Models.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalEquipment_Sharing.Models
{
	public class Order
	{
		[Key, Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long OrderId { get; set; }

		[Required, ForeignKey(nameof(MedicalEquipment.Id))]
		public long EquipmentId { get; set; }

		[Required, ForeignKey(nameof(User.Email))]
		public string SellerEmail { get; set; }

		[Required, ForeignKey(nameof(User.Email))]
		public string BuyerEmail { get; set; }

		[Required]
		public DateTime SoldDate { get;set;}

		[Required]
		public DateTime RevertedDate { get;set;}

		public OrderStatus State { get; set; }

		[ForeignKey(nameof(PayOrder.OrderNum))]
		public string PayOrderNum { get; set; }

		[ForeignKey(nameof(PayOrder.OrderNum))]
		public string DepositOrderNum { get; set; }

		public bool PaySuccess { get; set; }

		public bool PayDepositSuccess { get; set; }

		public Order()
		{
			PayDepositSuccess = false;
			PaySuccess = false;
		}
	}
}
