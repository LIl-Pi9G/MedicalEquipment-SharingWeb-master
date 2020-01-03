using MedicalEquipment_Sharing.Models.Enum;
using System;

namespace MedicalEquipment_Sharing.Tools
{
	public class PayHelper
	{
		/// <summary>
		/// 生成订单号，前缀+年月日时分秒毫秒
		/// </summary>
		/// <param name="orderType"></param>
		/// <returns></returns>
		public static string GeneratePayOrderNum(PayOrderType orderType)
		{
			DateTime n = DateTime.Now;
			switch (orderType)
			{
				case PayOrderType.Transfer:
					return "T" + $"{n.Year}{n.Month}{n.Day}{n.Hour}{n.Minute}{n.Second}{n.Millisecond}";
				case PayOrderType.Borrow:
					return "B" + $"{n.Year}{n.Month}{n.Day}{n.Hour}{n.Minute}{n.Second}{n.Millisecond}";
				case PayOrderType.Sold:
					return "S" + $"{n.Year}{n.Month}{n.Day}{n.Hour}{n.Minute}{n.Second}{n.Millisecond}";
				default:
					throw new Exception();
			}
		}


		/// <summary>
		/// 根据前缀判断订单类型
		/// </summary>
		/// <param name="orderNum"></param>
		/// <returns></returns>
		public static PayOrderType ObtainPayOrderType(string orderNum)
		{
			if (orderNum.StartsWith("T"))
			{
				return PayOrderType.Transfer;
			}
			if (orderNum.StartsWith("B"))
			{
				return PayOrderType.Borrow;
			}
			if (orderNum.StartsWith("S"))
			{
				return PayOrderType.Sold;
			}
			throw new Exception();
		}
	}
}
