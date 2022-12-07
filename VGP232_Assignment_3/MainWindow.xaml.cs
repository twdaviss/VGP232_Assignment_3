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
using TextureAtlasLib;
using static TextureAtlasLib.SpriteInfo;
using static TextureAtlasLib.Spritesheet;
using static VGP232_Assignment_3.SpriteList;


namespace VGP232_Assignment_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Spritesheet();
        }

        private void Browse_Clicked(object sender, RoutedEventArgs e)
        {
            Sprites.Clear();
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "PNG | *.png;";
            Nullable<bool> result = dlg.ShowDialog();

            //dlg.Multiselect = true;

            //sheet.OutputFile = dlg.FileName;
            OutputDirectory = System.IO.Path.GetDirectoryName(dlg.FileName);
            OutputFile = System.IO.Path.GetFileName(dlg.FileName);

            Output_Text.Text = OutputDirectory;
            Filename_Text.Text = OutputFile;


        }

        private void Ouput(object sender, TextChangedEventArgs e)
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
                InputPaths.Add(element);
                Images.Add(new BitmapImage(new Uri(element)));
            }
            //Image_List.ItemsSource = Images;
        }

        private void Columns_Text_Changed(object sender, TextChangedEventArgs e)
        {

            if (Columns_Text.Text != "")
            {
                Columns = int.Parse(Columns_Text.Text);
            }
        }

        private void MetaBox_Checked(object sender, RoutedEventArgs e)
        {
            if (MetaBox.IsChecked == true)
            {
                IncludeMetaData = true;
            }
            else
            {
                IncludeMetaData = false;
            }
        }

        private void Generate_Button_Clicked(object sender, RoutedEventArgs e)
        {
            Generate(true);
        }

        private void Save_Button_Clicked(object sender, RoutedEventArgs e)
        {
            SaveFile(false);
        }

        private void Open_Button_Clicked(object sender, RoutedEventArgs e)
        {
            LoadFile();
        }

        private void Exit_Button_Clicked(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Are you sure you want to exit? If so, do you want to save changes?";
            string caption = "Word Processor";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;
            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    if (SaveFile(true)){
                        this.Close();
                    }
                    else
                    {
                        Exit_Button_Clicked(sender, e);
                    }
                    break;
                case MessageBoxResult.No:
                    this.Close();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void Save_As_Button_Clicked(object sender, RoutedEventArgs e)
        {
            SaveFile(true);
            Save_Button.IsEnabled = true;
        }

        private void New_Button_Clicked(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Are you sure you want to start over?\nChanges will be lost.\nIf yes, do you want to save changes first?";
            string caption = "Word Processor";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;
            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    if (SaveFile(true)){
                        Reset();
                    }
                    else
                    {
                        New_Button_Clicked(sender,e);
                    }
                    break;
                case MessageBoxResult.No:
                    Reset();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void Update_Directory(object sender, RoutedEventArgs e)
        {
            OutputDirectory = Output_Text.Text;
        }

        private void Update_File(object sender, RoutedEventArgs e)
        {
            OutputFile = Filename_Text.Text;
        }

        private void Remove_Button_Clicked(object sender, RoutedEventArgs e)
        {
            Images.RemoveAt(Image_List.SelectedIndex);
        }

        private void Columns_Text_LostFocus(object sender, RoutedEventArgs e)
        {
            if(Columns_Text.Text == "")
            {
                Columns_Text.Text = 1.ToString();
            }
        }
    }
}
