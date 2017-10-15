using System;
using System.Runtime.InteropServices;

using System.IO;
using System.Drawing;
using System.Windows.Forms;

using UnityEngine;

public class WindowController : MonoBehaviour
{
    public bool hasTrayIcon = false;
    public Texture2D trayIconTex;

    [HideInInspector] public bool windowVisible = true;


    private TurnSignal_Director director;

    private const int GWL_EXSTYLE = -0x14;
    private const int WS_EX_TOOLWINDOW = 0x0080;
    private const int SWP_HIDEWINDOW = 0x0080;

    private IntPtr winHandle;
    private int oldWinStyle = 0;

    private TurnSignalTrayForm trayForm;


#if UNITY_STANDALONE_WIN && !UNITY_EDITOR

    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)] static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")] private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
    [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32.dll", SetLastError = true)] static extern int GetWindowLong(IntPtr hWnd, int nIndex);

#else

    static IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName) { return IntPtr.Zero; }
    static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong) { return 0; }
    static int GetWindowLong(IntPtr hWnd, int nIndex) { return 0; }
    private static bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags) { return true; }
    private static bool ShowWindow(IntPtr hWnd, uint nCmdShow) { return true; }

#endif
    void Start()
    {
        director = GetComponent<TurnSignal_Director>();

        winHandle = FindWindowByCaption(IntPtr.Zero, UnityEngine.Application.productName);

        if(hasTrayIcon)
            CreateTray();
    }

    void OnEnable()
    {
        if(hasTrayIcon)
            CreateTray();
    }

    void OnDestroy() 
    {
        DestroyTray();
    }

    void OnApplicationQuit()
    {
        DestroyTray();
    }

    void OnDisable()
    {
        DestroyTray();
    }

    public void CreateTray()
    {
#if !UNITY_EDITOR
        if(trayForm == null)
        {
            trayForm = new TurnSignalTrayForm(trayIconTex); //CreateIcon(trayIconTex));

            trayForm.onExitCallback += OnExit;
            trayForm.onShowWindow += OnShowWindow;
        }
#endif
    }

    public void DestroyTray()
    {
        if(trayForm != null)
        {
            trayForm.Dispose();
            trayForm = null;
        }
    }

    public void OnExit() 
    {
        UnityEngine.Application.Quit();
    }

    public void OnShowWindow()
    {
        director.OnShowWindow();
    }

    public void ShowTrayIcon()
    {
        if(trayForm != null)
            trayForm.ShowTray();
    }

    public void HideTrayIcon()
    {
        if(trayForm != null)
            trayForm.HideTray();
    }

    public bool HideTaskbarIcon() 
    {
        bool res = false;

        res = ShowWindow(winHandle, (uint)0);
        SetWindowLong(winHandle, GWL_EXSTYLE, GetWindowLong(winHandle, GWL_EXSTYLE) | WS_EX_TOOLWINDOW);
        res = ShowWindow(winHandle, (uint)5);

        return res;
    }

    public bool ShowTaskbarIcon()
    {
        bool res = false;

        res = ShowWindow(winHandle, (uint) 0);
        SetWindowLong(winHandle, GWL_EXSTYLE, oldWinStyle);
        res = ShowWindow(winHandle, (uint) 5);

        return res;
    }

    public bool MinimizeUnityWindow()
    {
        bool res = ShowWindow(winHandle, (uint) 6);
        return res;
    }

    public bool RestoreUnityWindow() 
    {
        bool res = ShowWindow(winHandle, (uint) 1);
        return res;
    }

    public bool HideUnityWindow()
    {
        Debug.Log("Attempting to Hide Window!");

        if(!windowVisible)
            return true;

        bool res = false;
        
        res = MinimizeUnityWindow();
        res = HideTaskbarIcon();

        windowVisible = false;

        return res;
    }

    public bool ShowUnityWindow()
    {
        Debug.Log("Attempting to Show Window!");

        if(windowVisible)
            return true;
            
        bool res = false;

        res = RestoreUnityWindow();
        res = ShowTaskbarIcon();
        
        windowVisible = true;

        return res;
    }    
}

public class TurnSignalTrayForm : System.Windows.Forms.Form 
{
    public delegate void OnExitDel();
    public delegate void OnShowWindowDel();

    public OnExitDel onExitCallback;
    public OnShowWindowDel onShowWindow;

    private System.Windows.Forms.ContextMenuStrip trayMenu;
    private NotifyIcon trayIcon;

    public TurnSignalTrayForm(Texture2D tex = null)
    {
        trayMenu = new System.Windows.Forms.ContextMenuStrip();
        
        if(tex)
            trayMenu.Items.Add("Show TurnSignal", CreateBitmap(tex), OnShowWindow);
        else
            trayMenu.Items.Add("Show TurnSignal", CreateBitmap(Texture2D.whiteTexture), OnShowWindow);

        trayMenu.Items.Add("Exit", CreateBitmap(Texture2D.whiteTexture), OnExit);

        trayIcon = new NotifyIcon();
        trayIcon.Text = "TurnSignal";

        trayIcon.ContextMenuStrip = trayMenu;

        if(tex == null)
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
        else
            trayIcon.Icon = Icon.FromHandle(CreateBitmap(tex).GetHicon());
    }

    public Bitmap CreateBitmap(Texture2D tex)
    {
        MemoryStream memS = new MemoryStream(tex.EncodeToPNG());
        memS.Seek(0, System.IO.SeekOrigin.Begin);

        Bitmap bitmap = new Bitmap(memS);
        
        return bitmap;
    }

    protected override void OnLoad(EventArgs e)
    {
        Visible = false;
        ShowInTaskbar = false;

        base.OnLoad(e);
    }

    ~TurnSignalTrayForm()
    {
        Dispose(true);
    }

    public void ShowTray()
    {
        trayIcon.Visible = true;
    }

    public void HideTray()
    {
        trayIcon.Visible = false;
    }

    protected void OnExit(object sender, EventArgs e)
    {
        onExitCallback.Invoke();
    }

    protected void OnShowWindow(object sender, EventArgs e)
    {
        onShowWindow.Invoke();
    }

    protected override void Dispose(bool disposing)
    {
        if(disposing)
            trayIcon.Dispose();

        base.Dispose(disposing);
    }
}