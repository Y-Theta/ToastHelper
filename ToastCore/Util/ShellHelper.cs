///------------------------------------------------------------------------------
/// @ Y_Theta
///------------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ToastCore.Util {

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PropertyKey {
        #region Private Fields

        private Guid formatId;
        private Int32 propertyId;
        #endregion

        #region Public Construction

        public PropertyKey(Guid formatId, Int32 propertyId) {
            this.formatId = formatId;
            this.propertyId = propertyId;
        }
        #endregion
    }

    internal enum STGM : long {
        STGM_READ = 0x00000000L,
        STGM_WRITE = 0x00000001L,
        STGM_READWRITE = 0x00000002L,
        STGM_SHARE_DENY_NONE = 0x00000040L,
        STGM_SHARE_DENY_READ = 0x00000030L,
        STGM_SHARE_DENY_WRITE = 0x00000020L,
        STGM_SHARE_EXCLUSIVE = 0x00000010L,
        STGM_PRIORITY = 0x00040000L,
        STGM_CREATE = 0x00001000L,
        STGM_CONVERT = 0x00020000L,
        STGM_FAILIFTHERE = 0x00000000L,
        STGM_DIRECT = 0x00000000L,
        STGM_TRANSACTED = 0x00010000L,
        STGM_NOSCRATCH = 0x00100000L,
        STGM_NOSNAPSHOT = 0x00200000L,
        STGM_SIMPLE = 0x08000000L,
        STGM_DIRECT_SWMR = 0x00400000L,
        STGM_DELETEONRELEASE = 0x04000000L,
    }

    [ComImport,
    Guid("000214F9-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellLinkW {
        UInt32 GetPath(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
            int cchMaxPath,
            //ref _WIN32_FIND_DATAW pfd,
            IntPtr pfd,
            uint fFlags);
        UInt32 GetIDList(out IntPtr ppidl);
        UInt32 SetIDList(IntPtr pidl);
        UInt32 GetDescription(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
            int cchMaxName);
        UInt32 SetDescription(
            [MarshalAs(UnmanagedType.LPWStr)] string pszName);
        UInt32 GetWorkingDirectory(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
            int cchMaxPath
            );
        UInt32 SetWorkingDirectory(
            [MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        UInt32 GetArguments(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs,
            int cchMaxPath);
        UInt32 SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        UInt32 GetHotKey(out short wHotKey);
        UInt32 SetHotKey(short wHotKey);
        UInt32 GetShowCmd(out uint iShowCmd);
        UInt32 SetShowCmd(uint iShowCmd);
        UInt32 GetIconLocation(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] out StringBuilder pszIconPath,
            int cchIconPath,
            out int iIcon);
        UInt32 SetIconLocation(
            [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
            int iIcon);
        UInt32 SetRelativePath(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
            uint dwReserved);
        UInt32 Resolve(IntPtr hwnd, uint fFlags);
        UInt32 SetPath(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    [ComImport]
    [Guid("0000010b-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersistFile {
        UInt32 GetCurFile(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile
        );
        UInt32 IsDirty();
        UInt32 Load(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            [MarshalAs(UnmanagedType.U4)] STGM dwMode);
        UInt32 Save(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            bool fRemember);
        UInt32 SaveCompleted(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
    }

    [ComImport]
    [Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IPropertyStore {
        UInt32 GetCount([Out] out uint propertyCount);
        UInt32 GetAt([In] uint propertyIndex, out PropertyKey key);
        UInt32 GetValue([In] ref PropertyKey key, [Out] PropVariant pv);
        UInt32 SetValue([In] ref PropertyKey key, [In] PropVariant pv);
        UInt32 Commit();
    }

    /// <summary>
    /// 承载IShellLinkW
    /// </summary>
    [ComImport,
    Guid("00021401-0000-0000-C000-000000000046"),
    ClassInterface(ClassInterfaceType.None)]
    internal class CShellLink { }

    public static class ErrorHelper {
        public static void VerifySucceeded(UInt32 hresult) {
            if (hresult > 1) {
                throw new Exception("Failed with HRESULT: " + hresult.ToString("X"));
            }
        }
    }

    /// <summary>
    /// 对c++中一种结构的封装
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public sealed class PropVariant : IDisposable {
        #region Fields

        [FieldOffset(0)]
        decimal _decimal;
        [FieldOffset(0)]
        ushort _valueType;
        [FieldOffset(8)]
        IntPtr _ptr2;
        [FieldOffset(8)]
        IntPtr _ptr;

        #endregion 

        #region Constructors
        public PropVariant(string value) {
            if (value == null) {
                throw new ArgumentException();
            }
            _valueType = (ushort)VarEnum.VT_LPWSTR;
            _ptr = Marshal.StringToCoTaskMemUni(value);
        }

        public PropVariant(Guid guid) {
            if (guid == null) {
                throw new ArgumentException();
            }
            byte[] bytes = guid.ToByteArray();
            _valueType = (ushort)VarEnum.VT_CLSID;
            _ptr2 = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, _ptr2, bytes.Length);
        }

        #endregion

        #region IDisposable Members
        [DllImport("Ole32.dll", PreserveSig = false)] // returns hresult
        internal extern static void PropVariantClear([In, Out] PropVariant pvar);

        public void Dispose() {
            PropVariantClear(this);
            GC.SuppressFinalize(this);
        }

        ~PropVariant() {
            Dispose();
        }

        #endregion
    }
}
