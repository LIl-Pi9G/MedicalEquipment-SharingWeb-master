using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.Core.Response;
using MedicalEquipment_Sharing.DataAccess;
using MedicalEquipment_Sharing.Models;
using MedicalEquipment_Sharing.Models.Enum;
using MedicalEquipment_Sharing.Models.ViewModels;
using MedicalEquipment_Sharing.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace MedicalEquipment_Sharing.Controllers
{
	public class PayController : Controller
    {
		private readonly IConfiguration _Configuration;
		private readonly AlipayService _AlipayService;

		/// <summary>
		/// 手续费百分百（支付价格）
		/// </summary>
		private readonly double _Gratuity;

		/// <summary>
		/// 押金百分百（设备售价）
		/// </summary>
		private readonly double _Deposit;

		/// <summary>
		/// 构造
		/// </summary>
		public PayController(AlipayService alipayService, IConfiguration configuration)
		{
			_Configuration = configuration;
			_AlipayService = alipayService;
			_Gratuity = Convert.ToDouble(configuration["Gratuity"]);
			_Deposit = Convert.ToDouble(configuration["Deposit"]);
		}



		/// <summary>
		/// 借用
		/// </summary>
		/// <param name="id"></param>
		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> Borrow(long id)
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode($"/Home/Details/{id}") });
			}

			if (await Task.Run(() =>
			{
				return new OrderManager(_Configuration).QueryEquipmentStatus(id) != OrderStatus.Holding;
			}))
			{
				return RedirectToAction(nameof(HomeController.SoldOut), "Home");
			}

			string orderNum = PayHelper.GeneratePayOrderNum(PayOrderType.Borrow);

			EquipmentViewModel equipment = new EquipmentManager(_Configuration).QueryEquipment(id);

			if (HttpHelper.GetLoginUserEmail(HttpContext).Equals(equipment.Owner.Email))
			{
				return RedirectToAction(nameof(HomeController.Details), "Home", new { id });
			}

			decimal amount = equipment.SoldPrice * Convert.ToDecimal(_Deposit);

			if (!await Task.Run(() =>
			{
				return new PayOrderManager(_Configuration).CreatePayOrder(new PayOrder
				{
					Amount = amount,
					OrderType = PayOrderType.Borrow,
					OrderNum = orderNum,
					Account = HttpHelper.GetLoginUserEmail(HttpContext)
				});
			}))
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}

			if (!await Task.Run(() =>
			{
				return new OrderManager(_Configuration).CreateOrder(new CreateOderViewModel
				{
					BuyerEmail = HttpHelper.GetLoginUserEmail(HttpContext),
					EquipmentId = equipment.Id,
					DepositOrderNum = orderNum,
					State = OrderStatus.Sharing,
					SoldDate = DateTime.Now,
					SellerEmail = equipment.Owner.Email
				});
			}))
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}


			AlipayTradePagePayModel model = new AlipayTradePagePayModel
			{
				Subject = "租赁设备押金",
				Body = $"租赁设备:{equipment.Owner.UserName}共享的{equipment.Name}",
				TotalAmount = Math.Round(amount, 2).ToString(),
				OutTradeNo = orderNum,
				//电脑支付
				ProductCode = "FAST_INSTANT_TRADE_PAY"
			};

			AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();

			// 设置同步回调地址
			request.SetReturnUrl($"http://{Request.Host}/Pay/Callback");

			// 设置异步通知接收地址
			//request.SetNotifyUrl("");

			request.SetBizModel(model);

			var response = _AlipayService.SdkExecute(request);

			//重定向到支付宝支付
			return Redirect(_AlipayService.Options.Gatewayurl + "?" + response.Body);
		}



		/// <summary>
		/// 购买
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost]
		[AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Buy(long id)
        {
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode($"/Home/Details/{id}") });
			}

			if (await Task.Run(() =>
			{
				return new OrderManager(_Configuration).QueryEquipmentStatus(id) != OrderStatus.Holding;
			}))
			{
				return RedirectToAction(nameof(HomeController.SoldOut), "Home");
			}

			string orderNum = PayHelper.GeneratePayOrderNum(PayOrderType.Sold);

			EquipmentViewModel equipment = new EquipmentManager(_Configuration).QueryEquipment(id);

			if (HttpHelper.GetLoginUserEmail(HttpContext).Equals(equipment.Owner.Email))
			{
				return RedirectToAction(nameof(HomeController.Details), "Home", new { id });
			}

			decimal amount = equipment.SoldPrice;

			if (!await Task.Run(() =>
			{
				return new PayOrderManager(_Configuration).CreatePayOrder(new PayOrder
				{
					Amount = amount,
					OrderType = PayOrderType.Sold,
					OrderNum = orderNum,
					Account = HttpHelper.GetLoginUserEmail(HttpContext)
				});
			}))
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}

			if (!await Task.Run(() =>
			{
				return new OrderManager(_Configuration).CreateOrder(new CreateOderViewModel
				{
					BuyerEmail = HttpHelper.GetLoginUserEmail(HttpContext),
					SellerEmail = equipment.Owner.Email,
					EquipmentId = equipment.Id,
					PayOrderNum = orderNum,
					State = OrderStatus.SoldOut,
					SoldDate = DateTime.Now
				});
			}))
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}

			AlipayTradePagePayModel model = new AlipayTradePagePayModel
			{
				Subject = "购买设备",
				Body = $"购买设备:{equipment.Owner.UserName}共享的{equipment.Name}",
				TotalAmount = Math.Round(amount, 2).ToString(),
				OutTradeNo = orderNum,
				//电脑支付
				ProductCode = "FAST_INSTANT_TRADE_PAY"
			};
			
			AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();

			// 设置同步回调地址
			request.SetReturnUrl($"http://{Request.Host}/Pay/Callback");

			// 设置异步通知接收地址
			//request.SetNotifyUrl("");
			
			request.SetBizModel(model);

			var response = _AlipayService.SdkExecute(request);

			//重定向到支付宝支付
			return Redirect(_AlipayService.Options.Gatewayurl + "?" + response.Body);
		}



		/// <summary>
		/// 获取请求参数
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> GetRequestGet()
		{
			Dictionary<string, string> sArray = new Dictionary<string, string>();

			ICollection<string> requestItem = Request.Query.Keys;
			foreach (var item in requestItem)
			{
				sArray.Add(item, Request.Query[item]);
			}

			return sArray;
		}



		/// <summary>
		/// 查询订单状态和支付金额
		/// </summary>
		/// <param name="orderNum"></param>
		/// <returns></returns>
		private async Task<(AlipayTradeStatus, decimal)> QueryOrder(string orderNum)
		{
			AlipayTradeQueryModel model = new AlipayTradeQueryModel
			{
				OutTradeNo = orderNum
			};

			AlipayTradeQueryRequest request = new AlipayTradeQueryRequest();
			request.SetBizModel(model);

			AlipayTradeQueryResponse response = await _AlipayService.ExecuteAsync(request);

			if ("TRADE_CLOSED".Equals(response.TradeStatus))
			{
				return (AlipayTradeStatus.TRADE_CLOSED, Convert.ToDecimal(response.TotalAmount));
			}
			else if ("TRADE_SUCCESS".Equals(response.TradeStatus))
			{
				return (AlipayTradeStatus.TRADE_SUCCESS, Convert.ToDecimal(response.TotalAmount));
			}
			else if ("WAIT_BUYER_PAY".Equals(response.TradeStatus))
			{
				return (AlipayTradeStatus.WAIT_BUYER_PAY, Convert.ToDecimal(response.TotalAmount));
			}
			else if ("TRADE_FINISHED".Equals(response.TradeStatus))
			{
				return (AlipayTradeStatus.TRADE_FINISHED, Convert.ToDecimal(response.TotalAmount));
			}
			else
			{
				return (AlipayTradeStatus.TRADE_CLOSED, Convert.ToDecimal(response.TotalAmount));
			}
		}



		/// <summary>
		/// 借用、购买回调
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> Callback()
		{
			Dictionary<string, string> sArray = GetRequestGet();

			string orderNum = sArray["out_trade_no"];
			var (tradeStatus, a) = await QueryOrder(orderNum);
			if (tradeStatus == AlipayTradeStatus.TRADE_SUCCESS)
			{
				double totalAmount = Convert.ToDouble(sArray["total_amount"]);
				decimal income = Convert.ToDecimal(totalAmount * (1 - _Gratuity));
				if (income == 0)
				{
					income = Convert.ToDecimal(totalAmount);
				}
				PayOrderType type = PayHelper.ObtainPayOrderType(orderNum);

				if (sArray.Count != 0)
				{
					if (_AlipayService.RSACheckV1(sArray))
					{
						if (type == PayOrderType.Sold)
						{
							if (new PayOrderManager(_Configuration).SavePaySuccess(orderNum)
							&& new OrderManager(_Configuration).SavePaySuccess(orderNum))
							{
								if (new UserManager(_Configuration).AddIncome(
									new OrderManager(_Configuration).QuerySellerEmailByPayOrderNum(orderNum), income))
								{
									return RedirectToAction(nameof(HomeController.Purchase), "Home");
								}
							}
						}
						else if (type == PayOrderType.Borrow)
						{
							if (new PayOrderManager(_Configuration).SavePaySuccess(orderNum)
							&& new OrderManager(_Configuration).SavePayDepositSuccess(orderNum))
							{
								return RedirectToAction(nameof(HomeController.Borrow), "Home");
							}
						}
					}
				}
			}

			new OrderManager(_Configuration).DeleteOrderByPayOrderNumber(orderNum);
			return RedirectToAction(nameof(ErrorController.PayFaild), "Error");
		}



		/// <summary>
		/// 提现到支付宝账户
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> Transfer()
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode("/Account") });
			}

			UserManager userManager = new UserManager(_Configuration);
			User user = await Task.Run(() =>
			{
				return userManager.QueryUser(HttpHelper.GetLoginUserEmail(HttpContext));
			});

			if (user == null)
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}

			if (user.Income <= 0)
			{
				return RedirectToAction(nameof(AccountController.Index), "Account");
			}

			string amount = Math.Round(user.Income, 2).ToString();
			string account = user.TelePhone;

			AlipayFundTransToaccountTransferModel model = new AlipayFundTransToaccountTransferModel
			{
				OutBizNo = PayHelper.GeneratePayOrderNum(PayOrderType.Transfer),
				PayeeType = "ALIPAY_LOGONID",
				PayeeAccount = account,
				Amount = amount,
				Remark = "共享医疗设备收益"
			};

			AlipayFundTransToaccountTransferRequest request = new AlipayFundTransToaccountTransferRequest();
			request.SetBizModel(model);

			AlipayFundTransToaccountTransferResponse response = await _AlipayService.ExecuteAsync(request);

			if ("Success".Equals(response.Msg))
			{
				if (userManager.ClearIncome(user.Email))
				{
					return RedirectToAction(nameof(AccountController.Transfer), "Account",
					new { account = HttpUtility.UrlEncode(account), amount = HttpUtility.UrlEncode(amount) });
				}
			}
			
			return RedirectToAction(nameof(ErrorController.TransferFaild), "Error");

		}



		/// <summary>
		/// 支付租金
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> GiveBack(long id)
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode($"/Home/Details/{id}") });
			}

			if (await Task.Run(() =>
			{
				return new OrderManager(_Configuration).QueryEquipmentStatus(id) != OrderStatus.Sharing;
			}))
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}


			Order order = new OrderManager(_Configuration).QueryOrder(id);
			TimeSpan span = DateTime.Now.Subtract(order.SoldDate);
			int xDay = span.Days + 1;

			EquipmentViewModel equipment = new EquipmentManager(_Configuration).QueryEquipment(id);

			string orderNum = PayHelper.GeneratePayOrderNum(PayOrderType.Borrow);

			if (!HttpHelper.GetLoginUserEmail(HttpContext).Equals(equipment.Buyer.Email))
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}

			decimal payAmount = equipment.Price * xDay;

			if (!await Task.Run(() =>
			{
				return new PayOrderManager(_Configuration).CreatePayOrder(new PayOrder
				{
					Amount = payAmount,
					OrderType = PayOrderType.Borrow,
					OrderNum = orderNum,
					Account = HttpHelper.GetLoginUserEmail(HttpContext)
				});
			}))
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}

			if (!await Task.Run(() =>
			{
				return new OrderManager(_Configuration).SavePayOrderNum(id, orderNum);
			}))
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}

			AlipayTradePagePayModel model = new AlipayTradePagePayModel
			{
				Subject = "支付租金",
				Body = $"支付租金:{equipment.Owner.UserName}共享的{equipment.Name}",
				TotalAmount = Math.Round(payAmount, 2).ToString(),
				OutTradeNo = orderNum,
				//电脑支付
				ProductCode = "FAST_INSTANT_TRADE_PAY"
			};

			AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();

			// 设置同步回调地址
			request.SetReturnUrl($"http://{Request.Host}/Pay/CallBackGiveBack");

			// 设置异步通知接收地址
			//request.SetNotifyUrl("");

			request.SetBizModel(model);

			var response = _AlipayService.SdkExecute(request);

			//重定向到支付宝支付
			return Redirect(_AlipayService.Options.Gatewayurl + "?" + response.Body);
		}



		/// <summary>
		/// 支付租金回调
		/// 支付成功后返还押金
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> CallBackGiveBack()
		{
			Dictionary<string, string> sArray = GetRequestGet();

			string orderNum = sArray["out_trade_no"];
			double totalAmount = Convert.ToDouble(sArray["total_amount"]);
			decimal income = Convert.ToDecimal(totalAmount * (1 - _Gratuity));

			if (income == 0)
			{
				income = Convert.ToDecimal(totalAmount);
			}

			if (sArray.Count != 0)
			{
				if (_AlipayService.RSACheckV1(sArray))
				{
					var (tradeStatus, a) = await QueryOrder(orderNum);
					if (tradeStatus == AlipayTradeStatus.TRADE_SUCCESS)
					{
						if (new PayOrderManager(_Configuration).SavePaySuccess(orderNum)
							&& new OrderManager(_Configuration).SavePaySuccess(orderNum))
						{
							if (new UserManager(_Configuration).AddIncome(
								new OrderManager(_Configuration)
								.QuerySellerEmailByPayOrderNum(orderNum), income))
							{
								Order order = new OrderManager(_Configuration)
									.QueryOrderByPayOrderNum(orderNum);

								if (order != null)
								{
									var (status, amount) = await QueryOrder(order.DepositOrderNum);
									if (status == AlipayTradeStatus.TRADE_SUCCESS)
									{
										var model = new AlipayTradeRefundModel
										{
											OutTradeNo = order.DepositOrderNum,
											RefundAmount = Convert.ToString(amount),
											RefundReason = "租赁押金退还"
										};

										var request = new AlipayTradeRefundRequest();
										request.SetBizModel(model);

										var response = await _AlipayService.ExecuteAsync(request);

										if ("Success".Equals(response.Msg))
										{
											if (new OrderManager(_Configuration)
												.GiveBackEquipment(order.EquipmentId, orderNum))
											{
												return RedirectToAction(nameof(EquipmentController.GiveBack), "Equipment");
											}
										}
									}
								}
							}
						}
					}
				}
			}

			return RedirectToAction(nameof(ErrorController.PayFaild), "Error");
		}



		/// <summary>
		/// 确认订单
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public IActionResult Confirm(long id, PayOrderType type)
		{
			if (!HttpHelper.IsLogin(HttpContext))
			{
				return RedirectToAction(nameof(AccountController.Login), "Account",
					new { returnUrl = HttpUtility.UrlEncode($"/Home/Details/{id}") });
			}

			EquipmentViewModel equipment = new EquipmentManager(_Configuration).QueryEquipment(id);

			double amount;
			if (type == PayOrderType.Borrow)
			{
				amount = Math.Round(Convert.ToDouble(equipment.SoldPrice) * _Deposit, 2);
			}
			else if (type == PayOrderType.Sold)
			{
				amount = Math.Round(Convert.ToDouble(equipment.SoldPrice), 2);
			}
			else
			{
				return RedirectToAction(nameof(ErrorController.Index), "Error");
			}

			return View(new ConfirmOrderViewModel
			{
				Type = type,
				EquipmentId = id,
				TotalAmount = amount,
				Subject = equipment.Name,
				Body = equipment.Describe
			});
		}

	}
}