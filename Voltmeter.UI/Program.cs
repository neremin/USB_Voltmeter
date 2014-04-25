using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using Voltmeter.Common;
using Voltmeter.Configuration;
using Voltmeter.Services;
using Voltmeter.Services.Emulation;
using Voltmeter.UI.Properties;
using Voltmeter.UI.ViewModel;
using System.Globalization;
using Voltmeter.UI.Resources;

namespace Voltmeter.UI
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            try
            {
                RunApplication();
            }
            catch (WarningException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.ExceptionError(ex, Strings.Unhandled_Exception);
                MessageBox.Show
                    (
                        string.Format(Strings.Unhandled_Exception_Message_0, Log.Source), 
                        Application.ProductName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
            }
        }

        static void RunApplication()
        {
            // Use invariant numbers formatting
            Application.CurrentCulture = CultureInfo.InvariantCulture;
//            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture; // Uncomment to test Neutral UI

            InitializeSettings();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool useEmulation = Environment.GetCommandLineArgs().Any(a => a == "-e" || a == "--emulation");
            
            Application.Run(new MainForm(() =>
                new VoltmeterViewModel(
                        useEmulation
                            ? (IDeviceManager) new PseudoDeviceManager()
                            : new DeviceManager(new CurrentContextTaskQueue())
                    )
                )
            );
        }

        static void InitializeSettings()
        {
            var provider = Environment.GetCommandLineArgs().Any(a => a == "-p" || a == "--portable")
                ? new PortableXmlFileSettingsProvider() 
                : new XmlFileSettingsProvider();
            RedirectSettings(Settings.Default, provider);
        }

        static void RedirectSettings(ApplicationSettingsBase settings, SettingsProvider provider)
        {
            settings.Providers.Add(provider);
            foreach (SettingsProperty property in settings.Properties)
            {
                property.Provider = provider;
            }
            
            settings.Reload();
        }
    }
}