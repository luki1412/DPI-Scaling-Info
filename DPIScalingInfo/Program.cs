using System;
using System.Runtime.InteropServices;

namespace DPIGetTest
{
    static class Program
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        public enum DeviceCap
        {
            HORZRES = 8,
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            DESKTOPHORZRES = 118,
        }

        static double GetWindowsScreenScalingFactor(IntPtr DeviceContextHandle, bool percentage = true, bool horiz = false)
        {
            //Divide the Screen Heights to get the scaling factor and round it to two decimals
            double ScreenScalingFactor;
            if (!horiz)
            {
                int LogicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.VERTRES);
                int PhysicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.DESKTOPVERTRES);
                Console.WriteLine("Logical Screen Height: " + LogicalScreenHeight);
                Console.WriteLine("Physical Screen Height: " + PhysicalScreenHeight);
                ScreenScalingFactor = Math.Round((double)PhysicalScreenHeight / (double)LogicalScreenHeight, 2);
            }
            else
            {
                int LogicalScreenWidth = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.HORZRES);
                int PhysicalScreenWidth = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.DESKTOPHORZRES);
                Console.WriteLine("Logical Screen Width: " + LogicalScreenWidth);
                Console.WriteLine("Physical Screen Width: " + PhysicalScreenWidth);
                ScreenScalingFactor = Math.Round((double)PhysicalScreenWidth / (double)LogicalScreenWidth, 2);
            }
            //If requested as percentage - convert it
            if (percentage)
            {
                ScreenScalingFactor *= 100.0;
            }

            //Return the Scaling Factor
            return ScreenScalingFactor;
        }

        static int Main(string[] arguments)
        {
            try
            {
                bool nowait = false, help = false;
                if (arguments.Length > 0)
                {
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        switch (arguments[i])
                        {
                            case "-nw":
                                {
                                    nowait = true;
                                    break;
                                }
                            case "/nw":
                                {
                                    nowait = true;
                                    break;
                                }
                            case "/?":
                                {
                                    help = true;
                                    break;
                                }
                            case "-?":
                                {
                                    help = true;
                                    break;
                                }
                        }
                    }
                }
                
                if (help == true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("DPI Scaling Info");
                    Console.ResetColor();
                    Console.WriteLine("Version: 1.0");
                    Console.WriteLine("Author: Lukas Ceremeta (luki1412)");
                    Console.WriteLine("Description: This console application returns DPI scaling and resolution for the current monitor. If successful, returns results and exit code 0.If there is an error, returns an error message and exit code 1.");
                    Console.WriteLine("-------------");
                    Console.WriteLine("Parameters:");
                    Console.WriteLine("-nw /nw  No waiting. Returns output and terminates.");
                    Console.WriteLine("-? /?  Returns this help. Skips dpi testing.");
                }
                else
                {
                    IntPtr DeviceContextHandle = GetDC(IntPtr.Zero);
                    Console.WriteLine("Horizontal DPI scaling: " + GetWindowsScreenScalingFactor(DeviceContextHandle, true, true).ToString() + " %");
                    Console.WriteLine();
                    Console.WriteLine("Vertical DPI scaling: " + GetWindowsScreenScalingFactor(DeviceContextHandle).ToString() + " %");
                    ReleaseDC(IntPtr.Zero, DeviceContextHandle);
                }

                if (nowait == false)
                {
                    Console.ReadLine();
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return 1;
            }
        }
    }
}
