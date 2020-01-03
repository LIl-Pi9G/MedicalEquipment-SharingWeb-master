using Microsoft.AspNetCore.Http;
using System.Linq;

namespace MedicalEquipment_Sharing.Tools
{
	public class HttpHelper
	{
		/// <summary>
		/// 获取登录用户邮箱
		/// <param name="httpContext">HttpContext</param>
		/// </summary>
		public static string GetLoginUserName(HttpContext httpContext)
		{
			try
			{
				return httpContext.User.Claims.First().Value;
			}
			catch
			{
				return "";
			}
		}
		
		
		/// <summary>
		/// 获取登录用户邮箱
		/// <param name="httpContext">HttpContext</param>
		/// </summary>
		public static string GetLoginUserEmail(HttpContext httpContext)
		{
			try
			{
				return httpContext.User.Claims.Last().Value;
			}
			catch
			{
				return "";
			}
		}
		

		/// <summary>
		/// 是否登录
		/// </summary>
		/// <param name="httpContext">HttpContext</param>
		/// <returns>是否登录</returns>
		public static bool IsLogin(HttpContext httpContext)
		{
			return httpContext.User.Identity.IsAuthenticated;
		}

	}
}
