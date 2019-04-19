using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Vajehyar.Utility
{
    public class KeyboardHook
    {
        private int hHook;
        private Win32Api.HookProc KeyboardHookDelegate;

        public event System.Windows.Forms.KeyEventHandler OnKeyDownEvent;

        public event System.Windows.Forms.KeyEventHandler OnKeyUpEvent;

        public event KeyPressEventHandler OnKeyPressEvent;

        public KeyboardHook()
        {
        }

        public void SetHook()

        {
            KeyboardHookDelegate = new Win32Api.HookProc(KeyboardHookProc);

            Process cProcess = Process.GetCurrentProcess();

            ProcessModule cModule = cProcess.MainModule;

            IntPtr mh = Win32Api.GetModuleHandle(cModule.ModuleName);

            hHook = Win32Api.SetWindowsHookEx(Win32Api.WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);
        }

        public void UnHook()

        {
            Win32Api.UnhookWindowsHookEx(hHook);
        }

        private List<Keys> preKeysList = new List<Keys>();

        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)

        {
            if ((nCode >= 0) && (OnKeyDownEvent != null || OnKeyUpEvent != null || OnKeyPressEvent != null))

            {
                Win32Api.KeyboardHookStruct KeyDataFromHook =
                    (Win32Api.KeyboardHookStruct) Marshal.PtrToStructure(lParam, typeof(Win32Api.KeyboardHookStruct));

                Keys keyData = (Keys) KeyDataFromHook.vkCode;


                if ((OnKeyDownEvent != null || OnKeyPressEvent != null) &&
                    (wParam == Win32Api.WM_KEYDOWN || wParam == Win32Api.WM_SYSKEYDOWN))

                {
                    if (IsCtrlAltShiftKeys(keyData) && preKeysList.IndexOf(keyData) == -1)

                    {
                        preKeysList.Add(keyData);
                    }
                }


                if (OnKeyDownEvent != null && (wParam == Win32Api.WM_KEYDOWN || wParam == Win32Api.WM_SYSKEYDOWN))

                {
                    System.Windows.Forms.KeyEventArgs e = new System.Windows.Forms.KeyEventArgs(GetDownKeys(keyData));


                    OnKeyDownEvent(this, e);
                }


                if (OnKeyPressEvent != null && wParam == Win32Api.WM_KEYDOWN)

                {
                    byte[] keyState = new byte[256];

                    Win32Api.GetKeyboardState(keyState);

                    byte[] inBuffer = new byte[2];

                    if (Win32Api.ToAscii(KeyDataFromHook.vkCode, KeyDataFromHook.scanCode, keyState, inBuffer,
                            KeyDataFromHook.flags) == 1)

                    {
                        KeyPressEventArgs e = new KeyPressEventArgs((char) inBuffer[0]);

                        OnKeyPressEvent(this, e);
                    }
                }


                if ((OnKeyDownEvent != null || OnKeyPressEvent != null) &&
                    (wParam == Win32Api.WM_KEYUP || wParam == Win32Api.WM_SYSKEYUP))

                {
                    if (IsCtrlAltShiftKeys(keyData))

                    {
                        for (int i = preKeysList.Count - 1; i >= 0; i--)

                        {
                            if (preKeysList[i] == keyData)
                            {
                                preKeysList.RemoveAt(i);
                            }
                        }
                    }
                }


                if (OnKeyUpEvent != null && (wParam == Win32Api.WM_KEYUP || wParam == Win32Api.WM_SYSKEYUP))

                {
                    System.Windows.Forms.KeyEventArgs e = new System.Windows.Forms.KeyEventArgs(GetDownKeys(keyData));

                    OnKeyUpEvent(this, e);
                }
            }

            return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
        }


        private Keys GetDownKeys(Keys key)

        {
            Keys rtnKey = Keys.None;

            foreach (Keys i in preKeysList)

            {
                if (i == Keys.LControlKey || i == Keys.RControlKey)
                {
                    rtnKey = rtnKey | Keys.Control;
                }

                if (i == Keys.LMenu || i == Keys.RMenu)
                {
                    rtnKey = rtnKey | Keys.Alt;
                }

                if (i == Keys.LShiftKey || i == Keys.RShiftKey)
                {
                    rtnKey = rtnKey | Keys.Shift;
                }
            }

            return rtnKey | key;
        }

        private bool IsCtrlAltShiftKeys(Keys key)

        {
            if (key == Keys.LControlKey || key == Keys.RControlKey || key == Keys.LMenu || key == Keys.RMenu ||
                key == Keys.LShiftKey || key == Keys.RShiftKey)
            {
                return true;
            }

            return false;
        }
    }
}