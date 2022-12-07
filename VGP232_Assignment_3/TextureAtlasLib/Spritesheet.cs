using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media;
using VGP232_Assignment_3;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TextureAtlasLib
{
    public class DummyClass : Spritesheet
    {
        public int _dcolumns;
        public String dOutputFile { get; set; }
        public String dOutputDirectory { get; set; }
        public bool _dincludeMetaData;
        public List<string> _dimages = new List<string>();

        public DummyClass() { }
        public DummyClass(bool go)
        {
            _dcolumns = Columns;
            dOutputFile = OutputFile;
            dOutputDirectory = OutputDirectory;
            _dincludeMetaData = IncludeMetaData;
            foreach(string element in InputPaths)
            {
                _dimages.Add(element);
            }
        }
    }
    public class Spritesheet
    {
        public static event PropertyChangedEventHandler StaticPropertyChanged;
        private static int _columns = 1;
        public static int Columns {get {return _columns; } set {_columns=value; NotifyStaticPropertyChanged();}}
        private static String _outputFile;
        public static String OutputFile { get { return _outputFile; } set { _outputFile = value; NotifyStaticPropertyChanged(); } }
        private static String _outputDirectory;
        public static String OutputDirectory { get { return _outputDirectory; } set { _outputDirectory = value; NotifyStaticPropertyChanged(); } }
        private static bool _includeMetaData;
        public static bool IncludeMetaData
        {
            get
            {
                return _includeMetaData;
            }
            set
            {
                if (value == null)
                {
                    _includeMetaData = false;
                }
                else
                {
                    _includeMetaData = value;
                }
                NotifyStaticPropertyChanged();
            }
        }
        public static List<String> InputPaths = new List<string>();
        public static ObservableCollection<ImageSource> _images = new ObservableCollection<ImageSource>();
        public static ObservableCollection<ImageSource> Images
        {
            get { return _images; }
            set
            {
                _images = value;
            }
        }
        public static Spritesheet instance = null;
        public static Spritesheet Instance { get {if (instance == null){ instance = new Spritesheet(); } return instance;}}
        public static bool prevSaved;
        public static string prevSavedLocation;

        /// <summary>
        /// Checks if the properties are set correctly. If it is not correct, then it will throw an exception.
        /// </summary>
        public static void CopyDummy (DummyClass dummy)
        {
            Columns = dummy._dcolumns;
            OutputFile = dummy.dOutputFile;
            OutputDirectory = dummy.dOutputDirectory;
            IncludeMetaData = dummy._dincludeMetaData;

            foreach (string element in dummy._dimages)
            {
                InputPaths.Add(element);
                Images.Add(new BitmapImage(new Uri (element)));
            }

        }
        private static void Validate()
        {
            if (InputPaths == null || InputPaths.Count == 0)
            {
                throw new Exception("The input path cannot be empty");
            }

            if (string.IsNullOrEmpty(OutputFile))
            {
                throw new Exception("The output file cannot be empty");
            }

            if (string.IsNullOrEmpty(OutputDirectory))
            {
                throw new Exception("The output directory cannot be empty");
            }

            if (Columns < 1)
            {
                throw new Exception("Column size must be at least 1");
            }
        }

        /// <summary>
        /// Generates the a texture atlas and meta data.
        /// </summary>
        /// <param name="overwrite">enable to allow the output file to be overwritten</param>
        /// 
        public static void Generate(bool overwrite)
        {
            Validate();

            string outputPath = Path.Combine(OutputDirectory, OutputFile);

            if (File.Exists(outputPath))
            {
                if (overwrite)
                {
                    File.Delete(outputPath);
                }
                else
                {
                    throw new Exception("The output file already exists.");
                }
            }

            // The original Logic was from: https://forum.unity.com/threads/quick-and-dirty-c-sprite-sheet-generator.436802/
            // Retrieve the png input files in the Input Path specified.
            // string[] files = Directory.GetFiles(InputPath, "*.png");
            int fileCount = InputPaths.Count;
            if (fileCount > 0)
            {
                // Determines the largest sprite and use that as the cell size.
                int maxWidth = 0;
                int maxHeight = 0;
                foreach (string f in InputPaths)
                {
                    Image img = Image.FromFile(f);
                    maxWidth = Math.Max(maxWidth, img.Width);
                    maxHeight = Math.Max(maxHeight, img.Height);
                    img.Dispose();
                }

                if (fileCount < Columns)
                {
                    // Single row, reducing column count to match file count.
                    Columns = fileCount;
                }

                int width = Columns * maxWidth;
                int rows = (fileCount / Columns) + ((fileCount % Columns > 0) ? 1 : 0);
                int height = rows * maxHeight;

                // Console.WriteLine("Output: {0} rows, {1} cols, {2} x {3} resolution.", rows, Columns, width, height);

                List<SpriteInfo> spriteInfos = new List<SpriteInfo>();
                Bitmap sheet = new Bitmap(width, height);
                using (Graphics gfx = Graphics.FromImage(sheet))
                {
                    int col = 0;
                    int row = 0;
                    foreach (string f in InputPaths)
                    {
                        // Sprite Info intialization
                        SpriteInfo sprite = new SpriteInfo();
                        sprite.Name = System.IO.Path.GetFileNameWithoutExtension(f);

                        Image img = Image.FromFile(f);

                        int x = (col * maxWidth) + (maxWidth / 2 - img.Width / 2);
                        int y = (row * maxHeight) + (maxHeight / 2 - img.Height / 2);

                        Rectangle srcRect = new Rectangle(0, 0, img.Width, img.Height);
                        Rectangle destRect = new Rectangle(x, y, img.Width, img.Height);

                        // Populate the coordinates for the sprite.
                        sprite.Left = x;
                        sprite.Top = y;
                        sprite.Right = x + img.Width;
                        sprite.Bottom = y + img.Height;

                        gfx.DrawImage(img, destRect, srcRect, GraphicsUnit.Pixel);

                        img.Dispose();

                        col++;
                        if (col == Columns)
                        {
                            col = 0;
                            row++;
                        }
                    }
                }

                if (IncludeMetaData)
                {
                    string exportedMetaData = Path.GetFileNameWithoutExtension(outputPath) + ".json";
                    string json = JsonSerializer.Serialize<List<SpriteInfo>>(spriteInfos);
                    File.WriteAllText(exportedMetaData, json);
                }

                sheet.Save(outputPath);
                string messageBoxText = "Sheet Successfully Generated. Would you like to view output?";
                string caption = "Word Processor";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Information;
                MessageBoxResult result;
                result = MessageBox.Show(messageBoxText, caption, button, icon);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Process.Start("explorer.exe",@OutputDirectory);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }
        public static bool SaveFile(bool newSave)
        {
            if (newSave)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.Filter = "XML | *.xml;";
                Nullable<bool> result = dlg.ShowDialog();
                if (dlg.FileName != "")
                {
                    DummyClass dummy = new DummyClass(true);
                    FileStream fs = new FileStream(dlg.FileName, FileMode.Create);

                    XmlSerializer xs = new XmlSerializer(typeof(DummyClass));
                    xs.Serialize(fs, dummy);
                    fs.Close();
                    string messageBoxText = "Save Successful";
                    string caption = "Word Processor";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBox.Show(messageBoxText, caption, button, icon);
                    return true;
                }
            }
            else
            {
                DummyClass dummy = new DummyClass(true);

                FileStream fs = new FileStream(prevSavedLocation, FileMode.Create);

                XmlSerializer xs = new XmlSerializer(typeof(DummyClass));
                xs.Serialize(fs, dummy);
                fs.Close();
                string messageBoxText = "Save Successful";
                string caption = "Word Processor";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Information;
                MessageBox.Show(messageBoxText, caption, button, icon);
                return true;
            }
            return false;
        }
        public static void LoadFile()
        {
            var mySerializer = new XmlSerializer(typeof(DummyClass));
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "XML | *.xml;";
            Nullable<bool> result = dlg.ShowDialog();
            if (dlg.FileName != "")
            {
                using var myFileStream = new FileStream(dlg.FileName, FileMode.Open);
                DummyClass dummyObject = (DummyClass)mySerializer.Deserialize(myFileStream);

                CopyDummy(dummyObject);
            }
            NotifyStaticPropertyChanged();
        }
        public static void Reset()
        {
            Columns = 0;
            OutputFile = "";
            OutputDirectory = "";
            IncludeMetaData = false;
            Images.Clear();
            NotifyStaticPropertyChanged();
            string messageBoxText = "All Fields Have Been Reset";
            string caption = "Word Processor";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show(messageBoxText, caption, button, icon);
        }
        private static void NotifyStaticPropertyChanged([CallerMemberName] string name = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }
    }
}
