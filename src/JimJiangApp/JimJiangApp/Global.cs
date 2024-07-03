using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace JimJiangApp
{
    public class Global
    {
        public static string LocalAppDataDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KSpark");

        public static string LogsDirectory { get; } = Path.Combine(LocalAppDataDirectory, "logs");
        public static string CurrentVersionString
        {
            get
            {
                string appVersion = $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
                return appVersion;
            }
        }

        public static string Title { get; } = $"Jim Jiang App v{CurrentVersionString}";

    }
}
