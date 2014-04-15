/**
 * This file is part of PokeParty.
 * 
 * Copyright (C) 2014 Ashlee Katzenbaer
 * All Rights Reserved.
 * 
 * @github im420blaziken
 *  
 * PokeParty is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * PokeParty is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with PokeParty.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokeParty
{
    public enum GameType
    {
        UNKNOWN,
        RBY,
        CRYSTAL,
        EMERALD,
        LEAFGREEN
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Run with optional arguments: <save directory> <save state filename>
        /// </summary>
        private enum ProgramArgument
        {
            DefaultPath,
        };
        [STAThread]
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Main";
            string defaultPath = null;

            for (uint i = 0; i < args.Count(); i++)
            {
                string argv = args[i];
                switch ((ProgramArgument)i)
                {
                    case ProgramArgument.DefaultPath: defaultPath = argv; break;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(defaultPath));
        }

        public static bool IsMainThread
        {
            get
            {
                return Thread.CurrentThread.Name == "Main";
            }
        }
    }
}
