using MedicalEquipment_Sharing.Models;
using MedicalEquipment_Sharing.Models.Enum;
using MedicalEquipment_Sharing.Models.ViewModels;
using MedicalEquipment_Sharing.Tools;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MedicalEquipment_Sharing.DataAccess
{
	public class EquipmentManager
	{
		private readonly string _ConnectionString;

		private readonly string _TableName = "MedicalEquipments";
		private readonly string _TableName_User = "Users";
		private readonly string _TableName_Order = "Orders";

		private readonly string _SqlParamName = "@Name";
		private readonly string _SqlParamDescribe = "@Describe";
		private readonly string _SqlParamImageUrl = "@ImageUrl";
		private readonly string _SqlParamUserEmail = "@UserEmail";

		/// <summary>
		/// 构造
		/// 获取配置文件中的数据库链接字符串
		/// </summary>
		/// <param name="env">IConfiguration</param>
		public EquipmentManager(IConfiguration configuration)
		{
			_ConnectionString = configuration.GetConnectionString("DefaultConnection");
		}


		
		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="connectionString">连接字符串</param>
		public EquipmentManager(string connectionString)
		{
			_ConnectionString = connectionString;
		}



		/// <summary>
		/// 插入一个医疗设备
		/// </summary>
		/// <param name="equipment">MedicalEquipment</param>
		/// <returns>成功与否</returns>
		public bool CreateEquipment(MedicalEquipment equipment)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "insert into " + _TableName
						+ " (" + nameof(MedicalEquipment.Name)
						+ ", " + nameof(MedicalEquipment.Price)
						+ ", " + nameof(MedicalEquipment.Describe)
						+ ", " + nameof(MedicalEquipment.ImageUrl)
						+ ", " + nameof(MedicalEquipment.UserEmail)
						+ ", " + nameof(MedicalEquipment.SoldPrice)
						+ ", " + nameof(MedicalEquipment.OriginalPrice)
						+ ", " + nameof(MedicalEquipment.Type)
						+ ", " + nameof(MedicalEquipment.Manufacturer)
						+ ") values(" + _SqlParamName
						+ ", " + Convert.ToString(equipment.Price)
						+ ", " + _SqlParamDescribe
						+ ", " + _SqlParamImageUrl
						+ ", " + _SqlParamUserEmail
						+ ", " + Convert.ToString(equipment.SoldPrice)
						+ ", " + Convert.ToString(equipment.OriginalPrice)
						+ ", " + Convert.ToString((int)equipment.Type)
						+ ", " + Convert.ToString((int)equipment.Manufacturer) + ")";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamName, equipment.Name));
						command.Parameters.Add(new SqlParameter(_SqlParamDescribe, equipment.Describe));
						command.Parameters.Add(new SqlParameter(_SqlParamImageUrl, equipment.ImageUrl));
						command.Parameters.Add(new SqlParameter(_SqlParamUserEmail, equipment.UserEmail));

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
		/// 删除一个医疗设备
		/// </summary>
		/// <param name="id">id</param>
		/// <returns>成功与否</returns>
		public bool DeleteEquipment(long id)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "delete from " + _TableName
						+ " where " + nameof(MedicalEquipment.Id)
						+ " = " + Convert.ToString(id);

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						if (command.ExecuteNonQuery() <= 0)
						{
							return false;
						}
					}
					return true;
				}
			}
			catch
			{
				return false;
			}
		}


		/// <summary>
		/// 获取设备图片路径
		/// </summary>
		/// <param name="id">设备ID</param>
		/// <returns>图片路径</returns>
		public string GetEquipmentImgPath(long id)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select " + nameof(MedicalEquipment.ImageUrl)
						+ " from " + _TableName
						+ " where " + nameof(MedicalEquipment.Id)
						+ " = " + Convert.ToString(id);

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							string imgPath;
							if (reader.Read())
							{
								imgPath = Convert.ToString(reader[nameof(MedicalEquipment.ImageUrl)]);
							}
							else
							{
								throw new Exception();
							}
							return imgPath;
						}
					}
				}
			}
			catch
			{
				return string.Empty;
			}
		}


		/// <summary>
		/// 更新一个医疗设备信息
		/// </summary>
		/// <param name="equipment">更新数据</param>
		/// <param name="id">更新的行id</param>
		public bool UpdateEquipment(EquipmentViewModel equipment, long id)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "update " + _TableName
						+ " set " + nameof(MedicalEquipment.Name) + " = " + _SqlParamName + ", "
						+ nameof(MedicalEquipment.Price) + " = " + Convert.ToString(equipment.Price) + ", "
						+ nameof(MedicalEquipment.Describe) + " = " + _SqlParamDescribe + ", "
						+ nameof(MedicalEquipment.ImageUrl) + " = " + _SqlParamImageUrl + ", "
						+ nameof(MedicalEquipment.SoldPrice) + " = " + Convert.ToString(equipment.SoldPrice) + ", "
						+ nameof(MedicalEquipment.OriginalPrice) + " = " + Convert.ToString(equipment.OriginalPrice) + ", "
						+ nameof(MedicalEquipment.Type) + " = " + Convert.ToString((int)equipment.Type) + ", "
						+ nameof(MedicalEquipment.Manufacturer) + " = " + Convert.ToString((int)equipment.Manufacturer)
						+ " where " + nameof(MedicalEquipment.Id) + " = " + Convert.ToString(id);

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamName, equipment.Name));
						command.Parameters.Add(new SqlParameter(_SqlParamDescribe, equipment.Describe));
						command.Parameters.Add(new SqlParameter(_SqlParamImageUrl, equipment.ImageUrl));

						if (command.ExecuteNonQuery() <= 0)
						{
							return false;
						}
					}
					return true;
				}
			}
			catch
			{
				return false;
			}
		}


		/// <summary>
		/// 获取用户名下的设备列表
		/// </summary>
		/// <param name="email">账号</param>
		/// <returns>List<EquipmentViewModel></returns>
		public List<EquipmentViewModel> QueryEquipmentByEmail(string email)
		{
			try
			{
				List<EquipmentViewModel> equipments = new List<EquipmentViewModel>();

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select * from " + _TableName
						+ " inner join " + _TableName_User
						+ " on " + _TableName + "." + nameof(MedicalEquipment.UserEmail)
						+ " = " + _TableName_User + "." + nameof(User.Email)
						+ " where " + nameof(MedicalEquipment.UserEmail)
						+ " = " + _SqlParamUserEmail;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamUserEmail, email));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							OrderManager orderManager = new OrderManager(_ConnectionString);
							while (reader.Read())
							{
								equipments.Add(new EquipmentViewModel
								{
									Id = Convert.ToInt64(reader[nameof(MedicalEquipment.Id)]),
									Name = Convert.ToString(reader[nameof(MedicalEquipment.Name)]),
									Price = Convert.ToDecimal(reader[nameof(MedicalEquipment.Price)]),
									Describe = Convert.ToString(reader[nameof(MedicalEquipment.Describe)]),
									ImageUrl = Convert.ToString(reader[nameof(MedicalEquipment.ImageUrl)]),
									Owner = new User
									{
										Email = Convert.ToString(reader[nameof(User.Email)]),
										UserName = Convert.ToString(reader[nameof(User.UserName)]),
										TelePhone = Convert.ToString(reader[nameof(User.TelePhone)]),
										Password = string.Empty
									},
									EquipmentState = orderManager.QueryEquipmentStatus(Convert.ToInt64(reader[nameof(MedicalEquipment.Id)])),
									SoldDate = orderManager.QuerySoldDate((long)reader[nameof(MedicalEquipment.Id)]),
									SoldPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.SoldPrice)]),
									OriginalPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.OriginalPrice)]),
									Type = (EquipmentType)reader[nameof(MedicalEquipment.Type)],
									Manufacturer = (EquipmentManufacturer)reader[nameof(MedicalEquipment.Manufacturer)],
									Buyer = orderManager.QueryBuyer((long)reader[nameof(MedicalEquipment.Id)])
								});
							}
							orderManager = null;
							return equipments;
						}
					}
				}
			}
			catch
			{
				return new List<EquipmentViewModel>();
			}
		}



		/// <summary>
		/// 查询设备
		/// </summary>
		/// <param name="email">用户邮箱</param>
		/// <param name="value">查询的数据</param>
		/// <returns>List<EquipmentViewModel></returns>
		public EquipmentViewModel QueryEquipment(string email, long id)
		{
			try
			{
				EquipmentViewModel equipment = new EquipmentViewModel();

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select "
						+ _TableName + ".*, " + _TableName_User + ".*"
						+ " from " + _TableName
						+ " inner join " + _TableName_User
						+ " on " + _TableName + "." + nameof(MedicalEquipment.UserEmail)
						+ " = " + _TableName_User + "." + nameof(User.Email)
						+ " where " + nameof(MedicalEquipment.Id) + " = " + Convert.ToString(id)
						+ " and " + nameof(MedicalEquipment.UserEmail) + " = "
						+ _SqlParamUserEmail;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamUserEmail, email));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							OrderManager orderManager = new OrderManager(_ConnectionString);

							if (reader.Read())
							{
								equipment = new EquipmentViewModel
								{
									Id = Convert.ToInt64(reader[nameof(MedicalEquipment.Id)]),
									Name = Convert.ToString(reader[nameof(MedicalEquipment.Name)]),
									Price = Convert.ToDecimal(reader[nameof(MedicalEquipment.Price)]),
									Describe = Convert.ToString(reader[nameof(MedicalEquipment.Describe)]),
									ImageUrl = Convert.ToString(reader[nameof(MedicalEquipment.ImageUrl)]),
									Owner = new User
									{
										Email = Convert.ToString(reader[nameof(User.Email)]),
										UserName = Convert.ToString(reader[nameof(User.UserName)]),
										TelePhone = Convert.ToString(reader[nameof(User.TelePhone)]),
										Password = string.Empty
									},
									EquipmentState = orderManager.QueryEquipmentStatus((long)reader[nameof(MedicalEquipment.Id)]),
									SoldDate = orderManager.QuerySoldDate((long)reader[nameof(MedicalEquipment.Id)]),
									SoldPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.SoldPrice)]),
									OriginalPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.OriginalPrice)]),
									Type = (EquipmentType)reader[nameof(MedicalEquipment.Type)],
									Manufacturer = (EquipmentManufacturer)reader[nameof(MedicalEquipment.Manufacturer)],
									Buyer = orderManager.QueryBuyer((long)reader[nameof(MedicalEquipment.Id)])
								};
								orderManager = null;
							}
							else
							{
								orderManager = null;
								throw new Exception();
							}
							return equipment;
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
		/// 查询设备
		/// </summary>
		/// <param name="email">用户邮箱</param>
		/// <param name="value">查询的数据</param>
		/// <returns>List<EquipmentViewModel></returns>
		public EquipmentViewModel QueryEquipment(long id)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select "
						+ _TableName + ".*, " + _TableName_User + ".*"
						+ " from " + _TableName
						+ " inner join " + _TableName_User
						+ " on " + _TableName + "." + nameof(MedicalEquipment.UserEmail)
						+ " = " + _TableName_User + "." + nameof(User.Email)
						+ " where " + nameof(MedicalEquipment.Id) + " = " + Convert.ToString(id);

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							OrderManager orderManager = new OrderManager(_ConnectionString);

							EquipmentViewModel equipment;
							if (reader.Read())
							{
								equipment = new EquipmentViewModel
								{
									Id = Convert.ToInt64(reader[nameof(MedicalEquipment.Id)]),
									Name = Convert.ToString(reader[nameof(MedicalEquipment.Name)]),
									Price = Convert.ToDecimal(reader[nameof(MedicalEquipment.Price)]),
									Describe = Convert.ToString(reader[nameof(MedicalEquipment.Describe)]),
									ImageUrl = Convert.ToString(reader[nameof(MedicalEquipment.ImageUrl)]),
									Owner = new User
									{
										Email = Convert.ToString(reader[nameof(User.Email)]),
										UserName = Convert.ToString(reader[nameof(User.UserName)]),
										TelePhone = Convert.ToString(reader[nameof(User.TelePhone)]),
										Password = string.Empty
									},
									EquipmentState = orderManager.QueryEquipmentStatus((long)reader[nameof(MedicalEquipment.Id)]),
									SoldDate = orderManager.QuerySoldDate((long)reader[nameof(MedicalEquipment.Id)]),
									SoldPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.SoldPrice)]),
									OriginalPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.OriginalPrice)]),
									Type = (EquipmentType)reader[nameof(MedicalEquipment.Type)],
									Manufacturer = (EquipmentManufacturer)reader[nameof(MedicalEquipment.Manufacturer)],
									Buyer = orderManager.QueryBuyer((long)reader[nameof(MedicalEquipment.Id)])
								};
								orderManager = null;
							}
							else
							{
								orderManager = null;
								throw new Exception();
							}
							return equipment;
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
		/// 查询一页设备
		/// 已被借的设备不会被筛选
		/// </summary>
		/// <param name="page">页码</param>
		/// <param name="pageSize">一页的个数</param>
		/// <returns>List<EquipmentViewModel></returns>
		public List<EquipmentViewModel> QueryPageEquipment(int page, int pageSize)
		{
			try
			{
				if (page < 1)
				{
					throw new Exception();
				}

				if (pageSize < 1)
				{
					throw new Exception();
				}

				int m, n;
				m = (page - 1) * pageSize + 1;
				n = page * pageSize;

				List<EquipmentViewModel> equipments = new List<EquipmentViewModel>();

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand =
						"select top " + Convert.ToString(n - m + 1) + " * from " + _TableName
						//+ " inner join " + _TableName_User
						//+ " on " + _TableName + "." + nameof(MedicalEquipment.UserEmail)
						//+ " = " + _TableName_User + "." + nameof(User.Email)
						+ " where Id not in(select " + nameof(Order.EquipmentId) + " from " + _TableName_Order
						+ " where " + nameof(Order.State) + " != " + Convert.ToString((int)OrderStatus.Holding)
						+ " and(" + nameof(Order.PaySuccess) + "=1 or " + nameof(Order.PayDepositSuccess) + "=1))"
						+ " and Id not in(select top " + Convert.ToString(m - 1) + " Id from " + _TableName
						+ " where Id not in(select " + nameof(Order.EquipmentId) + " from " + _TableName_Order
						+ " where " + nameof(Order.State) + " != " + Convert.ToString((int)OrderStatus.Holding)
						+ " and(" + nameof(Order.PaySuccess) + "=1 or " + nameof(Order.PayDepositSuccess) + "=1))"
						+ "order by " + nameof(MedicalEquipment.Id) + " desc)"
						+ "order by " + nameof(MedicalEquipment.Id) + " desc";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								equipments.Add(new EquipmentViewModel
								{
									Id = Convert.ToInt64(reader[nameof(MedicalEquipment.Id)]),
									Name = Convert.ToString(reader[nameof(MedicalEquipment.Name)]),
									Price = Convert.ToDecimal(reader[nameof(MedicalEquipment.Price)]),
									Describe = Convert.ToString(reader[nameof(MedicalEquipment.Describe)]),
									ImageUrl = Convert.ToString(reader[nameof(MedicalEquipment.ImageUrl)]),
									//Owner = new User
									//{
									//	Email = Convert.ToString(reader[nameof(User.Email)]),
									//	UserName = Convert.ToString(reader[nameof(User.UserName)]),
									//	TelePhone = Convert.ToString(reader[nameof(User.TelePhone)]),
									//	Password = string.Empty
									//},
									SoldPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.SoldPrice)]),
									OriginalPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.OriginalPrice)]),
									Type = (EquipmentType)reader[nameof(MedicalEquipment.Type)],
									Manufacturer = (EquipmentManufacturer)reader[nameof(MedicalEquipment.Manufacturer)]
								});
							}
							return equipments;
						}
					}
				}
			}
			catch
			{
				return new List<EquipmentViewModel>();
			}
		}



		/// <summary>
		/// 获取设备页数
		/// 已被借的设备不会被筛选
		/// </summary>
		/// /// <param name="pageSize">每页个数</param>
		/// <returns>总页数</returns>
		public int GetPageNumber(int pageSize)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select count (Id) from " + _TableName
						+ " where Id not in(select " + nameof(Order.EquipmentId) + " from " + _TableName_Order
						+ " where " + nameof(Order.State) + " != " + Convert.ToString((int)OrderStatus.Holding)
						+ " and(" + nameof(Order.PaySuccess) + "=1 or " + nameof(Order.PayDepositSuccess) + "=1))";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						int count = (int)command.ExecuteScalar();

						return count % pageSize == 0 ? count / pageSize : (count / pageSize) + 1;
					}
				}
			}
			catch
			{
				return -1;
			}
		}



		/// <summary>
		/// 获取用户正在被共享的设备
		/// </summary>
		/// <param name="email">用户邮箱</param>
		/// <returns>List<EquipmentViewModel></returns>
		public List<EquipmentViewModel> QuerySharingEquipment(string email)
		{
			try
			{
				List<EquipmentViewModel> equipments = new List<EquipmentViewModel>();

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select * from " + _TableName
						+ " inner join " + _TableName_User
						+ " on " + _TableName + "." + nameof(MedicalEquipment.UserEmail)
						+ " = " + _TableName_User + "." + nameof(User.Email)
						+ " where " + nameof(MedicalEquipment.UserEmail)
						+ " = " + _SqlParamUserEmail + " and " + nameof(MedicalEquipment.Id)
						+ " in(select " + nameof(Order.EquipmentId) + " from "
						+ _TableName_Order + " where " + nameof(Order.State) + " = "
						+ Convert.ToString((int)OrderStatus.Sharing)
						+ " and " + nameof(Order.PayDepositSuccess) + "=1"
						+ " and " + nameof(Order.SellerEmail) + " = " + _SqlParamUserEmail + ")";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamUserEmail, email));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							OrderManager orderManager = new OrderManager(_ConnectionString);

							while (reader.Read())
							{
								equipments.Add(new EquipmentViewModel
								{
									Id = Convert.ToInt64(reader[nameof(MedicalEquipment.Id)]),
									Name = Convert.ToString(reader[nameof(MedicalEquipment.Name)]),
									Price = Convert.ToDecimal(reader[nameof(MedicalEquipment.Price)]),
									Describe = Convert.ToString(reader[nameof(MedicalEquipment.Describe)]),
									ImageUrl = Convert.ToString(reader[nameof(MedicalEquipment.ImageUrl)]),
									Owner = new User
									{
										Email = Convert.ToString(reader[nameof(User.Email)]),
										UserName = Convert.ToString(reader[nameof(User.UserName)]),
										TelePhone = Convert.ToString(reader[nameof(User.TelePhone)]),
										Password = string.Empty
									},
									EquipmentState = OrderStatus.Sharing,
									SoldDate = orderManager.QuerySoldDate((long)reader[nameof(MedicalEquipment.Id)]),
									SoldPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.SoldPrice)]),
									OriginalPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.OriginalPrice)]),
									Type = (EquipmentType)reader[nameof(MedicalEquipment.Type)],
									Manufacturer = (EquipmentManufacturer)reader[nameof(MedicalEquipment.Manufacturer)],
									Buyer = orderManager.QueryBuyer((long)reader[nameof(MedicalEquipment.Id)])
								});
							}
							orderManager = null;
							return equipments;
						}
					}
				}
			}
			catch
			{
				return new List<EquipmentViewModel>();
			}
		}



		/// <summary>
		/// 查询用户借用的设备
		/// </summary>
		/// <param name="email">用户邮箱</param>
		/// <returns>List<EquipmentViewModel></returns>
		public List<EquipmentViewModel> QueryBorrowEquipment(string email)
		{
			try
			{
				List<EquipmentViewModel> equipments = new List<EquipmentViewModel>();

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select * from " + _TableName
						+ " inner join " + _TableName_User
						+ " on " + _TableName + "." + nameof(MedicalEquipment.UserEmail)
						+ " = " + _TableName_User + "." + nameof(User.Email)
						+ " where " + nameof(MedicalEquipment.Id)
						+ " in(select " + nameof(Order.EquipmentId) + " from "
						+ _TableName_Order + " where " + nameof(Order.State) + " = "
						+ Convert.ToString((int)OrderStatus.Sharing)
						+ " and " + nameof(Order.PayDepositSuccess) + "=1"
						+ " and " + nameof(Order.BuyerEmail) + " = " + _SqlParamUserEmail + ")";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamUserEmail, email));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							OrderManager orderManager = new OrderManager(_ConnectionString);
							while (reader.Read())
							{
								equipments.Add(new EquipmentViewModel
								{
									Id = Convert.ToInt64(reader[nameof(MedicalEquipment.Id)]),
									Name = Convert.ToString(reader[nameof(MedicalEquipment.Name)]),
									Price = Convert.ToDecimal(reader[nameof(MedicalEquipment.Price)]),
									Describe = Convert.ToString(reader[nameof(MedicalEquipment.Describe)]),
									ImageUrl = Convert.ToString(reader[nameof(MedicalEquipment.ImageUrl)]),
									Owner = new User
									{
										Email = Convert.ToString(reader[nameof(User.Email)]),
										UserName = Convert.ToString(reader[nameof(User.UserName)]),
										TelePhone = Convert.ToString(reader[nameof(User.TelePhone)]),
										Password = string.Empty
									},
									EquipmentState = OrderStatus.Sharing,
									SoldDate = orderManager.QuerySoldDate((long)reader[nameof(MedicalEquipment.Id)]),
									SoldPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.SoldPrice)]),
									OriginalPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.OriginalPrice)]),
									Type = (EquipmentType)reader[nameof(MedicalEquipment.Type)],
									Manufacturer = (EquipmentManufacturer)reader[nameof(MedicalEquipment.Manufacturer)],
									Buyer = orderManager.QueryBuyer((long)reader[nameof(MedicalEquipment.Id)])
								});
							}
							orderManager = null;
							return equipments;
						}
					}
				}
			}
			catch
			{
				return new List<EquipmentViewModel>();
			}
		}



		/// <summary>
		/// 查询已购买的设备
		/// </summary>
		/// <param name="email">用户邮箱</param>
		/// <returns>List<EquipmentViewModel></returns>
		public List<EquipmentViewModel> QueryBuyedEquipment(string email)
		{
			try
			{
				List<EquipmentViewModel> equipments = new List<EquipmentViewModel>();

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select * from " + _TableName
						+ " inner join " + _TableName_User
						+ " on " + _TableName + "." + nameof(MedicalEquipment.UserEmail)
						+ " = " + _TableName_User + "." + nameof(User.Email)
						+ " where " + nameof(MedicalEquipment.Id)
						+ " in(select " + nameof(Order.EquipmentId) + " from "
						+ _TableName_Order + " where " + nameof(Order.State) + " = "
						+ Convert.ToString((int)OrderStatus.SoldOut)
						+ " and " + nameof(Order.PaySuccess) + "=1"
						+ " and " + nameof(Order.BuyerEmail) + " = " + _SqlParamUserEmail + ")";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamUserEmail, email));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							OrderManager orderManager = new OrderManager(_ConnectionString);

							while (reader.Read())
							{
								equipments.Add(new EquipmentViewModel
								{
									Id = Convert.ToInt64(reader[nameof(MedicalEquipment.Id)]),
									Name = Convert.ToString(reader[nameof(MedicalEquipment.Name)]),
									Price = Convert.ToDecimal(reader[nameof(MedicalEquipment.Price)]),
									Describe = Convert.ToString(reader[nameof(MedicalEquipment.Describe)]),
									ImageUrl = Convert.ToString(reader[nameof(MedicalEquipment.ImageUrl)]),
									EquipmentState = OrderStatus.SoldOut,
									SoldDate = orderManager.QuerySoldDate((long)reader[nameof(MedicalEquipment.Id)]),
									SoldPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.SoldPrice)]),
									OriginalPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.OriginalPrice)]),
									Type = (EquipmentType)reader[nameof(MedicalEquipment.Type)],
									Manufacturer = (EquipmentManufacturer)reader[nameof(MedicalEquipment.Manufacturer)]
								});
							}
							orderManager = null;
							return equipments;
						}
					}
				}
			}
			catch
			{
				return new List<EquipmentViewModel>();
			}
		}



		/// <summary>
		/// 查询已出售的设备
		/// </summary>
		/// <param name="email">用户Email</param>
		/// <returns>List<EquipmentViewModel></returns>
		public List<EquipmentViewModel> QuerySoldEquipment(string email)
		{
			try
			{
				List<EquipmentViewModel> equipments = new List<EquipmentViewModel>();

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select * from " + _TableName
						+ " inner join " + _TableName_User
						+ " on " + _TableName + "." + nameof(MedicalEquipment.UserEmail)
						+ " = " + _TableName_User + "." + nameof(User.Email)
						+ " where " + nameof(MedicalEquipment.Id)
						+ " in(select " + nameof(Order.EquipmentId) + " from "
						+ _TableName_Order + " where " + nameof(Order.State) + " = "
						+ Convert.ToString((int)OrderStatus.SoldOut)
						+ " and " + nameof(Order.PaySuccess) + "=1"
						+ " and " + nameof(Order.SellerEmail) + " = " + _SqlParamUserEmail + ")";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamUserEmail, email));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							OrderManager orderManager = new OrderManager(_ConnectionString);

							while (reader.Read())
							{
								equipments.Add(new EquipmentViewModel
								{
									Id = Convert.ToInt64(reader[nameof(MedicalEquipment.Id)]),
									Name = Convert.ToString(reader[nameof(MedicalEquipment.Name)]),
									Price = Convert.ToDecimal(reader[nameof(MedicalEquipment.Price)]),
									Describe = Convert.ToString(reader[nameof(MedicalEquipment.Describe)]),
									ImageUrl = Convert.ToString(reader[nameof(MedicalEquipment.ImageUrl)]),
									EquipmentState = OrderStatus.SoldOut,
									SoldDate = orderManager.QuerySoldDate((long)reader[nameof(MedicalEquipment.Id)]),
									SoldPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.SoldPrice)]),
									OriginalPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.OriginalPrice)]),
									Type = (EquipmentType)reader[nameof(MedicalEquipment.Type)],
									Manufacturer = (EquipmentManufacturer)reader[nameof(MedicalEquipment.Manufacturer)]
								});
							}
							orderManager = null;
							return equipments;
						}
					}
				}
			}
			catch
			{
				return new List<EquipmentViewModel>();
			}
		}



		/// <summary>
		/// 查询一页设备
		/// 已被借的设备不会被筛选
		/// </summary>
		/// <param name="page">页数</param>
		/// <param name="pageSize">每页个数</param>
		/// <param name="searchStrings">搜索的数据</param>
		/// <returns></returns>
		public List<EquipmentViewModel> QueryPageEquipment(int page, int pageSize, string[] searchStrings, bool andOr)
		{
			if (searchStrings == null)
			{
				throw new Exception();
			}

			var (s, p) = SqlHelper.ObtainSqlParam(searchStrings);
			List<SqlParameter> parameters = p;
			string[] sqlParamName = s;

			try
			{
				if (page < 1)
				{
					throw new Exception();
				}

				if (pageSize < 1)
				{
					throw new Exception();
				}

				int m, n;
				m = (page - 1) * pageSize + 1;
				n = page * pageSize;

				List<EquipmentViewModel> equipments = new List<EquipmentViewModel>();

				string searchCondition = " where ((" + nameof(MedicalEquipment.Name) + " like '%'+" + sqlParamName[0] + "+'%' or "
						+ nameof(MedicalEquipment.Describe) + " like '%'+" + sqlParamName[0] + "+'%')";

				for (int i = 1; i < sqlParamName.Length; i++)
				{
					searchCondition += andOr ? " and " : " or ";
					searchCondition += "(" + nameof(MedicalEquipment.Name) + " like '%'+" + sqlParamName[i] + "+'%' or "
						+ nameof(MedicalEquipment.Describe) + " like '%'+" + sqlParamName[i] + "+'%')";
				}
				searchCondition += ")";

				string sqlCommand =
					"select top " + Convert.ToString(n - m + 1) + " * from " + _TableName
					//+ " inner join " + _TableName_User
					//+ " on " + _TableName + "." + nameof(MedicalEquipment.UserEmail)
					//+ " = " + _TableName_User + "." + nameof(User.Email)
					+ searchCondition
					+ " and Id not in(select " + nameof(Order.EquipmentId) + " from " + _TableName_Order
					+ " where " + nameof(Order.State) + " != " + Convert.ToString((int)OrderStatus.Holding)
					+ " and(" + nameof(Order.PaySuccess) + "^" + nameof(Order.PayDepositSuccess) + ")=1"
					+ ")"
					+ " and Id not in(select top " + Convert.ToString(m - 1) + " Id from " + _TableName
					+ searchCondition
					+ " and Id not in(select " + nameof(Order.EquipmentId) + " from " + _TableName_Order
					+ " where " + nameof(Order.State) + " != " + Convert.ToString((int)OrderStatus.Holding)
					+ " and(" + nameof(Order.PaySuccess) + "^" + nameof(Order.PayDepositSuccess) + ")=1"
					+ ")"
					+ "order by " + nameof(MedicalEquipment.Id) + " desc)"
					+ "order by " + nameof(MedicalEquipment.Id) + " desc";

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						foreach (SqlParameter param in parameters)
						{
							command.Parameters.Add(param);
						}

						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								equipments.Add(new EquipmentViewModel
								{
									Id = Convert.ToInt64(reader[nameof(MedicalEquipment.Id)]),
									Name = Convert.ToString(reader[nameof(MedicalEquipment.Name)]),
									Price = Convert.ToDecimal(reader[nameof(MedicalEquipment.Price)]),
									Describe = Convert.ToString(reader[nameof(MedicalEquipment.Describe)]),
									ImageUrl = Convert.ToString(reader[nameof(MedicalEquipment.ImageUrl)]),
									//Owner = new User
									//{
									//	Email = Convert.ToString(reader[nameof(User.Email)]),
									//	UserName = Convert.ToString(reader[nameof(User.UserName)]),
									//	TelePhone = Convert.ToString(reader[nameof(User.TelePhone)]),
									//	Password = string.Empty
									//},
									SoldPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.SoldPrice)]),
									OriginalPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.OriginalPrice)]),
									Type = (EquipmentType)reader[nameof(MedicalEquipment.Type)],
									Manufacturer = (EquipmentManufacturer)reader[nameof(MedicalEquipment.Manufacturer)]
								});
							}
							return equipments;
						}
					}
				}
			}
			catch
			{
				return new List<EquipmentViewModel>();
			}
		}



		/// <summary>
		/// 获取页数
		/// 已被借的设备不会被筛选
		/// </summary>
		/// <param name="pageSize">每页个数</param>
		/// <param name="searchStrings">搜索数据</param>
		/// <returns>总页数</returns>
		public int GetPageNumber(int pageSize, string[] searchStrings, bool andOr)
		{
			if (searchStrings == null)
			{
				throw new Exception();
			}

			var (s, p) = SqlHelper.ObtainSqlParam(searchStrings);
			List<SqlParameter> parameters = p;
			string[] sqlParamName = s;

			try
			{
				string searchCondition;

				searchCondition = " where ((" + nameof(MedicalEquipment.Name) + " like '%'+" + sqlParamName[0] + "+'%' or "
						+ nameof(MedicalEquipment.Describe) + " like '%'+" + sqlParamName[0] + "+'%')";
				for (int i = 1; i < sqlParamName.Length; i++)
				{
					searchCondition += andOr ? " and " : " or ";
					searchCondition += "(" + nameof(MedicalEquipment.Name) + " like '%'+" + sqlParamName[i] + "+'%' or "
						+ nameof(MedicalEquipment.Describe) + " like '%'+" + sqlParamName[i] + "+'%')";
				}
				searchCondition += ")";

				string sqlCommand = "select count (Id) from " + _TableName
					+ searchCondition
					+ " and Id not in(select " + nameof(Order.EquipmentId) + " from " + _TableName_Order
					+ " where " + nameof(Order.State) + " != " + Convert.ToString((int)OrderStatus.Holding)
					+ " and(" + nameof(Order.PaySuccess) + "^" + nameof(Order.PayDepositSuccess) + ")=1"
					+ ")";

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						foreach (SqlParameter param in parameters)
						{
							command.Parameters.Add(param);
						}

						int count = (int)command.ExecuteScalar();

						return count % pageSize == 0 ? count / pageSize : (count / pageSize) + 1;
					}
				}
			}
			catch
			{
				return -1;
			}
		}



		/// <summary>
		/// 筛选设备
		/// </summary>
		/// <param name="page">页数</param>
		/// <param name="pageSize">每页个数</param>
		/// <param name="filterData">筛选数据</param>
		/// <param name="andOr"></param>
		/// <returns>List<EquipmentViewModel></returns>
		public List<EquipmentViewModel> FilterPageEquipment(int page, int pageSize, FilterDataViewModel filterData, bool andOr)
		{
			try
			{
				if (page < 1)
				{
					throw new Exception();
				}

				if (pageSize < 1)
				{
					throw new Exception();
				}

				var (s, p) = SqlHelper.ObtainSqlParam(Units.SplitString(filterData.SearchString));
				List<SqlParameter> parameters = p;
				string[] sqlParamName = s;

				int m, n;
				m = (page - 1) * pageSize + 1;
				n = page * pageSize;

				List<EquipmentViewModel> equipments = new List<EquipmentViewModel>();

				string searchCondition, sortCondition;

				if (sqlParamName == null)
				{
					searchCondition = " where ";
				}
				else
				{
					searchCondition = " where ((" + nameof(MedicalEquipment.Name) + " like '%'+" + sqlParamName[0] + "+'%' or "
							+ nameof(MedicalEquipment.Describe) + " like '%'+" + sqlParamName[0] + "+'%')";
					for (int i = 1; i < sqlParamName.Length; i++)
					{
						searchCondition += andOr ? " and " : " or ";
						searchCondition += "(" + nameof(MedicalEquipment.Name) + " like '%'+" + sqlParamName[i] + "+'%' or "
							+ nameof(MedicalEquipment.Describe) + " like '%'+" + sqlParamName[i] + "+'%')";
					}
					searchCondition += ")";
				}

				if (filterData.EquipmentType >= 0)
				{
					if (!" where ".Equals(searchCondition))
					{
						searchCondition += " and ";
					}
					searchCondition +=  nameof(MedicalEquipment.Type)
						+ " = " + Convert.ToString(filterData.EquipmentType);
				}

				if (filterData.EquipmentManufacturer >= 0)
				{
					if (!" where ".Equals(searchCondition))
					{
						searchCondition += " and ";
					}
					searchCondition += nameof(MedicalEquipment.Manufacturer)
						+ " = " + Convert.ToString(filterData.EquipmentManufacturer);
				}

				if (filterData.MaxPrice > 0 && filterData.MinPrice >= 0)
				{
					if (!" where ".Equals(searchCondition))
					{
						searchCondition += " and ";
					}
					searchCondition += nameof(MedicalEquipment.Price)
						+ " >= " + Convert.ToString(filterData.MinPrice)
						+ " and " + nameof(MedicalEquipment.Price)
						+ " <= " + Convert.ToString(filterData.MaxPrice);
				}

				if (filterData.MaxSoldPrice > 0 && filterData.MinSoldPrice >= 0)
				{
					if (!" where ".Equals(searchCondition))
					{
						searchCondition += " and ";
					}
					searchCondition += nameof(MedicalEquipment.SoldPrice)
						+ " >= " + Convert.ToString(filterData.MinSoldPrice)
						+ " and " + nameof(MedicalEquipment.SoldPrice)
						+ " <= " + Convert.ToString(filterData.MaxSoldPrice);
				}

				switch (filterData.SortType)
				{
					case SortType.DateAscend:
						sortCondition = "order by " + nameof(MedicalEquipment.Id) + " desc";
						break;
					case SortType.DateDescend:
						sortCondition = "order by " + nameof(MedicalEquipment.Id);
						break;
					case SortType.PriceAscend:
						sortCondition = "order by " + nameof(MedicalEquipment.Price) + " desc";
						break;
					case SortType.PriceDescend:
						sortCondition = "order by " + nameof(MedicalEquipment.Price);
						break;
					case SortType.SoldPriceAscend:
						sortCondition = "order by " + nameof(MedicalEquipment.SoldPrice) + " desc";
						break;
					case SortType.SoldPriceDescend:
						sortCondition = "order by " + nameof(MedicalEquipment.SoldPrice);
						break;
					default:
						sortCondition = "";
						break;
				}

				string and = " where ".Equals(searchCondition) ? "" : "and";

				string sqlCommand =
					"select top " + Convert.ToString(n - m + 1) + " * from " + _TableName
					+ searchCondition + and
					+ " Id not in(select " + nameof(Order.EquipmentId) + " from " + _TableName_Order
					+ " where " + nameof(Order.State) + " != " + Convert.ToString((int)OrderStatus.Holding)
					+ " and (" + nameof(Order.PaySuccess) + "^" + nameof(Order.PayDepositSuccess) + ")=1"
					+ ")"
					+ " and Id not in(select top " + Convert.ToString(m - 1) + " Id from " + _TableName
					+ searchCondition + and
					+ " Id not in(select " + nameof(Order.EquipmentId) + " from " + _TableName_Order
					+ " where " + nameof(Order.State) + " != " + Convert.ToString((int)OrderStatus.Holding)
					+ " and (" + nameof(Order.PaySuccess) + "^" + nameof(Order.PayDepositSuccess) + ")=1"
					+ ")"
					+ sortCondition + ") " + sortCondition;

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						foreach(SqlParameter param in parameters)
						{
							command.Parameters.Add(param);
						}

						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								equipments.Add(new EquipmentViewModel
								{
									Id = Convert.ToInt64(reader[nameof(MedicalEquipment.Id)]),
									Name = Convert.ToString(reader[nameof(MedicalEquipment.Name)]),
									Price = Convert.ToDecimal(reader[nameof(MedicalEquipment.Price)]),
									Describe = Convert.ToString(reader[nameof(MedicalEquipment.Describe)]),
									ImageUrl = Convert.ToString(reader[nameof(MedicalEquipment.ImageUrl)]),
									//Owner = new User
									//{
									//	Email = Convert.ToString(reader[nameof(User.Email)]),
									//	UserName = Convert.ToString(reader[nameof(User.UserName)]),
									//	TelePhone = Convert.ToString(reader[nameof(User.TelePhone)]),
									//	Password = string.Empty
									//},
									SoldPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.SoldPrice)]),
									OriginalPrice = Convert.ToDecimal(reader[nameof(MedicalEquipment.OriginalPrice)]),
									Type = (EquipmentType)reader[nameof(MedicalEquipment.Type)],
									Manufacturer = (EquipmentManufacturer)reader[nameof(MedicalEquipment.Manufacturer)]
								});
							}
							return equipments;
						}
					}
				}
			}
			catch
			{
				return new List<EquipmentViewModel>();
			}
		}



		/// <summary>
		/// 获取筛选页数
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <param name="filterData"></param>
		/// <param name="andOr"></param>
		/// <returns></returns>
		public int GetFilterPageNumber(int page, int pageSize, FilterDataViewModel filterData, bool andOr)
		{
			try
			{
				if (page < 1)
				{
					throw new Exception();
				}

				if (pageSize < 1)
				{
					throw new Exception();
				}

				var (s, p) = SqlHelper.ObtainSqlParam(Units.SplitString(filterData.SearchString));
				List<SqlParameter> parameters = p;
				string[] sqlParamName = s;

				string searchCondition;

				if (sqlParamName == null)
				{
					searchCondition = " where ";
				}
				else
				{
					searchCondition = " where ((" + nameof(MedicalEquipment.Name) + " like '%'+" + sqlParamName[0] + "+'%' or "
							+ nameof(MedicalEquipment.Describe) + " like '%'+" + sqlParamName[0] + "+'%')";
					for (int i = 1; i < sqlParamName.Length; i++)
					{
						searchCondition += andOr ? " and " : " or ";
						searchCondition += "(" + nameof(MedicalEquipment.Name) + " like '%'+" + sqlParamName[i] + "+'%' or "
							+ nameof(MedicalEquipment.Describe) + " like '%'+" + sqlParamName[i] + "+'%')";
					}
					searchCondition += ")";
				}

				if (filterData.EquipmentType >= 0)
				{
					if (!" where ".Equals(searchCondition))
					{
						searchCondition += " and ";
					}
					searchCondition += nameof(MedicalEquipment.Type)
						+ " = " + Convert.ToString(filterData.EquipmentType);
				}

				if (filterData.EquipmentManufacturer >= 0)
				{
					if (!" where ".Equals(searchCondition))
					{
						searchCondition += " and ";
					}
					searchCondition += nameof(MedicalEquipment.Manufacturer)
						+ " = " + Convert.ToString(filterData.EquipmentManufacturer);
				}

				if (filterData.MaxPrice > 0 && filterData.MinPrice >= 0)
				{
					if (!" where ".Equals(searchCondition))
					{
						searchCondition += " and ";
					}
					searchCondition += nameof(MedicalEquipment.Price)
						+ " >= " + Convert.ToString(filterData.MinPrice)
						+ " and " + nameof(MedicalEquipment.Price)
						+ " <= " + Convert.ToString(filterData.MaxPrice);
				}

				if (filterData.MaxSoldPrice > 0 && filterData.MinSoldPrice >= 0)
				{
					if (!" where ".Equals(searchCondition))
					{
						searchCondition += " and ";
					}
					searchCondition += nameof(MedicalEquipment.SoldPrice)
						+ " >= " + Convert.ToString(filterData.MinSoldPrice)
						+ " and " + nameof(MedicalEquipment.SoldPrice)
						+ " <= " + Convert.ToString(filterData.MaxSoldPrice);
				}

				string and = " where ".Equals(searchCondition) ? "" : "and";

				string sqlCommand = "select count (Id) from " + _TableName
					+ searchCondition + and
					+ " Id not in(select " + nameof(Order.EquipmentId) + " from " + _TableName_Order
					+ " where " + nameof(Order.State) + " != " + Convert.ToString((int)OrderStatus.Holding)
					+ " and (" + nameof(Order.PaySuccess) + "^" + nameof(Order.PayDepositSuccess) + ")=1"
					+ ")";

				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						foreach (SqlParameter param in parameters)
						{
							command.Parameters.Add(param);
						}

						int count = (int)command.ExecuteScalar();

						return count % pageSize == 0 ? count / pageSize : (count / pageSize) + 1;
					}
				}
			}
			catch
			{
				return -1;
			}
		}


	} // end of class
}
