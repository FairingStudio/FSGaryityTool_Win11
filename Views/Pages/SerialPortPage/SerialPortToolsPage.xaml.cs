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
using static System.Net.Mime.MediaTypeNames;
using Tommy;
using static FSGaryityTool_Win11.Page1;
using static FSGaryityTool_Win11.Views.Pages.SerialPortPage.MainPage1;
using System.IO.Ports;
using System.Management;
using Microsoft.UI;
using Application = Microsoft.UI.Xaml.Application;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using FSGaryityTool_Win11.McuToolpage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FSGaryityTool_Win11.Views.Pages.SerialPortPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SerialPortToolsPage : Page
    {
        public static int getPortInfo = 0;
        public static int portIsConnect = 0;//Con
        public static string conCom = "";
        public static int txHex = 0; //TXHEX
        public static int rxHex = 0; //RXHEX
        public static int dtr = 0;//FTR
        public static int rts = 0;//RTS
        public static int shtime = 0;//ShowTime
        public static int autotr = 0;//AUTOScroll
        public static int autosaveset;
        public static int autosercom;
        public static int autoconnect;
        public static int txnewline = 0;

        public static string[] ArryPort; //�����ַ������飬������Ϊ ArryPort

        public static int baudrate = 0;

        public System.Threading.Timer timer;
        public System.Threading.Timer timerSerialPort;

        private bool _isLoaded;



        public static SerialPortToolsPage serialPortToolsPage;
        public SerialPortToolsPage()
        {
            this.InitializeComponent();
            serialPortToolsPage = this;
            this.Loaded += SerialPortToolsPage_Loaded;
        }
        private void FsBorderIsChecked(int isChecked, Border border, TextBlock textBlock)
        {
            var foregroundColor = COMButton.Foreground as SolidColorBrush;
            var backgroundColor = (SolidColorBrush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
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
                border.Background = backgroundColor;
                textBlock.Foreground = foregroundColor;
            }
        }
        private T TomlGetValueOrDefault<T>(TomlTable table, string menu, string name, T defaultValue)
        {
            if (table[menu][name] != "Tommy.TomlLazy")
            {
                var value = table[menu][name].AsString.Value;
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else
            {
                return defaultValue;
            }
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
        private void SerialPortToolsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                _isLoaded = true;

                string DefaultBAUD;
                string DefaultPart;
                string DefaultSTOP;
                int DefaultDATA;

                using (StreamReader reader = File.OpenText(Page1.FSSetToml))
                {
                    TomlTable SPsettingstomlr = TOML.Parse(reader);             //��ȡTOML
                                                                                //Debug.WriteLine("Print:" + SPsettingstomlr["FSGravitySettings"]["DefaultNvPage"]);
                                                                                //NvPage = int.Parse(settingstomlr["FSGravitySettings"]["DefaultNvPage"]);
                    string spSettings = "SerialPortSettings";
                    //��������Ƿ�ΪNULL
                    DefaultBAUD = TomlGetValueOrDefault(SPsettingstomlr, spSettings, "DefaultBAUD", "115200");
                    DefaultPart = TomlGetValueOrDefault(SPsettingstomlr, spSettings, "DefaultParity", "None");
                    DefaultSTOP = TomlGetValueOrDefault(SPsettingstomlr, spSettings, "DefaultSTOP", "One");
                    DefaultDATA = int.Parse(TomlGetValueOrDefault(SPsettingstomlr, spSettings, "DefaultDATA", "8"));

                    txHex = int.Parse(TomlGetValueOrDefault(SPsettingstomlr, spSettings, "DefaultTXHEX", "0"));
                    rxHex = int.Parse(TomlGetValueOrDefault(SPsettingstomlr, spSettings, "DefaultRXHEX", "0"));
                    dtr = int.Parse(TomlGetValueOrDefault(SPsettingstomlr, spSettings, "DefaultDTR", "1"));
                    rts = int.Parse(TomlGetValueOrDefault(SPsettingstomlr, spSettings, "DefaultRTS", "0"));
                    shtime = int.Parse(TomlGetValueOrDefault(SPsettingstomlr, spSettings, "DefaultSTime", "0"));
                    autotr = int.Parse(TomlGetValueOrDefault(SPsettingstomlr, spSettings, "DefaultAUTOSco", "1"));
                    autosaveset = int.Parse(TomlGetValueOrDefault(SPsettingstomlr, spSettings, "AutoDaveSet", "1"));
                    autosercom = int.Parse(TomlGetValueOrDefault(SPsettingstomlr, spSettings, "AutoSerichCom", "1"));
                    autoconnect = int.Parse(TomlGetValueOrDefault(SPsettingstomlr, spSettings, "AutoConnect", "1"));
                    txnewline = int.Parse(TomlGetValueOrDefault(SPsettingstomlr, spSettings, "DefaultTXNewLine", "0"));

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
                LaunageSetting();
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
                DATANumberBox.Value = DefaultDATA;

                COMButton_Click(this, new RoutedEventArgs());

                ToggleButtonIsChecked(rx, RXHEXButton);
                ToggleButtonIsChecked(tx, TXHEXButton);

                ToggleButtonIsChecked(dtr, DTRButton);
                if (dtr == 1)
                {
                    CommonRes._serialPort.DtrEnable = true;
                }
                else
                {
                    CommonRes._serialPort.DtrEnable = false;
                }

                ToggleButtonIsChecked(rts, RTSButton);
                if (rts == 1)
                {
                    CommonRes._serialPort.RtsEnable = true;
                }
                else
                {
                    CommonRes._serialPort.RtsEnable = false;
                }

                ToggleButtonIsChecked(shtime, ShowTimeButton);
                ToggleButtonIsChecked(autotr, AUTOScrollButton);

                //BorderBackRX.Background = backgroundColor;

                ToggleButtonIsChecked(autosaveset, SaveSetButton);
                ToggleButtonIsChecked(autoconnect, AutoConnectButton);
                ToggleButtonIsChecked(txnewline, TXNewLineButton);

                ToggleButtonIsChecked(autosercom, AutoComButton);
                if (autosercom == 1)
                {
                    timerSerialPort = new System.Threading.Timer(TimerSerialPortTick, null, 0, 1500);
                    AutoSerchComProgressRing.IsActive = true;
                }
                else AutoSerchComProgressRing.IsActive = false;

                ToggleButtonIsChecked(autoconnect, AutoConnectButton);

            }

        }
        public void LaunageSetting()
        {
            BaudTextBlock.Text = LanguageText("baudRatel");
            PartTextBlock.Text = LanguageText("parityl");
            StopTextBlock.Text = LanguageText("stopBits");
            DataTextBlock.Text = LanguageText("dataBits");
            RXHEXButton.Content = LanguageText("rxHexl");
            TXHEXButton.Content = LanguageText("txHexl");
            TXNewLineButton.Content = LanguageText("txNewLinel");
            SaveSetButton.Content = LanguageText("autoSaveSetl");
            AUTOScrollButton.Content = LanguageText("autoScrolll");
            AutoComButton.Content = LanguageText("autoSerichComl");
            AutoConnectButton.Content = LanguageText("autoConnectl");
            
            //COMRstInfoBar.Message = LanguageText("comRstInfoBar");

            List<MCUTool> mcuTools = new List<MCUTool>()
                {
                    new MCUTool() { Name = "None", Description = LanguageText("mcuToolNone") },
                    new MCUTool() { Name = "ESP8266", Description = LanguageText("mcuToolEsp8266") },
                    new MCUTool() { Name = "RP2040        M", Description = LanguageText("mcuToolRP2040MPY") }
                };

            ChipToolKitComboBox.ItemsSource = mcuTools;
            ChipToolKitComboBox.SelectedItem = mcuTools[1];
        }
        public void TimerTick(Object stateInfo)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                //RXDATA_ClickAsync(null, null);
                page1.current_time = System.DateTime.Now;     //��ȡ��ǰʱ��

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


                if (CommonRes._serialPort.IsOpen == true)
                {
                    if (CommonRes._serialPort.DsrHolding == true)
                    {
                        FsBorderIsChecked(1, DSRBorder, DSRTextBlock);
                    }
                    else
                    {
                        FsBorderIsChecked(0, DSRBorder, DSRTextBlock);
                    }
                    if (CommonRes._serialPort.CtsHolding == true)
                    {
                        FsBorderIsChecked(1, CTSBorder, CTSTextBlock);
                    }
                    else
                    {
                        FsBorderIsChecked(0, CTSBorder, CTSTextBlock);
                    }
                    if (CommonRes._serialPort.CDHolding == true)
                    {
                        FsBorderIsChecked(1, CDHBorder, CDHTextBlock);
                    }
                    else
                    {
                        FsBorderIsChecked(0, CDHBorder, CDHTextBlock);
                    }
                }

            });

        }
        
        private void PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            if (e.EventType == System.IO.Ports.SerialPinChange.Ring)
            {
                int RI = 0;
                if (RI == 0)
                {
                    // RI �ź�ʹ��
                    RI = 1;
                    FsBorderIsChecked(RI, RIBorder, RITextBlock);

                }
                else
                {
                    // RI �ź�δʹ��
                    RI = 0;
                    FsBorderIsChecked(RI, RIBorder, RITextBlock);

                }
            }
        }

        public class SerialPortInfo
        {
            public string Name { get; protected set; }
            public string Description { get; protected set; }
            public string Manufacturer { get; protected set; }

            private static Dictionary<string, SerialPortInfo> portInfoDictionary = new Dictionary<string, SerialPortInfo>();

            static SerialPortInfo()
            {
                // ��Ӧ�ó�������ʱ��ȡ���д��ڵ��豸����
                RefreshPortInfo();
                getPortInfo = 1;
            }

            public static void RefreshPortInfo(string portName = null)
            {
                string queryString = "SELECT * FROM Win32_PnPEntity";
                if (portName != null)
                {
                    queryString += $" WHERE Name LIKE '%{portName}%'";
                }

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(queryString))
                {
                    var hardInfos = searcher.Get();
                    foreach (var hardInfo in hardInfos)
                    {
                        if ((hardInfo.Properties["Name"].Value != null) &&
                            (hardInfo.Properties["Name"].Value.ToString().Contains("COM")))
                        {
                            SerialPortInfo temp = new SerialPortInfo();
                            string s = hardInfo.Properties["Name"].Value.ToString();
                            int p = s.IndexOf('(');
                            temp.Description = s.Substring(0, p);
                            temp.Name = s.Substring(p + 1, s.Length - p - 2);
                            temp.Manufacturer = hardInfo.Properties["Manufacturer"].Value.ToString();
                            portInfoDictionary[temp.Name] = temp;
                        }
                    }
                }
            }

            public static SerialPortInfo GetPort(string portName)
            {
                if (!portInfoDictionary.ContainsKey(portName))
                {
                    // ����ֵ���û��ָ���Ĵ������ƣ�ˢ�´�����Ϣ
                    RefreshPortInfo(portName);
                }

                if (portInfoDictionary.ContainsKey(portName))
                {
                    return portInfoDictionary[portName];
                }

                return null;  // ���û���ҵ�ƥ��Ĵ��ڣ�����null
            }
        }

        public void TimerSerialPortTick(Object stateInfo)       //�����Ȳ�μ��
        {
            if (getPortInfo == 0) return;
            string[] NowPort = SerialPort.GetPortNames(); // ��ȡ��ǰ���п��õĴ�������
            NowPort = new HashSet<string>(NowPort).ToArray(); // �Ƴ����ܵ��ظ���

            string[] LastPort = ArryPort ?? NowPort; // ��ȡ��һ�μ�⵽�Ĵ������ƣ����û����ʹ�õ�ǰ��������
            ArryPort = NowPort; // ������һ�μ�⵽�Ĵ�������

            var lastPortSet = new HashSet<string>(LastPort); // ����һ��������һ�δ������Ƶ�HashSet
            var nowPortSet = new HashSet<string>(NowPort); // ����һ��������ǰ�������Ƶ�HashSet

            var insertedPorts = nowPortSet.Except(lastPortSet).ToArray(); // �ҳ��²���Ĵ���
            var removedPorts = lastPortSet.Except(nowPortSet).ToArray(); // �ҳ����γ��Ĵ���

            if (insertedPorts.Length > 0 || removedPorts.Length > 0) // ������²���Ĵ��ڻ����д��ڱ��γ�
            {
                DispatcherQueue.TryEnqueue(() => // ��UI�߳���ִ�����²���
                {
                    string selectedPort = (string)COMComboBox.SelectedItem; // ��ȡ��ǰѡ�еĴ���

                    foreach (var port in insertedPorts) // ���������²���Ĵ���
                    {
                        SerialPortInfo info = SerialPortInfo.GetPort(port); // ��ȡ���ڵ���Ϣ
                        page1.RXTextBox.Text += $"{port}{LanguageText("spPlogin")}\r\n"; // �����ı��������

                    }

                    foreach (var port in removedPorts) // �������б��γ��Ĵ���
                    {
                        page1.RXTextBox.Text += $"{port}{LanguageText("spPullout")}\r\n"; // �����ı��������
                        if (portIsConnect == 1 && port == selectedPort) // �����ǰ���ӵĴ��ڱ��γ�����Ͽ�����
                        {
                            mainPage1.SerialPortConnectToggleButton_Click(null, null);
                        }
                    }

                    COMComboBox.Items.Clear(); // �����Ͽ������
                    COMListview.Items.Clear(); // ����б���ͼ������

                    foreach (var port in NowPort) // ������ǰ���п��õĴ���
                    {
                        COMComboBox.Items.Add(port); // ������������ӵ���Ͽ���
                        COMListview.Items.Add(port); // ������������ӵ��б���ͼ��
                    }

                    COMComboBox.SelectedItem = selectedPort; // ��֮ǰѡ�еĴ�������ѡ��
                    COMListview.SelectedItem = selectedPort; // ��֮ǰѡ�еĴ�������ѡ��

                    if (portIsConnect == 0 && COMComboBox.SelectedItem == null && insertedPorts.Length > 0) // ���û��ѡ�еĴ��ڣ��������²���Ĵ���
                    {
                        COMComboBox.SelectedItem = insertedPorts[0]; // ѡ���²���Ĵ���
                        COMListview.SelectedItem = insertedPorts[0]; // ѡ���²���Ĵ���
                        if (AutoConnectButton.IsChecked == true) // ����������Զ����ӣ����������²���Ĵ���
                        {
                            mainPage1.SerialPortConnectToggleButton_Click(null, null);
                        }
                    }
                });
            }
        }
        private async void COMButton_Click(object sender, RoutedEventArgs e)
        {
            //COMButton.Content = "Clicked";
            await SearchAndAddSerialToComboBoxAsync(CommonRes._serialPort, COMComboBox);           //ɨ�貢����������������б�

            async Task SearchAndAddSerialToComboBoxAsync(SerialPort MyPort, ComboBox MyBox)
            {
                page1.RXTextBox.Text = page1.RXTextBox.Text + LanguageText("startSerichSP") + "\r\n";
                string commme = (string)COMComboBox.SelectedItem;           //���䴮����
                ArryPort = SerialPort.GetPortNames();                       //SerialPort.GetPortNames()��������Ϊ��ȡ��������п��ô��ڣ����ַ���������ʽ���
                ArryPort = new HashSet<string>(ArryPort).ToArray(); // �Ƴ����ܵ��ظ���
                string scom = String.Join("\r\n", ArryPort);
                //RXTextBox.Text = RXTextBox.Text + scom + "\r\n";
                MyBox.Items.Clear();                                        //�����ǰ��Ͽ������˵�����
                COMListview.Items.Clear();
                //COMListview.ItemsSource = null;
                //COMListview.ItemsSource = new ObservableCollection<ComDataItem>();

                for (int i = 0; i < ArryPort.Length; i++)
                {
                    MyBox.Items.Add(ArryPort[i]);                           //�����еĿ��ô��ں���ӵ��˿ڶ�Ӧ����Ͽ���
                    COMListview.Items.Add(ArryPort[i]/* + (portDescription != null ? " | " + portDescription : "")*/);
                    SerialPortInfo info = await Task.Run(() => SerialPortInfo.GetPort(ArryPort[i]));
                    if (info != null)
                    {
                        page1.RXTextBox.Text += ArryPort[i] + ": " + info.Description + "\r\n";
                    }
                    else
                    {
                        page1.RXTextBox.Text += ArryPort[i] + "\r\n";
                    }
                    //RXTextBox.Text += ArryPort[i] + "\r\n" + GetPortDescription(ArryPort[i]) + "\r\n";
                }
                //MyBox.Items.Add("COM0");
                page1.RXTextBox.Text = page1.RXTextBox.Text + LanguageText("overSerichSP") + "\r\n";
                COMComboBox.SelectedItem = commme;
                COMListview.SelectedItem = commme;
            }
            
            Thread COMButtonIconRotation = new Thread(COMButtonIcon_Rotation);
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


        public void SerialPortConnect()
        {
            conCom = (string)COMComboBox.SelectedItem;

            string portName = (string)COMComboBox.SelectedItem;
            int bandRate = Convert.ToInt32(BANDComboBox.SelectedItem);
            string parity = (string)PARComboBox.SelectedItem;
            string stopBits = (string)STOPComboBox.SelectedItem;
            int dataBits = Convert.ToInt32(DATAComboBox.SelectedItem);

            page1.SerialPortConnrct(portName,bandRate,parity,stopBits,dataBits,1500, Encoding.UTF8);

            page1.RXTextBox.Text = page1.RXTextBox.Text + "BaudRate = " + Convert.ToInt32(BANDComboBox.SelectedItem) + "\r\n";
            page1.RXTextBox.Text = page1.RXTextBox.Text + "Parity = " + (Parity)Enum.Parse(typeof(Parity), (string)PARComboBox.SelectedItem) + "\r\n";
            page1.RXTextBox.Text = page1.RXTextBox.Text + "StopBits = " + (StopBits)Enum.Parse(typeof(StopBits), (string)STOPComboBox.SelectedItem) + "\r\n";
            page1.RXTextBox.Text = page1.RXTextBox.Text + "DataBits = " + Convert.ToInt32(DATAComboBox.SelectedItem) + "\r\n";
            page1.RXTextBox.Text = page1.RXTextBox.Text + LanguageText("serialPortl") + " " + COMComboBox.SelectedItem + LanguageText("spConnect") + "\r\n";

            timer = new System.Threading.Timer(TimerTick, null, 0, 250); // ÿ�봥��8��

            portIsConnect = 1;

        }

        public void SerialPortConnectcatch()
        {
            page1.RXTextBox.Text = page1.RXTextBox.Text + LanguageText("openSPErr") + "\r\n";
            //MessageBox.Show("�򿪴���ʧ�ܣ������������", "����");

            portIsConnect = 0;
            //CONTButton.Content = "CONNECT";
        }
        public void SerialPortClose()
        {
            CommonRes._serialPort.Close();                                                                              //�رմ���
            page1.RXTextBox.Text = page1.RXTextBox.Text + "\n" + LanguageText("serialPortl") + " " + COMComboBox.SelectedItem + LanguageText("spClose") + "\r\n";
        }
        public void SerialPortDisconnect()
        {
            portIsConnect = 0;
            timer.Dispose();
        }

        private void COMComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string ComIs;
            ComIs = (string)COMComboBox.SelectedItem;
            COMListview.SelectedItem = ComIs;
            if (portIsConnect == 1)
            {
                if (conCom != ComIs)
                    page1.COMRstInfoBar.IsOpen = true;
                else
                    page1.COMRstInfoBar.IsOpen = false;
            }
        }
        private void ClearCOMCombobox_Click(object sender, RoutedEventArgs e)
        {
            COMComboBox.SelectedItem = null;
            COMListview.SelectedItem = null;
        }
        private void AutoComButton_Click(object sender, RoutedEventArgs e)
        {
            if (autosercom == 0)
            {
                timerSerialPort = new System.Threading.Timer(TimerSerialPortTick, null, 0, 1500);
                AutoSerchComProgressRing.IsActive = true;
                autosercom = 1;
            }
            else
            {
                timerSerialPort.Dispose();
                AutoSerchComProgressRing.IsActive = false;
                autosercom = 0;
            }
            if (autosaveset == 1)
            {
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);

                    settingstomlr["SerialPortSettings"]["AutoSerichCom"] = Convert.ToString(autosercom);
                }

                using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                {
                    settingstomlr.WriteTo(writer);
                    writer.Flush();
                }
            }
        }
        private void AutoConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (autoconnect == 0)
            {
                autoconnect = 1;
            }
            else
            {
                autoconnect = 0;
            }
            if (autosaveset == 1)
            {
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);

                    settingstomlr["SerialPortSettings"]["AutoConnect"] = Convert.ToString(autoconnect);
                }

                using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                {
                    settingstomlr.WriteTo(writer);
                    writer.Flush();
                }
            }
        }
        private void COMListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string ComIs;
            ComIs = (string)COMListview.SelectedItem;
            COMComboBox.SelectedItem = ComIs;

        }
        private void BANDComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ���������Ƿ�Ϊ����
            if (!int.TryParse((string)BANDComboBox.SelectedItem, out int baudRate) || baudRate == 0)
            {
                // �������Ĳ������֣�ʹ�������ļ��е����ָ�����
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);
                    BANDComboBox.SelectedItem = ((Tommy.TomlString)settingstomlr["SerialPortSettings"]["DefaultBAUD"]).Value;
                }
            }
            else
            {
                if (autosaveset == 1)
                {
                    using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                    {
                        settingstomlr = TOML.Parse(reader);

                        settingstomlr["SerialPortSettings"]["DefaultBAUD"] = (string)BANDComboBox.SelectedItem;
                    }

                    using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                    {
                        settingstomlr.WriteTo(writer);
                        writer.Flush();
                    }
                }
                if (portIsConnect == 1)
                {
                    CommonRes._serialPort.BaudRate = Convert.ToInt32(BANDComboBox.SelectedItem);
                    page1.RXTextBox.Text = page1.RXTextBox.Text + "BaudRate = " + Convert.ToInt32(BANDComboBox.SelectedItem) + "\r\n";
                }
                baudrate = Convert.ToInt32(BANDComboBox.SelectedItem);
            }
            if (Convert.ToInt32(BANDComboBox.SelectedItem) <= 7200)
            {
                BaudrateIcon.Glyph = "\uEC48";
            }
            else if (Convert.ToInt32(BANDComboBox.SelectedItem) > 7200 && Convert.ToInt32(BANDComboBox.SelectedItem) < 128000)
            {
                BaudrateIcon.Glyph = "\uEC49";
            }
            else
            {
                BaudrateIcon.Glyph = "\uEC4A";
            }
        }

        private void PARComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (autosaveset == 1)
            {
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);

                    settingstomlr["SerialPortSettings"]["DefaultParity"] = (string)PARComboBox.SelectedItem;
                }

                using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                {
                    settingstomlr.WriteTo(writer);
                    writer.Flush();
                }
            }
            if (portIsConnect == 1)
            {
                CommonRes._serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), (string)PARComboBox.SelectedItem);
                page1.RXTextBox.Text = page1.RXTextBox.Text + "Parity = " + (Parity)Enum.Parse(typeof(Parity), (string)PARComboBox.SelectedItem) + "\r\n";
            }
        }
        private void STOPComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (autosaveset == 1)
            {
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);

                    settingstomlr["SerialPortSettings"]["DefaultSTOP"] = (string)STOPComboBox.SelectedItem;
                }

                using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                {
                    settingstomlr.WriteTo(writer);
                    writer.Flush();
                }
            }
            if (portIsConnect == 1)
            {
                CommonRes._serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), (string)STOPComboBox.SelectedItem);
                page1.RXTextBox.Text = page1.RXTextBox.Text + "StopBits = " + (StopBits)Enum.Parse(typeof(StopBits), (string)STOPComboBox.SelectedItem) + "\r\n";
            }
        }
        private void DATAComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (autosaveset == 1)
            {
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);

                    settingstomlr["SerialPortSettings"]["DefaultDATA"] = Convert.ToString(DATAComboBox.SelectedItem);
                }

                using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                {
                    settingstomlr.WriteTo(writer);
                    writer.Flush();
                }
            }
            if (portIsConnect == 1)
            {
                CommonRes._serialPort.DataBits = Convert.ToInt32(DATAComboBox.SelectedItem);
                page1.RXTextBox.Text = page1.RXTextBox.Text + "DataBits = " + Convert.ToInt32(DATAComboBox.SelectedItem) + "\r\n";
            }
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
            if (autosaveset == 1)
            {
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);

                    settingstomlr["SerialPortSettings"]["DefaultRXHEX"] = Convert.ToString(rx);
                }

                using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                {
                    settingstomlr.WriteTo(writer);
                    writer.Flush();
                }
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
            if (autosaveset == 1)
            {
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);

                    settingstomlr["SerialPortSettings"]["DefaultTXHEX"] = Convert.ToString(tx);
                }

                using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                {
                    settingstomlr.WriteTo(writer);
                    writer.Flush();
                }
            }
        }
        private void DTRButton_Click(object sender, RoutedEventArgs e)      //DTR�ź�ʹ��
        {
            //FsButtonChecked(dtr, DTRButton);

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
            if (autosaveset == 1)
            {
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);

                    settingstomlr["SerialPortSettings"]["DefaultDTR"] = Convert.ToString(dtr);
                }

                using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                {
                    settingstomlr.WriteTo(writer);
                    writer.Flush();
                }
            }
        }
        private void RTSButton_Click(object sender, RoutedEventArgs e)      //RTS�ź�ʹ��
        {
            //FsButtonChecked(rts, RTSButton);

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
            if (autosaveset == 1)
            {
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);

                    settingstomlr["SerialPortSettings"]["DefaultRTS"] = Convert.ToString(rts);
                }

                using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                {
                    settingstomlr.WriteTo(writer);
                    writer.Flush();
                }
            }
        }
        private void TXNewLineButton_Click(object sender, RoutedEventArgs e)
        {
            if (txnewline == 0)
            {
                txnewline = 1;
            }
            else
            {
                txnewline = 0;
            }
            if (autosaveset == 1)
            {
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);

                    settingstomlr["SerialPortSettings"]["DefaultTXNewLine"] = Convert.ToString(txnewline);
                }

                using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                {
                    settingstomlr.WriteTo(writer);
                    writer.Flush();
                }
            }
        }
        private void SaveSetButton_Click(object sender, RoutedEventArgs e)
        {
            if (autosaveset == 0)
            {
                autosaveset = 1;
            }
            else
            {
                autosaveset = 0;
            }
            using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
            {
                settingstomlr = TOML.Parse(reader);

                settingstomlr["SerialPortSettings"]["AutoDaveSet"] = Convert.ToString(autosaveset);
            }

            using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
            {
                settingstomlr.WriteTo(writer);
                writer.Flush();
            }
        }
        private void AUTOScrollButton_Click(object sender, RoutedEventArgs e)
        {
            //FsButtonChecked(autotr, AUTOScrollButton);

            if (autotr == 0)
            {
                autotr = 1;
            }
            else
            {
                autotr = 0;
            }
            if (autosaveset == 1)
            {
                using (StreamReader reader = File.OpenText(FSSetToml))                    //��TOML�ļ�
                {
                    settingstomlr = TOML.Parse(reader);

                    settingstomlr["SerialPortSettings"]["DefaultAUTOSco"] = Convert.ToString(autotr);
                }

                using (StreamWriter writer = File.CreateText(FSSetToml))                  //������д��TOML�ļ�
                {
                    settingstomlr.WriteTo(writer);
                    writer.Flush();
                }
            }
        }
        private void ChipToolKitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            MCUTool selectedTool = (MCUTool)ChipToolKitComboBox.SelectedItem;

            if (selectedTool != null)
            {
                switch (selectedTool.Name)
                {
                    case "None":
                        McuToolsFrame.Navigate(typeof(NoneTools));
                        break;
                    case "ESP8266":
                        McuToolsFrame.Navigate(typeof(ESP8266Tools));
                        break;
                    case "RP2040        M":
                        McuToolsFrame.Navigate(typeof(RP2040MPYTools));
                        break;
                    // ��������Ӹ����case�����������������
                    default:
                        break;
                }
            }
        }

        private void RXDataButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private Task RXDATA_ClickAsync(object sender, RoutedEventArgs e)
        {
            // �������������첽����
            // ���磺await SomeAsyncMethod();
            
            //RXTextBox.Text = RXTextBox.Text + current_time.ToString("HH:mm:ss") + "  ";
            //Timesr = current_time.ToString("HH:mm:ss");



            //rxpstr = System.Text.Encoding.UTF8.GetString(datapwate);
            //rxpstr = datapwate.ToString();                          //����������ֵ�����
            //page1.RXTextBox.Text = page1.RXTextBox.Text + rxpstr + "";          //������յ�����
            //datapwate.Clear();                                      //��ջ�����

            return Task.CompletedTask;
        }

        private void ShowTimeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        

        
    }
}
