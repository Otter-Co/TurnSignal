using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using System.Drawing;
using System.Windows.Forms;

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
    public bool trayHidden = false;

    [HideInInspector]
    public bool windowHidden = false;


    private Form windowForm;
    private NotifyIcon trayIcon;
    private Bitmap iconBitmap;
    private Icon icon;
    private System.Windows.Forms.ContextMenu contextMenu;
    private MenuItem hideShowItem;
    private MenuItem quitItem;

    private void _onShowClicked(object o, EventArgs e) =>
        onShowClicked.Invoke();
    private void _onQuitClicked(object o, EventArgs e) =>
        onExitClicked.Invoke();


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

        WindowStart();
        TrayIconStart();

        if (showTrayOnStart)
            ShowTrayIcon();

        if (hideWindowOnStart)
            HideWindow();
    }

    private void WindowStart()
    {
        if (UnityEngine.Application.isEditor)
            return;

        windowForm = GetCurrentWindowForm();
    }

    private void TrayIconStart()
    {
        if (UnityEngine.Application.isEditor)
            return;

        iconBitmap = GetBitmapFromTex(trayIconTexture);
        icon = Icon.FromHandle(iconBitmap.GetHicon());

        trayIcon = new NotifyIcon();
        trayIcon.Text = _taskbar_text;
        trayIcon.Icon = icon;

        contextMenu = new System.Windows.Forms.ContextMenu();

        hideShowItem = new MenuItem(_hide_text, _onShowClicked);
        quitItem = new MenuItem(_quit_text, _onQuitClicked);

        contextMenu.MenuItems.Add(hideShowItem);
        contextMenu.MenuItems.Add(quitItem);

        trayIcon.ContextMenu = contextMenu;
    }

    public void CloseAndRestartApp()
    {
        string appPath = System.Reflection.Assembly.GetEntryAssembly().Location;
        var p = Process.Start(appPath);
        UnityEngine.Application.Quit();
    }

    public void HideTrayIcon()
    {
        if (trayIcon == null)
            trayIcon.Visible = false;
    }
    public void ShowTrayIcon()
    {
        if (trayIcon == null)
            return;

        trayIcon.Visible = true;
    }

    public void HideWindow()
    {
        if (windowForm == null)
            return;

        windowForm.Visible = false;
        windowForm.ShowInTaskbar = false;

        hideShowItem.Text = _show_text;
    }

    public void ShowWindow()
    {
        if (windowForm == null)
            return;

        windowForm.ShowInTaskbar = true;
        windowForm.Visible = true;

        hideShowItem.Text = _hide_text;
    }


    public static Form GetCurrentWindowForm() =>
        Control.FromHandle(Process.GetCurrentProcess().MainWindowHandle) as Form;

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