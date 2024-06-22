using System.IO;
using System.Linq;
using System.Reflection;

namespace OCCPort
{
	public static class ResourceHelper
	{
		public static string ReadResourceTxt(string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var fr1 = assembly.GetManifestResourceNames().First(z => z.Contains(resourceName));

			using (Stream stream = assembly.GetManifestResourceStream(fr1))
			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}

}