using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.IO.Ports;
//using System.Management;
using System.Text;
using Windows.Networking.Sockets;
using System.Collections.ObjectModel;
using Microsoft.UI.Composition.SystemBackdrops;

using Windows.UI.Popups;
using System.Threading;

using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Microsoft.UI;           // Needed for WindowId.
using Microsoft.UI.Windowing; // Needed for AppWindow.
using WinRT.Interop;
using Windows.UI;          // Needed for XAML/HWND interop.
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Core;
using System.Xml.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FSGaryityTool_Win11
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page1 : Page
    {
        public static int Con = 0;
        public static int txf = 0;
        public static int tx = 0; //TXHEX
        public static int rx = 0; //RXHEX
        public static int dtr = 1;
        public static int rts = 0;
        public static int shtime = 0;//ShowTime
        public static int autotr = 1;//AUTOScroll

        

        private bool _isLoaded;
        public static string str;
        private DateTime current_time = new DateTime();

        public static class CommonRes
        {
            public static SerialPort _serialPort = new SerialPort();
            public static SerialPort serialPort2 = new SerialPort();

        }


        public Page1()
        {
            this.InitializeComponent();

            this.Loaded += Page1_Loaded;

            CommonRes._serialPort.DataReceived += _serialPort_DataReceived;

            // ����Ĵ����̨������һ��List<string>��Ϊ����Դ
            List<string> BaudRates = new List<string>()
            {
                "75", "110", "134", "150", "300", "600", "1200", "1800", "2400", "4800", "7200", "9600", "14400", "19200", "38400", "57600", "74880","115200", "128000", "230400", "250000", "500000", "1000000", "2000000"
            };
            // ��ComboBox��ItemsSource���԰󶨵��������Դ
            BANDComboBox.ItemsSource = BaudRates;
            // ����Ĭ��ѡ��
            BANDComboBox.SelectedItem = "115200"; // ��"9600"����ΪĬ��ѡ��

            List<string> ParRates = new List<string>()
            {
                "None", "Odd", "Even", "Mark", "Space"
            };
            PARComboBox.ItemsSource = ParRates;
            PARComboBox.SelectedItem = "None";

            List<string> StopRates = new List<string>()
            {
                "None", "One", "OnePointFive", "Two"
            };
            STOPComboBox.ItemsSource = StopRates;
            STOPComboBox.SelectedItem = "One";
            
            for (int j = 5; j < 10; ++j)
            {
                DATAComboBox.Items.Add(j);
            }
            DATAComboBox.SelectedItem = 8;

            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = COMButton.Background as SolidColorBrush;
            var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            var theme = Application.Current.RequestedTheme;

            if (theme == ApplicationTheme.Dark)
            {
                // ��ǰ������ɫģʽ
                DTRButton.Background = new SolidColorBrush(darkaccentColor);
                DTRButton.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (theme == ApplicationTheme.Light)
            {
                // ��ǰ����ǳɫģʽ
                DTRButton.Background = new SolidColorBrush(ligtaccentColor);
                DTRButton.Foreground = new SolidColorBrush(Colors.White);
            }
            CommonRes._serialPort.DtrEnable = true;

            if (theme == ApplicationTheme.Dark)
            {
                // ��ǰ������ɫģʽ
                AUTOScrollButton.Background = new SolidColorBrush(darkaccentColor);
                AUTOScrollButton.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (theme == ApplicationTheme.Light)
            {
                // ��ǰ����ǳɫģʽ
                AUTOScrollButton.Background = new SolidColorBrush(ligtaccentColor);
                AUTOScrollButton.Foreground = new SolidColorBrush(Colors.White);
            }
            
            
        }

        private void Page1_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                COMButton_Click(this, new RoutedEventArgs());
                _isLoaded = true;
            }
                
        }

        //public event SerialDataReceivedEventHandler DataReceived;

        private void COMButton_Click(object sender, RoutedEventArgs e)
        {
            //COMButton.Content = "Clicked";
            SearchAndAddSerialToComboBox(CommonRes._serialPort, COMComboBox);           //ɨ�貢����������������б�

            void SearchAndAddSerialToComboBox(SerialPort MyPort, ComboBox MyBox)
            {
                RXTextBox.Text = RXTextBox.Text + "Start search SerialPort\r\n";
                string[] ArryPort;                                          // �����ַ������飬������Ϊ Buffer
                ArryPort = SerialPort.GetPortNames();                       // SerialPort.GetPortNames()��������Ϊ��ȡ��������п��ô��ڣ����ַ���������ʽ���
                string scom = String.Join("\r\n", ArryPort);
                RXTextBox.Text = RXTextBox.Text + scom + "\r\n";
                MyBox.Items.Clear();                                        // �����ǰ��Ͽ������˵�����                  
                for (int i = 0; i < ArryPort.Length; i++)
                {
                    MyBox.Items.Add(ArryPort[i]);                           // �����еĿ��ô��ں���ӵ��˿ڶ�Ӧ����Ͽ���
                }
                RXTextBox.Text = RXTextBox.Text + "Search SerialPort succeed!\r\n";

            }
        }

        private void Settings_ColorValuesChanged(Windows.UI.ViewManagement.UISettings sender, object args)
        {
            var color = sender.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
            CONTButton.Background = new SolidColorBrush(color);
        }

        private void CONTButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = new Windows.UI.ViewManagement.UISettings();
            var color = settings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);


            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = COMButton.Background as SolidColorBrush;
            var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            var theme = Application.Current.RequestedTheme;


            settings.ColorValuesChanged += Settings_ColorValuesChanged;

            if (Con == 0)
            {
                try
                {
                    CommonRes._serialPort.PortName = (string)COMComboBox.SelectedItem;                  //�����Ĵ�������Ϊѡ�񴮿ڵ�ComboBox����е�����
                    CommonRes._serialPort.BaudRate = Convert.ToInt32(BANDComboBox.SelectedItem);        //��ѡ������ComboBox����е�����תΪInt�ͣ����ҽ��в����ʵ�����

                    //RXTextBox.Foreground = foregroundColor;
                    /*
                    if (theme == ApplicationTheme.Dark)
                    {
                        // ��ǰ������ɫģʽ
                        RXTextBox.Foreground = new SolidColorBrush(darkaccentColor);
                    }
                    else if (theme == ApplicationTheme.Light)
                    {
                        // ��ǰ����ǳɫģʽ
                        RXTextBox.Foreground = new SolidColorBrush(ligtaccentColor);
                    }*/

                    RXTextBox.Text = RXTextBox.Text + "BaudRate = " + Convert.ToInt32(BANDComboBox.SelectedItem) + "\r\n";

                    CommonRes._serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), (string)PARComboBox.SelectedItem);        //У��λ
                    CommonRes._serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), (string)STOPComboBox.SelectedItem); //ֹͣλ

                    RXTextBox.Text = RXTextBox.Text + "Parity = " + (Parity)Enum.Parse(typeof(Parity), (string)PARComboBox.SelectedItem) + "\r\n";
                    RXTextBox.Text = RXTextBox.Text + "StopBits = " + (StopBits)Enum.Parse(typeof(StopBits), (string)STOPComboBox.SelectedItem) + "\r\n";

                    CommonRes._serialPort.DataBits = Convert.ToInt32(DATAComboBox.SelectedItem);                                //����λ

                    RXTextBox.Text = RXTextBox.Text + "DataBits = " + Convert.ToInt32(DATAComboBox.SelectedItem) + "\r\n";

                    CommonRes._serialPort.ReadTimeout = 1500;
                    //_SerialPort.DtrEnable = true;                                                                             //���������ն˾�����Ϣ
                    CommonRes._serialPort.Encoding = Encoding.UTF8;
                    CommonRes._serialPort.ReceivedBytesThreshold = 1;                                               //DataReceived����ǰ�ڲ����뻺�������ֽ���

                    //RXTextBox.Foreground = foregroundColor;

                    RXTextBox.Text = RXTextBox.Text + "SerialPort " + COMComboBox.SelectedItem + " IS OPEN" + "\r\n";

                    CommonRes._serialPort.Open();                                                                               //�򿪴���
                    CONTButton.Content = "DISCONNECT";
                    Con = 1;

                    //CONTButton.Background = new SolidColorBrush(color);
                    if (theme == ApplicationTheme.Dark)                                                                         //�������Ӱ�ť������ɫ
                    {
                        // ��ǰ������ɫģʽ
                        CONTButton.Background = new SolidColorBrush(darkaccentColor);
                        CONTButton.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    else if (theme == ApplicationTheme.Light)
                    {
                        // ��ǰ����ǳɫģʽ
                        CONTButton.Background = new SolidColorBrush(ligtaccentColor);
                        CONTButton.Foreground = new SolidColorBrush(Colors.White);
                    }


                }
                catch                                                                                                     //����򿪴���ʧ�� ��Ҫ�����¾�ʾ
                {
                    RXTextBox.Text = RXTextBox.Text + "�򿪴���ʧ�ܣ������������" + "\r\n";
                    //MessageBox.Show("�򿪴���ʧ�ܣ������������", "����");
                    Con = 0;
                    CONTButton.Content = "CONNECT";
                    CONTButton.Background = backgroundColor;
                    CONTButton.Foreground = foregroundColor;
                }

            }
            else
            {
                //RXTextBox.Text = RXTextBox.Text + "SerialPort IS DISCONNECT\r\n";

                try
                {
                    CommonRes._serialPort.Close();                                                                              //�رմ���
                    RXTextBox.Text = RXTextBox.Text + "SerialPort IS CLOSE" + "\r\n";
                }
                catch (Exception err)                                                                       //һ������¹رմ��ڲ���������Բ���Ҫ�Ӵ������
                {
                    RXTextBox.Text = RXTextBox.Text + err + "\r\n";
                }
                CONTButton.Content = "CONNECT";
                Con = 0;
                CONTButton.Background = backgroundColor;
                CONTButton.Foreground = foregroundColor;
            }
        }


        /*
        private async Task InitializeSerialPort()
        {
            var devices = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());

            // ���û���ҵ��κ��豸����ʾ�û�
            if (devices.Count == 0)
            {
                RXTextBox.Text = "û���ҵ��κδ����豸����������";
                return;
            }

            var device = devices[0];

            // ���豸��Ϣ�д���һ�� SerialDevice ����
            var serialDevice = await SerialDevice.FromIdAsync(device.Id);

            // ���ô��ڵĲ��������粨���ʣ�����λ��ֹͣλ��
            serialDevice.BaudRate = 9600;
            serialDevice.DataBits = 8;
            serialDevice.StopBits = SerialStopBitCount.One;
            serialDevice.Parity = SerialParity.None;

            // �� SerialDevice ����ֵ�� SerialPort ����� Device ����
            _SerialPort.Device = serialDevice;

            // ����һ�� DataReader �������ڴӴ��ڶ�ȡ����
            _SerialPort.DataReader = new DataReader(_SerialPort.Device.InputStream);

            // ����һ�� DataWriter ���������򴮿�д������
            _SerialPort.DataWriter = new DataWriter(_SerialPort.Device.OutputStream);

            // ע�� DataReceived �¼��Ĵ������
            _SerialPort.Device.DataReceived += SerialPort_DataReceived;
        }


        private async void SerialPort_DataReceived(SerialDevice sender, object args)
        {
            // ��ȡ��ǰʱ��
            var current_time = System.DateTime.Now;

            // ��ȡ���ڽ��ջ��������ֽ���
            var length = await _SerialPort.DataReader.LoadAsync(1024);

            // �ж��Ƿ����ַ�����ʽ��ȡ
            if (rx == 0)
            {
                // ��ȡ���ڽ��ջ������ַ���
                var str = _SerialPort.DataReader.ReadString(length);

                // ʹ�� DispatcherQueue.TryEnqueue ���������� UI
                DispatcherQueue.TryEnqueue(() =>
                {
                    // �ж��Ƿ���ʾʱ��
                    if (ShowTimeCheckBox.IsChecked == true)
                    {
                        // �ڽ����ı�������ʾʱ��
                        RXTextBox.Text += current_time.ToString("HH:mm:ss") + "  ";
                    }

                    // �ڽ����ı�������ʾ�ַ���
                    RXTextBox.Text += str + "\r\n";
                });
            }
            else // ����ֵ��ʽ��ȡ
            {
                // ����һ���ֽ����飬���ڴ洢���յ�������
                byte[] data = new byte[length];

                // �� DataReader �ж�ȡ���ݵ��ֽ�������
                _SerialPort.DataReader.ReadBytes(data);

                // ʹ�� DispatcherQueue.TryEnqueue ���������� UI
                DispatcherQueue.TryEnqueue(() =>
                {
                    // �����ֽ�����
                    for (int i = 0; i < length; i++)
                    {
                        // ������ת��Ϊ�ַ�����ʽ
                        string str = Convert.ToString(data[i], 16).ToUpper();

                        // �ڽ����ı�������ʾ����
                        RXTextBox.Text += "0x" + (str.Length == 1 ? "0" + str + " " : str + " ");
                    }

                    // �ڽ����ı����л���
                    RXTextBox.Text += "\r\n";
                });
            }
        }
        */

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (txf == 1)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    RXTextBox.Text += "\r\n";
                });
                
                txf = 0;
            }

            if (rx == 0)                                         // ������ַ�����ʽ��ȡ
            {
                string str = CommonRes._serialPort.ReadExisting();                    // ��ȡ���ڽ��ջ������ַ���
                if (shtime == 1)
                {
                    //��ʾʱ��
                    current_time = System.DateTime.Now;     //��ȡ��ǰʱ��

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        RXTextBox.Text = RXTextBox.Text + current_time.ToString("HH:mm:ss") + "  ";
                    });

                }

                DispatcherQueue.TryEnqueue(() =>
                {
                    RXTextBox.Text = RXTextBox.Text + str + "\r\n";                          // �ڽ����ı����н�����ʾ
                });

                if (autotr == 1)
                {
                    //RXTextBox.ScrollToEnd();
                }

                else
                {

                }
                

            }
            else                                                            // ����ֵ��ʽ��ȡ
            {
                int length = CommonRes._serialPort.ReadByte();                       // ��ȡ���ڽ��ջ������ֽ���

                byte[] data = new byte[length];                             // ������ͬ�ֽڵ�����

                CommonRes._serialPort.Read(data, 0, length);                          // ���ڶ�ȡ���������ݵ�������

                for (int i = 0; i < length; i++)
                {
                    string str = Convert.ToString(data[i], 16).ToUpper();                                   // ������ת��Ϊ�ַ�����ʽ

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        RXTextBox.Text = RXTextBox.Text + "0x" + (str.Length == 1 ? "0" + str + " " : str + " ");        // ��ӵ����ڽ����ı�����
                    });


                    if (autotr == 1)
                    {
                        //RXTextBox.ScrollToEnd();
                    }

                    else
                    {

                    }
                }
            }
        }


        private void COMComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TXTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TXButton_Click(object sender, RoutedEventArgs e)//��������
        {
            if (CommonRes._serialPort.IsOpen)            // ��������豸�Ѿ�����
            {
                if (tx == 0)        // ��������ַ�����ʽ��������
                {
                    char[] str = new char[1];  // ����һ���ַ����飬ֻ��һλ

                    try
                    {
                        if (shtime == 1)
                        {
                            //��ʾʱ��
                            current_time = System.DateTime.Now;     //��ȡ��ǰʱ��
                            RXTextBox.Text = RXTextBox.Text + current_time.ToString("HH:mm:ss") + "  ";

                        }
                        for (int i = 0; i < TXTextBox.Text.Length; i++)
                        {
                            str[0] = Convert.ToChar(TXTextBox.Text.Substring(i, 1));  // ȡ�������ı����еĵ�i���ַ�
                            CommonRes._serialPort.Write(str, 0, 1);                            // д�봮���豸���з���
                        }
                        RXTextBox.Text = RXTextBox.Text + "TX: " + TXTextBox.Text + "\r\n";

                        if (autotr == 1)
                        {
                            //RXTextBox.ScrollToEnd();
                        }

                        else
                        {

                        }
                        txf = 1;
                    }
                    catch
                    {
                        //MessageBox.Show("�����ַ�д�����!", "����");   // �������ʹ���Ի���
                        RXTextBox.Text = RXTextBox.Text + "�����ַ�д�����!" + "\r\n";
                        
                        CONTButton_Click(sender, e);
                    }
                }
                else                                                  // �������ֵ����ʽ����
                {
                    byte[] Data = new byte[1];                        // ����һ��byte�������ݣ��൱��C���Ե�unsigned char����
                    int flag = 0;                                     // ����һ����־����־���ǵڼ�λ
                    try
                    {
                        if (shtime == 1)
                        {
                            //��ʾʱ��
                            current_time = System.DateTime.Now;     //��ȡ��ǰʱ��
                            RXTextBox.Text = RXTextBox.Text + current_time.ToString("HH:mm:ss") + "  ";

                        }
                        for (int i = 0; i < TXTextBox.Text.Length; i++)
                        {
                            if (TXTextBox.Text.Substring(i, 1) == " " && flag == 0)                // ����ǵ�һλ������Ϊ���ַ�
                            {
                                continue;
                            }

                            if (TXTextBox.Text.Substring(i, 1) != " " && flag == 0)                // ����ǵ�һλ������Ϊ���ַ�
                            {
                                flag = 1;                                                         // ��־ת���ڶ�λ����ȥ
                                if (i == TXTextBox.Text.Length - 1)                                // ��������ı����ַ��������һ���ַ�
                                {
                                    Data[0] = Convert.ToByte(TXTextBox.Text.Substring(i, 1), 16);  // ת��Ϊbyte�������ݣ���16������ʾ
                                    CommonRes._serialPort.Write(Data, 0, 1);                                // ͨ�����ڷ���
                                    RXTextBox.Text = RXTextBox.Text + Data + " ";
                                    flag = 0;                                                     // ��־�ص���һλ����ȥ
                                }
                                continue;
                            }
                            else if (TXTextBox.Text.Substring(i, 1) == " " && flag == 1)           // ����ǵڶ�λ���ҵڶ�λ�ַ�Ϊ��
                            {
                                Data[0] = Convert.ToByte(TXTextBox.Text.Substring(i - 1, 1), 16);  // ֻ����һλ�ַ�ת��Ϊbyte�������ݣ���ʮ��������ʾ
                                CommonRes._serialPort.Write(Data, 0, 1);                                    // ͨ�����ڷ���
                                RXTextBox.Text = RXTextBox.Text + Data + " ";
                                flag = 0;                                                         // ��־�ص���һλ����ȥ
                                continue;
                            }
                            else if (TXTextBox.Text.Substring(i, 1) != " " && flag == 1)           // ����ǵڶ�λ�ַ����ҵ�һλ�ַ���Ϊ��
                            {
                                Data[0] = Convert.ToByte(TXTextBox.Text.Substring(i - 1, 2), 16);  // ����һ����λ�ַ�ת��Ϊbyte�������ݣ���ʮ��������ʾ
                                CommonRes._serialPort.Write(Data, 0, 1);                                    // ͨ�����ڷ���
                                RXTextBox.Text = RXTextBox.Text + Data + " ";
                                flag = 0;                                                         // ��־�ص���һλ����ȥ
                                continue;
                            }

                        }
                        RXTextBox.Text += "\r\n";

                        if (autotr == 1)
                        {
                            //RXTextBox.ScrollToEnd();
                        }

                        else
                        {

                        }
                        txf = 1;
                    }
                    catch
                    {
                        //MessageBox.Show("������ֵд�����!", "����");
                        RXTextBox.Text = RXTextBox.Text + "�����ַ�д�����!" + "\r\n";

                        CONTButton_Click(sender, e);
                    }
                }
                TXTextBox.Text = "";
            }
        }
    
        private void CLEARButton_Click(object sender, RoutedEventArgs e)
        {
            RXTextBox.Text = "";    //����ı�������
        }

        private void RXTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void BANDComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void PARComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void STOPComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void DATAComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        

        private void RXHEXButton_Click(object sender, RoutedEventArgs e)    //������ʮ����������ʾ
        {
            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = COMButton.Background as SolidColorBrush;
            var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            var theme = Application.Current.RequestedTheme;
            if (rx == 0)
            {
                if (theme == ApplicationTheme.Dark)
                {
                    // ��ǰ������ɫģʽ
                    RXHEXButton.Background = new SolidColorBrush(darkaccentColor);
                    RXHEXButton.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (theme == ApplicationTheme.Light)
                {
                    // ��ǰ����ǳɫģʽ
                    RXHEXButton.Background = new SolidColorBrush(ligtaccentColor);
                    RXHEXButton.Foreground = new SolidColorBrush(Colors.White);
                }
                rx = 1;
            }
            else
            {
                RXHEXButton.Background = backgroundColor;
                RXHEXButton.Foreground = foregroundColor;
                rx = 0;
            }
        }

        private void TXHEXButton_Click(object sender, RoutedEventArgs e)    //������ʮ����������ʾ
        {
            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = COMButton.Background as SolidColorBrush;
            var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            var theme = Application.Current.RequestedTheme;
            if (tx == 0)
            {
                if (theme == ApplicationTheme.Dark)
                {
                    // ��ǰ������ɫģʽ
                    TXHEXButton.Background = new SolidColorBrush(darkaccentColor);
                    TXHEXButton.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (theme == ApplicationTheme.Light)
                {
                    // ��ǰ����ǳɫģʽ
                    TXHEXButton.Background = new SolidColorBrush(ligtaccentColor);
                    TXHEXButton.Foreground = new SolidColorBrush(Colors.White);
                }
                tx = 1;
            }
            else
            {
                TXHEXButton.Background = backgroundColor;
                TXHEXButton.Foreground = foregroundColor;
                tx = 0;
            }
        }


        private void RSTButton_Click(object sender, RoutedEventArgs e)      //�Զ�����
        {
            CommonRes._serialPort.RtsEnable = true;
            Thread.Sleep(10);
            CommonRes._serialPort.DtrEnable = true;
            Thread.Sleep(10);
            CommonRes._serialPort.DtrEnable = false;
            Thread.Sleep(10);
            CommonRes._serialPort.RtsEnable = false;
            if(dtr == 1)
            {
                DTRButton_Click(sender, e);
                Thread.Sleep(50);
                DTRButton_Click(sender, e);
            }
            else
            {
                Thread.Sleep(50);
                DTRButton_Click(sender, e);
            }
            //CommonRes._serialPort.DtrEnable = true;
        }

        private void DTRButton_Click(object sender, RoutedEventArgs e)      //DTR�ź�ʹ��
        {
            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = COMButton.Background as SolidColorBrush;
            var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            var theme = Application.Current.RequestedTheme;
            if (dtr == 0)
            {
                if (theme == ApplicationTheme.Dark)
                {
                    // ��ǰ������ɫģʽ
                    DTRButton.Background = new SolidColorBrush(darkaccentColor);
                    DTRButton.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (theme == ApplicationTheme.Light)
                {
                    // ��ǰ����ǳɫģʽ
                    DTRButton.Background = new SolidColorBrush(ligtaccentColor);
                    DTRButton.Foreground = new SolidColorBrush(Colors.White);
                }
                CommonRes._serialPort.DtrEnable = true;
                dtr = 1;
            }
            else
            {
                DTRButton.Background = backgroundColor;
                DTRButton.Foreground = foregroundColor;

                CommonRes._serialPort.DtrEnable = false;
                dtr = 0;
            }
        }

        private void RTSButton_Click(object sender, RoutedEventArgs e)      //RTS�ź�ʹ��
        {
            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = COMButton.Background as SolidColorBrush;
            var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            var theme = Application.Current.RequestedTheme;
            if (rts == 0)
            {
                if (theme == ApplicationTheme.Dark)
                {
                    // ��ǰ������ɫģʽ
                    RTSButton.Background = new SolidColorBrush(darkaccentColor);
                    RTSButton.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (theme == ApplicationTheme.Light)
                {
                    // ��ǰ����ǳɫģʽ
                    RTSButton.Background = new SolidColorBrush(ligtaccentColor);
                    RTSButton.Foreground = new SolidColorBrush(Colors.White);
                }
                CommonRes._serialPort.RtsEnable = true;
                rts = 1;
            }
            else
            {
                RTSButton.Background = backgroundColor;
                RTSButton.Foreground = foregroundColor;

                CommonRes._serialPort.RtsEnable = false;
                rts = 0;
            }
        }

        private void ShowTimeButton_Click(object sender, RoutedEventArgs e)
        {
            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = COMButton.Background as SolidColorBrush;
            var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            var theme = Application.Current.RequestedTheme;
            if (shtime == 0)
            {
                if (theme == ApplicationTheme.Dark)
                {
                    // ��ǰ������ɫģʽ
                    ShowTimeButton.Background = new SolidColorBrush(darkaccentColor);
                    ShowTimeButton.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (theme == ApplicationTheme.Light)
                {
                    // ��ǰ����ǳɫģʽ
                    ShowTimeButton.Background = new SolidColorBrush(ligtaccentColor);
                    ShowTimeButton.Foreground = new SolidColorBrush(Colors.White);
                }
                shtime = 1;
            }
            else
            {
                ShowTimeButton.Background = backgroundColor;
                ShowTimeButton.Foreground = foregroundColor;

                shtime = 0;
            }
        }

        private void AUTOScrollButton_Click(object sender, RoutedEventArgs e)
        {
            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = COMButton.Background as SolidColorBrush;
            var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            var theme = Application.Current.RequestedTheme;
            if (autotr == 0)
            {
                if (theme == ApplicationTheme.Dark)
                {
                    // ��ǰ������ɫģʽ
                    AUTOScrollButton.Background = new SolidColorBrush(darkaccentColor);
                    AUTOScrollButton.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (theme == ApplicationTheme.Light)
                {
                    // ��ǰ����ǳɫģʽ
                    AUTOScrollButton.Background = new SolidColorBrush(ligtaccentColor);
                    AUTOScrollButton.Foreground = new SolidColorBrush(Colors.White);
                }
                autotr = 1;
            }
            else
            {
                AUTOScrollButton.Background = backgroundColor;
                AUTOScrollButton.Foreground = foregroundColor;

                autotr = 0;
            }
        }

    }
}
