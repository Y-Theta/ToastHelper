///------------------------------------------------------------------------------
/// @ Y_Theta
///------------------------------------------------------------------------------
using System.Collections.Generic;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace ToastCore.Notification {

    /// <summary>
    /// Win10通知
    /// Toast通知回调
    /// </summary>
    /// <param name="app">app的id</param>
    /// <param name="arg">按钮的参数</param>
    /// <param name="kvs">用户输入</param>
    public delegate void ToastAction(string app, string arg, List<KeyValuePair<string, string>> kvs);

    /// <summary>
    /// 继承自NotificationActivator 本来是为了使用OnActivated回调
    /// 在继承类上添加
    /// [ClassInterface(ClassInterfaceType.None)]
    /// [ComSourceInterfaces(typeof(INotificationActivationCallback))]
    /// [Guid("7ddba60f-e2f0-4373-8098-0eafb79ba54a"), ComVisible(true)]
    /// 换上自己的GUID
    /// </summary>
    public class NotificationService : NotificationActivator {
        #region Methods
        public void Init<T>(string appid) where T : NotificationActivator {
            // Console.WriteLine("Init" + Thread.CurrentThread.ManagedThreadId);
            DesktopNotificationManagerCompat.RegisterAumidAndComServer<T>(appid);
            DesktopNotificationManagerCompat.RegisterActivator<T>();
        }

        /// <summary>
        /// 通知响应事件,在使用时接收
        /// </summary>
        public static event ToastAction ToastCallback;

        /// <summary>
        /// 微软提供的回调,调用者不在当前上下文线程中
        /// </summary>
        public override void OnActivated(string arguments, NotificationUserInput userInput, string appUserModelId) {
            List<KeyValuePair<string, string>> kvs = new List<KeyValuePair<string, string>>();
            if (userInput != null && userInput.Count > 0)
                foreach (var key in userInput.Keys) {
                    kvs.Add(new KeyValuePair<string, string>(key, userInput[key]));
                }
            ToastCallback?.Invoke(appUserModelId, arguments, kvs);
        }

        /// <summary>
        /// 发送一条自定义格式通知
        /// </summary>
        public void Notify() {
            XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            OnSetNotifyXML(xml);
            ShowToast(xml);
        }

        /// <summary>
        /// 重写此方法以自定义通知xml内容
        /// </summary>
        /// <param name="xml"></param>
        protected virtual void OnSetNotifyXML(XmlDocument xml) {

        }

        /// <summary>
        /// 发送一条通知 （标题/文本）
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">文本</param>
        public void Notify(string title, string content) {
            XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            AddTitle(xml, title, content);
            ShowToast(xml);
        }

        /// <summary>
        /// 发送一条通知 （标题/文本/自定义命令）
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">文本</param>
        /// <param name="commands">自定义命令组</param>
        public void Notify(string title, string content, params ToastCommands[] commands) {
            XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            AddTitle(xml, title, content);
            AddCommands(xml, commands);
            ShowToast(xml);
        }

        /// <summary>
        /// 发送一条通知 （自定义图标/标题/文本）
        /// </summary>
        /// <param name="picuri">自定义图标路径</param>
        /// <param name="title">标题</param>
        /// <param name="content">文本</param>
        public void Notify(string picuri, string title, string content) {
            XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            AddTitle(xml, title, content);
            AddBigLogo(xml, picuri);
            ShowToast(xml);
        }

        /// <summary>
        /// 发送一条通知 （自定义图标/标题/文本/自定义命令）
        /// </summary>
        /// <param name="picuri">自定义图标路径</param>
        /// <param name="title">标题</param>
        /// <param name="content">文本</param>
        /// <param name="commands">自定义命令组</param>
        public void Notify(string picuri, string title, string content, params ToastCommands[] commands) {
            XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            AddTitle(xml, title, content);
            AddBigLogo(xml, picuri);
            AddCommands(xml, commands);
            ShowToast(xml);
        }

        public void Notify(string title, string content, ToastCommands[] paras, ToastCommands[] commands) {
            XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            AddTitle(xml, title, content);
            AddInput(xml, paras);
            AddCommands(xml, commands);
            ShowToast(xml);
        }

        /// <summary>
        /// 发送通知
        /// </summary>
        protected static void ShowToast(XmlDocument xml) {
            ToastNotification toast = new ToastNotification(xml);
            DesktopNotificationManagerCompat.CreateToastNotifier().Show(toast);
        }

        /// <summary>
        /// 添加标题和内容描述
        /// </summary>
        protected static void AddTitle(XmlDocument xml, string title, string content) {
            XmlNodeList lines = xml.GetElementsByTagName("text");
            lines[0].AppendChild(xml.CreateTextNode(title));
            lines[1].AppendChild(xml.CreateTextNode(content));
        }

        /// <summary>
        /// 为当前通知添加交互操作
        /// </summary>
        protected static void AddCommands(XmlDocument xml, params ToastCommands[] commands) {
            XmlElement actions = GetAction(xml);

            foreach (var command in commands) {
                XmlElement action = xml.CreateElement("action");
                action.SetAttribute("activationType", "foreground");
                action.SetAttribute("arguments", command.Argument);
                action.SetAttribute("content", command.Content);
                actions.AppendChild(action);
            }
        }

        /// <summary>
        /// 添加输入框
        /// </summary>
        protected static void AddInput(XmlDocument xml, params ToastCommands[] paras) {
            XmlElement actions = GetAction(xml);

            foreach (var para in paras) {
                XmlElement input = xml.CreateElement("input");
                input.SetAttribute("type", "text");
                input.SetAttribute("id", para.Argument);
                input.SetAttribute("placeHolderContent", para.Content);
                actions?.AppendChild(input);
            }
        }

        /// <summary>
        /// 为通知添加大标签
        /// </summary>
        protected static void AddBigLogo(XmlDocument xml, string logopath) {
            //获得binding组
            XmlElement binding = (XmlElement)xml.GetElementsByTagName("binding")[0];
            binding.SetAttribute("template", "ToastGeneric");
            //创建大图标
            XmlElement image = xml.CreateElement("image");
            image.SetAttribute("placement", "appLogoOverride");
            image.SetAttribute("src", logopath);
            binding.AppendChild(image);
        }

        /// <summary>
        /// 获取action组
        /// </summary>
        private static XmlElement GetAction(XmlDocument xml) {
            XmlElement actions = null;
            if (xml.GetElementsByTagName("actions").Count != 0)
                actions = (XmlElement)xml.GetElementsByTagName("actions")[0];
            else {
                actions = xml.CreateElement("actions");
                ((XmlElement)xml.GetElementsByTagName("toast")[0]).AppendChild(actions);
            }
            return actions;
        }

        /// <summary>
        /// 清除对应App的所有通知
        /// </summary>
        /// <param name="appid">app标识</param>
        public void ClearHistory(string appid) {
            new DesktopNotificationHistoryCompat(appid).Clear();
        }
        #endregion
    }
}
