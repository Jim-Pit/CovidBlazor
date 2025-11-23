using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoViDAccountant.Components
{
    public partial class ModalDialog<TModel> //: ComponentBase
        where TModel : class
    {
        [Obsolete] [Parameter] public TModel Model { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string Header { get; set; }
        [Parameter] public EventCallback<TModel> Submit { get; set; }
        [Parameter] public bool SystemicDialog { get; set; }
        [Parameter] public string CssStyle { get; set; }

        [CascadingParameter] public string Error { get; set; }

        private EditContext _editContext;
        //private ValidationMessageStore _validationMessages;

        private bool _onDisplay;

        //protected override void OnParametersSet()
        //{
        //    base.OnParametersSet();
        //}
        public void Show(TModel model)
        {
            Error = string.Empty;
            if (model != null)
                _editContext = new EditContext(model);
            _onDisplay = true;
            if(SystemicDialog)
                StateHasChanged();
        }
        public void Hide()
        {
            _onDisplay = false;
        }
        public void ReValidate(FieldIdentifier  field)
        {
            _editContext.NotifyFieldChanged(field);
            //_editContext.NotifyValidationStateChanged();
            //_editContext.Validate();
            //var errors = _editContext.GetValidationMessages();
            //_editContext.NotifyValidationStateChanged();
        }

        private async Task OnSubmit()
        {
            //if (!_editContext.Validate())
            //    return;
            await Submit.InvokeAsync(_editContext.Model as TModel);
            //await Submit.InvokeAsync(Model);
            if (string.IsNullOrEmpty(Error)) { }
                //Hide();
        }
    }
}
