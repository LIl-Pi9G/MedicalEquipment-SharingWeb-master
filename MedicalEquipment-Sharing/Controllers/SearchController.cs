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
	public class SearchController : Controller
	{
		public static readonly int SEARCH_ALL_VALUE = -1;

		private readonly int _PageSize = HomeController._PageSize;

		private readonly IConfiguration _Configuration;

		public SearchController(IConfiguration configuration)
		{
			_Configuration = configuration;
		}


		/// <summary>
		/// 搜索设备
		/// </summary>
		/// <param name="searchString">关键字</param>
		/// <param name="page">页码</param>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> Index(string searchString, int page)
		{
			if (page <= 0)
			{
				page = 1;
			}

			if (string.IsNullOrEmpty(searchString))
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			EquipmentManager equipmentManager = new EquipmentManager(_Configuration);

			List<EquipmentViewModel> equipments = await Task.Run(() =>
			{
				return equipmentManager.QueryPageEquipment(page, _PageSize, Units.SplitString(searchString), true);
			});

			if (equipments.Count == 0)
			{
				return RedirectToAction(nameof(NoResult), new { searchString });
			}

			int pageNum = await Task.Run(() =>
			{
				return equipmentManager.GetPageNumber(_PageSize, Units.SplitString(searchString), true);
			});

			return View(new HomeShowViewModel
			{
				EquipmentViewModels = equipments,
				SearchString = searchString,
				Page = page,
				PageCount = pageNum
			});
		}



		/// <summary>
		/// 重定向到Index 传递搜索字符串
		/// </summary>
		/// <param name="searchString">搜索字符串</param>
		/// <returns>ViewResult</returns>
		[HttpPost]
		public IActionResult Search(string searchString)
		{
			return RedirectToAction(nameof(Index), new { searchString, page = 1 });
		}




		/// <summary>
		/// 筛选
		/// </summary>
		/// <param name="search">默认填入参数</param>
		/// <param name="type">默认填入参数</param>
		/// <param name="manufacturer">默认填入参数</param>
		/// <param name="sort">默认填入参数</param>
		/// <param name="minPrice">默认填入参数</param>
		/// <param name="maxPrice">默认填入参数</param>
		/// <param name="minSoldPrice">默认填入参数</param>
		/// <param name="maxSoldPrice">默认填入参数</param>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public IActionResult Filter(
				string search,
				int? type,
				int? manufacturer,
				int sort,
				decimal minPrice,
				decimal maxPrice,
				decimal minSoldPrice,
				decimal maxSoldPrice)
		{
			if (type == null)
			{
				type = SEARCH_ALL_VALUE;
			}

			if (manufacturer == null)
			{
				manufacturer = SEARCH_ALL_VALUE;
			}

			return View(new FilterDataViewModel
			{
				SearchString = search,
				EquipmentType = (int)type,
				EquipmentManufacturer = (int)manufacturer,
				SortType = (SortType)sort,
				MinPrice = minPrice,
				MaxPrice = maxPrice,
				MinSoldPrice = minSoldPrice,
				MaxSoldPrice = maxSoldPrice
			});
		}



		/// <summary>
		/// 筛选post
		/// </summary>
		/// <param name="filterData">FilterDataViewModel</param>
		/// <returns>ViewResult</returns>
		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public IActionResult Filter(FilterDataViewModel filterData)
		{
			return RedirectToAction(nameof(Result), new
				{
					search = filterData.SearchString,
					type = filterData.EquipmentType,
					manufacturer = filterData.EquipmentManufacturer,
					sort = (int)filterData.SortType,
					minPrice = filterData.MinPrice,
					maxPrice = filterData.MaxPrice,
					minSoldPrice = filterData.MinSoldPrice,
					maxSoldPrice = filterData.MaxSoldPrice,
					page = 1
				});
		}



		/// <summary>
		/// 搜索结果
		/// </summary>
		/// <param name="search">关键字</param>
		/// <param name="type">设备类型</param>
		/// <param name="manufacturer">制造商</param>
		/// <param name="sort">排序类型</param>
		/// <param name="minPrice">最小租赁价格</param>
		/// <param name="maxPrice">最小租赁价格</param>
		/// <param name="minSoldPrice">最小购买价格</param>
		/// <param name="maxSoldPrice">最大购买价格</param>
		/// <param name="page">页码</param>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> Result(
				string search,
				int type,
				int manufacturer,
				int sort,
				decimal minPrice,
				decimal maxPrice,
				decimal minSoldPrice,
				decimal maxSoldPrice,
				int page)
		{
			FilterDataViewModel filterData = new FilterDataViewModel
			{
				SearchString = search,
				EquipmentType = type,
				EquipmentManufacturer = manufacturer,
				SortType = (SortType)sort,
				MinPrice = minPrice,
				MaxPrice = maxPrice,
				MinSoldPrice = minSoldPrice,
				MaxSoldPrice = maxSoldPrice
			};

			if (page == 0)
			{
				page = 1;
			}

			List<EquipmentViewModel> equipments = await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).FilterPageEquipment(page, _PageSize, filterData, true);
			});

			int pageCount = await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).GetFilterPageNumber(page, _PageSize, filterData, true);
			});
			
			return View(new HomeShowViewModel
			{
				EquipmentViewModels = equipments,
				Page = page,
				PageCount = pageCount,

				//作返回地址的参数
				SearchString = $"?search={search}&type={type}&manufacturer={manufacturer}" +
					$"&sort={sort}&minPrice={minPrice}&maxPrice={maxPrice}" +
					$"&minSoldPrice={minSoldPrice}&maxSoldPrice={maxSoldPrice}"
			});
		}



		/// <summary>
		/// 搜索无结果页面
		/// </summary>
		/// <param name="searchString">搜索字符串</param>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public IActionResult NoResult(string searchString)
		{
			return View(new NoFoundEquipmentViewModel
			{
				SearchString = searchString
			});
		}

	}
}