using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoViDAccountant.Services;

namespace CoViDAccountant
{
    public partial class App
    {
        //[Inject] private UIService UI { get; set; }

        private Components.Dialogs.ConfirmDialog _confirmDlg;
        private Components.Dialogs.ErrorDialog _errorDlg;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                UI.SetConfirmDialog(_confirmDlg);
                UI.SetErrorDialog(_errorDlg);
            }
        }
        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    await base.OnAfterRenderAsync(firstRender);
        //    if (firstRender)
        //    {
        //        UI.SetConfirmDialog(_confirmDlg);
        //        UI.SetErrorDialog(_errorDlg);
        //    }
        //}
    }
}
