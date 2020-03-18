Эта программа отображает скрины экрана и кликая по скрину, клики происходят на экране.
=================
Создание потока скриншота экрана. Преобразование изображения в массив байт. Помещение этого массива в поток. 
Вывод изображения из потока в элемент Image.
===================================================
16.03.2020
Программа, которая делает скриншот, помещает его в Image и по клику на Image, стрелка перемещается на экране.
ИТОГ: Написан метод ClickToImageRetransToMonitor. Мызывается он в оброботчиках событий нажатия мыши, первый аргумент - количество кликов от 1 до 3х, второй аргумент, это типа мышка. 
ЭТо код использования WINapi и с помощью которого происходит нажатие на кнопку мыши

        //This is a replacement for Cursor.Position in WinForms
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        //Это метод симуляции нажатия лемой кнопки мыши
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

=======================
16.03.2020 добавил перемещение курсора с зажатой ЛКП
========================