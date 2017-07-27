using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class WindowController : MonoBehaviour
{
    public bool minimizeOnStart = false;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR

    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
    static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    private const int GWL_EXSTYLE = -0x14;
    private const int WS_EX_TOOLWINDOW = 0x0080;

    private IntPtr winHandle;

    // Use this for initialization
    void Start()
    {
        // Find the application's Window
        winHandle = FindWindowByCaption(IntPtr.Zero, Application.productName);

        if (winHandle == IntPtr.Zero) 
            return;
        else if(minimizeOnStart)
            MinimizeUnityWindow();
    }

#else
    private IntPtr winHandle;
    private static bool ShowWindow(IntPtr hWnd, uint nCmdShow)
    {
        return true;
    }
    void Start()
    {
        winHandle = IntPtr.Zero;
    }

#endif

    public bool HideUnityWindow()
    {
        bool res = ShowWindow(winHandle, (uint) 0);
        Debug.Log("Try Hide Window: " + res);
        return res;
    }

    public bool ShowUnityWindow()
    {
        bool res = ShowWindow(winHandle, (uint) 5);
        Debug.Log("Try Show Window: " + res);
        return res;
    }

    public bool MinimizeUnityWindow()
    {
        bool res = ShowWindow(winHandle, (uint) 6);
        return res;
    }
}