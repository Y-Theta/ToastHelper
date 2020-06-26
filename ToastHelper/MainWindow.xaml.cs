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
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;

namespace ToastHelper {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        private ToastManager _manager;
        private Action _notify;
        private readonly IRunningObjectTable rot;

        private uint uid;


        [DllImport("Ole32.dll")]
        static extern int CreateClassMoniker([In] ref Guid rclsid,
            out IMoniker ppmk);


        [DllImport("Ole32.dll")]
        public static extern void GetRunningObjectTable(
           int reserved,
           out IRunningObjectTable pprot);

        [DllImport("Ole32.dll")]
        static extern int CoRegisterClassObject(
    [MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
    [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
    uint dwClsContext,
    uint flags,
    out uint lpdwRegister);

        public MainWindow() {
            InitializeComponent();
            _manager = new ToastManager();
            _manager.Init<ToastManager>("ToastTest");
            ToastManager.ToastCallback += ToastManager_ToastCallback;
            GetRunningObjectTable(0, out this.rot);
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

            // IMoniker mk;
            // Guid g = new Guid("DBCDABF6-9690-439F-9066-5269F63C287A");
            // CreateClassMoniker(ref g, out mk);

            //int status =   CoRegisterClassObject(typeof(TestCOM).GUID, new TestCOM(), (int)RegistrationClassContext.LocalServer, (int)RegistrationConnectionType.MultipleUse, out uid);

            // Console.WriteLine(status);

            //const int ROTFLAGS_REGISTRATIONKEEPSALIVE = 1;
            //int regCookie = this.rot.Register(ROTFLAGS_REGISTRATIONKEEPSALIVE, new TestCOM(), mk);

            //var regService = new RegistrationServices();

            //regService.RegisterTypeForComClients(
            //    typeof(TestCOM),
            //    RegistrationClassContext.LocalServer,
            //    RegistrationConnectionType.MultipleUse);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            //IMoniker mk;
            //Guid g = new Guid("475E398F-8AFA-43a7-A3BE-F4EF8D6787C9");
            //CreateClassMoniker(ref g, out mk);

            //int hr = this.rot.GetObject(mk, out object obj);
            //if (hr != 0) {
            //    Marshal.ThrowExceptionForHR(hr);
            //}

            //RegistrationServices ser = (obj as RegistrationServices);
          
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

    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    [Guid("DBCDABF6-9690-439F-9066-5269F63C287A")]
    public class TestCOM {
        public string Hello() {
            Console.WriteLine("Hello World");
            return "Hellow World";
        }
    }



    //[ClassInterface(ClassInterfaceType.None)]
    //[ComVisible(true)]
    //[Guid("475E398F-8AFA-43a7-A3BE-F4EF8D6787C9")]
    //public class RegistrationServices { }
}
