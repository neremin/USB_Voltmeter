using System;
using System.IO;
using System.Windows.Forms;

namespace Voltmeter.Configuration
{
    public sealed class PortableXmlFileSettingsProvider : XmlFileSettingsProvider
    {
        public static new readonly string DefaultDirectory = Path.GetDirectoryName(Application.ExecutablePath);

        public PortableXmlFileSettingsProvider() : base(DefaultDirectory, DefaultFileName)
        {
        }

        public PortableXmlFileSettingsProvider(string settingsDirectory, string settingsFileName)
            : base(settingsDirectory, settingsFileName)
        {
        }

        protected override string LocalSettingsNodeName
        {
            get { return "Portable"; }
        }

        protected override string MachineName
        {
            get { return Environment.MachineName; }
        }
    }
}