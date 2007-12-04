using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;

namespace ThoughtWorks.CruiseControl.CCTray
{
	public class Bootstrap
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.EnableVisualStyles();
			Application.DoEvents();

			try
			{
				ICruiseManagerFactory remoteCruiseManagerFactory = new RemoteCruiseManagerFactory();
				ICruiseProjectManagerFactory cruiseProjectManagerFactory = new CruiseProjectManagerFactory( remoteCruiseManagerFactory );
				CCTrayMultiConfiguration configuration = new CCTrayMultiConfiguration( cruiseProjectManagerFactory, GetSettingsFilename() );

				MainForm mainForm = new MainForm(configuration);

				Application.Run(mainForm);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to start: " + ex, AppDomain.CurrentDomain.FriendlyName);
			}
		}

		private static string GetSettingsFilename()
		{
			string oldFashionedSettingsFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"settings.xml");
			string newSettingsFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"cctray-settings.xml");
			
			if (File.Exists(oldFashionedSettingsFilename) && !File.Exists(newSettingsFilename))
            {
                if (File.GetLastWriteTime(oldFashionedSettingsFilename) >= File.GetLastWriteTime(newSettingsFilename))
                {
                    File.Delete(newSettingsFilename);
                }
                else
                {
                    File.Delete(oldFashionedSettingsFilename);
                    File.Move(newSettingsFilename, oldFashionedSettingsFilename);    
                }
                return oldFashionedSettingsFilename;
            }
			
			return newSettingsFilename;
		}

	}
}
