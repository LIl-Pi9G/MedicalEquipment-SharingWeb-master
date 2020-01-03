using System.ComponentModel.DataAnnotations;

namespace MedicalEquipment_Sharing.Models.ViewModels
{
	public class LoginViewModel
	{
		[Display(Name = "邮箱")]
		[Required(ErrorMessage = "请输入邮箱")]
		[EmailAddress(ErrorMessage = "邮箱格式错误")]
		public string Email { get; set; }

		[Display(Name = "密码")]
		[Required(ErrorMessage = "请输入密码")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
