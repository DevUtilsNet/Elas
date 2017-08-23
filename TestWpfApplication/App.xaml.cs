using System.Globalization;
using System.Threading;
using System.Windows;
using TestWpfApplication.Properties1;

namespace TestWpfApplication
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-Ru");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-Ru");
			//LinkedResources.Culture = new CultureInfo("ru-Ru");
			var string1 = LinkedResources.String1;
			var string3 = LinkedResources.Category_Docking;
			var image1 = LinkedResources.Image1;

			var form1 = new Form1();
			form1.Show();

			var form2 = new InnerFolder1.InnerFolder2.Form1();
			form2.Show();
		}
	}
}
