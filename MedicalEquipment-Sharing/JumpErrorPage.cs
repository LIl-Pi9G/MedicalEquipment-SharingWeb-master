using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MedicalEquipment_Sharing
{
	public class JumpErrorPage
	{
		private readonly RequestDelegate next;

		public JumpErrorPage(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			await next.Invoke(context);

			HttpResponse response = context.Response;

			if (response.StatusCode == 404)
			{
				response.Redirect("/Error/NotFound");
			}
		}
	}
}
