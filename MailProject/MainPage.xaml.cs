using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MailProject
{
    public sealed partial class MainPage : Page
    {
        private Frame mainView;
        private Settings settingsView = new Settings();

        public MainPage()
        {
            this.InitializeComponent();
            mainView = (Frame)FindName("MainView");
            mainView.Navigate(typeof(MailBox));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            mainView.Navigate(typeof(MailBox));
        }

        private void ContactButton_Click(object sender, RoutedEventArgs e)
        {
            mainView.Navigate(typeof(Contacts));
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            mainView.Navigate(typeof(Calendar));
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            mainView.Navigate(typeof(Settings));
        }
    }
}
