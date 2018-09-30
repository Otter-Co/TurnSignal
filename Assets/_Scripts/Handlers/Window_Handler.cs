using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class Window_Handler : MonoBehaviour
{
    private const string _show_text = "Show Window";
    private const string _hide_text = "Hide Window";
    private const string _quit_text = "Quit TurnSignal";

    [System.Serializable]
    public class TrayEvent : UnityEvent { };

    [Header("Tray Icon Settings")]
    public Texture2D iconTexture;

    [Header("Tray Icon Events")]
    public TrayEvent onShowClicked;
    public TrayEvent onExitClicked;

    [HideInInspector]
    public bool trayHidden = false;

    [HideInInspector]
    public bool windowHidden = false;

    #region Window

    private IntPtr _windowHandle = IntPtr.Zero;
    private int _originalWindowStyle = 0;

    public void HideWindow()
    {
        bool wasVisible = Win_Handler_Extern.ShowWindow(
            _windowHandle,
            (int)Win_Handler_Extern.ShowWindowType.Minimize
        );

        Win_Handler_Extern.SetWindowLong(
            _windowHandle,
            (int)Win_Handler_Extern.WindowLongType.EXSTYLE,
            (int)Win_Handler_Extern.ExtWindowStyles.WS_EX_TOOLWINDOW
        );

        windowHidden = true;

        if (_trayMenu_show != null)
            _trayMenu_show.Text = _show_text;
    }

    public void ShowWindow()
    {
        Win_Handler_Extern.SetWindowLong(
            _windowHandle,
            (int)Win_Handler_Extern.WindowLongType.EXSTYLE,
            _originalWindowStyle
        );

        bool wasHidden = Win_Handler_Extern.ShowWindow(
            _windowHandle,
            (int)Win_Handler_Extern.ShowWindowType.Restore
        );

        windowHidden = false;

        if (_trayMenu_show != null)
            _trayMenu_show.Text = _hide_text;
    }

    #endregion

    #region Tray Icon

    private NotifyIcon _trayNotifyIcon;
    private Bitmap _trayIconBitmap;
    private Icon _trayIcon;

    private ContextMenuStrip _trayMenu;

    private ToolStripItem _trayMenu_show;
    private ToolStripItem _trayMenu_quit;



    public void HideTrayIcon()
    {
        trayHidden = true;
        _trayNotifyIcon.Visible = false;
    }
    public void ShowTrayIcon()
    {
        trayHidden = false;
        _trayNotifyIcon.Visible = true;
    }

    #endregion

    void Start()
    {
        if (onExitClicked == null)
            onExitClicked = new TrayEvent();

        if (onShowClicked == null)
            onShowClicked = new TrayEvent();

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        WindowStart();
        TrayIconStart();
#endif
    }

    private void WindowStart()
    {
        _windowHandle = Win_Handler_Extern.FindWindowByCaption(
            IntPtr.Zero,
            UnityEngine.Application.productName
        );

        _originalWindowStyle = Win_Handler_Extern.GetWindowLong(
            _windowHandle,
            (int)Win_Handler_Extern.WindowLongType.EXSTYLE
        );
    }

    private void TrayIconStart()
    {
        _trayNotifyIcon = new NotifyIcon() { Text = "TurnSignal", Visible = true };
        _trayMenu = new ContextMenuStrip();

        _trayIconBitmap = GetBitmapFromTex(iconTexture != null ? iconTexture : Texture2D.whiteTexture);
        _trayIcon = Icon.FromHandle(_trayIconBitmap.GetHicon());

        _trayMenu_show = _trayMenu.Items.Add(_hide_text, _trayIconBitmap, _onShowClicked);
        _trayMenu_quit = _trayMenu.Items.Add(_quit_text, null, _onQuitClicked);

        _trayNotifyIcon.ContextMenuStrip = _trayMenu;
        _trayNotifyIcon.Icon = _trayIcon;
    }

    private void _onShowClicked(object o, EventArgs e)
    {
        Debug.Log("Show Clicked!");
        onShowClicked.Invoke();
    }
    private void _onQuitClicked(object o, EventArgs e)
    {
        onExitClicked.Invoke();
    }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    void OnDestroy() => CleanUp();
    void OnApplicationQuit() => CleanUp();
#endif

    void CleanUp()
    {
        this._trayMenu.Dispose();
        this._trayMenu_show.Dispose();
        this._trayMenu_quit.Dispose();

        this._trayNotifyIcon.Dispose();

        this._trayIcon.Dispose();
        this._trayIconBitmap.Dispose();
    }

    public static Bitmap GetBitmapFromTex(Texture2D tex)
    {
        Bitmap ret = null;

        using (MemoryStream memS = new MemoryStream(tex.EncodeToPNG()))
        {
            memS.Seek(0, SeekOrigin.Begin);
            ret = new Bitmap(memS);
        }

        return ret;
    }
}

