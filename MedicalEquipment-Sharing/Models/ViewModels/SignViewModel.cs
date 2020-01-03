using System.ComponentModel.DataAnnotations;

namespace MedicalEquipment_Sharing.Models.ViewModels
{
	public class SignViewModel
	{
		[Display(Name = "邮箱")]
		[Required(ErrorMessage = "请输入邮箱")]
		[EmailAddress(ErrorMessage = "邮箱格式错误")]
		public string Email { get; set; }

		[Display(Name = "昵称")]
		[Required(ErrorMessage = "请输入昵称")]
		[StringLength(20, ErrorMessage = "昵称过长")]
		public string UserName { get; set; }

		[Display(Name = "密码")]
		[Required(ErrorMessage = "请输入密码")]
		[DataType(DataType.Password)]
		[RegularExpression(@"^(?=.*[a-zA-Z])(?=.*[0-9])[A-Za-z0-9]{6,18}$",
			ErrorMessage = "密码为6~18个字符，且至少需要一个字母和数字")]
		public string Password { get; set; }

		[Display(Name = "确认密码")]
		[Required(ErrorMessage = "请确认密码")]
		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "两次输入的密码不一致")]
		public string MulPassword { get; set; }

		[Display(Name = "手机号码")]
		[Required(ErrorMessage = "请输入手机号码")]
		[MinLength(11, ErrorMessage = "请输入11位手机号码")]
		[MaxLength(11, ErrorMessage = "请输入11位手机号码")]
		[DataType(DataType.PhoneNumber)]
		public string TelePhone { get; set; }
	}
}
