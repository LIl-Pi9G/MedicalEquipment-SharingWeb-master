using Microsoft.AspNetCore.Mvc;

namespace MedicalEquipment_Sharing.Controllers
{
	public class ErrorController : Controller
    {

		/// <summary>
		/// 错误页
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}


		/// <summary>
		/// 404页
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
#pragma warning disable CS0114 // 成员隐藏继承的成员；缺少关键字 override
		public IActionResult NotFound()
#pragma warning restore CS0114 // 成员隐藏继承的成员；缺少关键字 override
		{
			return View();
		}



		/// <summary>
		/// 支付失败
		/// </summary>
		/// <returns>ViewR</returns>
		[HttpGet]
		public IActionResult PayFaild()
		{
			return View();
		}
		
		
		
		/// <summary>
		/// 提现失败
		/// </summary>
		/// <returns>ViewR</returns>
		[HttpGet]
		public IActionResult TransferFaild()
		{
			return View();
		}

	}
}