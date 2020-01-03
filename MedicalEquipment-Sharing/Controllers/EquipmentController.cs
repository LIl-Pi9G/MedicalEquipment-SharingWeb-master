using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MedicalEquipment_Sharing.DataAccess;
using MedicalEquipment_Sharing.Models;
using MedicalEquipment_Sharing.Models.Enum;
using MedicalEquipment_Sharing.Models.ViewModels;
using MedicalEquipment_Sharing.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MedicalEquipment_Sharing.Controllers
{
	public class EquipmentController : Controller
	{
		private readonly IHostingEnvironment _HostingEnvironment;
		private readonly IConfiguration _Configuration;

		public EquipmentController(
				IHostingEnvironment hostingEnvironment,
				IConfiguration configuration)
		{
			_HostingEnvironment = hostingEnvironment;
			_Configuration = configuration;
		}

		/// <summary>
		/// 我的设备
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode($"/Equipment") });
			}

			return View(await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).QueryEquipmentByEmail(HttpHelper.GetLoginUserEmail(HttpContext));
			}));
		}



		/// <summary>
		/// 创建页面
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public IActionResult Create()
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode($"/Equipment/Create") });
			}
			return View();
		}



		/// <summary>
		/// 创建页面post
		/// </summary>
		/// <param name="equipment">创建信息</param>
		/// <returns>ViewResult</returns>
		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> Create(CreateEquipmentViewModel equipment)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			if (equipment.Image.Length > 3145728L)
			{
				ModelState.AddModelError(string.Empty, "图片过大");
				return View();
			}

			string imgUrl = await FileHelper.UploadImageAsync(_HostingEnvironment.WebRootPath, equipment.Image);

			if (!await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).CreateEquipment(
					new MedicalEquipment
					{
						Name = equipment.Name,
						Price = equipment.Price,
						Describe = equipment.Describe,
						ImageUrl = imgUrl,
						UserEmail = HttpHelper.GetLoginUserEmail(HttpContext),
						SoldPrice = equipment.SoldPrice,
						OriginalPrice = equipment.OriginalPrice,
						Type = equipment.Type,
						Manufacturer = equipment.Manufacturer
					});
			}))
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}

			return RedirectToAction(nameof(Index));
		}



		/// <summary>
		/// 详情页
		/// 无法查看其他账号添加的设备
		/// </summary>
		/// <param name="id">设备ID</param>
		/// <param name="returnUrl">返回地址（需要URLEncode）</param>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> Details(long id, string returnUrl)
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode($"/Equipment/Details/{id}") });
			}

			ViewBag.Email = HttpHelper.GetLoginUserEmail(HttpContext);

			ViewBag.ReturnUrl = HttpUtility.UrlDecode(string.IsNullOrEmpty(returnUrl) ? "/Equipment" : returnUrl);

			EquipmentViewModel equipment = await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).QueryEquipment(id);
			});

			if (equipment == null)
			{
				return RedirectToAction(nameof(ErrorController.NotFound), "Error");
			}

			return View(equipment);
		}



		/// <summary>
		/// 删除确认页
		/// </summary>
		/// <param name="id">设备ID</param>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> Delete(long id)
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode($"/Equipment/Delete/{id}") });
			}

			if (await Task.Run(() =>
			{
				return new OrderManager(_Configuration).QueryEquipmentStatus(id);
			}) != OrderStatus.Holding)
			{
				return RedirectToAction(nameof(Details), new { id });
			}

			EquipmentViewModel equipment = await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).QueryEquipment(HttpHelper.GetLoginUserEmail(HttpContext), id);
			});

			if (equipment == null)
			{
				return RedirectToAction(nameof(AccountController.NotFound), "Error");
			}
			return View(equipment);
		}



		/// <summary>
		/// 删除设备
		/// </summary>
		/// <param name="deleteEquipment">设备ID</param>
		/// <returns>ViewResult</returns>
		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> Delete(DeleteEquipmentViewModel deleteEquipment)
		{
			if (await Task.Run(() =>
			{
				return new OrderManager(_Configuration).QueryEquipmentStatus(deleteEquipment.Id);
			}) != OrderStatus.Holding)
			{
				return RedirectToAction(nameof(Details), new { id = deleteEquipment.Id });
			}

			string imgPath = await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).GetEquipmentImgPath(deleteEquipment.Id);
			});

			if (await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).DeleteEquipment(deleteEquipment.Id);
			}))
			{
				FileHelper.DeleteImage(_HostingEnvironment.WebRootPath, imgPath);
				return RedirectToAction(nameof(Index));
			}
			else
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}
		}



		/// <summary>
		/// 编辑设备信息
		/// </summary>
		/// <param name="id">设备ID</param>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode($"/Equipment/Edit/{id}") });
			}

			if (await Task.Run(() =>
			{
				return new OrderManager(_Configuration).QueryEquipmentStatus(id);
			}) != OrderStatus.Holding)
			{
				return RedirectToAction(nameof(Details), new { id });
			}

			EquipmentViewModel equipment = await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).QueryEquipment(HttpHelper.GetLoginUserEmail(HttpContext), id);
			});

			if (equipment == null)
			{
				return RedirectToAction(nameof(ErrorController.NotFound), "Error");
			}

			return View(equipment);
		}



		/// <summary>
		/// 编辑设备信息post
		/// </summary>
		/// <param name="equipment">信息</param>
		/// <returns>ViewResult</returns>
		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> Edit(EquipmentViewModel equipment)
		{
			if (await Task.Run(() =>
			{
				return new OrderManager(_Configuration).QueryEquipmentStatus(equipment.Id);
			}) != OrderStatus.Holding)
			{
				return RedirectToAction(nameof(Details), new { equipment.Id });
			}

			if (!ModelState.IsValid)
			{
				return View(equipment);
			}

			if (equipment.Image != null)
			{
				if (equipment.Image.Length > 3145728L)
				{
					ModelState.AddModelError(string.Empty, "图片过大");
					return View(equipment);
				}

				FileHelper.DeleteImage(_HostingEnvironment.WebRootPath, await Task.Run(() =>
				{
					return new EquipmentManager(_Configuration).GetEquipmentImgPath(equipment.Id);
				}));

				equipment.ImageUrl = await FileHelper.UploadImageAsync(_HostingEnvironment.WebRootPath, equipment.Image);
			}

			if (!await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).UpdateEquipment(equipment, equipment.Id);
			}))
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}
			return RedirectToAction(nameof(Index));
		}



		/// <summary>
		/// 我的共享
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> MySharing()
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode("/Equipment/MySharing") });
			}

			return View(await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration)
				.QuerySharingEquipment(HttpHelper.GetLoginUserEmail(HttpContext));
			}));
		}



		/// <summary>
		/// 我的借用
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> MyBorrow()
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode("/Equipment/MySharing") });
			}

			return View(await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration)
				.QueryBorrowEquipment(HttpHelper.GetLoginUserEmail(HttpContext));
			}));
		}



		/// <summary>
		/// 我的购买
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> MyPurchase()
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode("/Equipment/MyPurchase") });
			}

			return View(await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).QueryBuyedEquipment(HttpHelper.GetLoginUserEmail(HttpContext));
			}));
		}



		/// <summary>
		/// 我售出的设备
		/// </summary>
		/// <returns>ViewResult</returns>
		public async Task<IActionResult> MySold()
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
						new { returnUrl = HttpUtility.UrlEncode("/Equipment/MySold") });
			}

			return View(await Task.Run(() =>
			{
				return new EquipmentManager(_Configuration).QuerySoldEquipment(HttpHelper.GetLoginUserEmail(HttpContext));
			}));
		}



		/// <summary>
		/// 归还设备
		/// </summary>
		/// <param name="id">id</param>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public IActionResult GiveBack(long id)
		{
			return View();
		}
	}
}