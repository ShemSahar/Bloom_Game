using System.Runtime.InteropServices;
using UnityEngine;

public static class InputSimulator
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    private const int KEYEVENTF_KEYUP = 0x0002;
    private const byte VK_E = 0x45; // Virtual key code for 'E'

    public static void SimulateKeyDown(KeyCode key)
    {
        if (key == KeyCode.E)
        {
            keybd_event(VK_E, 0, 0, 0);
        }
    }

    public static void SimulateKeyUp(KeyCode key)
    {
        if (key == KeyCode.E)
        {
            keybd_event(VK_E, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}
