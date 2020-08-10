using System;
using System.Collections.Generic;
using System.Text;

namespace InstallPOC.Models
{
	public class PhotoCollection
	{
		public string Title { get; set; }
		public List<Photo> Photos { get; set; } = new List<Photo>();
	}
}
