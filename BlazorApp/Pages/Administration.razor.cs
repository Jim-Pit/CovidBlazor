using CoViDAccountant.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoViDAccountant.Pages
{
    public partial class Administration : ITabs
    {
		//public bool Refreshing { get; set; } = true;

		// public property that is set by tab control
		public string ActiveTabSign { get; set; }

		// this property is used to retain the active tab after an add or an update
		public bool Modified { get; set; }

		public async Task LoadData(string selectedTab)
		{
			ActiveTabSign = selectedTab;
			switch (selectedTab)
			{
				
			}
			//Refreshing = false;
		}
    }
}
