///------------------------------------------------------------------------------
/// @ Y_Theta
///------------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;

namespace ToastCore.Notification {
    public abstract class NotificationActivator : INotificationActivationCallback {

        #region Methods
        public void Activate(string appUserModelId, string invokedArgs, NOTIFICATION_USER_INPUT_DATA[] data, uint dataCount) {
            OnActivated(invokedArgs, new NotificationUserInput(data), appUserModelId);
        }

        public abstract void OnActivated(string arguments, NotificationUserInput userInput, string appUserModelId);

        #endregion
    }

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct NOTIFICATION_USER_INPUT_DATA {

        [MarshalAs(UnmanagedType.LPWStr)]
        public string Key;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string Value;
    }

    [ComImport, Guid("53E31837-6600-4A81-9395-75CFFE746F94"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INotificationActivationCallback {
        void Activate(
            [In, MarshalAs(UnmanagedType.LPWStr)] string appUserModelId,
            [In, MarshalAs(UnmanagedType.LPWStr)] string invokedArgs,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] NOTIFICATION_USER_INPUT_DATA[] data,
            [In, MarshalAs(UnmanagedType.U4)] uint dataCount
            );
    }

}
