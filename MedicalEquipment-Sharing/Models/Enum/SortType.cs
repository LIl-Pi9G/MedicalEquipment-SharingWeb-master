using System.ComponentModel.DataAnnotations;

namespace MedicalEquipment_Sharing.Models.Enum
{
	public enum SortType
	{
		[Display(Name = "发布日期从新到旧")]
		DateAscend = 0,

		[Display(Name = "发布日期从旧到新")]
		DateDescend = 1,

		[Display(Name = "租赁价格从低到高")]
		PriceDescend = 2,

		[Display(Name = "租赁价格从高到低")]
		PriceAscend = 3,

		[Display(Name = "购买价格从低到高")]
		SoldPriceDescend = 4,

		[Display(Name = "购买价格从高到低")]
		SoldPriceAscend = 5
	}
}
