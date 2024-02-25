﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Collections;


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
using System.IO.Ports;
using Microsoft.UI;           // Needed for WindowId.
using Microsoft.UI.Windowing; // Needed for AppWindow.
using WinRT;
using WinRT.Interop;
using Windows.UI;          // Needed for XAML/HWND interop.
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel;
using Windows.Graphics;

using System.Diagnostics;
using System.IO;

using Tommy;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;
using Windows.ApplicationModel.Activation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FSGaryityTool_Win11
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>

    

    public sealed partial class MainWindow : Window
    {

        public static string FSSoftVersion = "0.2.8";

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if ((string)selectedItem.Tag == "MainPage1") FSnvf.Navigate(typeof(MainPage1));
            else if ((string)selectedItem.Tag == "Page2") FSnvf.Navigate(typeof(Page2));
            else if ((string)selectedItem.Tag == "Page3") FSnvf.Navigate(typeof(Page3));
            else if ((string)selectedItem.Tag == "FSPage") FSnvf.Navigate(typeof(FSPage));

            if (args.IsSettingsSelected)
            {
                FSnvf.Navigate(typeof(MainSettingsPage));
            }

        }

        private AppWindow m_AppWindow;

        public NavigationFailedEventHandler OnNavigationFailed { get; private set; }


        public MainWindow()
        {
            this.InitializeComponent();

            // 将窗口的标题栏设置为自定义标题栏
            this.ExtendsContentIntoTitleBar = true;

            m_AppWindow = GetAppWindowForCurrentWindow();
            m_AppWindow.Title = "FSGravityTool";//Set AppWindow
            m_AppWindow.SetIcon("FSsoftH.ico");


            string SYSAPLOCAL = Environment.GetFolderPath(folder: Environment.SpecialFolder.LocalApplicationData);
            string FSFolder = Path.Combine(SYSAPLOCAL, "FAIRINGSTUDIO");
            string FSGravif = Path.Combine(FSFolder, "FSGravityTool");
            string FSSetJson = Path.Combine(FSGravif, "Settings.json");
            string FSSetToml = Path.Combine(FSGravif, "Settings.toml");

            //Debug.WriteLine("开始搜索文件夹");
            Debug.WriteLine("开始搜索文件夹  " + FSFolder);            //新建FS文件夹


            if (Directory.Exists(FSFolder))
            {
                Debug.WriteLine("找到文件夹,跳过新建文件夹");
            }
            else
            {
                Debug.WriteLine("没有找到文件夹");
                Directory.CreateDirectory(FSFolder);
                Debug.WriteLine("新建文件夹");
            }


            if (Directory.Exists(FSGravif))
            {
                Debug.WriteLine("找到文件夹,跳过新建文件夹");
            }
            else
            {
                Debug.WriteLine("没有找到文件夹");
                Directory.CreateDirectory(FSGravif);
                Debug.WriteLine("新建文件夹");
            }


            /*
            if (File.Exists(FSSetJson))
            {
                Debug.WriteLine("找到JSON文件,跳过新建文件");
            }
            else
            {
                Debug.WriteLine("没有找到JSON文件");
                var SettJson = new              //创建对象
                {
                    FSGravity = "Tool",
                    SerialSettings = "1",
                    DefaultBAUD = "115200",
                    DefaultParity = "None",
                    DefaultSTOP = "One",
                    DefaultDATA = "8",
                    DefaultRXHEX = "0",
                    DefaultTXHEX = "0",
                    DefaultDTR = "1",
                    DefaultRTS = "0",
                    DefaultSTime = "0",
                    DefaultAUTOSco = "1",
                };
                var jsonstring1 = JsonConvert.SerializeObject(SettJson);        //序列化Json
                using (StreamWriter file = File.CreateText(FSSetJson))          //创建json
                {
                    file.WriteLine(jsonstring1);
                }
                Debug.WriteLine("新建JSON");
            }
            */

            if (File.Exists(FSSetToml))             //生成TOML
            {
                Debug.WriteLine("找到TOML文件,跳过新建文件");
            }
            else
            {
                Debug.WriteLine("没有找到TOML文件");

                TomlTable settingstoml = new TomlTable
                {
                    ["Version"] = FSSoftVersion,

                    ["FSGravitySettings"] =
                    {
                        Comment =
                        "FSGaryityTool Settings:",
                        ["DefaultNvPage"] = "0",
                    },

                    ["SerialPortSettings"] =
                    {
                        Comment = 
                        "FSGaryityTool SerialPort Settings:\r\n" +
                        "Parity:None,Odd,Even,Mark,Space\r\n" +
                        "STOPbits:None,One,OnePointFive,Two\r\n" +
                        "DATAbits:5~9",

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
                    },

                };

                using (StreamWriter writer = File.CreateText(FSSetToml))
                {
                    settingstoml.WriteTo(writer);
                    Debug.WriteLine("写入Toml");
                    // Remember to flush the data if needed!
                    writer.Flush();
                }
                Debug.WriteLine("新建TOML");
            }



            string TomlfsVersion;       //版本号比较

            using (StreamReader reader = File.OpenText(FSSetToml))
            {
                TomlTable settingstomlr = TOML.Parse(reader);
                TomlfsVersion = settingstomlr["Version"];
            }

            Version TomlVersion = new Version(TomlfsVersion);
            Version FSGrVersion = new Version(FSSoftVersion);

            if (FSGrVersion > TomlVersion)
            {
                Debug.WriteLine(">");
            }
            else
            {
                Debug.WriteLine("<=");
            }

                                                     //设置默认页面

            using (StreamReader reader = File.OpenText(FSSetToml))
            {
                TomlTable settingstomlr = TOML.Parse(reader);
                Debug.WriteLine("Print:" + settingstomlr["FSGravitySettings"]["DefaultNvPage"]);
                int NvPage = int.Parse(settingstomlr["FSGravitySettings"]["DefaultNvPage"]);
                FSnv.SelectedItem = FSnv.MenuItems[NvPage];             //设置默认页面
            }

            

            /*
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                AppWindowTitleBar m_TitleBar = m_AppWindow.TitleBar;

                // Set active window colors.
                // Note: No effect when app is running on Windows 10
                // because color customization is not supported.
                m_TitleBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);
                m_TitleBar.BackgroundColor = Color.FromArgb(255, 22, 22, 22);
                m_TitleBar.ButtonForegroundColor = Color.FromArgb(255, 255, 255, 255);
                m_TitleBar.ButtonBackgroundColor = Color.FromArgb(255, 22, 22, 22);
                m_TitleBar.ButtonHoverForegroundColor = Color.FromArgb(255, 0, 0, 0);
                m_TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 230, 224, 0);
                m_TitleBar.ButtonPressedForegroundColor = Color.FromArgb(255, 0, 0, 0);
                m_TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 230, 224, 0);

                // Set inactive window colors.
                // Note: No effect when app is running on Windows 10
                // because color customization is not supported.
                m_TitleBar.InactiveForegroundColor = Colors.Gainsboro;
                m_TitleBar.InactiveBackgroundColor = Color.FromArgb(255, 22, 22, 22);
                m_TitleBar.ButtonInactiveForegroundColor = Colors.Gainsboro;
                m_TitleBar.ButtonInactiveBackgroundColor = Color.FromArgb(255, 22, 22, 22);

                
            }*/

            TitleBarTextBlock.Text = "FSGravityTool";
            //SystemBackdrop = new MicaBackdrop()
            //{ Kind = MicaKind.BaseAlt };
            SystemBackdrop = new DesktopAcrylicBackdrop();

            
        }

        // Call your extend acrylic code in the OnLaunched event, after
        // calling Window.Current.Activate.



        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            
        }


        /*
        
        public class Tab1
        {
            private SerialPort serialPort1;

            public Tab1()
            {
                this.serialPort1 = new SerialPort();
                // 配置并打开serialPort1
            }

            // 使用serialPort1进行通信
        }

        public class Tab2
        {
            private SerialPort serialPort2;

            public Tab2()
            {
                this.serialPort2 = new SerialPort();
                // 配置并打开serialPort2
            }

            // 使用serialPort2进行通信
        }
        */

    }

}
