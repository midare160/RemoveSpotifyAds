﻿using Spare.Extensions;
using Spare.UI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Spare.Tools
{
    public class Installer
    {
        #region Static
        private const string InstallerHash = "a0f36ca24bf9df230afe59b6e4dd4f53";
        #endregion

        #region Fields
        private int _installExitCode;
        #endregion

        #region Constructors
        public Installer(RichTextBox outputTextBox)
            => OutputTextBox = outputTextBox;
        #endregion

        #region Properties
        public RichTextBox OutputTextBox { get; }
        public string InstallerPath => Path.Combine(Application.StartupPath, "Data", "spotify_installer1.0.8.exe");
        #endregion

        #region Events
        private void InstallProcessExited(object sender, EventArgs e)
            => _installExitCode = ((Process)sender).ExitCode;
        #endregion

        #region Methods
        public bool Exists()
        {
            if (!File.Exists(InstallerPath))
            {
                return false;
            }

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(InstallerPath))
                {
                    return string.Equals(
                        BitConverter
                            .ToString(md5.ComputeHash(stream))
                            .Replace("-", null),
                        InstallerHash,
                        StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        /// <summary>
        /// Executes the Spotify installer and waits until it terminates.
        /// </summary>
        public bool Install()
        {
            OutputTextBox.AppendText("Installing Spotify...");

            // HACK: Start the Spotify installer with non-admin rights, otherwise it wouldnt execute
            Process.Start("explorer.exe", InstallerPath)?.WaitForExit();

            try
            {
                using (var installProcess = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(InstallerPath)).First())
                {
                    installProcess.EnableRaisingEvents = true;
                    installProcess.Exited += InstallProcessExited;

                    while (!installProcess.HasExited) ;
                }
            }
            catch (InvalidOperationException)
            {
                _installExitCode = -1;
            }

            if (_installExitCode == 0)
            {
                OutputTextBox.AppendText(SpareForm.TaskFinishedString, Color.Green);
                return true;
            }

            OutputTextBox.AppendText(" Aborted!\r\n", Color.Red);
            return false;
        }
        #endregion
    }
}
