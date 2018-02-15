using System;
using System.Linq;
using System.Reflection;

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Utils;

namespace MatthiWare.UpdateLib.Tasks
{
    public class LoadCacheTask : AsyncTask<CacheFile, LoadCacheTask>
    {
        private Lazy<UpdateVersion> m_lazyUpdateVersion = new Lazy<UpdateVersion>(() =>
        {
            ApplicationVersionAttribute attr = Assembly.GetEntryAssembly()?.GetCustomAttributes(typeof(ApplicationVersionAttribute), true).FirstOrDefault() as ApplicationVersionAttribute;

            return attr?.Version ?? "0.0.0";
        });



        protected override void DoWork()
        {
            try
            {
                Result = FileManager.LoadFile<CacheFile>();
            }
            catch (Exception e)
            {
                Updater.Instance.Logger.Error(nameof(LoadCacheTask), nameof(DoWork), e);

                Result = new CacheFile();
                Result.CurrentVersion = m_lazyUpdateVersion;
                Result.Save();
            }
        }
    }
}
