using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using CoreTest;

using ToastCore.Notification;

namespace CoreTets {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private ToastManagerCore _manager;
        private Action _notify;

        public MainWindow() {
            InitializeComponent();
            _manager = new ToastManagerCore();
            _manager.Init<ToastManagerCore>("ToastTestCore");
            ToastManagerCore.ToastCallback += ToastManager_ToastCallback;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            _notify?.Invoke();
        }

        private void ToastManager_ToastCallback(string app, string arg, List<KeyValuePair<string, string>> kvs) {
            string res = $"appid : {app}  arg : {arg} \n";
            kvs.ForEach(kv => res += $"key : {kv.Key}  value : {kv.Value} \n");
            App.Current.Dispatcher.Invoke(() => {
                Input.Text = res;
            });
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            switch ((sender as ComboBox).SelectedIndex) {
                case 0:
                    _notify = new Action(() => _manager.Notify("Hello", "Toast"));
                    break;
                case 1:
                    _notify = new Action(() => _manager.Notify("Hello", "Toast", new ToastCommands { Content = "OK", Argument = "OKarg" }, new ToastCommands { Content = "NO", Argument = "NOarg" }));
                    break;
                case 2:
                    _notify = new Action(() => _manager.Notify("Hello", "Toast", new ToastCommands[] { new ToastCommands { Content = "Input", Argument = "input" } }, new ToastCommands[] { new ToastCommands { Content = "Reply", Argument = "btn" } }));
                    break;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {

        }
    }

}
