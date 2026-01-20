using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Flasma_IOT_01.Views
{
	public partial class NgContentWindow : Window
	{
		public string? Result { get; private set; }

		public NgContentWindow()
		{
			InitializeComponent();
		}

		private void Ok_Click(object? sender, RoutedEventArgs e)
		{
			Result = InputBox.Text;
			Close(true);
		}

		private void Cancel_Click(object? sender, RoutedEventArgs e)
		{
			Close(false);
		}
	}
}
