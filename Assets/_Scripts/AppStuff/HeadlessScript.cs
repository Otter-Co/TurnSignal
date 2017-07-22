using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class HeadlessScript : MonoBehaviour
{
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

    // Use this for initialization
    void Start()
    {
        // Find the application's Window
        var handle = FindWindowByCaption(IntPtr.Zero, Application.productName);
        if (handle == IntPtr.Zero) return;

        // Move the Window Off Screen
        SetWindowPos(handle, 0, -720, 0, 720, 480, 0);

        // Remove the Window from the Taskbar
        ShowWindow(handle, (uint)0);
        SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_TOOLWINDOW);
        ShowWindow(handle, (uint)5);
    }
#endif
}