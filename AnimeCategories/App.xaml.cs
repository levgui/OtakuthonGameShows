using ControlzEx.Theming;
using System.Configuration;
using System.Windows;

namespace AnimeCategories
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var theme = ConfigurationManager.AppSettings["THEME"];

            if (theme == null)
            {
                theme = "Dark.Red";
            }

            ThemeManager.Current.ChangeTheme(this, theme);
        }
    }

}
