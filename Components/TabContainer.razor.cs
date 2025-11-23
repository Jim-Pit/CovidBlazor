using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace CoViDAccountant.Components
{
	public static class TabSign
    {
		public const string Users = "Users";
		public const string Vaccines = "Vaccines";
		public const string DiagnosticCenters = "Diagnostic Centers";
		public const string Cities = "Cities";
	}

	public partial class TabContainer //<TPage> where TPage : ITabsInterface
	{
		//[CascadingParameter]
		//private TPage Parent { get; set; }
		[CascadingParameter]
		private ITabs Parent { get; set; }

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		//[Parameter]
		//public EventCallback<string> InnerTabsCallback { get; set; }

		public Tab ActiveTab { get; set; }
		public List<Tab> Tabs = new List<Tab>();

		internal void AddTab(Tab tab)
		{
			Tabs.Add(tab);
			// first tab to be added will be active
			if (Tabs.Count == 1)
			{
				ActiveTab = tab;
				Parent.ActiveTabSign = tab.Sign;
				//if (InnerTabsCallback.HasDelegate)
				//{
				//	InnerTabsCallback.InvokeAsync(ActiveTab.Sign);
				//}
			}
			//StateHasChanged();
		}

		private string GetButtonClass(Tab tab)
		{
			return tab == ActiveTab ? "btn-primary" : "btn-secondary";
		}

		private async Task ActivatePage(Tab tab)
		{
			if (Parent.ActiveTabSign == tab.Sign)
			{
				return;
			}
			ActiveTab = tab;
			Parent.ActiveTabSign = tab.Sign;
			await Parent.LoadData(tab.Sign);
			//if (InnerTabsCallback.HasDelegate)
			//{
			//	InnerTabsCallback.InvokeAsync(ActiveTab.Sign);
			//}
		}

		//public void SetActiveTab()
		//{
		//	ActiveTab = ComponentTabs.FirstOrDefault(t => t.Sign == Parent.ActiveTabSign);
		//}
	}
}
