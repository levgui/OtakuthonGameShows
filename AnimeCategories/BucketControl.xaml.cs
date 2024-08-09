using Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnimeCategories
{
    /// <summary>
    /// Interaction logic for BucketControl.xaml
    /// </summary>
    public partial class BucketControl : UserControl
    {
        public BucketControl()
        {
            InitializeComponent();
        }

        private void Item_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var anime = (sender as FrameworkElement).DataContext as Anime;
                var bucket = (sender as FrameworkElement).Tag as CategoryBucket;

                if (anime != null && bucket != null)
                {
                    var data = new DataObject();
                    data.SetData(typeof(Anime), anime);
                    data.SetData(typeof(CategoryBucket), bucket);
                    DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
                }

            }

        }
    }
}
