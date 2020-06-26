///------------------------------------------------------------------------------
/// @ Y_Theta
///------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using ToastCore.Notification;

namespace CoreTest {

    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(INotificationActivationCallback))]
    [Guid("F6EDC682-FB72-4DE0-A071-C12D0CBF2A48"), ComVisible(true)]
    public class ToastManagerCore : NotificationService {
        #region Properties
        #endregion

        #region Methods
        public override void OnActivated(string arguments, NotificationUserInput userInput, string appUserModelId) {
            base.OnActivated(arguments, userInput, appUserModelId);
        }
        #endregion

        #region Constructors
        #endregion
    }
}
