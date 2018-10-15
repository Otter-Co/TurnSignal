using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Events;

public class TrayIcon : MonoBehaviour
{
    [System.Serializable]
    public class ItemClicked : UnityEvent { }

    const string _tray_icon_text = "TurnSignal";
    const string _hide_text = "Hide/Show TurnSignal";
    const string _quit_text = "Quit TurnSignal";


    [Header("Tray Icon Settings")]
    public bool createAtStart = true;
    [Space(10)]
    public Texture2D trayIconTexture;
    public bool visibleOnStart = true;

    [Header("Tray Events")]
    public ItemClicked OnHideShowClicked;
    public ItemClicked OnQuitClicked;

    public bool IconVisible { get => trayIcon != null ? trayIcon.Visible : false; }

    private System.ComponentModel.IContainer ctx;
    private NotifyIcon trayIcon;
    private Bitmap iconBitmap;
    private Icon icon;

    private ContextMenuStrip contextMenu;
    private ToolStripItem hideShowItem;

    private void _OnShowHideClicked(object data, EventArgs e) => OnHideShowClicked.Invoke();
    private void _OnQuitClicked(object data, EventArgs e) => OnQuitClicked.Invoke();

    // Use this for initialization
    void Start()
    {
        if (OnHideShowClicked == null)
            OnHideShowClicked = new ItemClicked();

        if (OnQuitClicked == null)
            OnQuitClicked = new ItemClicked();

        if (createAtStart)
            SetupTrayIcon(trayIconTexture);
    }

    void OnApplicationQuit() => CleanUp();
    void OnDestroy() => CleanUp();

    public void SetupTrayIcon(Texture2D trayIconTexture)
    {
        if (UnityEngine.Application.isEditor || ctx != null)
            return;

        if (trayIconTexture == null)
            trayIconTexture = this.trayIconTexture;

        ctx = new System.ComponentModel.Container();

        contextMenu = new System.Windows.Forms.ContextMenuStrip();
        hideShowItem = contextMenu.Items.Add(_hide_text, null, (EventHandler)_OnShowHideClicked);
        contextMenu.Items.Add(_quit_text, null, (EventHandler)_OnQuitClicked);

        iconBitmap = GetBitmapFromTex(trayIconTexture);
        icon = Icon.FromHandle(iconBitmap.GetHicon());

        trayIcon = new NotifyIcon(ctx)
        {
            Icon = icon,
            Text = _tray_icon_text,
            ContextMenuStrip = contextMenu,
            Visible = visibleOnStart,
        };
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

    public void CleanUp()
    {
        if (ctx != null)
            ctx.Dispose();
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
