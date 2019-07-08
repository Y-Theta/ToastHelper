///------------------------------------------------------------------------------
/// @ Y_Theta
///------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ToastCore.Notification;

namespace ToastHelper {

    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(INotificationActivationCallback))]
    [Guid("7ddba60f-e2f0-4373-8098-0eafb79ba54a"), ComVisible(true)]
    public class ToastManager : NotificationService {
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
