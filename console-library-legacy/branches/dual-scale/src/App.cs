using Edu.Wisc.Forest.Flel.Util;
using log4net;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

using RasterIO = Wisc.Flel.GeospatialModeling.RasterIO;

namespace Landis
{
    public static class App
    {
        public static int Main(string[] args)
        {
            try {
                // The log4net section in the application's configuration file
                // requires the environment variable WORKING_DIR be set to the
                // current working directory.
                Environment.SetEnvironmentVariable("WORKING_DIR", Environment.CurrentDirectory);
                log4net.Config.XmlConfigurator.Configure();

                UI.TextWriter = Console.Out;

                string version = GetAppSetting("version");
                if (version == "")
                    throw new Exception("The application setting \"version\" is empty or blank");
                string release = GetAppSetting("release");
                if (release != "")
                    release = string.Format(" ({0})", release);
                UI.WriteLine("Dual-scale LANDIS-II {0}{1}", version, release);

                Assembly assembly = Assembly.GetExecutingAssembly();
                foreach (object attribute in assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false))
                    UI.WriteLine(((AssemblyCopyrightAttribute) attribute).Copyright);
                UI.WriteLine();

                if (args.Length == 0) {
                    UI.WriteLine("Error: No scenario file specified.");
                    return 1;
                }
                if (args.Length > 1) {
                    UI.WriteLine("Error: Extra argument(s) on command line:");
                    StringBuilder argsList = new StringBuilder();
                    argsList.Append(" ");
                    for (int i = 1; i < args.Length; ++i)
                        argsList.AppendFormat(" {0}", args[i]);
                    UI.WriteLine(argsList.ToString());
                    return 1;
                }

                string appDir = Application.Directory;
                PlugIns.IDataset plugIns = PlugIns.Admin.Dataset.LoadOrCreate(PlugIns.Admin.Dataset.DefaultPath);
                RasterIO.DriverManager driverMgr = new RasterIO.DriverManager(Path.Combine(appDir, "raster-drivers.xml"));
                Model model = new Model(plugIns, driverMgr);
                model.Run(args[0]);
                return 0;
            }
            catch (ApplicationException exc) {
                UI.WriteLine(exc.Message);
                return 1;
            }
            catch (Exception exc) {
                UI.WriteLine("Internal error occurred within the program:");
                UI.WriteLine("  {0}", exc.Message);
                if (exc.InnerException != null) {
                    UI.WriteLine("  {0}", exc.InnerException.Message);
                }
                UI.WriteLine();
                UI.WriteLine("Stack trace:");
                UI.WriteLine(exc.StackTrace);
                return 1;
            }
        }

        //---------------------------------------------------------------------

        public static string GetAppSetting(string settingName)
        {
            string setting = ConfigurationManager.AppSettings[settingName];
            if (setting == null)
                throw new Exception("The application setting \"" + settingName + "\" is not set");
            return setting.Trim(null);
        }
    }
}
