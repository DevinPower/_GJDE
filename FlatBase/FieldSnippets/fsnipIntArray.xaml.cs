using System;
using System.Collections.Generic;
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

namespace FlatBase.FieldSnippets
{
    /// <summary>
    /// Interaction logic for fsnipBool.xaml
    /// </summary>
    public partial class fsnipIntArray : UserControl
    {
        WriteableBitmap wbitmap;
        byte[,,] pixels;
        const int width = 100;
        const int height = 100;

        public int[] values { get; set; }

        public static DependencyProperty ValuesProperty =
         DependencyProperty.Register("values", typeof(int[]), typeof(int[]));

        public fsnipIntArray()
        {
            InitializeComponent();

            values = new int[100];

            wbitmap = new WriteableBitmap(
                width, height, 400, 400, PixelFormats.Bgra32, null);
            pixels = new byte[height, width, 4];

            // Clear to black.
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int i = 0; i < 3; i++)
                        pixels[row, col, i] = 0;
                    pixels[row, col, 3] = 255;
                }
            }

            /*for (int iX = 0; iX < 100; iX++)
            {
                for (int iY = 0; iY < iX; iY++)
                {
                    pixels[iX, iY, 0] = 255;
                    pixels[iX, iY, 0] = 255;
                    pixels[iX, iY, 0] = 255;
                    pixels[iX, iY, 0] = 255;
                }
            }*/

            // Copy the data into a one-dimensional array.
            byte[] pixels1d = new byte[height * width * 4];
            int index = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int i = 0; i < 4; i++)
                        pixels1d[index++] = pixels[row, col, i];
                }
            }



            Int32Rect rect = new Int32Rect(0, 0, width, height);
            int stride = 4 * width;
            wbitmap.WritePixels(rect, pixels1d, stride, 0);

            pbDisplay.Source = wbitmap;
        }
        bool editing = false;

        public void manipulateImage(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!editing)
                return;
            Point p = e.GetPosition(pbDisplay);

            p.X /= 4;
            p.Y /= 4;

            if ((int)p.X >= 0 && (int)p.X < 100)
                values[(int)p.X] = (int)p.Y;

            updateImage();
        }

        public void updateImage()
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int i = 0; i < 3; i++)
                        pixels[row, col, i] = 0;
                    pixels[row, col, 3] = 255;
                }
            }

            for (int iX = 0; iX < 100; iX++)
            {
                for (int iY = 0; iY < values[iX]; iY++)
                {
                    pixels[iY, iX, 0] = 255;
                    pixels[iY, iX, 0] = 255;
                    pixels[iY, iX, 0] = 255;
                    pixels[iY, iX, 0] = 255;
                }
            }

            // Copy the data into a one-dimensional array.
            byte[] pixels1d = new byte[height * width * 4];
            int index = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int i = 0; i < 4; i++)
                        pixels1d[index++] = pixels[row, col, i];
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            int stride = 4 * width;
            wbitmap.WritePixels(rect, pixels1d, stride, 0);
        }

        private void EndEdit(object sender, MouseButtonEventArgs e)
        {
            editing = false;
        }

        private void PbDisplay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            editing = true;
        }

        private void PbDisplay_MouseLeave(object sender, MouseEventArgs e)
        {
            editing = false;
        }
    }
}
