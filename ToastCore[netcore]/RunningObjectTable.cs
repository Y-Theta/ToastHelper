///------------------------------------------------------------------------------
/// @ Y_Theta
///------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;


namespace ToastCore {


    public class RunningObjectTable : IDisposable {
        #region Properties
        #endregion

        #region Methods
        private readonly IRunningObjectTable rot;
        private bool isDisposed = false;

        public RunningObjectTable() {
            Ole32.GetRunningObjectTable(0, out this.rot);
        }

        public void Dispose() {
            if (this.isDisposed) {
                return;
            }

            Marshal.ReleaseComObject(this.rot);
            this.isDisposed = true;
        }

        /// <summary>
        /// Attempts to register an item in the ROT.
        /// </summary>
        public IDisposable Register(string itemName, object obj) {
            IMoniker moniker = CreateMoniker(itemName);

            const int ROTFLAGS_REGISTRATIONKEEPSALIVE = 1;
            int regCookie = this.rot.Register(ROTFLAGS_REGISTRATIONKEEPSALIVE, obj, moniker);

            return new RevokeRegistration(rot, regCookie);
        }

        /// <summary>
        /// Attempts to retrieve an item from the ROT.
        /// </summary>
        public object GetObject(string itemName) {
            
            IMoniker mk = CreateMoniker(itemName);
            
            int hr = this.rot.GetObject(mk, out object obj);
            if (hr != 0) {
                Marshal.ThrowExceptionForHR(hr);
            }

            return obj;
        }

        private void Revoke(int regCookie) {
            this.rot.Revoke(regCookie);
        }

        private IMoniker CreateMoniker(string itemName) {
            Ole32.CreateItemMoniker("!", itemName, out IMoniker mk);
            return mk;
        }

    
        #endregion

        #region Constructors
        #endregion
    }

    internal class RevokeRegistration : IDisposable {
        private readonly IRunningObjectTable rot;
        private readonly int regCookie;

        public RevokeRegistration(IRunningObjectTable rot, int regCookie) {
            this.rot = rot;
            this.regCookie = regCookie;
        }

        public void Dispose() {
            this.rot.Revoke(this.regCookie);
        }

    }

    internal static class Ole32 {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [DllImport("Ole32.dll")]
        static extern int CreateClassMoniker([In] ref Guid rclsid,
      out IMoniker ppmk);

        [DllImport(nameof(Ole32))]
        public static extern void CreateItemMoniker(
            [MarshalAs(UnmanagedType.LPWStr)] string lpszDelim,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszItem,
            out IMoniker ppmk);

        [DllImport(nameof(Ole32))]
        public static extern void GetRunningObjectTable(
            int reserved,
            out IRunningObjectTable pprot);
    }

}