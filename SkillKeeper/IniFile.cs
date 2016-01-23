using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;

namespace SkillKeeper
{
    public class IniFile
    {
        private static IniFile _instance;

        private const string IniFileName = "settings.ini";

        public const string AccountSectionName = "account";
        public const string AccountApiKeyKeyName = "apikey";
        public const string AccountSubdomainKeyName = "subdomain";

        public const string IconUrlsSectionName = "iconurls";

        public IniData Data
        {
            get;
            private set;
        }

        private IniFile()
        {
            Data = new IniData();

            // If ini file is not present, then just exit.
            if (!System.IO.File.Exists(IniFile.IniFileName))
            {
                return;
            }

            // Else parse the data.
            var iniParser = new FileIniDataParser();
            Data = iniParser.ReadFile(IniFileName);
        }

        /// <summary>
        /// Yeah singletons are bad.
        /// </summary>
        /// <returns>A singleton.</returns>
        public static IniFile GetInstance()
        {
            if(_instance == null)
            {
                _instance = new IniFile();
            }

            return _instance;
        }
    }
}
