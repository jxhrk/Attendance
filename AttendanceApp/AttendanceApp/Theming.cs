using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;



namespace AttendanceApp
{
    

    public static class Theming
    {
        [DllImport("dwmapi.dll", PreserveSig = true)]

        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref bool attrValue, int attrSize);

        private static void EnableTheme(Window window)
        {
            var value = true;

            IntPtr handle = new WindowInteropHelper(window).Handle;

            DwmSetWindowAttribute(handle, 20, ref value, Marshal.SizeOf(value));
        }



        /// <summary>
        /// Set the titlebar theme whenever Windows system-wide application theme is changed.
        /// </summary>
        /// <param name="window">The WPF window for which the titlebar theme is set.</param>
        public static void InitTitlebarTheme(this Window window)
        {
            ArgumentNullException.ThrowIfNull(window);

            //HwndSource? hwndSource = null;

            void sourceInitializedHandler(object? sender, EventArgs e)
            {
                window.SourceInitialized -= sourceInitializedHandler;
                EnableTheme(window);
            }

            void closedHandler(object? sender, EventArgs e)
            {
                window.SourceInitialized -= sourceInitializedHandler;
                window.Closed -= closedHandler;
            }

            if (new WindowInteropHelper(window).Handle == IntPtr.Zero)
            {
                window.SourceInitialized -= sourceInitializedHandler;
                window.SourceInitialized += sourceInitializedHandler;
            }
            else
            {
                sourceInitializedHandler(null, EventArgs.Empty);
            }

            window.Closed -= closedHandler;
            window.Closed += closedHandler;
        }
    }
}
