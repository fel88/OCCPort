using System.IO;
using System.Text;

namespace OCCPort.OpenGL
{
    public class ILog
    {
        public static ILog Instance;
        public static bool Enabled = false;
        internal static void Log(string name, object[] args)
        {
            if (!Enabled )
                return;
            StringBuilder str = new StringBuilder();
            str.Append($"<log func=\"{name}\">");
            foreach (var item in args)
            {
                str.AppendLine($"<param type=\"{item.GetType().Name}\" value=\"{item}\"/>)");
            }
            str.Append("</log>");
            File.AppendAllText("log.txt", str.ToString());
        }

        
    }
}