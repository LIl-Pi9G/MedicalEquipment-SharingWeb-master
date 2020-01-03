using System.ComponentModel.DataAnnotations;

namespace MedicalEquipment_Sharing.Models.Enum
{
	public enum EquipmentManufacturer
	{
		[Display(Name = "其他")]
		Other = 0,

		[Display(Name = "强生")]
		Johnson = 1,

		[Display(Name = "鱼跃")]
		YuWell = 2,

		[Display(Name = "斯曼峰")]
		Smaf = 3,

		[Display(Name = "百捷")]
		BeneCheck = 4,

		[Display(Name = "日光祥")]
		Sunlight = 5,

		[Display(Name = "诺泰")]
		NuoTai = 6,

		[Display(Name = "欧姆龙")]
		Omoron = 7,

		[Display(Name = "可孚")]
		Cofoe = 8,

		[Display(Name = "西恩")]
		Scian = 9,

		[Display(Name = "西格玛")]
		Sigama = 10,

		[Display(Name = "安诺")]
		Anonoroad = 11,

		[Display(Name = "施莱")]
		SteriLance = 12,

		[Display(Name = "飞利浦")]
		Philips = 13
	}
}
