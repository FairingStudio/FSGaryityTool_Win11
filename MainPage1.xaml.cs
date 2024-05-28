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
using Microsoft.UI.Xaml.Media.Animation;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FSGaryityTool_Win11
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage1 : Page
    {
        public static int Pge = 0;

        private CancellationTokenSource delayCts = new CancellationTokenSource();
        public MainPage1()
        {
            this.InitializeComponent();

            //TabView_AddTabButtonClick(SPTabView, null);

            SerialPort.Text = Page1.LanguageText("serialPort");

        }

        private async void SPSelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBarItem selectorBarItem = sender.SelectedItem;
            int currentSelectionIndex = sender.Items.IndexOf(selectorBarItem);
            int previousSelectedIndex = 0;
            System.Type pageType = typeof(Page1);

            switch (currentSelectionIndex)
            {
                case 0:
                    pageType = typeof(Page1);
                    break;
                case 1:
                    pageType = typeof(Page2);
                    break;
            }
            var slideNavigationTransitionEffect = currentSelectionIndex - previousSelectedIndex > 0 ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft;

            FSSPagf.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = slideNavigationTransitionEffect });

            previousSelectedIndex = currentSelectionIndex;

            // ȡ��֮ǰ����ʱ
            delayCts.Cancel();
            delayCts = new CancellationTokenSource();

            try
            {
                // �ȴ�2�룬��������ڼ䱻ȡ�������׳� OperationCanceledException
                await Task.Delay(500, delayCts.Token);

                // Ȼ�󵼺���ͬһ��ҳ�棬�������Ź��ɶ���
                FSSPagf.Navigate(pageType, null, null);
            }
            catch (OperationCanceledException)
            {
                // �����ʱ��ȡ���������κ�����
            }
        }


        /*
        // Add a new Tab to the TabView
        private void TabView_AddTabButtonClick(TabView sender, object args)
        {
            var newTab = new TabViewItem();
            newTab.IconSource = new SymbolIconSource() { Symbol = Symbol.Sort };
            newTab.Header = "Serial Port " + Pge;

            // The Content of a TabViewItem is often a frame which hosts a page.
            Frame frame = new Frame();
            newTab.Content = frame;
            frame.Navigate(typeof(Page1));

            sender.TabItems.Add(newTab);

            Pge++;
        }

        // Remove the requested tab from the TabView
        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
            Pge--;
        }

        
        */


    }
}
