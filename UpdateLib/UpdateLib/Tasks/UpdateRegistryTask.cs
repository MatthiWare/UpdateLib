/*  Copyright
 *  
 *  UpdateLib - .Net auto update library <https://github.com/MatthiWare/UpdateLib>
 *  
 *  File: UpdateRegistryTask.cs v0.5
 *  
 *  Author: Matthias Beerens
 *  
 *  Copyright (C) 2016 - MatthiWare
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <https://github.com/MatthiWare/UpdateLib/blob/master/LICENSE>.
 */

using MatthiWare.UpdateLib.Common;
using MatthiWare.UpdateLib.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

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

            Updater.Instance.Logger.Info(
                nameof(UpdateRegistryTask), 
                nameof(UpdateKey),
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

                    Updater.Instance.Logger.Warn(
                        nameof(UpdateRegistryTask), 
                        nameof(Rollback),
                        $"Deleted ->  {data.path}\\{data.key}");

                    return;
                }

                key.SetValue(data.key, data.cachedValue, data.type);

                Updater.Instance.Logger.Warn(
                    nameof(UpdateRegistryTask), 
                    nameof(Rollback),
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
