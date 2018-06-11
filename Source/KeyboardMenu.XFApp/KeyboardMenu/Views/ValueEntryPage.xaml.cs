using System;
using KeyboardMenu.Interfaces;
using Xamarin.Forms;

namespace KeyboardMenu.Views
{
	public partial class ValueEntryPage : ContentPage
	{
		public ValueEntryPage ()
		{
			InitializeComponent ();
		}

	    private void ValueEntryCompleted(object sender, EventArgs e)
	    {
	        if (BindingContext is IValueEntryCompleted completed)
	        {
                completed.ValueEntryCompletedCommand.Execute();
	        }
	    }
	}
}