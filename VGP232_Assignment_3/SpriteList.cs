using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VGP232_Assignment_3
{
    class SpriteList 
    {
        public static List<ImageSource> Sprites = new List<ImageSource>();
        public static SpriteList instance = null;

        public static SpriteList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SpriteList();
                }
                return instance;
            }
        }

        public static List<ImageSource> GetSprites
        {
            get { return Sprites; }
            set { 
            }
        }


    }
}
