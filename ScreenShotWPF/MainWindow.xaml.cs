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
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Windows.Threading;
using System.Threading;

namespace ScreenShotWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer timer = new DispatcherTimer();

        

        //This is a replacement for Cursor.Position in WinForms
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;

        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;

        //This simulates a left mouse click
        //Это метод симуляции нажатия лемой кнопки мыши
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        public static void RightMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, xpos, ypos, 0, 0);
        }

        /// <summary>
        /// Передвижение курсора с зажадой левой кнопкой мыши
        /// </summary>
        /// <param name="xpos1"></param>
        /// <param name="ypos1"></param>
        /// <param name="xpos2"></param>
        /// <param name="ypos2"></param>
        public static void LeftMouseClickAndMove(int xpos1, int ypos1, int xpos2, int ypos2)
        {
            SetCursorPos(xpos1, ypos1);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos1, ypos1, 0, 0);
            SetCursorPos(xpos2, ypos2);
            Thread.Sleep(300);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos1, ypos1, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos2, ypos2, 0, 0);
        }



        public MainWindow()
        {
            InitializeComponent();
        }

        private void timer_Tick(object sender, EventArgs e)
        {

            screnshotTimer();
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            /// <summary>
            /// в переменной resolution будут храниться ширина и высота экрана (разрешение)
            /// </summary>
            System.Drawing.Size resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            timer.Interval = new TimeSpan(1000);
            timer.Tick += timer_Tick;
            timer.Start();
            lblHeightMonitor.Content += resolution.Height.ToString();
            lblWidthMonitor.Content += resolution.Width.ToString();
          
        }

        void screnshotTimer()
        {


            //Работает !!! Хочу спать. Разберись потом, как это всё работает. В Частности интересуют методы i.
            //Помогло вот это https://issue.life/questions/55427008
            MemoryStream ms = new MemoryStream();
            BitmapImage i = new BitmapImage();
            Bitmap imageBitap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(imageBitap as System.Drawing.Image);

            graphics.CopyFromScreen(0, 0, 0, 0, imageBitap.Size);
            imageBitap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            ms.Position = 0;
            i.BeginInit();
            i.CacheOption = BitmapCacheOption.OnLoad;
            i.StreamSource = ms;
            i.EndInit();
            image.Source = i;
            ms.Dispose();
            graphics.Dispose();
            imageBitap.Dispose();
            lblwidthImage.Content = "Ширина скрина: "+image.ActualWidth.ToString();
            lblHeightImage.Content = "высота скрина: " + image.ActualHeight.ToString();

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseButtonClickToImageRetransToMonitor(e);
        }

        /// <summary>
        /// Метод эмуляции клика, кликаем по скрину, клик происходит на экране. countClick=1,2,3
        /// </summary>
        /// <param name="countClick"></param>
        /// <param name="e"></param>
        void MouseButtonClickToImageRetransToMonitor(MouseButtonEventArgs e) {
            /// <summary>
            /// в переменной resolution будут храниться ширина и высота экрана (разрешение)
            /// </summary>
            System.Drawing.Size resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            int x, y;//координаты клика на скрине
            x = Convert.ToInt32(e.GetPosition(image).X);
            y = Convert.ToInt32(e.GetPosition(image).Y);
            double xDifference = 0, yDifference = 0;//разница в размерах скрина и монитора
            xDifference = resolution.Width / image.ActualWidth;
            yDifference = resolution.Height / image.ActualHeight;
            int coorMonX, coorMonY;//координаты нажатия на мониторе
            coorMonX = Convert.ToInt32(x * xDifference);
            coorMonY = Convert.ToInt32(y * xDifference);
            tbxX1.Text = coorMonX.ToString();
            tbxY1.Text = coorMonY.ToString();
            //сон нужен только если программа тестируется на одном компьютере
            Thread.Sleep(300);

            //Если нажата левая кнопка мыши
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mainWindow.Title = e.ClickCount.ToString();
                switch (e.ClickCount)
                {
                    case 1:
                        LeftMouseClick(coorMonX, coorMonY);//функция имитации нажатия кнопки мыши                     
                        break;
                    case 2:
                        LeftMouseClick(coorMonX, coorMonY);
                        LeftMouseClick(coorMonX, coorMonY);               
                        break;
                    case 3:
                        LeftMouseClick(coorMonX, coorMonY);
                        LeftMouseClick(coorMonX, coorMonY);
                        LeftMouseClick(coorMonX, coorMonY);                     
                        break;
                }
                

            }
            //Если нажата правая кнопка мыши
            if (e.RightButton == MouseButtonState.Pressed)
            {
                RightMouseClick(coorMonX, coorMonY);

            }



            lblCoordinateXYInMonitor.Content = "Координаты на монитре: X=" + coorMonX + " Y=" + coorMonY;
            lblCoordinateXYInImage.Content = "Координаты на скрине: X=" + x + " Y=" + y;
            DifferenceHeight.Content = "Разница высоты: " + yDifference;
            DifferenceWidth.Content = "Разница ширины: " + xDifference;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnMoveCoursor_Click(object sender, RoutedEventArgs e)
        {
            //Thread.Sleep(1000);
            //LeftMouseClick(Convert.ToInt32(tbxX.Text), Convert.ToInt32(tbxY.Text));
            LeftMouseClickAndMove(Convert.ToInt32(tbxX1.Text), Convert.ToInt32(tbxY1.Text), Convert.ToInt32(tbxX2.Text), Convert.ToInt32(tbxY2.Text));
        }

         
    }
}
