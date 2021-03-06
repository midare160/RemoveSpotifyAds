﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Spare.UI
{
    public class ExceptionBox
    {
        #region Static
        private static TaskDialog _taskDialog;
        private static Exception _exception;

        public static TaskDialogResult Show(IWin32Window owner, Exception exception)
        {
            _exception = exception;

            _taskDialog = new TaskDialog
            {
                InstructionText = "Critical error occured!",
                Text = exception.Message,
                Icon = TaskDialogStandardIcon.Error,
                Cancelable = true,
                DetailsExpandedText = exception.ToString(),
                StartupLocation = TaskDialogStartupLocation.CenterScreen,
                OwnerWindowHandle = owner.Handle
            };

            var commandLinkSend = new TaskDialogCommandLink("SendFeedbackButton", "Report", "Create an issue on Github (Stacktrace will be copied to clipboard).");
            commandLinkSend.Click += CommandLinkSend_Click;

            var commandLinkIgnore = new TaskDialogCommandLink("IgnoreButton", "Ignore", "Proceed and ignore this error.");
            commandLinkIgnore.Click += (s, e) => _taskDialog.Close();

            _taskDialog.Controls.Add(commandLinkSend);
            _taskDialog.Controls.Add(commandLinkIgnore);

            return _taskDialog.Show();
        }

        private static void CommandLinkSend_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(_exception.ToString());
            Process.Start("https://github.com/midare160/SpotifyAdRemover/issues/new");

            _taskDialog.Close();
        }
        #endregion
    }
}
