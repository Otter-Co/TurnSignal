using System;

using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public class Window_Handler : MonoBehaviour
{
    private const string _taskbar_text = "TurnSignal";
    private const string _show_text = "Show Window";
    private const string _hide_text = "Hide Window";
    private const string _quit_text = "Quit TurnSignal";

    [System.Serializable]
    public class TrayEvent : UnityEvent { };

    [Header("Tray Icon Settings")]
    public Texture2D trayIconTexture;
    public bool showTrayOnStart = true;
    public bool hideWindowOnStart = false;

    [Header("Tray Icon Events")]
    public TrayEvent onShowClicked;
    public TrayEvent onExitClicked;

    [HideInInspector]
    public bool TrayVisible { get => (trayIcon != null) ? trayIcon.Visible : true; }

    [HideInInspector]
    public bool WindowVisible { get => (windowO != null) ? windowO.Visible : true; }

    private NewWheelWindow windowO;

    private NotifyIcon trayIcon;
    private Bitmap iconBitmap;
    private Icon icon;

    private ContextMenuStrip contextMenu;
    private ToolStripItem hideShowItem;

    void OnDestroy() => CleanUp();
    void OnApplicationQuit() => CleanUp();

    void CleanUp()
    {
        if (trayIcon != null)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
        }

        if (iconBitmap != null)
        {
            iconBitmap.Dispose();
            icon.Dispose();
        }
    }

    void Start()
    {
        if (onShowClicked == null)
            onShowClicked = new TrayEvent();

        if (onExitClicked == null)
            onExitClicked = new TrayEvent();


#if !UNITY_EDITOR
        windowO = NewWheelWindow.CreateFromWindowFromCaption(UnityEngine.Application.productName);
        TrayIconStart();
#endif

        if (showTrayOnStart)
            ShowTrayIcon();

        if (hideWindowOnStart)
            HideWindow();
    }

    private void TrayIconStart()
    {
        iconBitmap = GetBitmapFromTex(trayIconTexture);
        icon = Icon.FromHandle(iconBitmap.GetHicon());

        trayIcon = new NotifyIcon
        {
            Text = _taskbar_text,
            Icon = icon
        };

        contextMenu = new System.Windows.Forms.ContextMenuStrip();
        trayIcon.ContextMenuStrip = contextMenu;

        hideShowItem = contextMenu.Items.Add(_hide_text, null, (object o, EventArgs e) => onShowClicked.Invoke());
        contextMenu.Items.Add(_quit_text, null, (object o, EventArgs e) => onExitClicked.Invoke());
    }

    public void CloseAndRestartApp()
    {
        string appPath = $"{UnityEngine.Application.dataPath}\\..\\turnsignal.exe";
        UnityEngine.Application.Quit();
        Process.Start(appPath).Dispose();
    }

    public void HideTrayIcon()
    {
        if (trayIcon != null)
            trayIcon.Visible = false;
    }
    public void ShowTrayIcon()
    {
        if (trayIcon != null)
            trayIcon.Visible = true;
    }

    public void HideWindow()
    {
        if (hideShowItem != null)
            hideShowItem.Text = _show_text;

        windowO?.Hide();
    }

    public void ShowWindow()
    {
        if (hideShowItem != null)
            hideShowItem.Text = _hide_text;

        windowO?.Show();
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

public class NewWheelWindow
{
    public bool Visible { get; private set; } = true;

    private IntPtr _hWnd { get; set; } = IntPtr.Zero;

    private readonly int initialExStyle = 0;

    public NewWheelWindow(IntPtr hWnd)
    {
        this._hWnd = hWnd;

        if (hWnd == IntPtr.Zero)
            UnityEngine.Debug.Log("Bad Window Pointer!");
        else
            UnityEngine.Debug.Log("Good Window Pointer: " + hWnd);

        initialExStyle = GetWindowExStyle(hWnd);
    }

    public void Hide()
    {
        NativeMethods.ShowWindow(_hWnd, (int)ShowWindowOptions.Minimize);
        SetWindowExtStyle(_hWnd, WindowExtStyles.ToolWindow);
        Visible = false;
    }

    public void Show()
    {
        SetWindowExtStyle(_hWnd, initialExStyle);
        NativeMethods.ShowWindow(_hWnd, (int)ShowWindowOptions.Restore);
        Visible = true;
    }

    public static NewWheelWindow CreateFromWindowFromCaption(string caption) => new NewWheelWindow(NativeMethods.FindWindowByCaption(IntPtr.Zero, caption));

    public static int SetWindowExtStyle(IntPtr hWnd, int newStyleValue) => NativeMethods.SetWindowLong(hWnd, -20, newStyleValue);
    public static int SetWindowExtStyle(IntPtr hWnd, WindowExtStyles newStyleValue) => NativeMethods.SetWindowLong(hWnd, -20, (int)newStyleValue);
    public static int GetWindowExStyle(IntPtr hWnd) => NativeMethods.GetWindowLong(hWnd, -20);

    #region Externs
    static class NativeMethods
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }

    public enum ShowWindowOptions
    {
        Hide = 0,
        Maximize = 3,
        Show = 5,
        Minimize = 6,
        Restore = 9,
    }

    public enum WindowStyles
    {
        Unset = 0,
        DLGFRAME = (int)0x00400000L,
    }
    public enum WindowExtStyles
    {
        ToolWindow = (int)0x00000080L,
    }
    #endregion
}