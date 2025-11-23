using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoViDAccountant.Components.Dialogs;
using Microsoft.AspNetCore.Components;

namespace CoViDAccountant.Services
{
    public class UIService
    {
        private ConfirmDialog _confirmDialogRef;
        private ErrorDialog _errorDialogRef;

        #region Confirm
        public void SetConfirmDialog(ConfirmDialog confirmDialogRef)
        {
            _confirmDialogRef = confirmDialogRef;
        }

        public void ShowConfirmDialog(string message, EventCallback onConfirm)//Func<Task> onConfirm)
        {
            _confirmDialogRef?.Show(message, onConfirm);
        }

        public void ShowConfirmDialog(string message, Func<Task> onConfirm)
        {
            _confirmDialogRef?.Show(message, onConfirm);
        }
        #endregion

        #region Error
        public void SetErrorDialog(ErrorDialog errorDialogRef)
        {
            _errorDialogRef = errorDialogRef;
        }

        public void ShowError(Exception exception, Action onClose = null)
        {
            _errorDialogRef?.Show("Exception", exception.Message, exception.ToString(), onClose);
        }

        public void ShowError(string message, string details = "", Action onClose = null)
        {
            _errorDialogRef?.Show("ERROR", message, details, onClose);
        }
        public void ShowErrorGeneric(string details = "", Action onClose = null)
        {
            _errorDialogRef?.Show("ERROR", "An error has occurred.", details, onClose);
        }
        #endregion
    }
}
