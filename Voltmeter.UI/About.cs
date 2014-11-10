using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Voltmeter.Common.Extensions;
using Voltmeter.UI.Resources;

namespace Voltmeter.UI
{
    partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            this.Text = String.Format(Strings.About_Title_0, AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format(Strings.About_Version_0, AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                var attribute = Assembly.GetEntryAssembly().GetAttribute<AssemblyTitleAttribute>();
                if (attribute == null || string.IsNullOrWhiteSpace(attribute.Title))
                {
                    return Path.GetFileNameWithoutExtension(Application.ExecutablePath);
                }
                return attribute.Title;
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyProduct
        {
            get
            {
                var attribute = Assembly.GetEntryAssembly().GetAttribute<AssemblyProductAttribute>();
                if (attribute == null)
                {
                    return string.Empty;
                }
                return attribute.Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                var attribute = Assembly.GetEntryAssembly().GetAttribute<AssemblyCopyrightAttribute>();
                if (attribute == null)
                {
                    return string.Empty;
                }
                return attribute.Copyright;
            }
        }
        #endregion
    }
}