using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToastCore.Notification;

namespace ToastHelper {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        private ToastManager _manager;
        private Action _notify;

        public MainWindow() {
            InitializeComponent();
            _manager = new ToastManager();
            _manager.Init<ToastManager>("ToastTest");
            ToastManager.ToastCallback += ToastManager_ToastCallback;
        }

        private void ToastManager_ToastCallback(string app, string arg, List<KeyValuePair<string, string>> kvs) {
            string res = $"appid : {app}  arg : {arg} \n";
            kvs.ForEach(kv => res += $"key : {kv.Key}  value : {kv.Value} \n");
            App.Current.Dispatcher.Invoke(() => {
                Input.Text = res;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            _notify?.BeginInvoke(null, null);
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
    }
}
