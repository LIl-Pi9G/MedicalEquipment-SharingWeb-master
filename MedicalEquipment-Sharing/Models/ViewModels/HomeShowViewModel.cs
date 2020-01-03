using System.Collections.Generic;

namespace MedicalEquipment_Sharing.Models.ViewModels
{
	public class HomeShowViewModel
	{
		public List<EquipmentViewModel> EquipmentViewModels { get; set; }
		public int Page { get; set; }
		public int PageCount { get; set; }
		public string SearchString { get; set; }
	}
}
