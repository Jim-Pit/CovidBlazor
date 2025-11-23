using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoViDAccountant.Components
{
	public interface ITabs
	{
		//bool Refreshing { get; set; }
		string ActiveTabSign { get; set; }
		Task LoadData(string selectedTabSign);
	}
}
