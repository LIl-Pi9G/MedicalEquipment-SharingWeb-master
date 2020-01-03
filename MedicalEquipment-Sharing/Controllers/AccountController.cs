using MedicalEquipment_Sharing.DataAccess;
using MedicalEquipment_Sharing.Models;
using MedicalEquipment_Sharing.Models.ViewModels;
using MedicalEquipment_Sharing.Tools;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace MedicalEquipment_Sharing.Controllers
{
	public class AccountController : Controller
	{
		private readonly IConfiguration _Configuration;

		public AccountController(IConfiguration configuration)
		{
			_Configuration = configuration;
		}


		/// <summary>
		/// 账号主页
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login));
			}

			User user = await Task.Run(() =>
			{
				return new UserManager(_Configuration).QueryUser(HttpHelper.GetLoginUserEmail(HttpContext));
			});

			if (user == null)
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}
			else
			{
				return View(new UserInfoViewModel
				{
					Email = user.Email,
					UserName = user.UserName,
					TelePhone = user.TelePhone,
					Income = user.Income
				});
			}
		}


		/// <summary>
		/// 登出账号
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			return View();
		}


		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="returnUrl">登陆后返回的URL（需要经过URLEncode）</param>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public IActionResult Login(string returnUrl)
		{
			if (HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Index));
			}

			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}


		/// <summary>
		/// 登录post
		/// </summary>
		/// <param name="loginViewModel">登录信息</param>
		/// <param name="returnUrl">返回的URL（需要经过URLEncode）</param>
		/// <returns>ViewResult</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			UserManager userManager = new UserManager(_Configuration);
			if (!await Task.Run(() =>
			{
				return userManager.ValidateUser(new User
				{
					Email = loginViewModel.Email,
					Password = loginViewModel.Password
				});
			}))
			{
				ModelState.AddModelError(string.Empty, "邮箱或密码错误");
				return View();
			}

			User loginUser = await Task.Run(() =>
			{
				return userManager.QueryUser(loginViewModel.Email);
			});

			var claims = new[] {
				new Claim(ClaimTypes.Name, loginUser.UserName),
				new Claim(ClaimTypes.Email, loginUser.Email)};

			var claimsIdentity = new ClaimsIdentity(claims,
				CookieAuthenticationDefaults.AuthenticationScheme);

			ClaimsPrincipal user = new ClaimsPrincipal(claimsIdentity);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);

			if (!string.IsNullOrEmpty(returnUrl))
			{
				return RedirectToLocal(HttpUtility.UrlDecode(returnUrl));
			}
			return RedirectToAction(nameof(AccountController.Index));
		}


		/// <summary>
		/// 注册
		/// </summary>
		/// <returns>ViewResult</returns>
		[HttpGet]
		public IActionResult Sign(string returnUrl)
		{
			if (HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Index));
			}
			ViewData["ReturnUrl"] = returnUrl;

			return View();
		}


		/// <summary>
		/// 注册post
		/// 注册完成登录该账号
		/// </summary>
		/// <param name="signViewModel">注册信息</param>
		/// <returns>ViewResult</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Sign(SignViewModel signViewModel, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			if (await Task.Run(() =>
			{
				return new UserManager(_Configuration).IsExistTelePhone(signViewModel.TelePhone);
			}))
			{
				ModelState.AddModelError(string.Empty, "该号码已被使用");
				return View();
			}

			if (await Task.Run(() =>
			{
				return new UserManager(_Configuration).IsExistUserName(signViewModel.UserName);
			}))
			{
				ModelState.AddModelError(string.Empty, "该昵称已被使用");
				return View();
			}

			if (!await Task.Run(() =>
			{
				return new UserManager(_Configuration).CreateUser(signViewModel);
			}))
			{
				ModelState.AddModelError(string.Empty, "邮箱已被注册");
				return View();
			}

			return await Login(new LoginViewModel
			{
				Email = signViewModel.Email,
				Password = signViewModel.Password
			}, returnUrl);
		}



		/// <summary>
		/// 完成提现
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public IActionResult Transfer(string account, string amount)
		{
			ViewBag.Account = HttpUtility.UrlDecode(account);
			ViewBag.Amount = HttpUtility.UrlDecode(amount);
			return View();
		}



		/// <summary>
		/// 重定向到URL
		/// </summary>
		/// <param name="returnUrl">URL</param>
		/// <returns>ViewResult</returns>
		private IActionResult RedirectToLocal(string returnUrl)
		{
			returnUrl = HttpUtility.UrlDecode(returnUrl);
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
		}

	}
}