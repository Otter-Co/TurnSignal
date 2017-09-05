using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowController : MonoBehaviour
{

    public bool hideOnStart = false;

    [Space(10)]

    public bool minimizeOnStart = false;
    public bool hideFromTaskbarOnStart = false;


    [Space(10)]

    public bool hasTrayIcon = false;
    public Texture2D trayIconTex;

    [HideInInspector] public bool windowVisible = true;



    private const int GWL_EXSTYLE = -0x14;
    private const int WS_EX_TOOLWINDOW = 0x0080;
    private const int SWP_HIDEWINDOW = 0x0080;

    private IntPtr winHandle;

    private int oldWinStyle = 0;


#if UNITY_STANDALONE_WIN && !UNITY_EDITOR

    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)] static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")] private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
    [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32.dll", SetLastError = true)] static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    void Start()
    {
        winHandle = FindWindowByCaption(IntPtr.Zero, Application.productName);
    }

#else
    static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong) { return 0; }
    static int GetWindowLong(IntPtr hWnd, int nIndex) { return 0; }
    private static bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags) { return true; }
    private static bool ShowWindow(IntPtr hWnd, uint nCmdShow) { return true; }

    void Start()
    {
        winHandle = IntPtr.Zero;
        oldWinStyle = GetWindowLong(winHandle, GWL_EXSTYLE);
    }

#endif

    public void StartWin() 
    {
        if(hideOnStart)
        {

        }
        else 
        {
            if(hideFromTaskbarOnStart)
                HideTaskbarIcon();

            if(minimizeOnStart)
                MinimizeUnityWindow();
        }
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