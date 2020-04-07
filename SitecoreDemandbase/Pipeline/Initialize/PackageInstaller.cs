using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Engines;
using Sitecore.Diagnostics;
using Sitecore.Install.Files;
using Sitecore.Install.Framework;
using Sitecore.Install.Items;
using Sitecore.Install.Utils;
using Sitecore.SecurityModel;

namespace SitecoreDemandbase.Pipeline.Initialize
{
    public class PackageInstaller
    {
        public bool NeedsInstalling(Database db)
        {
            return typeof(DemandbaseConstants)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(x => x.FieldType == typeof(string))
                .Any(f => db.GetItem(f.GetValue(null).ToString()) == null)
                ||
                typeof(DemandbaseConstants.GuardedConditions).GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Where(x => x.FieldType == typeof(string))
                    .Any(f => db.GetItem(f.GetValue(null).ToString()) == null);
        }
        public void InstallPackage()
        {
            var filepath = "";
            if (System.Text.RegularExpressions.Regex.IsMatch(Settings.DataFolder, @"^(([a-zA-Z]:\\)|(//)).*"))
                //if we have an absolute path, rather than relative to the site root
                filepath = Settings.DataFolder +
                           @"\packages\DemandbasePackage.zip";
            else
                filepath = HttpRuntime.AppDomainAppPath + Settings.DataFolder.Substring(1) +
                           @"\packages\DemandbasePackage.zip";
            try
            {
                GeneratePackage(filepath);
                int count = 0;
                while (true)
                {
                    if (count > 15)
                        throw new Exception("Demandbase package extracted to data folder packages however appears to be locked, unlock this file and recycle the app pool.");
                    if (!IsFileLocked(new FileInfo(filepath)))
                    {

                        using (new SecurityDisabler())
                        {
                            using (new SyncOperationContext())
                            {
                                IProcessingContext context = new SimpleProcessingContext();
                                IItemInstallerEvents events =
                                    new DefaultItemInstallerEvents(
                                        new BehaviourOptions(InstallMode.Overwrite, MergeMode.Undefined));
                                context.AddAspect(events);
                                IFileInstallerEvents events1 = new DefaultFileInstallerEvents(true);
                                context.AddAspect(events1);

                                Sitecore.Install.Installer installer = new Sitecore.Install.Installer();
                                installer.InstallPackage(MainUtil.MapPath(filepath), context);
                                break;
                            }
                        }
                    }
                    else
                        Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Log.Error("Demandbase was unable to initialize", e, this);
            }
        }

        private void GeneratePackage(string filepath)
        {
            using (var manifestResourceStream = GetType().Assembly
                .GetManifestResourceStream("SitecoreDemandbase.Resources.DemandbasePackage.zip"))
            {
                using (var filestream = new FileStream(filepath, FileMode.Create))
                {
                    manifestResourceStream?.CopyTo(filestream);
                    manifestResourceStream?.Close();
                    filestream.Close();
                }
            }
        }

        /// <summary>
        /// checks to see if the file is done being written to the filesystem
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                stream?.Close();
            }

            //file is not locked
            return false;
        }
    }
}
