using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MedicalEquipment_Sharing.Tools
{
	public class FileHelper
	{
		private static readonly string IMAGE_PATH = "/Files/Images/";

		/// <summary>
		/// 拼接地址
		/// </summary>
		/// <param name="paths">paths...</param>
		/// <returns>string</returns>
		public static string Combine(params string[] paths)
		{
			if (paths.Length == 0)
			{
				return string.Empty;
			}
			else
			{
				StringBuilder builder = new StringBuilder();
				string spliter = "/";
				string firstPath = paths[0];

				if (firstPath.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase)
					|| firstPath.StartsWith("HTTPS", StringComparison.OrdinalIgnoreCase))
				{
					spliter = "/";
				}

				if (!firstPath.EndsWith(spliter))
				{
					firstPath = firstPath + spliter;
				}

				builder.Append(firstPath);

				for (int i = 1; i < paths.Length; i++)
				{
					string nextPath = paths[i];
					if (nextPath.StartsWith("/") || nextPath.StartsWith(@"\"))
					{
						nextPath = nextPath.Substring(1);
					}

					if (i != paths.Length - 1)
					{
						if (nextPath.EndsWith("/") || nextPath.EndsWith(@"\"))
						{
							nextPath = nextPath.Substring(0, nextPath.Length - 1) + spliter;
						}
						else
						{
							nextPath = nextPath + spliter;
						}
					}

					builder.Append(nextPath);
				}

				return builder.ToString();
			}
		}

		/// <summary>
		/// 上传图片
		/// </summary>
		/// <param name="file">IFormFile</param>
		/// <returns>ImageUrl</returns>
		public static async Task<string> UploadImageAsync(string webRootPath, IFormFile file)
		{

			if (!Directory.Exists(Combine(webRootPath, IMAGE_PATH)))
			{
				Directory.CreateDirectory(Combine(webRootPath, IMAGE_PATH));
			}

			string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
			string filePath = Combine(webRootPath, IMAGE_PATH, fileName);

			using (FileStream stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
			return IMAGE_PATH + fileName;
		}



		/// <summary>
		/// 删除图片文件
		/// </summary>
		/// <param name="fileName">fileName</param>
		public static void DeleteImage(string webRootPath, string fileName)
		{
			string filePath = Combine(webRootPath, fileName);
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}
	}
}
