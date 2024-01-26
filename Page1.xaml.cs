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
        public static int tx = 0;
        public static int rx = 0;
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

            DTRCheckBox.IsChecked = true;
            //RTSCheckBox.IsChecked = true;

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

        }

        private void Page1_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                COMButton_Click(this, new RoutedEventArgs());
                _isLoaded = true;
            }
                
        }

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

        private void CONTButton_Click(object sender, RoutedEventArgs e)
        {
            if (Con == 0)
            {
                try
                {
                    CommonRes._serialPort.PortName = (string)COMComboBox.SelectedItem;                      //�����Ĵ�������Ϊѡ�񴮿ڵ�ComboBox����е�����
                    CommonRes._serialPort.BaudRate = Convert.ToInt32(BANDComboBox.SelectedItem);     //��ѡ������ComboBox����е�����תΪInt�ͣ����ҽ��в����ʵ�����

                    RXTextBox.Text = RXTextBox.Text + "BaudRate = " + Convert.ToInt32(BANDComboBox.SelectedItem) + "\r\n";

                    CommonRes._serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), (string)PARComboBox.SelectedItem);                       //У��λ
                    CommonRes._serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), (string)STOPComboBox.SelectedItem);                 //ֹͣλ

                    RXTextBox.Text = RXTextBox.Text + "Parity = " + (Parity)Enum.Parse(typeof(Parity), (string)PARComboBox.SelectedItem) + "\r\n";
                    RXTextBox.Text = RXTextBox.Text + "StopBits = " + (StopBits)Enum.Parse(typeof(StopBits), (string)STOPComboBox.SelectedItem) + "\r\n";

                    CommonRes._serialPort.DataBits = Convert.ToInt32(DATAComboBox.SelectedItem);     //����λ8

                    RXTextBox.Text = RXTextBox.Text + "DataBits = " + Convert.ToInt32(DATAComboBox.SelectedItem) + "\r\n";

                    CommonRes._serialPort.ReadTimeout = 1500;
                    //_SerialPort.DtrEnable = true;                               //���������ն˾�����Ϣ
                    CommonRes._serialPort.Encoding = Encoding.UTF8;
                    CommonRes._serialPort.ReceivedBytesThreshold = 1;                     //DataReceived����ǰ�ڲ����뻺�������ֽ���

                    RXTextBox.Text = RXTextBox.Text + "SerialPort " + COMComboBox.SelectedItem + " IS OPEN" + "\r\n";

                    CommonRes._serialPort.Open();                                         //�򿪴���
                    CONTButton.Content = "DISCONNECT";
                    Con = 1;
                }
                catch                                                           //����򿪴���ʧ�� ��Ҫ�����¾�ʾ
                {
                    RXTextBox.Text = RXTextBox.Text + "�򿪴���ʧ�ܣ������������" + "\r\n";
                    //MessageBox.Show("�򿪴���ʧ�ܣ������������", "����");
                    Con = 0;
                }

            }
            else
            {
                //RXTextBox.Text = RXTextBox.Text + "SerialPort IS DISCONNECT\r\n";

                try
                {
                    CommonRes._serialPort.Close();                                        //�رմ���
                    RXTextBox.Text = RXTextBox.Text + "SerialPort IS CLOSE" + "\r\n";
                }
                catch (Exception err)//һ������¹رմ��ڲ���������Բ���Ҫ�Ӵ������
                {
                    RXTextBox.Text = RXTextBox.Text + err + "\r\n";
                }
                CONTButton.Content = "CONNECT";
                Con = 0;
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
        private void COMComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TXTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TXButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CLEARButton_Click(object sender, RoutedEventArgs e)
        {
            RXTextBox.Text = "";
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
        private void RXHEXCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (RXHEXCheckBox.IsChecked == true)
            {
                rx = 1;
            }
            else
            {
                rx = 0;
            }
        }
        private void TXHEXCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void DTRCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (DTRCheckBox.IsChecked == true)
            {
                CommonRes._serialPort.DtrEnable = true;
            }
            else
            {
                CommonRes._serialPort.DtrEnable = false;
            }
        }
        private void RTSCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (RTSCheckBox.IsChecked == true)
            {
                CommonRes._serialPort.RtsEnable = true;
            }
            else
            {
                CommonRes._serialPort.RtsEnable = false;
            }
        }
        private void AUTOScrollCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void ShowTimeCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void RXTCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (txf == 1)
            {
                RXTextBox.Text += "\r\n";
                txf = 0;
            }

            if (rx == 0)                                         // ������ַ�����ʽ��ȡ
            {
                string str = CommonRes._serialPort.ReadExisting();                    // ��ȡ���ڽ��ջ������ַ���
                if (ShowTimeCheckBox.IsChecked == true)
                {
                    //��ʾʱ��
                    current_time = System.DateTime.Now;     //��ȡ��ǰʱ��
                    RXTextBox.Text = RXTextBox.Text + current_time.ToString("HH:mm:ss") + "  ";

                }
                RXTextBox.Text = RXTextBox.Text + str + "";                          // �ڽ����ı����н�����ʾ
                /*
                if (AUTOScrollCheckBox.IsChecked == true)
                {
                    //RXTextBox.ScrollToCaret();
                }
                else
                {

                }
                */

            }
            else                                                            // ����ֵ��ʽ��ȡ
            {
                int length = CommonRes._serialPort.ReadByte();                       // ��ȡ���ڽ��ջ������ֽ���

                byte[] data = new byte[length];                             // ������ͬ�ֽڵ�����

                CommonRes._serialPort.Read(data, 0, length);                          // ���ڶ�ȡ���������ݵ�������

                for (int i = 0; i < length; i++)
                {
                    string str = Convert.ToString(data[i], 16).ToUpper();                                   // ������ת��Ϊ�ַ�����ʽ
                    RXTextBox.Text = RXTextBox.Text + "0x" + (str.Length == 1 ? "0" + str + " " : str + " ");        // ��ӵ����ڽ����ı�����
                    /*
                    if (AUTOScrollCheckBox.IsChecked == true)
                    {
                        //RXTextBox.ScrollToCaret();
                    }
                    else
                    {

                    }
                    */
                }
            }
        }
    }
}
