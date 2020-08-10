using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstallPOC.Models;
using MvvmHelpers;

namespace InstallPOC.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		public List<string> PhotoCollections { get; set; } = new List<string>() 
		{
			"Shading Analysis",
			"Roof",
			"Panels",
			"Racks",
		};
	}
}