public static class Win_Handler_Extern
{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
    public static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public  static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public  static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
#else
    public static IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName) => IntPtr.Zero;
    public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong) => 0;
    public static int GetWindowLong(IntPtr hWnd, int nIndex) => 0;
    public static bool ShowWindow(IntPtr hWnd, int nCmdShow) => true;
    public static bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags) => true;
#endif

    public enum WindowLongType
    {
        USERDATA = -21,
        EXSTYLE = -20,
        STYLE = -16,
        ID = -12,
        H_INSTANCE = -6,
        WNDPROC = -4,
    }

    public enum ShowWindowType
    {

        Hide = 0,
        Show_Normal = 1,
        Show_Minimized = 2,
        Maximize = 3,
        Show_NoActive = 4,
        Show = 5,
        Minimize = 6,
        Show_MinnoActive = 7,
        Show_NA = 8,
        Restore = 9,
        Show_Default = 10,
        ForceMinimize = 11,
    }

    public enum WindowStyle
    {
        WS_BORDER = (int)0x00800000L,
        WS_CAPTION = (int)0x00C00000L,
        WS_CHILD = (int)0x40000000L,
        WS_CHILDWINDOW = (int)0x40000000L,
        WS_CLIPCHILDREN = (int)0x02000000L,
        WS_CLIPSIBLINGS = (int)0x04000000L,
        WS_DISABLED = (int)0x08000000L,
        WS_DLGFRAME = (int)0x00400000L,
        WS_GROUP = (int)0x00020000L,
        WS_HSCROLL = (int)0x00100000L,
        WS_ICONIC = (int)0x20000000L,
        WS_MAXIMIZE = (int)0x01000000L,
        WS_MAXIMIZEBOX = (int)0x00010000L,
        WS_MINIMIZE = (int)0x20000000L,
        WS_MINIMIZEBOX = (int)0x00020000L,
        WS_OVERLAPPED = (int)0x00000000L,
        WS_OVERLAPPEDWINDOW = (int)(WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
        WS_POPUP = unchecked((int)0x80000000L), // Soooooo dumb.
        WS_POPUPWINDOW = (int)(WS_POPUP | WS_BORDER | WS_SYSMENU),
        WS_SIZEBOX = (int)0x00040000L,
        WS_SYSMENU = (int)0x00080000L,
        WS_TABSTOP = (int)0x00010000L,
        WS_THICKFRAME = (int)0x00040000L,
        WS_TILED = (int)0x00000000L,
        WS_TILEDWINDOW = (int)(WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
        WS_VISIBLE = (int)0x10000000L,
        WS_VSCROLL = (int)0x00200000L
    }

    public enum ExtWindowStyles
    {
        WS_EX_ACCEPTFILES = (int)0x00000010L,
        WS_EX_APPWINDOW = (int)0x00040000L,
        WS_EX_CLIENTEDGE = (int)0x00000200L,
        WS_EX_COMPOSITED = (int)0x02000000L,
        WS_EX_CONTEXTHELP = (int)0x00000400L,
        WS_EX_CONTROLPARENT = (int)0x00010000L,
        WS_EX_DLGMODALFRAME = (int)0x00000001L,
        WS_EX_LAYERED = (int)0x00080000,
        WS_EX_LAYOUTRTL = (int)0x00400000L,
        WS_EX_LEFT = (int)0x00000000L,
        WS_EX_LEFTSCROLLBAR = (int)0x00004000L,
        WS_EX_LTRREADING = (int)0x00000000L,
        WS_EX_MDICHILD = (int)0x00000040L,
        WS_EX_NOACTIVATE = (int)0x08000000L,
        WS_EX_NOINHERITLAYOUT = (int)0x00100000L,
        WS_EX_NOPARENTNOTIFY = (int)0x00000004L,
        WS_EX_NOREDIRECTIONBITMAP = (int)0x00200000L,
        WS_EX_OVERLAPPEDWINDOW = (int)(WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
        WS_EX_PALETTEWINDOW = (int)(WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
        WS_EX_RIGHT = (int)0x00001000L,
        WS_EX_RIGHTSCROLLBAR = (int)0x00000000L,
        WS_EX_RTLREADING = (int)0x00002000L,
        WS_EX_STATICEDGE = (int)0x00020000L,
        WS_EX_TOOLWINDOW = (int)0x00000080L,
        WS_EX_TOPMOST = (int)0x00000008L,
        WS_EX_TRANSPARENT = (int)0x00000020L,
        WS_EX_WINDOWEDGE = (int)0x00000100L,
    }
}
