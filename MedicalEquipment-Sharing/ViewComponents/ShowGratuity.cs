using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace MedicalEquipment_Sharing.ViewComponents
{
	public class ShowGratuity : ViewComponent
	{
		private readonly double _Gratuity;

		public ShowGratuity(IConfiguration configuration)
		{
			_Gratuity = Convert.ToDouble(configuration["Gratuity"]);
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			ViewBag.Pe = Math.Round(_Gratuity, 2).ToString("p");
			return await Task.Run(() => { return View(); });
		}
	}
}
