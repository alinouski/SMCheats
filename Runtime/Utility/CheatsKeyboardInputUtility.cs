using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SMCheats
{
    internal static class CheatsKeyboardInputUtility
    {
        public static bool HasAnyKeyboardInputThisFrame()
        {
            try
            {
                if (Keyboard.current != null)
                {
                    return Keyboard.current.anyKey.wasPressedThisFrame;
                }
                
                return Input.anyKeyDown;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public static bool IsKeyPressedThisFrame(KeyCode keyCode)
        {
            try
            {
                if (TryMapKeyCodeToInputSystemKey(keyCode, out var key) && Keyboard.current != null)
                {
                    return Keyboard.current[key].wasPressedThisFrame;
                }
                
                return Input.GetKeyDown(keyCode);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private static bool TryMapKeyCodeToInputSystemKey(KeyCode keyCode, out Key key)
        {
            switch (keyCode)
            {
                case KeyCode.None: key = Key.None; return false;
                case KeyCode.Backspace: key = Key.Backspace; return true;
                case KeyCode.Tab: key = Key.Tab; return true;
                case KeyCode.Return: key = Key.Enter; return true;
                case KeyCode.KeypadEnter: key = Key.NumpadEnter; return true;
                case KeyCode.Pause: key = Key.Pause; return true;
                case KeyCode.Escape: key = Key.Escape; return true;
                case KeyCode.Space: key = Key.Space; return true;
                case KeyCode.Quote: key = Key.Quote; return true;
                case KeyCode.Comma: key = Key.Comma; return true;
                case KeyCode.Minus: key = Key.Minus; return true;
                case KeyCode.Period: key = Key.Period; return true;
                case KeyCode.Slash: key = Key.Slash; return true;
                case KeyCode.Alpha0: key = Key.Digit0; return true;
                case KeyCode.Alpha1: key = Key.Digit1; return true;
                case KeyCode.Alpha2: key = Key.Digit2; return true;
                case KeyCode.Alpha3: key = Key.Digit3; return true;
                case KeyCode.Alpha4: key = Key.Digit4; return true;
                case KeyCode.Alpha5: key = Key.Digit5; return true;
                case KeyCode.Alpha6: key = Key.Digit6; return true;
                case KeyCode.Alpha7: key = Key.Digit7; return true;
                case KeyCode.Alpha8: key = Key.Digit8; return true;
                case KeyCode.Alpha9: key = Key.Digit9; return true;
                case KeyCode.Semicolon: key = Key.Semicolon; return true;
                case KeyCode.Equals: key = Key.Equals; return true;
                case KeyCode.LeftBracket: key = Key.LeftBracket; return true;
                case KeyCode.Backslash: key = Key.Backslash; return true;
                case KeyCode.RightBracket: key = Key.RightBracket; return true;
                case KeyCode.BackQuote: key = Key.Backquote; return true;
                case KeyCode.A: key = Key.A; return true;
                case KeyCode.B: key = Key.B; return true;
                case KeyCode.C: key = Key.C; return true;
                case KeyCode.D: key = Key.D; return true;
                case KeyCode.E: key = Key.E; return true;
                case KeyCode.F: key = Key.F; return true;
                case KeyCode.G: key = Key.G; return true;
                case KeyCode.H: key = Key.H; return true;
                case KeyCode.I: key = Key.I; return true;
                case KeyCode.J: key = Key.J; return true;
                case KeyCode.K: key = Key.K; return true;
                case KeyCode.L: key = Key.L; return true;
                case KeyCode.M: key = Key.M; return true;
                case KeyCode.N: key = Key.N; return true;
                case KeyCode.O: key = Key.O; return true;
                case KeyCode.P: key = Key.P; return true;
                case KeyCode.Q: key = Key.Q; return true;
                case KeyCode.R: key = Key.R; return true;
                case KeyCode.S: key = Key.S; return true;
                case KeyCode.T: key = Key.T; return true;
                case KeyCode.U: key = Key.U; return true;
                case KeyCode.V: key = Key.V; return true;
                case KeyCode.W: key = Key.W; return true;
                case KeyCode.X: key = Key.X; return true;
                case KeyCode.Y: key = Key.Y; return true;
                case KeyCode.Z: key = Key.Z; return true;
                case KeyCode.Delete: key = Key.Delete; return true;
                case KeyCode.Keypad0: key = Key.Numpad0; return true;
                case KeyCode.Keypad1: key = Key.Numpad1; return true;
                case KeyCode.Keypad2: key = Key.Numpad2; return true;
                case KeyCode.Keypad3: key = Key.Numpad3; return true;
                case KeyCode.Keypad4: key = Key.Numpad4; return true;
                case KeyCode.Keypad5: key = Key.Numpad5; return true;
                case KeyCode.Keypad6: key = Key.Numpad6; return true;
                case KeyCode.Keypad7: key = Key.Numpad7; return true;
                case KeyCode.Keypad8: key = Key.Numpad8; return true;
                case KeyCode.Keypad9: key = Key.Numpad9; return true;
                case KeyCode.KeypadPeriod: key = Key.NumpadPeriod; return true;
                case KeyCode.KeypadDivide: key = Key.NumpadDivide; return true;
                case KeyCode.KeypadMultiply: key = Key.NumpadMultiply; return true;
                case KeyCode.KeypadMinus: key = Key.NumpadMinus; return true;
                case KeyCode.KeypadPlus: key = Key.NumpadPlus; return true;
                case KeyCode.UpArrow: key = Key.UpArrow; return true;
                case KeyCode.DownArrow: key = Key.DownArrow; return true;
                case KeyCode.RightArrow: key = Key.RightArrow; return true;
                case KeyCode.LeftArrow: key = Key.LeftArrow; return true;
                case KeyCode.Insert: key = Key.Insert; return true;
                case KeyCode.Home: key = Key.Home; return true;
                case KeyCode.End: key = Key.End; return true;
                case KeyCode.PageUp: key = Key.PageUp; return true;
                case KeyCode.PageDown: key = Key.PageDown; return true;
                case KeyCode.F1: key = Key.F1; return true;
                case KeyCode.F2: key = Key.F2; return true;
                case KeyCode.F3: key = Key.F3; return true;
                case KeyCode.F4: key = Key.F4; return true;
                case KeyCode.F5: key = Key.F5; return true;
                case KeyCode.F6: key = Key.F6; return true;
                case KeyCode.F7: key = Key.F7; return true;
                case KeyCode.F8: key = Key.F8; return true;
                case KeyCode.F9: key = Key.F9; return true;
                case KeyCode.F10: key = Key.F10; return true;
                case KeyCode.F11: key = Key.F11; return true;
                case KeyCode.F12: key = Key.F12; return true;
                case KeyCode.F13: key = Key.F13; return true;
                case KeyCode.F14: key = Key.F14; return true;
                case KeyCode.F15: key = Key.F15; return true;
                case KeyCode.Numlock: key = Key.NumLock; return true;
                case KeyCode.CapsLock: key = Key.CapsLock; return true;
                case KeyCode.ScrollLock: key = Key.ScrollLock; return true;
                case KeyCode.RightShift: key = Key.RightShift; return true;
                case KeyCode.LeftShift: key = Key.LeftShift; return true;
                case KeyCode.RightControl: key = Key.RightCtrl; return true;
                case KeyCode.LeftControl: key = Key.LeftCtrl; return true;
                case KeyCode.RightAlt: key = Key.RightAlt; return true;
                case KeyCode.LeftAlt: key = Key.LeftAlt; return true;
                case KeyCode.LeftCommand: key = Key.LeftMeta; return true;
                case KeyCode.RightCommand: key = Key.RightMeta; return true;
                case KeyCode.Menu: key = Key.ContextMenu; return true;
                default:
                    key = Key.None;
                    return false;
            }
        }
    }
}
