using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MedicalEquipment_Sharing.Tools
{
	public class Units
	{
		public enum OmitDire
		{
			//左
			Left = 0,
			//右
			Right = 1,
			//无
			Non = -1
		}

		/// <summary>
		/// 判断是否需要用省略号代替页码
		/// 返回省略的方向OmitDire
		/// </summary>
		/// <param name="page">当前页页码</param>
		/// <param name="iterPage">迭代页数（判断的页码按钮）</param>
		/// <param name="otherPageCount">左右两侧钮个数（不包括省略）</param>
		/// <param name="pageCount">总页数</param>
		/// <returns>OmitDire</returns>
		public static OmitDire IsOmit(int page, int iterPage, int otherPageCount, int pageCount)
		{
			if (page == 1 && iterPage < otherPageCount + 1)
			{
				return OmitDire.Non;
			}
			if (page == pageCount && iterPage > pageCount - otherPageCount)
			{
				return OmitDire.Non;
			}
			if (iterPage < page - otherPageCount)
			{
				return OmitDire.Left;
			}
			if (iterPage > page + otherPageCount)
			{
				return OmitDire.Right;
			}
			return OmitDire.Non;
		}


		/// <summary>
		/// 获取枚举名称
		/// </summary>
		/// <param name="items">IEnumerable<SelectListItem></param>
		/// <param name="value">枚举值</param>
		/// <returns>string</returns>
		public static string GetEnumName(IEnumerable<SelectListItem> items, object value)
		{
			foreach (var item in items)
			{
				if (item.Value.Equals($"{(int)value}"))
				{
					return item.Text;
				}
			}
			return string.Empty;
		}


		/// <summary>
		/// 按照空白符分割字符串
		/// </summary>
		/// <param name="str">string</param>
		/// <returns>string[]</returns>
		public static string[] SplitString(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}

			string[] strs = Regex.Split(str, "\\s+", RegexOptions.IgnoreCase);

			List<string> buffer = new List<string>();

			foreach(string s in strs)
			{
				if (!string.IsNullOrEmpty(s))
				{
					buffer.Add(s);
				}
			}

			return buffer.ToArray();
		}


		/// <summary>
		/// 拼接字符串
		/// </summary>
		/// <param name="separator">separator</param>
		/// <param name="strs">string[]</param>
		/// <returns>string</returns>
		public static string JointString(char separator, string[] strs)
		{
			if (strs.Length == 0)
			{
				return string.Empty;
			}
			StringBuilder builder = new StringBuilder();
			builder.AppendJoin(separator, strs);
			return builder.ToString();
		}
	}
}
