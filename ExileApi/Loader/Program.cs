﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using Serilog;
using SharpDX;
using SharpDX.Windows;

namespace Loader
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            AskToKillOtherRunningProcesses();
            var loader = new Loader();
            loader.Load(args);
        }

        private static void AskToKillOtherRunningProcesses()
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);

            if (processes.Length > 1)
            {
                var text = "Kill already running HUD process? (program configs will not be saved)";
                var caption = "Hud process is already running";
                var msgBoxResult = MessageBox.Show(text, caption, MessageBoxButtons.OKCancel);

                if (msgBoxResult == DialogResult.OK)
                {
                    foreach (var process in processes)
                    {
                        if (process.Id != currentProcess.Id)
                        {
                            process.Kill();
                        }
                    }
                }
                else if (msgBoxResult == DialogResult.Cancel)
                {
                    currentProcess.Kill();
                }
            }
        }
    }
}
