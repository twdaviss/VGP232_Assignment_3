using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static TextureAtlasLib.SpriteInfo;
using static TextureAtlasLib.Spritesheet;
using static VGP232_Assignment_3.SpriteList;


namespace VGP232_Assignment_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //DataContext = new SpriteList();
        }

        private void Browse_Clicked(object sender, RoutedEventArgs e)
        {
            Sprites.Clear();
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "PNG | *.png;";
            Nullable<bool> result = dlg.ShowDialog();

            //dlg.Multiselect = true;

            Output_Text.Text = dlg.;

        }

        private void Ouput(object sender, TextChangedEventArgs e)x  
        {

        }

        private void Add_Clicked(object sender, RoutedEventArgs e)
        {
            Sprites.Clear();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "PNG | *.png;";
            dlg.Multiselect = true;

            Nullable<bool> result = dlg.ShowDialog();
            foreach (string element in dlg.FileNames)
            {
                Sprites.Add(new BitmapImage(new Uri(element)));
            }
            Image_List.ItemsSource = Sprites;
        }
    }
}
