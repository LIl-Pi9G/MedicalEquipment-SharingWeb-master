using MedicalEquipment_Sharing.Models.Enum;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MedicalEquipment_Sharing.Models.ViewModels
{
	public class CreateEquipmentViewModel
	{
		[Display(Name = "设备名称")]
		[Required(ErrorMessage = "请输入设备名称")]
		[StringLength(255, ErrorMessage = "设备名称过长")]
		public string Name { get; set; }

		[Display(Name = "租赁价格（元/天）")]
		[Required(ErrorMessage = "请输入租赁价格")]
		[Range(typeof(decimal), "0.01", "999999999.99", ErrorMessage = "请输入正确的数值")]
		public decimal Price { get; set; }

		[Display(Name = "描述")]
		[Required(ErrorMessage = "请输入设备描述")]
		public string Describe { get; set; }

		[Display(Name = "图片")]
		[Required(ErrorMessage = "请选择图片")]
		public IFormFile Image { get; set; }

		[Display(Name = "出售价格")]
		[Required(ErrorMessage = "请输入出售价格")]
		[Range(typeof(decimal), "0.01", "999999999.99", ErrorMessage = "请输入正确的数值")]
		public decimal SoldPrice { get; set; }

		[Display(Name = "入手价")]
		[Required(ErrorMessage = "请输入入手价")]
		[Range(typeof(decimal), "0.01", "999999999.99", ErrorMessage = "请输入正确的数值")]
		
		public decimal OriginalPrice { get; set; }

		[Display(Name = "类型")]
		[Required(ErrorMessage = "请选择类型")]
		public EquipmentType Type { get; set; }

		[Display(Name = "制造商")]
		[Required(ErrorMessage = "请选择制造商")]
		public EquipmentManufacturer Manufacturer { get; set; }
	}
}
