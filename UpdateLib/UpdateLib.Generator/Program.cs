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

using System;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.UpdateLib.Generator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            List<Stack1> Unavailability = new List<Stack1>
{
        new Stack1{  Key = "A", StartDate = new DateTime(2018,1,1), EndDate = new DateTime(2018,1,30) },
        new Stack1{  Key = "B", StartDate = new DateTime(2018,1,2), EndDate = new DateTime(2018,1,30)},
        new Stack1{  Key = "C", StartDate = new DateTime(2018,1,2), EndDate = new DateTime(2018,1,30)}
};

            bool allUnique = Unavailability.Select(_ => new { _.StartDate, _.EndDate }).Distinct().Count() <= 1;

            Console.WriteLine(allUnique);
            Console.ReadKey();
        }

        public class Stack1
        {
            public string Key { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}
