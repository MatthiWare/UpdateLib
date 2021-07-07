/*  UpdateLib - .Net auto update library
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

using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.DataAnnotations;

namespace MatthiWare.UpdateLib.Generator
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            CommandLineApplication.Execute<Program>(args);

#if DEBUG
            Console.ReadKey();
#endif
        }

        [Required]
        [DirectoryExists]
        [Option("--from <DIR>", Description = "Required. 'Old' program directory")]
        private string PreviousVersionDir { get; }

        [Required]
        [DirectoryExists]
        [Option("--to <DIR>", Description = "Required. 'new' program directory")]
        private string CurrentVersionDirectory { get; }

        [Required]
        [DirectoryExists]
        [Option("--output <DIR>", Description = "Required. Delta output directory")]
        private string OutputDirectory { get; }

        private async void OnExecuteAsync()
        {
            var patchBuilder = new PatchBuilder(PreviousVersionDir, CurrentVersionDirectory, OutputDirectory);

            await patchBuilder.CreatePatch((progress) => Console.WriteLine($"Progress: {progress}"));
        }


    }
}
