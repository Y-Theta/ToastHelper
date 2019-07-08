///------------------------------------------------------------------------------
/// @ Y_Theta
///------------------------------------------------------------------------------
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ToastCore.Util;
using Windows.UI.Notifications;

namespace ToastCore.Notification {
    internal class DesktopNotificationManagerCompat {
        #region Properties
        public const string TOAST_ACTIVATED_LAUNCH_ARG = "-ToastActivated";
        static readonly Guid TOAST_G = new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3");

        private static bool _registeredAumidAndComServer;
        private static string _aumid;
        private static bool _registeredActivator;

        #endregion

        #region Methods
        public static void RegisterAumidAndComServer<T>(string aumid)
            where T : NotificationActivator {
            if (string.IsNullOrWhiteSpace(aumid)) {
                throw new ArgumentException("You must provide an AUMID.", nameof(aumid));
            }

            // If running as Desktop Bridge
            if (DesktopBridgeHelpers.IsRunningAsUwp()) {
                _aumid = null;
                _registeredAumidAndComServer = true;
                return;
            }

            _aumid = aumid;

            string exename = Assembly.GetExecutingAssembly().GetName().Name;
            string shortpath = "\\Microsoft\\Windows\\Start Menu\\A" + exename;
            var shortcut = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + shortpath + $"\\{_aumid}.lnk";
            if (!File.Exists(shortcut))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + shortpath);
                // 不需要从通知打开程序则不需要这项操作
                // RegisterComServer<T>(exePath);
                CreatShortcut<T>(shortcut);
            }

            _registeredAumidAndComServer = true;
        }

        /// <summary>
        /// 使应用程序可以从toast启动
        /// </summary>
        private static void RegisterComServer<T>(String exePath)
            where T : NotificationActivator {
            // We register the EXE to start up when the notification is activated
            string regString = $"SOFTWARE\\Classes\\CLSID\\{{{typeof(T).GUID.ToString()}}}\\LocalServer32";
            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            else
                localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);

            var key = localKey.CreateSubKey(regString);

            key.SetValue(null, '"' + exePath + '"');
        }

        /// <summary>
        /// Registers the activator type as a COM server client so that Windows can launch your activator.
        /// </summary>
        public static void RegisterActivator<T>()
            where T : NotificationActivator {
            // Register type
            var regService = new RegistrationServices();

            regService.RegisterTypeForComClients(
                typeof(T),
                RegistrationClassContext.LocalServer,
                RegistrationConnectionType.MultipleUse);

            _registeredActivator = true;
        }

        /// <summary>
        /// 创建快捷方式
        /// </summary>
        private static void CreatShortcut<T>(string shortcutPath) {

            String exePath = Process.GetCurrentProcess().MainModule.FileName;
            IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

            ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));

            IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

            using (PropVariant appId = new PropVariant(_aumid)) {
                ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(new PropertyKey(TOAST_G, 5), appId));
            }

            using (PropVariant toastid = new PropVariant(typeof(T).GUID))  {
                ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(new PropertyKey(TOAST_G, 26), toastid));
            }
            ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());

            // Commit the shortcut to disk
            IPersistFile newShortcutSave = (IPersistFile)newShortcut;

            ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
        }

        /// <summary>
        /// 创建通知
        /// </summary>
        public static ToastNotifier CreateToastNotifier() {
            EnsureRegistered();

            if (_aumid != null) {
                // Non-Desktop Bridge
                return ToastNotificationManager.CreateToastNotifier(_aumid);
            }
            else {
                // Desktop Bridge
                return ToastNotificationManager.CreateToastNotifier();
            }
        }

        /// <summary>
        /// Gets the <see cref="DesktopNotificationHistoryCompat"/> object. You must have called <see cref="RegisterActivator{T}"/> first (and also <see cref="RegisterAumidAndComServer(string)"/> if you're a classic Win32 app), or this will throw an exception.
        /// </summary>
        public static DesktopNotificationHistoryCompat History {
            get {
                EnsureRegistered();

                return new DesktopNotificationHistoryCompat(_aumid);
            }
        }

        private static void EnsureRegistered() {
            // If not registered AUMID yet
            if (!_registeredAumidAndComServer) {
                // Check if Desktop Bridge
                if (DesktopBridgeHelpers.IsRunningAsUwp()) {
                    // Implicitly registered, all good!
                    _registeredAumidAndComServer = true;
                }

                else {
                    // Otherwise, incorrect usage
                    throw new Exception("You must call RegisterAumidAndComServer first.");
                }
            }

            // If not registered activator yet
            if (!_registeredActivator) {
                // Incorrect usage
                throw new Exception("You must call RegisterActivator first.");
            }
        }

        /// <summary>
        /// Gets a boolean representing whether http images can be used within toasts. This is true if running under Desktop Bridge.
        /// </summary>
        public static bool CanUseHttpImages { get { return DesktopBridgeHelpers.IsRunningAsUwp(); } }

        /// <summary>
        /// Code from https://github.com/qmatteoq/DesktopBridgeHelpers/edit/master/DesktopBridge.Helpers/Helpers.cs
        /// </summary>
        private class DesktopBridgeHelpers {
            const long APPMODEL_ERROR_NO_PACKAGE = 15700L;

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

            private static bool? _isRunningAsUwp;
            public static bool IsRunningAsUwp() {
                if (_isRunningAsUwp == null) {
                    if (IsWindows7OrLower) {
                        _isRunningAsUwp = false;
                    }
                    else {
                        int length = 0;
                        StringBuilder sb = new StringBuilder(0);
                        int result = GetCurrentPackageFullName(ref length, sb);

                        sb = new StringBuilder(length);
                        result = GetCurrentPackageFullName(ref length, sb);

                        _isRunningAsUwp = result != APPMODEL_ERROR_NO_PACKAGE;
                    }
                }

                return _isRunningAsUwp.Value;
            }

            private static bool IsWindows7OrLower {
                get {
                    int versionMajor = Environment.OSVersion.Version.Major;
                    int versionMinor = Environment.OSVersion.Version.Minor;
                    double version = versionMajor + (double)versionMinor / 10;
                    return version <= 6.1;
                }
            }
        }
        #endregion

    }
}