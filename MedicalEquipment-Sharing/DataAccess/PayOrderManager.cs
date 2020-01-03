using MedicalEquipment_Sharing.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

namespace MedicalEquipment_Sharing.DataAccess
{
	public class PayOrderManager
	{
		private string _ConnectionString;

		private readonly string _TableName = "PayOrders";

		private readonly string _SqlParamNum = "@OrderNum";
		private readonly string _SqlParamAccount = "@Account";

		/// <summary>
		/// 构造
		/// 获取配置文件中的数据库链接字符串
		/// </summary>
		/// <param name="env">IHostingEnvironment</param>
		public PayOrderManager(IConfiguration configuration)
		{
			_ConnectionString = configuration.GetConnectionString("DefaultConnection");
		}

		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="connectString">连接字符串</param>
		public PayOrderManager(string connectionString)
		{
			_ConnectionString = connectionString;
		}



		/// <summary>
		/// 生成支付订单
		/// </summary>
		/// <param name="order">PayOrder</param>
		/// <returns>成功与否</returns>
		public bool CreatePayOrder(PayOrder order)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "insert into " + _TableName
						+ " (" + nameof(PayOrder.OrderNum)
						+ ", " + nameof(PayOrder.OrderType)
						+ ", " + nameof(PayOrder.PaySuccess)
						+ ", " + nameof(PayOrder.Amount)
						+ ", " + nameof(PayOrder.Account) + ")"
						+ " values(" + _SqlParamNum
						+ ", " + Convert.ToString((int)order.OrderType)
						+ ", 0"
						+ ", " + Convert.ToString(order.Amount)
						+ ", " + _SqlParamAccount + ")";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamNum, order.OrderNum));
						command.Parameters.Add(new SqlParameter(_SqlParamAccount, order.Account));

						if (command.ExecuteNonQuery() <= 0)
						{
							return false;
						}
						return true;
					}
				}
			}
			catch
			{
				return false;
			}
		}



		/// <summary>
		/// 将订单设为支付成功
		/// </summary>
		/// <param name="orderNum">orderNum</param>
		/// <returns>成功与否</returns>
		public bool SavePaySuccess(string orderNum)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "update " + _TableName
						+ " set " + nameof(PayOrder.PaySuccess) + " = 1 "
						+ " where " + nameof(PayOrder.OrderNum)
						+ " = " + _SqlParamNum;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamNum, orderNum));

						if (command.ExecuteNonQuery() <= 0)
						{
							return false;
						}
						return true;
					}
				}
			}
			catch
			{
				return false;
			}
		}

	}
}
