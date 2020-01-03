using System.ComponentModel.DataAnnotations;

namespace MedicalEquipment_Sharing.Models.Enum
{
	public enum EquipmentType
	{
		[Display(Name = "其他")]
		Other = 0,

		[Display(Name = "血糖仪")]
		BloodGlucoseMeter = 1,

		[Display(Name = "雾化器")]
		Nebulizer = 2,

		[Display(Name = "血压计")]
		Sphygmomanometer = 3,

		[Display(Name = "吸引器")]
		Aspirator = 4,

		[Display(Name = "尿酸检测仪")]
		UricAcidDetector = 5,

		[Display(Name = "制氧机")]
		Oxygenerator = 6,

		[Display(Name = "吸痰器")]
		SputumAspirator = 7,

		[Display(Name = "体温计")]
		Thermometer = 8,

		[Display(Name = "痔疮器")]
		HemorrhoidDevice = 9,

		[Display(Name = "医用护腰带")]
		Medicalbelts = 10,

		[Display(Name = "光纤检耳镜")]
		FiberOpticOtoscope = 11,

		[Display(Name = "医用润滑剂")]
		Lubricant = 12,

		[Display(Name = "白癜风光疗仪")]
		VitiligoPhototherapy = 13,

		[Display(Name = "双通道注射泵")]
		InjectionPump = 14
	}
}
