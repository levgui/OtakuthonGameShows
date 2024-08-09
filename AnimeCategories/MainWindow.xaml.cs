using Classes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace AnimeCategories
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MetroDialogSettings dialogSettings = new MetroDialogSettings();
        public MainWindow()
        {
            InitializeComponent();

            var vm = new MainWindowViewModel();
            DataContext = vm;

            dialogSettings.ColorScheme = MetroDialogColorScheme.Accented;
            dialogSettings.DialogMessageFontSize = 40;
            dialogSettings.DialogTitleFontSize = 40;

            StartGame();
        }

        public void BucketControl_DragOver(object sender, DragEventArgs e)
        {
            var data = e.Data as DataObject;

            if (data != null)
            {
                var anime = e.Data.GetData(typeof(Anime)) as Anime;
                var sourceBucket = e.Data.GetData(typeof(CategoryBucket)) as CategoryBucket;
                var destinationBucket = (sender as FrameworkElement).DataContext as CategoryBucket;

                if (anime != null && sourceBucket != null && destinationBucket != null && sourceBucket != destinationBucket)
                {
                    destinationBucket.AddAnime(anime);
                    sourceBucket.RemoveAnime(anime);
                    e.Data.SetData(typeof(CategoryBucket), destinationBucket);
                }

            }
        }

        private async void Check_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            if (vm != null)
            {
                var result = vm.CheckResults();

                await this.ShowMessageAsync("Result", result, MessageDialogStyle.Affirmative, dialogSettings);
            }
        }

        private async void Hint_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            if (vm != null)
            {
                var hints = vm.GetHints();

                await this.ShowMessageAsync("Hints...", hints, MessageDialogStyle.Affirmative, dialogSettings);
            }
        }

        private async void Reset_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            if (vm != null)
            {
                var message = $"This will generate a completely new game.{Environment.NewLine}Number of Anime: {vm.AnimeCount}{Environment.NewLine}Number of Categories: {vm.TagCount}{Environment.NewLine}Top Anime: {vm.TopAnimeCount}";
                if (vm.HardMode)
                {
                    message = $"This will generate a completely new game.{Environment.NewLine}Number of Anime: {vm.AnimeCount}{Environment.NewLine}Number of Categories: {vm.TagCount}";
                }

                var result = await this.ShowMessageAsync("Reset", message, MessageDialogStyle.AffirmativeAndNegative, dialogSettings);

                if (result == MessageDialogResult.Affirmative)
                {
                    StartGame();
                }

            }
        }

        private async void StartGame()
        {
            var vm = DataContext as MainWindowViewModel;
            if (vm != null)
            {
                try
                {
                    vm.StartGame();
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("OOPS", ex.Message, MessageDialogStyle.Affirmative, dialogSettings);
                }
            }
        }

        private async void Solve_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            if (vm != null)
            {
                var result = await this.ShowMessageAsync("Solve", $"This will solve the game.", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);

                if (result == MessageDialogResult.Affirmative)
                {
                    vm.SolveGame();
                }

            }
        }
    }
}