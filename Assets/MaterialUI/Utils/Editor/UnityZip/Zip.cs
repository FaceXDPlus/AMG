//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

#if UNITY_EDITOR
using Ionic.Zip;
using System.IO;

namespace MaterialUI
{
	public class ZipUtil
	{
		public static void Zip(string location, string zipFilePath)
		{
			using (ZipFile zip = new ZipFile())
			{
				zip.AddDirectory(location, Path.GetFileName(Path.GetDirectoryName(location)));
				zip.Save(zipFilePath);
			}
		}

		public static void Unzip(string zipFilePath, string location)
		{
			Directory.CreateDirectory(location);
			
			using (ZipFile zip = ZipFile.Read(zipFilePath))
			{
				zip.ExtractAll(location, ExtractExistingFileAction.OverwriteSilently);
			}
		}
	}
}
#endif