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
using System.Management;
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
using Microsoft.UI.Dispatching;
using WinRT.Interop;
using Windows.UI;          // Needed for XAML/HWND interop.
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Core;
using System.Xml.Linq;
using System.Diagnostics.Metrics;

using Tommy;
using System.Diagnostics;
using static System.Runtime.CompilerServices.RuntimeHelpers;

using System.Windows.Input;
using Windows.ApplicationModel.Contacts;
using System.Reflection.Metadata.Ecma335;
using static FSGaryityTool_Win11.Page1;
using Windows.Devices.Sensors;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FSGaryityTool_Win11
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public class DataItem
    {
        public string Timesr { get; set; }
        public string Rxstr { get; set; }
    }
    /*
    public class ComDataItem
    {
        public string ComName { get; set; }
    }
    */

    public sealed partial class Page1 : Page
    {
        public static int Con = 0;
        public static int txf = 0;
        public static int tx = 0; //TXHEX
        public static int rx = 0; //RXHEX
        public static int dtr = 0;//FTR
        public static int rts = 0;//RTS
        public static int shtime = 0;//ShowTime
        public static int autotr = 0;//AUTOScroll
        public static int autosaveset;
        public static int autosercom;
        public static int autoconnect;

        public static int rxs = 0;
        public static string[] ArryPort; //�����ַ������飬������Ϊ ArryPort
        public static string rxpstr;
        public static StringBuilder datapwate = new StringBuilder(2048);

        public static int Rollta = 0;

        public static int RunPBT = 0;
        public static int RunT = 0;

        public static string SYSAPLOCAL = Environment.GetFolderPath(folder: Environment.SpecialFolder.LocalApplicationData);
        public static string FSFolder = Path.Combine(SYSAPLOCAL, "FAIRINGSTUDIO");
        public static string FSGravif = Path.Combine(FSFolder, "FSGravityTool");
        public static string FSSetJson = Path.Combine(FSGravif, "Settings.json");
        public static string FSSetToml = Path.Combine(FSGravif, "Settings.toml");

        

        public Timer timer;
        public Timer timerSerialPort;

        private bool _isLoaded;
        public static string str;
        public DateTime current_time = new DateTime();

        public static class CommonRes
        {
            public static SerialPort _serialPort = new SerialPort();
            public static SerialPort serialPort2 = new SerialPort();

        }



        public Page1()
        {
            string DefaultBAUD;
            string DefaultPart;
            string DefaultSTOP;
            int DefaultDATA;

            using (StreamReader reader = File.OpenText(FSSetToml))
            {
                TomlTable SPsettingstomlr = TOML.Parse(reader);             //��ȡTOML
                //Debug.WriteLine("Print:" + SPsettingstomlr["FSGravitySettings"]["DefaultNvPage"]);
                //NvPage = int.Parse(settingstomlr["FSGravitySettings"]["DefaultNvPage"]);

                DefaultBAUD = SPsettingstomlr["SerialPortSettings"]["DefaultBAUD"];
                DefaultPart = SPsettingstomlr["SerialPortSettings"]["DefaultParity"];
                DefaultSTOP = SPsettingstomlr["SerialPortSettings"]["DefaultSTOP"];
                DefaultDATA = int.Parse(SPsettingstomlr["SerialPortSettings"]["DefaultDATA"]);

                tx = int.Parse(SPsettingstomlr["SerialPortSettings"]["DefaultTXHEX"]);
                rx = int.Parse(SPsettingstomlr["SerialPortSettings"]["DefaultRXHEX"]);
                dtr = int.Parse(SPsettingstomlr["SerialPortSettings"]["DefaultDTR"]);
                rts = int.Parse(SPsettingstomlr["SerialPortSettings"]["DefaultRTS"]);
                shtime = int.Parse(SPsettingstomlr["SerialPortSettings"]["DefaultSTime"]);
                autotr = int.Parse(SPsettingstomlr["SerialPortSettings"]["DefaultAUTOSco"]);
                autosaveset = int.Parse(SPsettingstomlr["SerialPortSettings"]["AutoDaveSet"]);
                autosercom = int.Parse(SPsettingstomlr["SerialPortSettings"]["AutoSerichCom"]);


                /*
                ["DefaultBAUD"] = "115200",
                ["DefaultParity"] = "None",
                ["DefaultSTOP"] = "One",
                ["DefaultDATA"] = "8",
                ["DefaultRXHEX"] = "0",
                ["DefaultTXHEX"] = "0",
                ["DefaultDTR"] = "1",
                ["DefaultRTS"] = "0",
                ["DefaultSTime"] = "0",
                ["DefaultAUTOSco"] = "1",
                */
            }

            this.InitializeComponent();

            this.Loaded += Page1_Loaded;

            RXListView.ItemsSource = new ObservableCollection<DataItem>();

            //COMListview.ItemsSource = new ObservableCollection<ComDataItem>();

            CommonRes._serialPort.DataReceived += _serialPort_DataReceived;

            // ����Ĵ����̨������һ��List<string>��Ϊ����Դ
            List<string> BaudRates = new List<string>()
            {
                "75", "110", "134", "150", "300", "600", "1200", "1800", "2400", "4800", "7200", "9600", "14400", "19200", "38400", "57600", "74880","115200", "128000", "230400", "250000", "500000", "1000000", "2000000"
            };
            // ��ComboBox��ItemsSource���԰󶨵��������Դ
            BANDComboBox.ItemsSource = BaudRates;
            // ����Ĭ��ѡ��
            BANDComboBox.SelectedItem = DefaultBAUD; // ��"9600"����ΪĬ��ѡ��

            List<string> ParRates = new List<string>()
            {
                "None", "Odd", "Even", "Mark", "Space"
            };
            PARComboBox.ItemsSource = ParRates;
            PARComboBox.SelectedItem = DefaultPart;

            List<string> StopRates = new List<string>()
            {
                "None", "One", "OnePointFive", "Two"
            };
            STOPComboBox.ItemsSource = StopRates;
            STOPComboBox.SelectedItem = DefaultSTOP;
            
            for (int j = 5; j < 10; ++j)
            {
                DATAComboBox.Items.Add(j);
            }
            DATAComboBox.SelectedItem = DefaultDATA;

            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = COMButton.Background as SolidColorBrush;
            //var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            //var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            //var theme = Application.Current.RequestedTheme;

            /*
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
            */

            

            ToggleButtonIsChecked(rx, RXHEXButton);
            ToggleButtonIsChecked(tx, TXHEXButton);

            FsButtonIsChecked(dtr, DTRButton);
            if (dtr == 1)
            {
                CommonRes._serialPort.DtrEnable = true;
            }
            else
            {
                CommonRes._serialPort.DtrEnable = false;
            }

            FsButtonIsChecked(rts, RTSButton);
            if (rts == 1)
            {
                CommonRes._serialPort.RtsEnable = true;
            }
            else
            {
                CommonRes._serialPort.RtsEnable = false;
            }

            FsButtonIsChecked(shtime, ShowTimeButton);
            FsButtonIsChecked(autotr, AUTOScrollButton);

            RunProgressBar.Visibility = Visibility.Collapsed;

            //BorderBackRX.Background = backgroundColor;

            ToggleButtonIsChecked(autosaveset, SaveSetButton);

        }

        private void ToggleButtonIsChecked(int isChecked, ToggleButton toggleButton)
        {
            if (isChecked == 1)
            {
                toggleButton.IsChecked = true;
            }
            else
            {
                toggleButton.IsChecked = false;
            }
        }

        private void FsButtonIsChecked(int isChecked, Button button)
        {
            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = COMButton.Background as SolidColorBrush;
            var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            var theme = Application.Current.RequestedTheme;

            if (isChecked == 1)
            {
                if (theme == ApplicationTheme.Dark)
                {
                    // ��ǰ������ɫģʽ
                    button.Background = new SolidColorBrush(darkaccentColor);
                    button.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (theme == ApplicationTheme.Light)
                {
                    // ��ǰ����ǳɫģʽ
                    button.Background = new SolidColorBrush(ligtaccentColor);
                    button.Foreground = new SolidColorBrush(Colors.White);
                }
            }
            else
            {
                button.Background = backgroundColor;
                button.Foreground = foregroundColor;
            }
        }

        private void FsBorderIsChecked(int isChecked, Border border, TextBlock textBlock)
        {
            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = (Windows.UI.Color)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
            var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            var theme = Application.Current.RequestedTheme;

            if (isChecked == 1)
            {
                if (theme == ApplicationTheme.Dark)
                {
                    // ��ǰ������ɫģʽ
                    border.Background = new SolidColorBrush(darkaccentColor);
                    textBlock.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (theme == ApplicationTheme.Light)
                {
                    // ��ǰ����ǳɫģʽ
                    border.Background = new SolidColorBrush(ligtaccentColor);
                    textBlock.Foreground = new SolidColorBrush(Colors.White);
                }
            }
            else
            {
                border.Background = new SolidColorBrush(backgroundColor);
                textBlock.Foreground = foregroundColor;
            }
        }

        private void Page1_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                COMButton_Click(this, new RoutedEventArgs());
                _isLoaded = true;

            }


            /*
            // ����һ��DispatcherQueueTimer����
            DispatcherQueueTimer timer = DispatcherQueue.GetForCurrentThread().CreateTimer();

            // ����Ĵ����г�ʼ�����DispatcherQueueTimer
            timer.Interval = TimeSpan.FromMilliseconds(500); // ע������ļ��ʱ����250���룬Ҳ����ÿ�봥���Ĵ�
            timer.Tick += (sender, args) =>
            {
                // �����������İ�ť����¼�
                AUTOScrollButton_ClickAsync(null, null);
            };
            timer.Start();
            */

            ToggleButtonIsChecked(autosercom, AutoComButton);
            if (autosercom == 1) 
            { 
                timerSerialPort = new Timer(TimerSerialPortTick, null, 0, 1500);
                AutoSerchComProgressRing.IsActive = true;
            }
            else AutoSerchComProgressRing.IsActive = false;

            ToggleButtonIsChecked(autoconnect, AutoConnectButton);
            //ToggleButtonIsChecked();

        }

        public void TimerTick(Object stateInfo)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                RXDATA_ClickAsync(null, null);

                if (RunT == 0) RunPBT += 2;
                    
                else RunPBT -= 2;

                RunTProgressBar.Value = RunPBT;
                if (RunPBT == 100)
                {
                    RunT = 1;
                }
                else if (RunPBT == 0)
                {
                    RunT = 0;
                }

                if(CommonRes._serialPort.DsrHolding == true)
                {
                    FsBorderIsChecked(1, DSRBorder, DSRTextBlock);
                }
                if (CommonRes._serialPort.CtsHolding == true)
                {
                    FsBorderIsChecked(1, CTSBorder, CTSTextBlock);
                }
                if (CommonRes._serialPort.CDHolding == true)
                {
                    FsBorderIsChecked(1, CDHBorder, CDHTextBlock);
                }

            });
            
        }
        public void TimerSerialPortTick(Object stateInfo)       //�����Ȳ�μ��
        {
            int InOut = 0;
            int i = 0;
            int j = 0;
            string[] LastPort = ArryPort;
            string[] NowPort = SerialPort.GetPortNames();
            string InOutCom;
            string commne = "";

            if (LastPort == null)
            {
                LastPort = SerialPort.GetPortNames();
            }
            if (Enumerable.SequenceEqual(LastPort, NowPort) == false || ArryPort == null)
            {
                /*
                for (int k = 0; k < LastPort.Length; k++)
                {
                    Debug.WriteLine(LastPort[k]);
                }
                
                for (int k = 0; k < NowPort.Length; k++)
                {
                    Debug.WriteLine(NowPort[k]);
                }
                */

                if (LastPort.Length < NowPort.Length) 
                {
                    InOut = 1;
                    for (j = 0; j < NowPort.Length; j++)         //����������豸
                    {
                        Debug.WriteLine("SER J " + j);
                        for (i = 0; i < LastPort.Length; i++)
                        {
                            if (NowPort[j] == LastPort[i])
                            {
                                Debug.WriteLine("SER I " + i);
                                break;
                            }
                        }
                        Debug.WriteLine("Now" + i);
                    }
                    Debug.WriteLine("=" + i);
                }

                else if (LastPort.Length > NowPort.Length)
                {
                    InOut = 0;
                    for (i = 0; i < LastPort.Length; i++)       //�����γ����豸
                    {
                        for (j = 0; j < NowPort.Length; j++)
                        {
                            if (LastPort[i] == NowPort[j])
                            {
                                break;
                            }
                        }
                    }
                    Debug.WriteLine("Last" + j);
                }
                Debug.WriteLine("INOUT" + InOut);

                
                if (InOut == 1)
                {
                    InOutCom = NowPort[i];
                }
                else
                {
                    InOutCom = LastPort[j];
                }
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (COMComboBox.SelectedItem != null)
                    {
                        commne = (string)COMComboBox.SelectedItem;
                    }
                    if (InOut != 0)
                    {
                        string commme = (string)COMComboBox.SelectedItem;
                        RXTextBox.Text = RXTextBox.Text + InOutCom + " is plug in\r\n";
                        COMComboBox.Items.Clear();
                        COMListview.Items.Clear();
                        //COMListview.ItemsSource = null;
                        //COMListview.ItemsSource = new ObservableCollection<ComDataItem>();
                        ArryPort = SerialPort.GetPortNames();
                        for (int k = 0; k < NowPort.Length; k++)
                        {
                            COMComboBox.Items.Add(ArryPort[k]);                           //�����еĿ��ô��ں���ӵ��˿ڶ�Ӧ����Ͽ���
                            COMListview.Items.Add(ArryPort[k]);
                        }
                        COMComboBox.SelectedItem = commme;
                        COMListview.SelectedItem = commne;
                        if (Con == 0)
                        {
                            if (COMComboBox.SelectedItem == null)
                            {
                                COMComboBox.SelectedItem = InOutCom;
                                COMListview.SelectedItem = InOutCom;
                                if (AutoConnectButton.IsChecked == true)
                                {
                                    CONTButton_Click(null, null);
                                }
                            }
                        }
                    }
                    else
                    {
                        RXTextBox.Text = RXTextBox.Text + InOutCom + " is pull out\r\n";
                        if (Con == 1)                                                   //�Զ��Ͽ��Ѱγ����豸��������
                        {
                            if (InOutCom == (string)COMComboBox.SelectedItem)
                            {
                                CONTButton_Click(null, null);
                            }
                        }

                        COMComboBox.Items.Clear();
                        COMListview.Items.Clear();
                        //COMListview.ItemsSource = null;
                        //COMListview.ItemsSource = new ObservableCollection<ComDataItem>();
                        ArryPort = SerialPort.GetPortNames();
                        for (int k = 0; k < NowPort.Length; k++)
                        {
                            COMComboBox.Items.Add(ArryPort[k]);                           //�����еĿ��ô��ں���ӵ��˿ڶ�Ӧ����Ͽ���
                            COMListview.Items.Add(ArryPort[k]);
                        }
                        COMComboBox.SelectedItem = commne;
                        COMListview.SelectedItem = commne;
                    }
                });
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
                string commme = (string)COMComboBox.SelectedItem;           //���䴮����
                ArryPort = SerialPort.GetPortNames();                       //SerialPort.GetPortNames()��������Ϊ��ȡ��������п��ô��ڣ����ַ���������ʽ���
                string scom = String.Join("\r\n", ArryPort);
                RXTextBox.Text = RXTextBox.Text + scom + "\r\n";
                MyBox.Items.Clear();                                        //�����ǰ��Ͽ������˵�����
                COMListview.Items.Clear();
                //COMListview.ItemsSource = null;
                //COMListview.ItemsSource = new ObservableCollection<ComDataItem>();
                for (int i = 0; i < ArryPort.Length; i++)
                {
                    MyBox.Items.Add(ArryPort[i]);                           //�����еĿ��ô��ں���ӵ��˿ڶ�Ӧ����Ͽ���
                    COMListview.Items.Add(ArryPort[i]);
                }
                //MyBox.Items.Add("COM0");
                RXTextBox.Text = RXTextBox.Text + "Search SerialPort succeed!\r\n";
                COMComboBox.SelectedItem = commme;
                COMListview.SelectedItem = commme;
            }
            //COMComboBox.SelectedItem = "COM0";
            /*
            if (Rollta == 360)
            {
                Rollta = 0;
            }
            else
            {
                Rollta = Rollta + 90;
            }
            COMButtonIcon.Rotation = Rollta;
            */
            Thread COMButtonIconRotation  = new Thread(COMButtonIcon_Rotation);
            COMButtonIconRotation.Start();
        }

        private void COMButtonIcon_Rotation(object name)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                COMButtonIcon.Rotation = -60;
                COMButtonIconScalar.Duration = TimeSpan.FromMilliseconds(250);
            });
            Thread.Sleep(300);
            DispatcherQueue.TryEnqueue(() =>
            {
                COMButtonIcon.Rotation = 420;
            });
            Thread.Sleep(250);
            DispatcherQueue.TryEnqueue(() =>
            {
                COMButtonIconScalar.Duration = TimeSpan.FromMilliseconds(0);
                COMButtonIcon.Rotation = 60;
                COMButtonIconScalar.Duration = TimeSpan.FromMilliseconds(250);
            });
            Thread.Sleep(60);
            DispatcherQueue.TryEnqueue(() =>
            {
                COMButtonIcon.Rotation = 0;
            });


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
                    
                    timer = new Timer(TimerTick, null, 0, 125); // ÿ�봥��8��
                    
                    CONTButton.Content = "DISCONNECT";
                    Con = 1;
                    RunProgressBar.ShowPaused = false;
                    RunProgressBar.IsIndeterminate = true;
                    RunProgressBar.Visibility = Visibility.Visible;
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
                    RunProgressBar.IsIndeterminate = true;
                    RunProgressBar.ShowPaused = true;
                    RunProgressBar.Visibility = Visibility.Visible;
                }

            }
            else
            {
                //RXTextBox.Text = RXTextBox.Text + "SerialPort IS DISCONNECT\r\n";

                try
                {
                    CommonRes._serialPort.Close();                                                                              //�رմ���
                    RXTextBox.Text = RXTextBox.Text + "\n" + "SerialPort IS CLOSE" + "\r\n";
                }
                catch (Exception err)                                                                       //һ������¹رմ��ڲ���������Բ���Ҫ�Ӵ������
                {
                    RXTextBox.Text = RXTextBox.Text + err + "\r\n";
                }
                CONTButton.Content = "CONNECT";
                Con = 0;
                CONTButton.Background = backgroundColor;
                CONTButton.Foreground = foregroundColor;
                RunProgressBar.IsIndeterminate = false;
                RunProgressBar.ShowPaused = false;
                RunProgressBar.Visibility = Visibility.Collapsed;
                timer.Dispose();
            }
        }


        /*
        
        */
        

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            /*
            if (txf == 1)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    RXTextBox.Text += "\r\n";
                });
                
                txf = 0;
            }
            */


            string rxstr;
            string rxntstr;
            string Timesr = current_time.ToString("yyyy-MM-dd HH:mm:ss:ff   "); //��ʾʱ��
            //StringBuilder datawate = new StringBuilder(1024);



            if (rx == 0)                                                        // ������ַ�����ʽ��ȡ
            {
                
                rxstr = CommonRes._serialPort.ReadExisting();                   // ��ȡ���ڽ��ջ������ַ���
                rxntstr = rxstr;
                if (shtime == 1)
                {
                    //rxstr = string.Concat(Timesr, rxstr);                     //��ʾʱ��
                }
                //datawate.Append(rxstr);

                
                DispatcherQueue.TryEnqueue(() =>
                {
                    datapwate.Append(rxstr);                                    // �ڽ����ı����н�����ʾ
                    UpdateItemsRepeater(Timesr, rxntstr);                       // �ڽ���listview�н�����ʾ
                });

                if (autotr == 1)
                {
                    //RXTextBox.ScrollToEnd();
                }
                
                
                

            }
            else                                                            // ����ֵ��ʽ��ȡ
            {
                int length = CommonRes._serialPort.BytesToRead;                       // ��ȡ���ڽ��ջ������ֽ���

                byte[] data = new byte[length];                             // ������ͬ�ֽڵ�����

                CommonRes._serialPort.Read(data, 0, length);                          // ���ڶ�ȡ���������ݵ�������

                for (int i = 0; i < length; i++)
                {
                    rxstr = Convert.ToString(data[i], 16).ToUpper();                                   // ������ת��Ϊ�ַ�����ʽ

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        //datapwate.Append("0x" + (rxhstr.Length == 1 ? "0" + rxhstr + " " : rxhstr + " "));        // ��ӵ����ڽ����ı�����
                    });
                }
                DispatcherQueue.TryEnqueue(() =>
                {
                    datapwate.Append("\r\n");
                });

                if (autotr == 1)
                {
                    //RXTextBox.ScrollToEnd();
                }

                
            }

            /*
            ++rxs;                                          //�����Զ����(������)
            if (rxs == 200)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    RXTextBox.Text = "";
                });
                rxs = 0;
            }
            */
            
        }

        private void UpdateItemsRepeater(string Timesr, string Rxstr)
        {
            // �������ItemsRepeater��������RXListView
            DataItem item = new DataItem { Timesr = Timesr, Rxstr = Rxstr };
            (RXListView.ItemsSource as ObservableCollection<DataItem>).Add(item);

            if (autotr == 1)
            {
                RXListView.ScrollIntoView(item);
            }
        }

        /*
        private void ComItemsRepeater(string Com)
        {
            ComDataItem citem = new ComDataItem { ComName = Com };
            (COMListview.ItemsSource as ObservableCollection<ComDataItem>).Add(citem);
        }
        */

        private void COMComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string ComIs;
            ComIs = (string)COMComboBox.SelectedItem;
            COMListview.SelectedItem = ComIs;
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
            RXListView.ItemsSource = null;
            RXTextBox.Text = "";    //����ı�������
            RXListView.ItemsSource = new ObservableCollection<DataItem>();
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
            if (rx == 0)
            {
                rx = 1;
            }
            else
            {
                rx = 0;
            }
        }

        private void TXHEXButton_Click(object sender, RoutedEventArgs e)    //������ʮ����������ʾ
        {

            if (tx == 0)
            {
                tx = 1;
            }
            else
            {
                tx = 0;
            }
        }


        private void RSTButton_Click(object sender, RoutedEventArgs e)      //�Զ�����
        {
            CommonRes._serialPort.BaudRate = 74880;// BANDComboBox.SelectedItem = "74880";//ESP12F

            CommonRes._serialPort.RtsEnable = true;
            Thread.Sleep(10);
            CommonRes._serialPort.DtrEnable = true;
            Thread.Sleep(10);
            CommonRes._serialPort.DtrEnable = false;
            Thread.Sleep(10);
            CommonRes._serialPort.RtsEnable = false;

            RSTButton_ClickAsync(null, null);
        }

        private Task RSTButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(200);
            if (dtr == 1)
            {
                DTRButton_Click(sender, e);
                Thread.Sleep(1);
                DTRButton_Click(sender, e);
            }
            else
            {
                Thread.Sleep(1);
                DTRButton_Click(sender, e);
            }

            //CommonRes._serialPort.DtrEnable = true;
            CommonRes._serialPort.BaudRate = Convert.ToInt32(BANDComboBox.SelectedItem);
            return Task.CompletedTask;
        }

        private void FsButtonChecked(int isChecked, Button button)
        {
            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = COMButton.Background as SolidColorBrush;
            var darkaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"];
            var ligtaccentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark1"];
            var theme = Application.Current.RequestedTheme;
            if (isChecked == 0)
            {
                if (theme == ApplicationTheme.Dark)
                {
                    // ��ǰ������ɫģʽ
                    button.Background = new SolidColorBrush(darkaccentColor);
                    button.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (theme == ApplicationTheme.Light)
                {
                    // ��ǰ����ǳɫģʽ
                    button.Background = new SolidColorBrush(ligtaccentColor);
                    button.Foreground = new SolidColorBrush(Colors.White);
                }
            }
            else
            {
                button.Background = backgroundColor;
                button.Foreground = foregroundColor;
            }
        }

        private void DTRButton_Click(object sender, RoutedEventArgs e)      //DTR�ź�ʹ��
        {
            FsButtonChecked(dtr, DTRButton);
            
            if (dtr == 0)
            {
                CommonRes._serialPort.DtrEnable = true;
                dtr = 1;
            }
            else
            {
                CommonRes._serialPort.DtrEnable = false;
                dtr = 0;
            }
        }

        private void RTSButton_Click(object sender, RoutedEventArgs e)      //RTS�ź�ʹ��
        {
            FsButtonChecked(rts, RTSButton);

            if (rts == 0)
            {
                CommonRes._serialPort.RtsEnable = true;
                rts = 1;
            }
            else
            {
                CommonRes._serialPort.RtsEnable = false;
                rts = 0;
            }
        }

        private void ShowTimeButton_Click(object sender, RoutedEventArgs e)
        {
            FsButtonChecked(shtime, ShowTimeButton);
            
            if (shtime == 0)
            {
                shtime = 1;

                //��ʾʱ��
                //current_time = System.DateTime.Now;     //��ȡ��ǰʱ��
                /*
                DispatcherQueue.TryEnqueue(() =>
                {
                    RXTextBox.Text = RXTextBox.Text + current_time.ToString("HH:mm:ss") + "  ";                          // �ڽ����ı����н�����ʾ
                });
                */
            }
            else
            {
                shtime = 0;
            }
        }

        private void AUTOScrollButton_Click(object sender, RoutedEventArgs e)
        {
            FsButtonChecked(autotr, AUTOScrollButton);
            
            if (autotr == 0)
            {
                autotr = 1;
            }
            else
            {
                autotr = 0;
            }
        }

        private void RXDataButton_Click(object sender, RoutedEventArgs e)
        {

        }
        /*
        private async void RXDataButton_Click(object sender, RoutedEventArgs e)
        {
            await AUTOScrollButton_ClickAsync(sender, e);
        }
        */

        private Task RXDATA_ClickAsync(object sender, RoutedEventArgs e)
        {
            // �������������첽����
            // ���磺await SomeAsyncMethod();
            current_time = System.DateTime.Now;     //��ȡ��ǰʱ��
            //RXTextBox.Text = RXTextBox.Text + current_time.ToString("HH:mm:ss") + "  ";
            //Timesr = current_time.ToString("HH:mm:ss");



            //rxpstr = System.Text.Encoding.UTF8.GetString(datapwate);
            rxpstr = datapwate.ToString();                          //����������ֵ�����
            RXTextBox.Text = RXTextBox.Text + rxpstr + "";          //������յ�����
            datapwate.Clear();                                      //��ջ�����

            return Task.CompletedTask;
        }

        private void TXTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                TXButton_Click(this, new RoutedEventArgs());
            }
        }

        private void SaveSetButton_Checked(object sender, RoutedEventArgs e)
        {
            SaveSetButton.IsChecked = true;
        }


        private void COMListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string ComIs;
            ComIs = (string)COMListview.SelectedItem;
            COMComboBox.SelectedItem = ComIs;
        }

        private void AutoConnectButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void AutoComButton_Click(object sender, RoutedEventArgs e)
        {
            if (autosercom == 0)
            {
                timerSerialPort = new Timer(TimerSerialPortTick, null, 0, 1500);
                AutoSerchComProgressRing.IsActive = true;
                autosercom = 1;
            }
            else
            {
                timerSerialPort.Dispose();
                AutoSerchComProgressRing.IsActive = false;
                autosercom = 0;
            }
        }
    }
}
