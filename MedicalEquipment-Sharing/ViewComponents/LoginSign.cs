using MedicalEquipment_Sharing.Models.Enum;
using MedicalEquipment_Sharing.Tools;
using MedicalEquipment_Sharing.ViewComponents.ComponentModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MedicalEquipment_Sharing.ViewComponents
{
	public class LoginSign : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			LoginUserInfo userInfo = new LoginUserInfo();

			if (HttpContext.User.Identity.IsAuthenticated)
			{
				userInfo.Logged = LoginStatus.Logged;
				userInfo.UserName = HttpHelper.GetLoginUserName(HttpContext);
			}
			else
			{
				userInfo.Logged = LoginStatus.NotLogin;
				userInfo.UserName = string.Empty;
			}

			return await Task.Run(() => { return View(userInfo); });
		}

	}
}
