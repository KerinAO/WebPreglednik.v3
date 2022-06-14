using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebPreglednik
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tsbtnGo_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tstxtBox.Text) || tstxtBox.Text.Equals("about:blank"))
            {
                MessageBox.Show("Enter a valid URL.");
                tstxtBox.Focus();
                return;
            }
            OpenURLInBrowser(tstxtBox.Text);

            if (!WBEmulator.IsBrowserEmulationSet())
            {
                WBEmulator.SetBrowserEmulationVersion();
            }
        }

        private void OpenURLInBrowser(string url)
        {

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }

            try
            {
                wb.Navigate(new Uri(url));
            }
            catch (System.UriFormatException)
            {
                return;
            }
        }

        private void tsbtnHome_Click(object sender, EventArgs e)
        {
            wb.GoHome();
        }

        private void tsbtnBack_Click(object sender, EventArgs e)
        {
            wb.GoBack();
        }

        private void tsbtnForward_Click(object sender, EventArgs e)
        {
            wb.GoForward();
        }

        public enum BrowserEmulationVersion
        {
            Default = 0,
            Version7 = 7000,
            Version8 = 8000,
            Version8Standards = 8888,
            Version9 = 9000,
            Version9Standards = 9999,
            Version10 = 10000,
            Version10Standards = 10001,
            Version11 = 11000,
            Version11Edge = 11001
        }
        public static class WBEmulator
        {
            private const string InternetExplorerRootKey = @"Software\Microsoft\Internet Explorer";

            public static int GetInternetExplorerMajorVersion()
            {
                int result;

                result = 0;

                try
                {
                    RegistryKey key;

                    key = Registry.LocalMachine.OpenSubKey(InternetExplorerRootKey);

                    if (key != null)
                    {
                        object value;

                        value = key.GetValue("svcVersion", null) ?? key.GetValue("Version", null);

                        if (value != null)
                        {
                            string version;
                            int separator;

                            version = value.ToString();
                            separator = version.IndexOf('.');
                            if (separator != -1)
                            {
                                int.TryParse(version.Substring(0, separator), out result);
                            }
                        }
                    }
                }
                catch (SecurityException)
                {
                    // The user does not have the permissions required to read from the registry key.
                }
                catch (UnauthorizedAccessException)
                {
                    // The user does not have the necessary registry rights.
                }

                return result;
            }
            private const string BrowserEmulationKey = InternetExplorerRootKey + @"\Main\FeatureControl\FEATURE_BROWSER_EMULATION";

            public static BrowserEmulationVersion GetBrowserEmulationVersion()
            {
                BrowserEmulationVersion result;

                result = BrowserEmulationVersion.Default;

                try
                {
                    RegistryKey key;

                    key = Registry.CurrentUser.OpenSubKey(BrowserEmulationKey, true);
                    if (key != null)
                    {
                        string programName;
                        object value;

                        programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                        value = key.GetValue(programName, null);

                        if (value != null)
                        {
                            result = (BrowserEmulationVersion)Convert.ToInt32(value);
                        }
                    }
                }
                catch (SecurityException)
                {
                    // The user does not have the permissions required to read from the registry key.
                }
                catch (UnauthorizedAccessException)
                {
                    // The user does not have the necessary registry rights.
                }

                return result;
            }
            public static bool SetBrowserEmulationVersion(BrowserEmulationVersion browserEmulationVersion)
            {
                bool result;

                result = false;

                try
                {
                    RegistryKey key;

                    key = Registry.CurrentUser.OpenSubKey(BrowserEmulationKey, true);

                    if (key != null)
                    {
                        string programName;

                        programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

                        if (browserEmulationVersion != BrowserEmulationVersion.Default)
                        {
                            // if it's a valid value, update or create the value
                            key.SetValue(programName, (int)browserEmulationVersion, RegistryValueKind.DWord);
                        }
                        else
                        {
                            // otherwise, remove the existing value
                            key.DeleteValue(programName, false);
                        }

                        result = true;
                    }
                }
                catch (SecurityException)
                {
                    // The user does not have the permissions required to read from the registry key.
                }
                catch (UnauthorizedAccessException)
                {
                    // The user does not have the necessary registry rights.
                }

                return result;
            }

            public static bool SetBrowserEmulationVersion()
            {
                int ieVersion;
                BrowserEmulationVersion emulationCode;

                ieVersion = GetInternetExplorerMajorVersion();

                if (ieVersion >= 11)
                {
                    emulationCode = BrowserEmulationVersion.Version11;
                }
                else
                {
                    switch (ieVersion)
                    {
                        case 10:
                            emulationCode = BrowserEmulationVersion.Version10;
                            break;
                        case 9:
                            emulationCode = BrowserEmulationVersion.Version9;
                            break;
                        case 8:
                            emulationCode = BrowserEmulationVersion.Version8;
                            break;
                        default:
                            emulationCode = BrowserEmulationVersion.Version7;
                            break;
                    }
                }

                return SetBrowserEmulationVersion(emulationCode);
            }
            public static bool IsBrowserEmulationSet()
            {
                return GetBrowserEmulationVersion() != BrowserEmulationVersion.Default;
            }
        }

        private void tsbtnTab_Click(object sender, EventArgs e)
        {
            TabPage tb = new TabPage("www.google.hr");
            WebBrowser wb = new WebBrowser();
            wb.Dock = DockStyle.Fill;
            wb.Navigate("www.google.com");

            tabControl1.TabPages.Add(tb);
            tb.Controls.Add(wb);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TabPage td = new TabPage("www.google.hr");
            WebBrowser wb = new WebBrowser();
            wb.Navigate("www.google.hr");
            wb.Dock = DockStyle.Fill;
            tabControl1.TabPages.Add(td);
            td.Controls.Add(wb);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tstxtBox_Click(object sender, EventArgs e)
        {

        }

        private void tsbtnSearch_Click(object sender, EventArgs e)
        {
            wb.Navigate(tstxtBox.Text);
        }

        private void tsbtnBack_Click_1(object sender, EventArgs e)
        {
            wb.GoBack();
        }

        private void tsbtnForward_Click_1(object sender, EventArgs e)
        {
            wb.GoForward();
        }

        private void tsbtnHome_Click_1(object sender, EventArgs e)
        {
            wb.Navigate("www.google.hr");
        }

        private void tstxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                tsbtnSearch.PerformClick();
            }
        }
    }
}
