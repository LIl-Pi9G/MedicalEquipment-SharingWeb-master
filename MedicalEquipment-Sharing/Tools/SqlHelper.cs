using System.Collections.Generic;
using System.Data.SqlClient;

namespace MedicalEquipment_Sharing.Tools
{
	public class SqlHelper
	{
		/// <summary>
		/// 从string[]生成对应SQLParmater
		/// 返回ParmaName和SQLParmater
		/// </summary>
		/// <param name="strs"></param>
		/// <returns></returns>
		public static (string[], List<SqlParameter>) ObtainSqlParam(string[] strs)
		{
			if (strs == null)
			{
				return (null, new List<SqlParameter>());
			}

			List<SqlParameter> parameters = new List<SqlParameter>();
			List<string> buffer = new List<string>();

			for (int i = 0; i < strs.Length; i++)
			{
				string paramName = $"@SearchString{i}";
				parameters.Add(new SqlParameter(paramName, strs[i]));
				buffer.Add(paramName);
			}

			return (buffer.ToArray(), parameters);
		}

	}
}
