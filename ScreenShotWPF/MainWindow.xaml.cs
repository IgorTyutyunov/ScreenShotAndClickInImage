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
            Thread.Sleep(100);
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
            lblwidthImage.Content = "Ширина скрина: " + image.ActualWidth.ToString();
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
        /// если flag == 0, то это один клик, если flag == 1, то это двойной клик.
        /// </summary>
        int flag = 0;

        /// <summary>
        /// В этих переменных будут храниться координаты нажатия и отпускания ЛКМ
        /// </summary>
        int corMonX1 = 0, corMonX2 = 0, corMonY1 = 0, corMonY2 = 0;

        /// <summary>
        /// Метод эмуляции клика, кликаем по скрину, клик происходит на экране. countClick=1,2,3
        /// </summary>
        /// <param name="countClick"></param>
        /// <param name="e"></param>
        void MouseButtonClickToImageRetransToMonitor(MouseButtonEventArgs e)
        {
            Dictionary<string, string> coorMonXY = CoorMonXY(image, e);
            //сон нужен только если программа тестируется на одном компьютере
            Thread.Sleep(1000);
            //Если нажата левая кнопка мыши
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                corMonX1 = Convert.ToInt32(coorMonXY["coorMonX"]);
                corMonY1 = Convert.ToInt32(coorMonXY["coorMonY"]);
                tbxX1.Text = coorMonXY["coorMonX"];
                tbxY1.Text = coorMonXY["coorMonY"];
                mainWindow.Title = e.ClickCount.ToString();
                if (e.ClickCount == 2) {
                    LeftMouseClick(corMonX1, corMonY1);
                    LeftMouseClick(corMonX1, corMonY1);
                    flag = 1;//flag = 1, это значит, что произошёл двойной клик, а значит, что при отпукании ЛКМ
                             // не нужно вызывать ещё один клик.
                }
            }

            if (e.LeftButton == MouseButtonState.Released)
            {
                corMonX2 = Convert.ToInt32(coorMonXY["coorMonX"]);
                corMonY2 = Convert.ToInt32(coorMonXY["coorMonY"]);
                tbxX2.Text = coorMonXY["coorMonX"];
                tbxY2.Text = coorMonXY["coorMonY"];
                mainWindow.Title = e.ClickCount.ToString();
                if((corMonX2 != corMonX1) || (corMonY1 != corMonY2))
                    LeftMouseClickAndMove(corMonX1, corMonY1, corMonX2, corMonY2);
                if ((corMonX2 == corMonX1) && (corMonY1 == corMonY2) && (flag==0))
                    LeftMouseClick(corMonX2, corMonY2);
                flag = 0;
            }
            lblCoordinateXYInMonitor.Content = "Координаты на монитре: X=" + coorMonXY["coorMonX"] + " Y=" + coorMonXY["coorMonY"];
            lblCoordinateXYInImage.Content = "Координаты на скрине: X=" + coorMonXY["xImage"] + " Y=" + coorMonXY["yImage"];
            DifferenceHeight.Content = "Разница высоты: " + coorMonXY["heightDifference"];
            DifferenceWidth.Content = "Разница ширины: " + coorMonXY["widthDifference"]; ;
        }

       
        private void btnMoveCoursor_Click(object sender, RoutedEventArgs e)
        {
            //Thread.Sleep(1000);
            //LeftMouseClick(Convert.ToInt32(tbxX.Text), Convert.ToInt32(tbxY.Text));
            LeftMouseClickAndMove(Convert.ToInt32(tbxX1.Text), Convert.ToInt32(tbxY1.Text), Convert.ToInt32(tbxX2.Text), Convert.ToInt32(tbxY2.Text));
        }

        /// <summary>
        /// Метод сопоставляет координаты на скрине и изображения экрана.
        /// Метод принимает элемент интерфейса Image и данные о собитиях кнопки мыши.
        /// Возвращает метод список [coorMonX,coorMonY,xImage,yImage,widthDifference,heightDifference], где:
        /// coorMonX,coorMonY - координаты клика на мониторе
        /// xImage,yImage - координаты клика на скрине
        /// widthDifference,heightDifference - разница в размерах скрина и монитора
        /// </summary>
        /// <param name="image"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private Dictionary<string, string> CoorMonXY(System.Windows.Controls.Image image, MouseButtonEventArgs e) {
            Dictionary<string, string> massCoorMonXY;
         
            /// <summary>
            /// в переменной resolution будут храниться ширина и высота экрана (разрешение)
            /// </summary>
            System.Drawing.Size resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            int xImage, yImage;//координаты клика на скрине
            xImage = Convert.ToInt32(e.GetPosition(image).X);
            yImage = Convert.ToInt32(e.GetPosition(image).Y);
            double widthDifference = 0, heightDifference = 0;//разница в размерах скрина и монитора
            widthDifference = resolution.Width / image.ActualWidth;
            heightDifference = resolution.Height / image.ActualHeight;
            int coorMonX, coorMonY;//координаты клика на мониторе
            coorMonX = Convert.ToInt32(xImage * widthDifference);
            coorMonY = Convert.ToInt32(yImage * heightDifference);
            massCoorMonXY = new Dictionary<string, string> {
                {"coorMonX",coorMonX.ToString() },
                {"coorMonY", coorMonY.ToString()},
                {"xImage", xImage.ToString()},
                {"yImage", yImage.ToString()},
                {"widthDifference", widthDifference.ToString()},
                {"heightDifference", heightDifference.ToString()}
            };
            return massCoorMonXY;
        }

        private void image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //сон нужен только если программа тестируется на одном компьютере
            Dictionary<string, string> coorMonXY = CoorMonXY(image, e);
            Thread.Sleep(500);
            RightMouseClick(Convert.ToInt32(coorMonXY["coorMonX"]), Convert.ToInt32(coorMonXY["coorMonY"]));
        }
    }
}
