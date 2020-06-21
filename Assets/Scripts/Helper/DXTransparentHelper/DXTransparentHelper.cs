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
            /// <summary>
            /// 置顶
            /// </summary>
            WinTop,
            /// <summary>
            /// 置顶并且透明
            /// </summary>
            WinTopApha,
            /// <summary>
            /// 置顶透明并且可以穿透
            /// </summary>
            WinTopAphaPenetrate
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

        private const int WS_POPUP = 0x800000;
        private const int GWL_EXSTYLE = -20;
        private const int GWL_STYLE = -16;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_BORDER = 0x00800000;
        private const int WS_CAPTION = 0x00C00000;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int LWA_COLORKEY = 0x00000001;
        private const int LWA_ALPHA = 0x00000002;
        private const int WS_EX_TRANSPARENT = 0x20;

        private const int ULW_COLORKEY = 0x00000001;
        private const int ULW_ALPHA = 0x00000002;
        private const int ULW_OPAQUE = 0x00000004;
        private const int ULW_EX_NORESIZE = 0x00000008;
        #endregion

        private bool isApha;//是否透明
        private bool isAphaPenetrate;//是否要穿透窗体

        public void SetTransparent(string windowName, enumWinStyle WinStyle) 
        {
            switch (WinStyle)
            {
                case enumWinStyle.WinTop:
                    isApha = false;
                    isAphaPenetrate = false;
                    break;
                case enumWinStyle.WinTopApha:
                    isApha = true;
                    isAphaPenetrate = false;
                    break;
                case enumWinStyle.WinTopAphaPenetrate:
                    isApha = true;
                    isAphaPenetrate = true;
                    break;
            }

            IntPtr hwnd = FindWindow(null, windowName);
            if (hwnd != null)
            {
                ShowWindow(hwnd, 5);
                if (isApha)
                {
                    //去边框并且透明
                    SetWindowLong(hwnd, GWL_STYLE, WS_POPUP);
                    int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE);
                    if (isAphaPenetrate)//是否透明穿透窗体
                    {
                        SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT | WS_EX_LAYERED);
                    }
                    //
                    SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_BORDER & ~WS_CAPTION);
                    SetWindowPos(hwnd, -1, 0, 0, dxInterface.renderTextureWidth, dxInterface.renderTextureHeight, SWP_SHOWWINDOW);
                    var margins = new MARGINS() { cxLeftWidth = -1 };
                    //
                    DwmExtendFrameIntoClientArea(hwnd, ref margins);
                }
                else
                {
                    //单纯去边框
                    SetWindowLong(hwnd, GWL_STYLE, WS_POPUP);
                    SetWindowPos(hwnd, -1, 0, 0, dxInterface.renderTextureWidth, dxInterface.renderTextureHeight, SWP_SHOWWINDOW);
                }
            }
        }

        public void SetMinimized(string windowName)
        {
            IntPtr hwnd = FindWindow(null, windowName);
            if (hwnd != null)
            {
                ShowWindow(hwnd, 0);
            }
        }
    }
}