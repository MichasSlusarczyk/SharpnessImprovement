/*
 * PROJEKT: Poprawa kontrastu
 * DATA WYKONANIA PROJEKTU: 21.01.2022
 * AUTOR: Ślusarczyk Michał
 * WERSJA: 1.0
*/

using System;
using System.IO;
using System.Windows;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;

namespace JA_PROJ
{
    public partial class MainWindow : Window
    {
        byte[] inputImageFile = null;
        byte[] outputImageFile = null;
        byte[] inputPixels = null;
        byte[] outputPixels = null;
        int numberOfThreads = 1;
        int imageWidth = 0;
        int imageHeight = 0;
        int dataLength = 0;
        int min = 0;
        int max = 0;


        Language language = JA_PROJ.Language.none;
        Stopwatch timer = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
        }

        void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            var filePath = string.Empty;
            inputImageFile = null;
            outputImageFile = null;
            inputPixels = null;
            outputPixels = null;
            timer.Reset();
            TimerLabel.Content = timer.ElapsedMilliseconds.ToString() + "ms";

            InputImage.Source = new BitmapImage(new Uri("/Images/InputImage.png", UriKind.Relative));
            OutputImage.Source = new BitmapImage(new Uri("/Images/OutputImage.png", UriKind.Relative));

            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {

                InitialDirectory = @"D:\",
                Title = "Browse Image Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "png",

                Filter = "Images (*.BMP;*.PNG;)|*.BMP;*.PNG;|" +
                "All files (*.*)|*.*",

                FilterIndex = 1,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            Nullable<bool> result = openFileDialog.ShowDialog();


            if (result == true)
            {
                filePath = openFileDialog.FileName;
                inputImageFile = File.ReadAllBytes(filePath);

                Bitmap tempBitmap = BitmapByteConverter.ByteToBitmap(inputImageFile);

                imageWidth = tempBitmap.Width;
                imageHeight = tempBitmap.Height;
                dataLength = 3*imageWidth*imageHeight;

                inputPixels = BitmapByteConverter.BitmapExtractByte(tempBitmap);

                InputImage.Source = new BitmapImage(new Uri(filePath));
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            var filePath = string.Empty;

            if (outputImageFile != null && inputImageFile != null)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new()
                {

                    InitialDirectory = @"D:\",
                    Title = "Browse Image Files",

                    CheckFileExists = false,
                    CheckPathExists = true,
                    AddExtension = true,

                    DefaultExt = "png",

                    Filter = "Images (*.BMP;*.PNG;)|*.BMP;*.PNG;|" +
                    "All files (*.*)|*.*",

                    FilterIndex = 1,

                };


                Nullable<bool> result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    filePath = saveFileDialog.FileName;

                    File.WriteAllBytes(filePath, outputImageFile);
                }
            }
        }

        private void CheckBox_ASM_Checked(object sender, RoutedEventArgs e)
        {

            if (ASM_CheckBox.IsChecked == true)
            {
                CPP_CheckBox.IsChecked = false;
                language = JA_PROJ.Language.ASM;
                timer.Reset();
            }
            else
            {
                language = JA_PROJ.Language.none;
            }

        }

        private void CheckBox_CPP_Checked(object sender, RoutedEventArgs e)
        {
            if (CPP_CheckBox.IsChecked == true)
            {
                ASM_CheckBox.IsChecked = false;
                language = JA_PROJ.Language.CPP;
                timer.Reset();
            }
            else
            {
                language = JA_PROJ.Language.none;
            }
        }

        private void NumberOfThreads_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("\\d");
            e.Handled = !regex.IsMatch(e.Text);

            int number = 0;
            Int32.TryParse(e.Text, out number);

            int holeNumber = 0;
            Int32.TryParse(NumberOfThreads.Text, out holeNumber);

            if (number == 0 && NumberOfThreads.Text == "")
            {
                e.Handled = true;
            }

            if (holeNumber > 6)
            {
                e.Handled = true;
            }

            if (number != 0 && number == 1 && number != 2 && number != 3 && number != 4 && holeNumber > 6)
            {
                e.Handled = true;
            }

            if ((number == 5 || number == 6 || number == 7 || number == 8 || number == 9) && NumberOfThreads.Text == "6")
            {
                e.Handled = true;
            }
        }

        private void NumberOfThreads_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (NumberOfThreads.Text != "")
            {
                numberOfThreads = Int32.Parse(NumberOfThreads.Text);
            }
        }

        private void ImproveContrast_Click(object sender, RoutedEventArgs e)
        {
            if (inputImageFile != null && language != JA_PROJ.Language.none)
            {
                {
                    ImproveContrast();
                }
            }
        }

        void ImproveContrast()
        {
            timer.Reset();

            dataLength = inputPixels.Length;
            byte[] tempPixels = new byte[dataLength];
            int tempLength = tempPixels.Length;

            for (int i = 0; i < dataLength - 2; i = i + 3)
            {
                tempPixels[i] = (byte)Math.Round((double)((inputPixels[i] + inputPixels[i + 1] + inputPixels[i + 2]) / 3));
            }

            min = tempPixels[0];
            max = tempPixels[0];

            for (int i = 0; i < tempLength - 2; i = i + 3)
            {
                if (tempPixels[i] < min)
                {
                    min = tempPixels[i];
                }
                else if (tempPixels[i] > max)
                {
                    max = tempPixels[i];
                }
            }

            for (int i = 0; i < tempLength - 2; i = i + 3)
            {
                tempPixels[i + 1] = tempPixels[i];
                tempPixels[i + 2] = tempPixels[i];
            }

            int brakujaceBajty = dataLength % 16;

            outputPixels = new byte[tempLength + brakujaceBajty];
            int outputLength = outputPixels.Length;

            tempPixels.CopyTo(outputPixels, 0);

            for (int i = tempLength; i < outputLength; i++)
            {
                outputPixels[i] = 0;
            }

            if (language == JA_PROJ.Language.ASM)
            {
                FuncASM();
                TimerLabel.Content = timer.ElapsedMilliseconds.ToString() + "ms";
            }
            else if (language == JA_PROJ.Language.CPP)
            {
                FuncCPP();
                TimerLabel.Content = timer.ElapsedMilliseconds.ToString() + "ms";
            }

            Array.Resize(ref outputPixels, tempLength);

            Bitmap tempbitmap = BitmapByteConverter.BitmapFromPixels(outputPixels, imageWidth, imageHeight);
            outputImageFile = BitmapByteConverter.BitmapToByte(tempbitmap);

            if (outputImageFile != null)
            {
                OutputImage.Source = LoadImage(outputImageFile);
            }
        }

        private void FuncASM()
        {
            [DllImport(@"D:\Michal_Slusarczyk\Studies\JA\Project\JA_PROJ\x64\Debug\JA_PROJ_ASM.dll")]
            static extern void ContrastASM(IntPtr bytes, int length, int min, int max);

            //BEZ WĄTKÓW
            //IntPtr buffer = Marshal.AllocHGlobal(outputPixels.Length * sizeof(byte));
            //Marshal.Copy(outputPixels, 0, buffer, outputPixels.Length);
            //IntPtr result = ContrastASM(buffer, outputPixels.Length, min, max);
            //Marshal.Copy(result, outputPixels, 0, outputPixels.Length);
            //Marshal.FreeHGlobal(buffer);

            //Z WĄTKAMI
            int outputLength = outputPixels.Length;

            double dataBlockLength = (double)outputLength / numberOfThreads;


            if (dataBlockLength < 16)
            {
                numberOfThreads = outputLength / 16;
                dataBlockLength = 16;
            }
            else
            {
                dataBlockLength = Math.Floor(dataBlockLength);
            }

            int BlockLength = (int)dataBlockLength;
            int roznica = outputLength - BlockLength * numberOfThreads;

            int[,] tab = new int[2, numberOfThreads];

            for (int i = 0; i < numberOfThreads; i++)
            {
                tab[0, i] = i * BlockLength;
                if (i != numberOfThreads - 1)
                {
                    tab[1, i] = BlockLength;
                }
                else
                {
                    tab[1, i] = BlockLength + roznica;
                }
            }

            IntPtr buffer = Marshal.AllocHGlobal(outputLength * sizeof(byte));
            Marshal.Copy(outputPixels, 0, buffer, outputLength);

            timer.Start();

            Thread[] arrayOfThreads = new Thread[numberOfThreads];

            for (int i = 0; i < numberOfThreads; i++)
            {
                IntPtr ptr = buffer + tab[0, i];
                int length = tab[1, i];

                arrayOfThreads[i] = new Thread(() =>
                {
                    ContrastASM(ptr, length, min, max);
                });

                arrayOfThreads[i].Start();
            }

            bool threadsWorking = true;

            while (threadsWorking)
            {
                threadsWorking = false;

                for (int i = 0; i < numberOfThreads; i++)
                {
                    if (arrayOfThreads[i].IsAlive)
                    {
                        threadsWorking = true;
                    }
                }
            }

            timer.Stop();

            Marshal.Copy(buffer, outputPixels, 0, outputLength);
            Marshal.FreeHGlobal(buffer);
        }

        private void FuncCPP()
        {
            [DllImport(@"D:\Michal_Slusarczyk\Studies\JA\Project\JA_PROJ\x64\Debug\JA_PROJ_CPP.dll")]
            static extern void ContrastCPP(IntPtr bytes, int length, int min, int max);

            //BEZ WĄTKÓW
            //IntPtr buffer = Marshal.AllocHGlobal(outputPixels.Length * sizeof(byte));
            //Marshal.Copy(outputPixels, 0, buffer, outputPixels.Length);
            //IntPtr result = ContrastCPP(buffer, outputPixels.Length, min, max);
            //Marshal.Copy(result, outputPixels, 0, outputPixels.Length);
            //Marshal.FreeHGlobal(buffer);

            //Z WĄTKAMI
            int outputLength = outputPixels.Length;

            double dataBlockLength = (double)outputLength/numberOfThreads;


            if (dataBlockLength < 16)
            {
                numberOfThreads = outputLength / 16;
                dataBlockLength = 16;
            }
            else
            {
                dataBlockLength = Math.Floor(dataBlockLength);
            }

            int BlockLength = (int)dataBlockLength;
            int roznica = outputLength - BlockLength*numberOfThreads;

            int[,] tab = new int[2, numberOfThreads];

            for (int i = 0; i < numberOfThreads; i++)
            {
                tab[0, i] = i * BlockLength;
                if (i != numberOfThreads - 1)
                {
                    tab[1, i] = BlockLength;
                }
                else
                {
                    tab[1, i] = BlockLength+roznica;
                }
            }


            IntPtr buffer = Marshal.AllocHGlobal(outputLength * sizeof(byte));
            Marshal.Copy(outputPixels, 0, buffer, outputLength);

            timer.Start();

            Thread[] arrayOfThreads = new Thread[numberOfThreads];

            for (int i = 0; i < numberOfThreads; i++)
            {
                IntPtr ptr = buffer + tab[0, i];
                int length = tab[1, i];

                arrayOfThreads[i] = new Thread(() =>
                {
                    ContrastCPP(ptr, length, min, max);
                });

                arrayOfThreads[i].Start();
            }

            bool threadsWorking = true;

            while (threadsWorking)
            {
                threadsWorking = false;

                for (int i = 0; i < numberOfThreads; i++)
                {
                    if (arrayOfThreads[i].IsAlive)
                    {
                        threadsWorking = true;
                    }
                }
            }

            timer.Stop();

            Marshal.Copy(buffer, outputPixels, 0, outputLength);
            Marshal.FreeHGlobal(buffer);
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
