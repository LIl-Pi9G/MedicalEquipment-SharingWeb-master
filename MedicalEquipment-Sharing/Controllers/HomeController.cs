using MedicalEquipment_Sharing.DataAccess;
using MedicalEquipment_Sharing.Models.Enum;
using MedicalEquipment_Sharing.Models.ViewModels;
using MedicalEquipment_Sharing.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MedicalEquipment_Sharing.Controllers
{
	public class HomeController : Controller
	{
		/// <summary>
		/// 每一页最多显示的设备数
		/// </summary>
		public static readonly int _PageSize = 12;

		private readonly IConfiguration _HostingEnvironment;

		public HomeController(IConfiguration hostingEnvironment)
		{
			_HostingEnvironment = hostingEnvironment;
		}


		/// <summary>
		/// /Home/Index
		/// 分页展示所有数据，被借的除外
		/// </summary>
		/// <param name="searchString">搜索字符串，留空显示所有</param>
		/// <param name="page">页码，留空第一页</param>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> Index(int page)
		{
			if (page <= 0)
			{
				page = 1;
			}

			EquipmentManager equipmentManager = new EquipmentManager(_HostingEnvironment);

			List<EquipmentViewModel> equipments = await Task.Run(() =>
			{
				return equipmentManager.QueryPageEquipment(page, _PageSize);
			});

			int pageNum = await Task.Run(() =>
			{
				return equipmentManager.GetPageNumber(_PageSize);
			});

			return View(new HomeShowViewModel
			{
				EquipmentViewModels = equipments,
				PageCount = pageNum,
				Page = page,
			});
		}


		/// <summary>
		/// 详情页
		/// </summary>
		/// <param name="id">设备ID</param>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> Details(long id)
		{
			if (await Task.Run(() => { return new OrderManager(_HostingEnvironment).QueryEquipmentStatus(id); }) != OrderStatus.Holding)
			{
				return RedirectToAction(nameof(SoldOut));
			}

			ViewBag.UserName = HttpHelper.GetLoginUserName(HttpContext);

			EquipmentViewModel equipmentViewModel = await Task.Run(() => { return new EquipmentManager(_HostingEnvironment).QueryEquipment(id); });

			if (equipmentViewModel == null)
			{
				return RedirectToAction(nameof(ErrorController.NotFound), "Error");
			}

			return View(equipmentViewModel);
		}


		/// <summary>
		/// 租赁设备成功
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public IActionResult Borrow()
		{
			return View();
		}


		/// <summary>
		/// 购买设备成功
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public IActionResult Purchase()
		{
			return View();
		}


		/// <summary>
		/// 已被租赁页面
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public IActionResult SoldOut()
		{
			return View();
		}

	}
}
