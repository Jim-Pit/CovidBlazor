using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace CoViDAccountant.Components
{
	public partial class Tabs<TPage> where TPage : ITabs
	{
		[CascadingParameter]
		private TPage Parent { get; set; }

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[Parameter]
		public List<string> TabsSigns { get; set; }

		private string GetButtonClass(string sign)
		{
			return sign == Parent.ActiveTabSign ? "btn-primary" : "btn-secondary";
		}

		private async Task RefreshContent(string sign)
		{
			if (Parent.ActiveTabSign == sign) return;
			Parent.ActiveTabSign = sign;
			//Parent.Refreshing = true;
			await Parent.LoadData(sign);
		}
	}
}
