using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace MouseGlobeHook
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 掛勾指標
        /// </summary>
        private IntPtr hookID = IntPtr.Zero;
        private IntPtr keyboardHookID = IntPtr.Zero;

        #region Window API

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        #region API結構定義
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public  struct POINT
        {
            public int x;
            public int y;
        }
        #endregion
        //CallBack系統級別的鍵盤和滑鼠事件
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc keyboardProc;
        
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc mouseProc;

        private IntPtr SetKeyboardHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        public Form1()
        {
            InitializeComponent();
            mouseProc = HookCallback;
            hookID = SetHook(mouseProc);

            keyboardProc = KeyboardHookCallback;
            keyboardHookID = SetKeyboardHook(keyboardProc);
        }
        private bool isCtrlPressed = false;
        private bool isShiftPressed = false;
        private bool isAltPressed = false;
        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

                if (wParam == (IntPtr)0x0100) // WM_KEYDOWN
                {
                    // 記錄修飾鍵的按下狀態
                    if (hookStruct.vkCode == (int)Keys.LControlKey || hookStruct.vkCode == (int)Keys.RControlKey)
                    {
                        isCtrlPressed = true;
                    }
                    else if (hookStruct.vkCode == (int)Keys.ShiftKey)
                    {
                        isShiftPressed = true;
                    }
                    else if (hookStruct.vkCode == (int)Keys.Menu) // Alt key
                    {
                        isAltPressed = true;
                    }
                    else
                    {
                        // 檢測到非修飾鍵時輸出複合鍵
                        string combo = "";

                        if (isCtrlPressed)
                        {
                            combo += "Ctrl+";
                        }
                        if (isShiftPressed)
                        {
                            combo += "Shift+";
                        }
                        if (isAltPressed)
                        {
                            combo += "Alt+";
                        }

                        // 輸出完整的複合鍵名稱
                        combo += ((Keys)hookStruct.vkCode).ToString();
                        Debug.WriteLine($"Key combination pressed: {combo}");
                    }
                }
                else if (wParam == (IntPtr)0x0101) // WM_KEYUP
                {
                    // 重置修飾鍵的狀態
                    if (hookStruct.vkCode == (int)Keys.LControlKey || hookStruct.vkCode == (int)Keys.RControlKey)
                    {
                        isCtrlPressed = false;
                    }
                    else if (hookStruct.vkCode == (int)Keys.ShiftKey)
                    {
                        isShiftPressed = false;
                    }
                    else if (hookStruct.vkCode == (int)Keys.Menu) // Alt key
                    {
                        isAltPressed = false;
                    }
                }
            }
            return CallNextHookEx(keyboardHookID, nCode, wParam, lParam);
        }

        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(14, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                // Get mouse location
                int mouseX = hookStruct.pt.x;
                int mouseY = hookStruct.pt.y;
                // Handle mouse events here (e.g., WM_LBUTTONDOWN, WM_MOUSEMOVE)
                Debug.WriteLine($"Mouse event detected at location: X={mouseX}, Y={mouseY}");
            }
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            UnhookWindowsHookEx(hookID);
            UnhookWindowsHookEx(keyboardHookID);
        }

        public enum ActionType { KeyPress, MouseMove, MouseClick }
        public struct RecordedAction
        {
            public ActionType ActionType;
            public int KeyCode; // For key presses
            public POINT MousePosition; // For mouse movements and clicks
            public DateTime Timestamp; // To record the time of the action
        }
    }
}
