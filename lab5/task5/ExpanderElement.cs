using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO;

namespace task5
{
    public class ExpanderElement
    {
        public ExpanderElement(string path)
        {
            image.BeginInit();
            image.UriSource = new Uri(path);
            image.EndInit();

            DateTime time = File.GetCreationTime(path);
            date = String.Format("{0:M/d/yyyy}", time);

            fileName = path.Substring(path.LastIndexOf('/'));
        }
        BitmapImage image = new BitmapImage();
        public string fileName;
        public string date;


    }
}
