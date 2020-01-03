using MedicalEquipment_Sharing.Models;
using MedicalEquipment_Sharing.Models.Enum;
using MedicalEquipment_Sharing.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MedicalEquipment_Sharing.DataAccess
{
	public class OrderManager
	{
		private string _ConnectionString;

		private readonly string _TableName = "Orders";
		private readonly string _TableName_User = "Users";

		private readonly string _SqlParamSellerEmail = "@SellerEmail";
		private readonly string _SqlParamBuyerEmail = "@BuyerEmail";
		private readonly string _SqlParamPayOrderNum = "@PayOrderNum";
		private readonly string _SqlParamDepositOrderNum = "@DepositOrderNum";

		private readonly DateTime _DefaultDate = new DateTime();

		/// <summary>
		/// 构造
		/// 获取配置文件中的数据库链接字符串
		/// </summary>
		/// <param name="env">IHostingEnvironment</param>
		public OrderManager(IConfiguration configuration)
		{
			_ConnectionString = configuration.GetConnectionString("DefaultConnection");
		}
		
		
		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="connectString">连接字符串</param>
		public OrderManager(string connectString)
		{
			_ConnectionString = connectString;
		}


		/// <summary>
		/// 创建订单
		/// </summary>
		/// <param name="order">CreateOderViewModel</param>
		/// <returns>成功与否</returns>
		public bool CreateOrder(CreateOderViewModel order)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string shareDateString = $"'{order.SoldDate.Year}-{order.SoldDate.Month}-{order.SoldDate.Day} "
							+ $"{order.SoldDate.Hour}:{order.SoldDate.Minute}:{order.SoldDate.Second}'";

					string defaultDateString = $"'{_DefaultDate.Year}-{_DefaultDate.Month}-{_DefaultDate.Day} "
							+ $"{_DefaultDate.Hour}:{_DefaultDate.Minute}:{_DefaultDate.Second}'";

					string sqlCommand = "insert into "
						+ _TableName
						+ " (" + nameof(Order.EquipmentId)
						+ ", " + nameof(Order.SellerEmail)
						+ ", " + nameof(Order.SoldDate)
						+ ", " + nameof(Order.RevertedDate)
						+ ", " + nameof(Order.State)
						+ ", " + nameof(Order.BuyerEmail);
					if (!string.IsNullOrEmpty(order.PayOrderNum))
					{
						sqlCommand += ", " + nameof(Order.PayOrderNum);
					}
					if (!string.IsNullOrEmpty(order.DepositOrderNum))
					{
						sqlCommand += ", " + nameof(Order.DepositOrderNum);
					}
					sqlCommand += ", " + nameof(Order.PaySuccess)
						+ ", " + nameof(Order.PayDepositSuccess)
						+ ")"
						+ " values(" + Convert.ToString(order.EquipmentId)
						+ ", " + _SqlParamSellerEmail
						+ ", " + shareDateString
						+ ", " + defaultDateString
						+ ", " + Convert.ToString((int)order.State)
						+ ", " + _SqlParamBuyerEmail;
					if (!string.IsNullOrEmpty(order.PayOrderNum))
					{
						sqlCommand += ", " + _SqlParamPayOrderNum;
					}
					if (!string.IsNullOrEmpty(order.DepositOrderNum))
					{
						sqlCommand += ", " + _SqlParamDepositOrderNum;
					}
					sqlCommand += ", 0, 0)";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamSellerEmail, order.SellerEmail));
						command.Parameters.Add(new SqlParameter(_SqlParamBuyerEmail, order.BuyerEmail));
						if (!string.IsNullOrEmpty(order.PayOrderNum))
						{
							command.Parameters.Add(new SqlParameter(_SqlParamPayOrderNum, order.PayOrderNum));
						}
						if (!string.IsNullOrEmpty(order.DepositOrderNum))
						{
							command.Parameters.Add(new SqlParameter(_SqlParamDepositOrderNum, order.DepositOrderNum));
						}

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
		/// 查询设备状态
		/// </summary>
		/// <param name="equipmentId">设备ID</param>
		/// <returns>被借与否</returns>
		public OrderStatus QueryEquipmentStatus(long equipmentId)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select top 1 * from " + _TableName
						+ " where " + nameof(Order.EquipmentId) + " = " + Convert.ToString(equipmentId)
						+ " order by " + nameof(Order.OrderId) + " desc";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							OrderStatus status;
							if (reader.Read())
							{
								status = (OrderStatus)reader[nameof(Order.State)];

								if (status == OrderStatus.Sharing)
								{
									if (!(bool)reader[nameof(Order.PayDepositSuccess)])
									{
										throw new Exception();
									}
								}
								else if (status == OrderStatus.SoldOut)
								{
									if (!(bool)reader[nameof(Order.PaySuccess)])
									{
										throw new Exception();
									}
								}
							}
							else
							{
								throw new Exception();
							}
							return status;
						}
					}
				}
			}
			catch
			{
				return OrderStatus.Holding;
			}
		}



		/// <summary>
		/// 归还设备
		/// </summary>
		/// <param name="equipmentId">设备ID</param>
		/// <param name="payOrderNum"></param>
		/// <returns>成功与否</returns>
		public bool GiveBackEquipment(long equipmentId, string payOrderNum)
		{
			try
			{
				if (QueryEquipmentStatus(equipmentId) != OrderStatus.Sharing)
				{
					throw new Exception();
				}

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					DateTime date = DateTime.Now;
					string revertedDateStr = $"'{date.Year}-{date.Month}-{date.Day} {date.Hour}:{date.Minute}:{date.Second}'";

					string sqlCommand = "update " + _TableName
						+ " set " + nameof(Order.State) + " = " + Convert.ToString((int)OrderStatus.Holding)
						+ " , " + nameof(Order.RevertedDate) + " = " + revertedDateStr
						+ " , " + nameof(Order.PayOrderNum) + " = " + "@PayOrderNum"
						+ " , " + nameof(Order.PaySuccess) + " =1 "
						+ " where " + nameof(Order.EquipmentId) + " = " + Convert.ToString(equipmentId) + " and "
						+ nameof(Order.State) + " = " + Convert.ToString((int)OrderStatus.Sharing);

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter("@PayOrderNum", payOrderNum));

						if (command.ExecuteNonQuery() <= 0)
						{
							throw new Exception();
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
		/// 查询借用日期
		/// </summary>
		/// <param name="equipmentId">设备ID</param>
		/// <returns>DateTime</returns>
		public DateTime QuerySoldDate(long equipmentId)
		{
			try
			{
				DateTime dateTime;

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select top 1 " + nameof(Order.SoldDate)
						+ " from " + _TableName + " where " + nameof(Order.EquipmentId)
						+ " = " + Convert.ToString(equipmentId) + " and " + nameof(Order.State)
						+ " = " + Convert.ToString((int)OrderStatus.Sharing)
						+ " order by " + nameof(Order.OrderId) + " desc";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								dateTime = (DateTime)reader[nameof(Order.SoldDate)];
							}
							else
							{
								throw new Exception();
							}
							return dateTime;
						}
					}
				}
			}
			catch
			{
				return _DefaultDate;
			}
		}


		/// <summary>
		/// 查询购买者
		/// </summary>
		/// <param name="equipmentId"></param>
		/// <returns></returns>
		public User QueryBuyer(long equipmentId)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select * from " + _TableName_User
						+ " where " + nameof(User.Email) + " in ("
						+ "select top 1 " + nameof(Order.BuyerEmail)
						+ " from " + _TableName + " where " + nameof(Order.EquipmentId)
						+ " = " + Convert.ToString(equipmentId) + " and " + nameof(Order.State)
						+ " != " + Convert.ToString((int)OrderStatus.Holding)
						+ " order by " + nameof(Order.OrderId) + " desc)";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								return new User
								{
									UserName = Convert.ToString(reader[nameof(User.UserName)]),
									Email = Convert.ToString(reader[nameof(User.Email)]),
									TelePhone = Convert.ToString(reader[nameof(User.TelePhone)]),
									Password = string.Empty
								};
							}
							else
							{
								throw new Exception();
							}
						}
					}
				}
			}
			catch
			{
				return null;
			}
		}



		/// <summary>
		/// 通过支付单号删除订单
		/// </summary>
		/// <param name="payOrderNum"></param>
		/// <returns></returns>
		public bool DeleteOrderByPayOrderNumber(string payOrderNum)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "delete from " + _TableName
						+ " where " + nameof(Order.PayOrderNum)
						+ " = " + _SqlParamPayOrderNum;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamPayOrderNum, payOrderNum));

						return command.ExecuteNonQuery() > 0;
					}
				}
			}
			catch
			{
				return false;
			}
		}



		/// <summary>
		/// 查询卖家邮箱
		/// </summary>
		/// <param name="payOrderNum"></param>
		/// <returns></returns>
		public string QuerySellerEmailByPayOrderNum(string payOrderNum)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select * from " + _TableName
						+ " where " + nameof(Order.PayOrderNum)
						+ " = " + _SqlParamPayOrderNum;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamPayOrderNum, payOrderNum));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								return Convert.ToString(reader[nameof(Order.SellerEmail)]);
							}
							else
							{
								throw new Exception();
							}
						}
					}
				}
			}
			catch
			{
				return null;
			}
		}



		/// <summary>
		/// 保存支付成功
		/// </summary>
		/// <param name="payOrderNum"></param>
		/// <returns></returns>
		public bool SavePaySuccess(string payOrderNum)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "update " + _TableName
						+ " set " + nameof(Order.PaySuccess) + "=1"
						+ " where " + nameof(Order.PayOrderNum)
						+ " = " + _SqlParamPayOrderNum;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamPayOrderNum, payOrderNum));

						return command.ExecuteNonQuery() > 0;
					}
				}
			}
			catch
			{
				return false;
			}
		}



		/// <summary>
		/// 保存支付押金成功
		/// </summary>
		/// <param name="payOrderNum"></param>
		/// <returns></returns>
		public bool SavePayDepositSuccess(string payOrderNum)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "update " + _TableName
						+ " set " + nameof(Order.PayDepositSuccess) + "=1"
						+ " where " + nameof(Order.DepositOrderNum)
						+ " = " + _SqlParamPayOrderNum;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamPayOrderNum, payOrderNum));

						return command.ExecuteNonQuery() > 0;
					}
				}
			}
			catch
			{
				return false;
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="orderNum"></param>
		/// <returns></returns>
		public bool SavePayOrderNum(long id, string orderNum)
		{
			Order order = QueryOrder(id);
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "update " + _TableName
						+ " set " + nameof(Order.PayOrderNum) + " = " + _SqlParamPayOrderNum
						+ " where " + nameof(Order.OrderId) + " = " + order.OrderId;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamPayOrderNum, orderNum));

						return command.ExecuteNonQuery() > 0;
					}
				}
			}
			catch
			{
				return false;
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="equipmentId"></param>
		/// <returns></returns>
		public Order QueryOrder(long equipmentId)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select top 1 * from " + _TableName
						+ " where " + nameof(Order.EquipmentId) + " = " + Convert.ToString(equipmentId)
						+ " order by " + nameof(Order.OrderId) + " desc";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								return new Order
								{
									OrderId = Convert.ToInt64(reader[nameof(Order.OrderId)]),
									EquipmentId = Convert.ToInt64(reader[nameof(Order.EquipmentId)]),
									BuyerEmail = Convert.ToString(reader[nameof(Order.BuyerEmail)]),
									SellerEmail = Convert.ToString(reader[nameof(Order.SellerEmail)]),
									SoldDate = (DateTime)reader[nameof(Order.SoldDate)],
									State = (OrderStatus)reader[nameof(Order.State)],
									DepositOrderNum = Convert.ToString(reader[nameof(Order.DepositOrderNum)]),
									PayOrderNum = Convert.ToString(reader[nameof(Order.PayOrderNum)]),
									PayDepositSuccess = Convert.ToBoolean(reader[nameof(Order.PayDepositSuccess)]),
									PaySuccess = Convert.ToBoolean(reader[nameof(Order.PaySuccess)])
								};
							}
							else
							{
								throw new Exception();
							}
						}
					}
				}
			}
			catch
			{
				return null;
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="equipmentId"></param>
		/// <returns></returns>
		public Order QueryOrderByPayOrderNum(string payOrderNum)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select top 1 * from " + _TableName
						+ " where " + nameof(Order.PayOrderNum) + " = " + _SqlParamPayOrderNum
						+ " order by " + nameof(Order.OrderId) + " desc";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamPayOrderNum, payOrderNum));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								return new Order
								{
									OrderId = Convert.ToInt64(reader[nameof(Order.OrderId)]),
									EquipmentId = Convert.ToInt64(reader[nameof(Order.EquipmentId)]),
									BuyerEmail = Convert.ToString(reader[nameof(Order.BuyerEmail)]),
									SellerEmail = Convert.ToString(reader[nameof(Order.SellerEmail)]),
									SoldDate = (DateTime)reader[nameof(Order.SoldDate)],
									State = (OrderStatus)reader[nameof(Order.State)],
									DepositOrderNum = Convert.ToString(reader[nameof(Order.DepositOrderNum)]),
									PayOrderNum = Convert.ToString(reader[nameof(Order.PayOrderNum)]),
									PayDepositSuccess = Convert.ToBoolean(reader[nameof(Order.PayDepositSuccess)]),
									PaySuccess = Convert.ToBoolean(reader[nameof(Order.PaySuccess)])
								};
							}
							else
							{
								throw new Exception();
							}
						}
					}
				}
			}
			catch
			{
				return null;
			}
		}

	} // end of class
}
