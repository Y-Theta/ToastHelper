///------------------------------------------------------------------------------
/// @ Y_Theta
///------------------------------------------------------------------------------


namespace ToastCore.Notification {

    /// <summary>
    /// Toast通知的按钮命令
    /// </summary>
    public struct ToastCommands {
        #region Properties
        public string Argument;

        public string Content;
        #endregion

        #region Constructors
        public ToastCommands(string arg, string content) {
            Argument = arg;
            Content = content;
        }
        #endregion
    }

}
