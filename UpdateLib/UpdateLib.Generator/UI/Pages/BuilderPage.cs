﻿/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
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
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using MatthiWare.UpdateLib.UI;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.Generator.Tasks;
using MatthiWare.UpdateLib.Files;
using System.Diagnostics;

namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    public partial class BuilderPage : PageControlBase
    {
        private FilesPage filesPage;
        private InformationPage infoPage;
        private RegistryPage registryPage;

        public BuilderPage()
        {
            InitializeComponent();
        }

        protected override void OnPageInitialize()
        {
            saveFileDialog.InitialDirectory = new DirectoryInfo("./Output").FullName;

            PageControlBase page;
            if (!TestForm.TryGetPage(nameof(FilesPage), out page))
            {
                throw new KeyNotFoundException($"Unable to get {nameof(FilesPage)}");
            }

            filesPage = page as FilesPage;

            if (!TestForm.TryGetPage(nameof(InformationPage), out page))
            {
                throw new KeyNotFoundException($"Unable to get {nameof(InformationPage)}");
            }

            infoPage = page as InformationPage;

            if (!TestForm.TryGetPage(nameof(RegistryPage), out page))
            {
                throw new KeyNotFoundException($"Unable to get {nameof(RegistryPage)}");
            }

            registryPage = page as RegistryPage;

            if (!filesPage.IsPageInitialized)
                filesPage.InitializePage(null);

            if (!infoPage.IsPageInitialized)
                infoPage.InitializePage(null);

            if (!registryPage.IsPageInitialized)
                registryPage.InitializePage(null);
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog(ParentForm) != DialogResult.OK)
                return;

            pbProgress.Value = 0;
            lblProgress.Text = "Progress: 0%";

            Build(saveFileDialog.OpenFile());
        }

        Stopwatch sw = new Stopwatch();

        private AsyncTask<UpdateFile> Build(Stream s)
        {
            UpdateGeneratorTask task = new UpdateGeneratorTask(filesPage.Root, infoPage,registryPage.Folders);

            btnBuild.Enabled = false;

            task.TaskProgressChanged += (o, e) =>
            {
                lblProgress.Text = $"Progress: {e.ProgressPercentage}%";
                pbProgress.Value = e.ProgressPercentage;

            };

            task.TaskCompleted += (o, e) =>
            {
                sw.Stop();

                Updater.Instance.Logger.Debug(nameof(BuilderPage), nameof(Build), $"File generation completed in {sw.ElapsedMilliseconds} ms.");



                btnBuild.Enabled = true;

                if (e.Cancelled)
                {
                    lblStatus.Text = $"Status: Cancelled";
                    return;
                }

                if (e.Error != null)
                {
                    lblStatus.Text = $"Status: Error";

                    MessageDialog.Show(
                        ParentForm,
                        "Builder",
                        "Build error",
                        "Check the logs for more information",
                        SystemIcons.Error,
                        MessageBoxButtons.OK);

                    return;
                }

                using (s)
                    task.Result.Save(s);

                lblStatus.Text = $"Status: Completed";

                MessageDialog.Show(
                        ParentForm,
                        "Builder",
                        "Build completed",
                        "The update file has been succesfully generated!\n" +
                        $"File generation completed in {sw.ElapsedMilliseconds} ms.",
                        SystemIcons.Information,
                        MessageBoxButtons.OK);
            };

            lblStatus.Text = "Status: Building..";

            sw.Reset();
            sw.Start();

            return task.Start();
        }
    }
}
