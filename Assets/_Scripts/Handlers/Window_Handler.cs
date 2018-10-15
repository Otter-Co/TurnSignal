using System;
using System.Diagnostics;
using UnityEngine;
using System.Runtime.InteropServices;

public class Window_Handler : MonoBehaviour
{
    [Header("Window Settings")]
    public bool hideWindowOnStart = false;
    public bool forceWindowSize = false;
    [Space(10)]
    public int targetWindowWidth = 1280;
    public int targetWindowHeight = 720;

    public bool WindowVisible { get => NewWheelWindow.Visible; }

    void Start()
    {
        if (forceWindowSize)
            SetWindowSize();

        if (hideWindowOnStart)
            HideWindow();
    }

    public void CloseAndRestartApp()
    {
        if (!UnityEngine.Application.isEditor)
        {
            string appPath = $"{UnityEngine.Application.dataPath}\\..\\turnsignal.exe";
            UnityEngine.Application.Quit();
            Process.Start(appPath).Dispose();
        }
    }

    public void HideWindow() => NewWheelWindow.HideWindow();
    public void ShowWindow() => NewWheelWindow.ShowWindow();

    public void SetWindowSize()
    {
        int loops = 0;
        while (loops++ < 5 && Screen.width != targetWindowWidth || Screen.height != targetWindowHeight)
            Screen.SetResolution(targetWindowWidth, targetWindowHeight, false);
    }

    #region Window Handling Stuff
    public static class NewWheelWindow
    {
        public static bool Visible { get; private set; } = true;
        private static IntPtr _hWnd { get; set; } = IntPtr.Zero;
        private static int initialExStyle = 0;

        static NewWheelWindow()
        {
            _hWnd = NativeMethods.FindWindowByCaption(IntPtr.Zero, UnityEngine.Application.productName);

            if (_hWnd == IntPtr.Zero)
                UnityEngine.Debug.Log("Window Pointer is IntPtr.Zero!");
            else
            {
                UnityEngine.Debug.Log("Window Pointer is not IntPtr.Zero: " + _hWnd);
                initialExStyle = GetWindowExStyle(_hWnd);
            }
        }

        public static void HideWindow()
        {
            if (_hWnd == IntPtr.Zero)
                return;

            NativeMethods.ShowWindow(_hWnd, (int)ShowWindowOptions.Minimize);
            SetWindowExtStyle(_hWnd, WindowExtStyles.ToolWindow);
            Visible = false;
        }

        public static void ShowWindow()
        {
            if (_hWnd == IntPtr.Zero)
                return;

            SetWindowExtStyle(_hWnd, initialExStyle);
            NativeMethods.ShowWindow(_hWnd, (int)ShowWindowOptions.Restore);
            Visible = true;
        }


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

    #endregion
}


