using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// 1. Import the InteropServices type
using System.Threading;
using System.Runtime.InteropServices;


namespace translatorAssistant
{
    
    public partial class Form1 : Form
    {
        string querySTR = "";
        string urlMT = "https://www.multitran.com/m.exe?l1=1&l2=2&s="; //посковая строка мультитрана
        string urlGl = "https://translate.google.com/?hl=ru#view=home&op=translate&sl=auto&tl=ru&text="; //посковая строка гугла
        string sourse = "Google translate";
        string outApp = "chrome.exe";
        int HotKeyCode = 39;
        int UniqueHotkeyId;
        bool nDelFlag = false;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public const int KEYEVENTF_KEYDOWN = 0x0000; // New definition
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public const int VK_LCONTROL = 0xA2; //Left Control key code
        public const int A = 0x41; //A key code
        public const int C = 0x43; //C key code
        public Form1()
        {
            InitializeComponent();
            // 3. Register HotKey
            //кол-во регестрируемых элементов
            UniqueHotkeyId = 1;
            // устанавливаем триггер на Right
            HotKeyCode = (int)Keys.Right;
            // Register the "Right" hotkey
            Boolean F9Registered = RegisterHotKey(
                this.Handle, UniqueHotkeyId, 0x0000, HotKeyCode
            );
            // убеждаемся что прошла успешная регисрация
            if (F9Registered)
            {/* MessageBox.Show("Выбранная кнопка зарегистрировалась");*/}
            else
            {/*MessageBox.Show("Выбранная кнопка не зарегистрировалась");*/}
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public static void Ctrl_C_Emulate()
        {
            keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(C, 0, 0, 0);
            keybd_event(C, 0, 2, 0);
            keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);
        }
        protected override void WndProc(ref Message m)
        {
            //Захват события на нажатие кнопки
            if (m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();

                if (id == 1)
                {
                    // эмулируем нажатие ctrl + c
                    Ctrl_C_Emulate();
                    // времменно отснавливаем процесс чтобы данные успели скопироваться в кэш
                    Thread.Sleep(500);
                    // берем данные с буфера
                    string query = Clipboard.GetText();
                    //MessageBox.Show(nDelFlag.ToString());
                    nDelFlag = checkBox1.Checked;
                    if (sourse == "Google translate")
                    {

                        if (nDelFlag)
                        {
                            query = query.Replace("\n", " ");
                        }
                        else
                        {
                            query = query.Replace("\n", "%0A");
                        }
                        query = query.Replace(" ", "%20");
                        querySTR = urlGl + query;
                    }
                    else if (sourse == "Multitran")
                    {
                        querySTR = urlMT + query.Replace(" ", "+");
                    }
                    System.Diagnostics.Process.Start(outApp, querySTR);
                }
            }

            base.WndProc(ref m);
        }
        private void VisitLink()
        {
            // Change the color of the link text by setting LinkVisited
            // to true.
            linkLabel1.LinkVisited = true;
            //Call the Process.Start method to open the default browser
            //with a URL:
            System.Diagnostics.Process.Start("https://sun9-61.userapi.com/impg/3U9wSt5ey4InxOZ6XauOI0SzCDu7kry8x-o6rg/YduOlXZLd-4.jpg?size=1220x475&quality=96&sign=023583015d06cca2d7c1516901a3cdd9&type=album");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitLink();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // получаем новый код
                HotKeyCode = int.Parse(textBox1.Text);
                Boolean UnRegistered = UnregisterHotKey(this.Handle, UniqueHotkeyId);
                Boolean Registered = RegisterHotKey(
                this.Handle, UniqueHotkeyId, 0x0000, HotKeyCode
                );
                
                // получаем внешнее приложение для запуска
                outApp = textBox2.Text;
                //  Обработка сервиса
                int serviceId = comboBox2.SelectedIndex;
                if (serviceId == 0)
                    sourse = "Google translate";
                if (serviceId == 1)
                    sourse = "Multitran";
                
            }
            catch
            {
                MessageBox.Show("Введены не корректные данные");
            }
        }
    }
}
