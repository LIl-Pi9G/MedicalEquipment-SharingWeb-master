using MedicalEquipment_Sharing.Models.Enum;

namespace MedicalEquipment_Sharing.Models.ViewModels
{
	public class FilterDataViewModel
	{
		public string SearchString { get; set; }

		public int EquipmentType { get; set; }

		public int EquipmentManufacturer { get; set; }

		public SortType SortType { get; set; }

		public decimal MinPrice { get; set; }

		public decimal MaxPrice { get; set; }

		public decimal MinSoldPrice { get; set; }

		public decimal MaxSoldPrice { get; set; }
	}
}
