using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Tasks
{
    public class UpdateRegistryTask : UpdatableTask
    {

        public IEnumerable<RegistryKeyEntry> Keys { get; set; }
        
        private List<RollbackData> cachedUpdates = new List<RollbackData>();
        
        public UpdateRegistryTask(IEnumerable<RegistryKeyEntry> keys)
        {
            Keys = keys;
        }

        protected override void DoWork()
        {
            cachedUpdates.Clear();

            int total = Keys.Count();
            int count = 0;

            foreach (RegistryKeyEntry key in Keys)
            {
                UpdateKey(key);

                OnTaskProgressChanged(++count, total);
            }
        }

        private void UpdateKey(RegistryKeyEntry key)
        {
            string path = key.Parent.DestinationLocation;

            RollbackData rollback = new RollbackData(key);

            cachedUpdates.Add(rollback);

            RegistryHelper.Update(key, rollback);

            Updater.Instance.Logger.Info(nameof(UpdateRegistryTask), nameof(UpdateKey),
                $"Succesfully updated {key.DestinationLocation}");
        }

        public override void Rollback()
        {
            int total = cachedUpdates.Count;
            int count = 0;

            foreach (RollbackData data in cachedUpdates)
            {
                RollbackFailSafe(data);

                OnTaskProgressChanged(++count, total);
            }
        }

        private void RollbackFailSafe(RollbackData data)
        {
            try
            {
                RegistryKey key = RegistryHelper.GetOrMakeKey(data.path);

                if (!data.existed)
                {
                    key.DeleteValue(data.key);

                    Updater.Instance.Logger.Warn(nameof(UpdateRegistryTask), nameof(Rollback),
                $"Deleted ->  {data.path}\\{data.key}");

                    return;
                }

                key.SetValue(data.key, data.cachedValue, data.type);

                Updater.Instance.Logger.Warn(nameof(UpdateRegistryTask), nameof(Rollback),
                $"Rolled back ->  {data.path}\\{data.key}");
            }
            catch (Exception e)
            {
                Updater.Instance.Logger.Error(nameof(UpdateRegistryTask), nameof(Rollback), e);
            }
        }

        public struct RollbackData
        {
            public bool existed;
            public string path;
            public string key;
            public object cachedValue;
            public RegistryValueKind type;

            public RollbackData(RegistryKeyEntry l_key)
            {
                key = l_key.Name;
                path = l_key.Parent.DestinationLocation;
                existed = RegistryHelper.Exists(l_key, out cachedValue);
                type = RegistryValueKind.Unknown;
            }
        }
    }
}
