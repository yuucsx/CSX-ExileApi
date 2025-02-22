using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ExileCore;
using ExileCore.Shared;

namespace CSX
{
    public static class Keyboard
    {
        private const string CoroutineKeyPress = "KeyPress";
        private static Coroutine _keyboardCoroutine;

        private const int KeyeventfExtendedkey = 0x0001;
        private const int KeyeventfKeyup = 0x0002;

        [DllImport("user32.dll")]
        private static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void KeyDown(Keys key)
        {
            keybd_event((byte)key, 0, KeyeventfExtendedkey | 0, 0);
        }

        public static void KeyUp(Keys key)
        {
            keybd_event((byte)key, 0, KeyeventfExtendedkey | KeyeventfKeyup, 0);
        }

        public static void KeyPress(Keys key)
        {
            _keyboardCoroutine = new Coroutine(KeyPressRoutine(key), null, CoroutineKeyPress);
            Core.ParallelRunner.Run(_keyboardCoroutine);
        }

        private static IEnumerator KeyPressRoutine(Keys key)
        {
            KeyDown(key);
            yield return new WaitTime(20);
            KeyUp(key);
        }

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        public static bool IsKeyDown(int nVirtKey)
        {
            return GetKeyState(nVirtKey) < 0;
        }
    }
}