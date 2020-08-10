using System;
using System.Collections.Generic;
using System.Text;
using InstallPOC.Models;
using Xamarin.Forms;

namespace InstallPOC.Controls
{
	public class PhotoTemplateSelector : DataTemplateSelector
	{
		public DataTemplate PhotoTemplate { get; set; }
		public DataTemplate EmptyTemplate { get; set; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			return string.IsNullOrWhiteSpace(((Photo)item).PhotoPath) ? EmptyTemplate : PhotoTemplate; ;
		}
	}
}
