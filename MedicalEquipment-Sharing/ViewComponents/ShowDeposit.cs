using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace MedicalEquipment_Sharing.ViewComponents
{
	public class ShowDeposit : ViewComponent
	{
		private readonly double _Deposit;

		public ShowDeposit(IConfiguration configuration)
		{
			_Deposit = Convert.ToDouble(configuration["Deposit"]);
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			ViewBag.Pe = Math.Round(_Deposit, 2).ToString("p");
			return await Task.Run(() => { return View(); });
		}
	}
}
