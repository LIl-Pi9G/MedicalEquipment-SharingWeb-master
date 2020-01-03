using MedicalEquipment_Sharing.Models;
using MedicalEquipment_Sharing.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

namespace MedicalEquipment_Sharing.DataAccess
{
	public class UserManager
	{
		private readonly string _ConnectionString;

		private readonly string _TableName = "Users";

		private readonly string _SqlParamEmail = "@Email";
		private readonly string _SqlParamUserName = "@UserName";
		private readonly string _SqlParamPassword = "@Password";
		private readonly string _SqlParamTelePhone = "@TelePhone";

		/// <summary>
		/// 构造
		/// 获取配置文件中的数据库链接字符串
		/// </summary>
		/// <param name="env">IHostingEnvironment</param>
		public UserManager(IConfiguration configuration)
		{
			_ConnectionString = configuration.GetConnectionString("DefaultConnection");
		}



		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="connectionString">连接字符串</param>
		public UserManager(string connectionString)
		{
			_ConnectionString = connectionString;
		}



		/// <summary>
		/// 创建一个新用户
		/// </summary>
		/// <param name="user">用户模型</param>
		/// <returns>是否创建成功</returns>
		public bool CreateUser(SignViewModel user)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "insert into " + _TableName
						+ "(" + nameof(User.Email)
						+ ", " + nameof(User.UserName)
						+ ", " + nameof(User.Password)
						+ ", " + nameof(User.TelePhone)
						+ ", " + nameof(User.Income) + ")"
						+ " values(" + _SqlParamEmail
						+ ", " + _SqlParamUserName
						+ ", " + _SqlParamPassword
						+ ", " + _SqlParamTelePhone 
						+ ", 0"
						+ ")";

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamEmail, user.Email));
						command.Parameters.Add(new SqlParameter(_SqlParamUserName, user.UserName));
						command.Parameters.Add(new SqlParameter(_SqlParamPassword, user.Password));
						command.Parameters.Add(new SqlParameter(_SqlParamTelePhone, user.TelePhone));

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
		/// 通过邮箱查询用户
		/// </summary>
		/// <param name="userEmail">用户邮箱</param>
		/// <returns></returns>
		public User QueryUser(string userEmail) {
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select * from " + _TableName
						+ " where " + nameof(User.Email)
						+ " = " + _SqlParamEmail;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamEmail, userEmail));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								return new User
								{
									Email = Convert.ToString(reader[nameof(User.Email)]),
									UserName = Convert.ToString(reader[nameof(User.UserName)]),
									TelePhone = Convert.ToString(reader[nameof(User.TelePhone)]),
									Income = Convert.ToDecimal(reader[nameof(User.Income)]),
									Password = string.Empty
								};
							}
							else
							{
								return null;
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
		/// 验证用户邮箱和密码
		/// </summary>
		/// <param name="user">用户模型</param>
		/// <returns>是否存在且正确</returns>
		public bool ValidateUser(User user)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select * from "
						+ _TableName
						+ " where "
						+ nameof(User.Email)
						+ " = " + _SqlParamEmail
						+ " and "
						+ nameof(User.Password)
						+ " = "
						+ _SqlParamPassword;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamEmail, user.Email));
						command.Parameters.Add(new SqlParameter(_SqlParamPassword, user.Password));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (!reader.Read())
							{
								return false;
							}
							return true;
						}
					}
				}
			}
			catch
			{
				return false;
			}
		}



		/// <summary>
		/// 是否存在电话号码
		/// </summary>
		/// <param name="phone"></param>
		/// <returns></returns>
		public bool IsExistTelePhone(string phone)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select count(*) from " + _TableName
						+ " where " + nameof(User.TelePhone)
						+ " = " + _SqlParamTelePhone;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamTelePhone, phone));

						return (int)command.ExecuteScalar() > 0;
					}
				}
			}
			catch
			{
				return false;
			}
		}



		/// <summary>
		/// 是否存在用户昵称
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool IsExistUserName(string name)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select count(*) from " + _TableName
						+ " where " + nameof(User.UserName)
						+ " = " + _SqlParamUserName;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamUserName, name));

						return (int)command.ExecuteScalar() > 0;
					}
				}
			}
			catch
			{
				return false;
			}
		}



		/// <summary>
		/// 查询收入
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public decimal QueryIncome(string email)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select * from " + _TableName
						+ " where " + nameof(User.Email)
						+ " = " + _SqlParamEmail;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamEmail, email));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								return Convert.ToDecimal(reader[nameof(User.Income)]);
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
				return 0;
			}
		}



		/// <summary>
		/// 添加收入
		/// </summary>
		/// <param name="email"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public bool AddIncome(string email, decimal amount)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "select * from " + _TableName
						+ " where " + nameof(User.Email)
						+ " = " + _SqlParamEmail;

					decimal totalIncome;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamEmail, email));

						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								totalIncome = Convert.ToDecimal(reader[nameof(User.Income)]);
							}
							else
							{
								throw new Exception();
							}
						}
					}

					totalIncome += amount;
					sqlCommand = "update " + _TableName + " set "
						+ nameof(User.Income) + " = " + Convert.ToString(totalIncome)
						+ " where " + nameof(User.Email)
						+ " = " + _SqlParamEmail;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamEmail, email));

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
		/// 清除收入
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public bool ClearIncome(string email)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_ConnectionString))
				{
					connection.Open();

					string sqlCommand = "update " + _TableName + " set "
						+ nameof(User.Income) + "=0"
						+ " where " + nameof(User.Email)
						+ " = " + _SqlParamEmail;

					using (SqlCommand command = new SqlCommand(sqlCommand, connection))
					{
						command.Parameters.Add(new SqlParameter(_SqlParamEmail, email));

						return command.ExecuteNonQuery() > 0;
					}

				}
			}
			catch
			{
				return false;
			}
		}

	} // end of class
}
