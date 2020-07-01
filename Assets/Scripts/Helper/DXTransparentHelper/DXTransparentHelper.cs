using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.IO;

namespace AMG
{
    public class DXTransparentHelper : MonoBehaviour
    {
        [SerializeField] private DXHelper dxInterface;
        public enum enumWinStyle
        {
            WinTopAphaPenetrate,
            WinNone,
        }
        #region Win函数常量
        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

        [DllImport("Dwmapi.dll")]
        static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        private const int WS_OVERLAPPED = 0;
        private const int WS_POPUP = 0x800000;
        private const int GWL_EXSTYLE = -20;
        private const int GWL_STYLE = -16;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_BORDER = 0x00800000;
        private const int WS_CAPTION = 0x00C00000;
        private const int SWP_NOMOVE = 2;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int LWA_COLORKEY = 0x00000001;
        private const int LWA_ALPHA = 0x00000002;
        private const int WS_EX_TRANSPARENT = 0x20;

        private const int ULW_COLORKEY = 0x00000001;
        private const int ULW_ALPHA = 0x00000002;
        private const int ULW_OPAQUE = 0x00000004;
        private const int ULW_EX_NORESIZE = 0x00000008;
        #endregion

        private bool isNone;//是否透明
        private bool isAlpha;//是否透明
        private bool isAlphaPenetrate;//是否要穿透窗体

        public void SetTransparent(string windowName, enumWinStyle WinStyle) 
        {
            switch (WinStyle)
            {
                case enumWinStyle.WinTopAphaPenetrate:
                    isNone = false;
                    break;
                case enumWinStyle.WinNone:
                    isNone = true;
                    break;
            }



            IntPtr hwnd = FindWindow(null, windowName);
            if (hwnd != null)
            {
                if (isNone)
                {
                    ShowWindow(hwnd, 11);
                    //SetWindowLong(hwnd, GWL_STYLE, WS_POPUP);
                    SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) & ~WS_EX_TRANSPARENT & ~WS_EX_LAYERED);
                    SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) | WS_BORDER | WS_CAPTION);
                }
                else
                {
                    ShowWindow(hwnd, 1);
                    SetWindowLong(hwnd, GWL_STYLE, WS_POPUP);
                    SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_LAYERED);
                    SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_BORDER & ~WS_CAPTION);
                    SetWindowPos(hwnd, -1, 0, 0, dxInterface.renderTextureWidth, dxInterface.renderTextureHeight, SWP_SHOWWINDOW | SWP_NOMOVE);
                    var margins = new MARGINS() { cxLeftWidth = -1 };
                    DwmExtendFrameIntoClientArea(hwnd, ref margins);
                }
            }
        }

        public void SetMinimized(string windowName)
        {
            IntPtr hwnd = FindWindow(null, windowName);
            if (hwnd != null)
            {
                ShowWindow(hwnd, 11);
            }
        }
    }
}