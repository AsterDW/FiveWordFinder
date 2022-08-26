using FiveWordFinderWpf.Interop;
using FiveWordFinderWpf.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FiveWordFinderWpf.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.IsActive = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;
            HwndSource windowSource = HwndSource.FromHwnd(windowHandle);
            windowSource.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr result = IntPtr.Zero;
            if ((WinUserAPI.WinMessages)msg == WinUserAPI.WinMessages.WM_NCHITTEST)
            {
                int x = (short)(long)lParam;
                int y = (short)((long)lParam >> 16);
                var screenPoint = new Point(x, y);
                var mPos = PointFromScreen(screenPoint);

                result = (IntPtr)WndHitTest(mPos);
                handled = result != IntPtr.Zero;
            }

            return result;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
            
            if (p.Y <= headerRow.ActualHeight)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    DragMove();
            }
        }

        #region Click events
        protected void MinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected void RestoreClick(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        protected void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


        #endregion

        #region Window Sizing
        private const int SizeBorderThickness = 5;

        [Flags]
        private enum WindowCardinals
        {
            None = 0,
            Left = 1 << 0,
            Top = 1 << 1,
            Right = 1 << 2,
            Bottom = 1 << 3
        }

        private List<WindowCardinals> _testList = new List<WindowCardinals>() { WindowCardinals.Left, WindowCardinals.Top, WindowCardinals.Right, WindowCardinals.Bottom };
        private Dictionary<WindowCardinals, WinUserAPI.HitTest> _cardinalHitTestMap = new Dictionary<WindowCardinals, WinUserAPI.HitTest>()
        {
            { WindowCardinals.None, WinUserAPI.HitTest.HTNOWHERE },
            { WindowCardinals.Left, WinUserAPI.HitTest.HTLEFT },
            { WindowCardinals.Top, WinUserAPI.HitTest.HTTOP },
            { WindowCardinals.Right, WinUserAPI.HitTest.HTRIGHT },
            { WindowCardinals.Bottom, WinUserAPI.HitTest.HTBOTTOM },
            { WindowCardinals.Top | WindowCardinals.Left, WinUserAPI.HitTest.HTTOPLEFT },
            { WindowCardinals.Top | WindowCardinals.Right, WinUserAPI.HitTest.HTTOPRIGHT },
            { WindowCardinals.Right | WindowCardinals.Bottom, WinUserAPI.HitTest.HTBOTTOMRIGHT },
            { WindowCardinals.Left | WindowCardinals.Bottom, WinUserAPI.HitTest.HTBOTTOMLEFT }
        };

        private WinUserAPI.HitTest WndHitTest(Point position)
        {
            WindowCardinals activeCardinals = WindowCardinals.None;
            WinUserAPI.HitTest result;

            if (!MouseInSizingrea(position))
            {
                return _cardinalHitTestMap[activeCardinals];
            }

            foreach (var cardinal in _testList)
            {
                if (GetRect(cardinal).Contains(position))
                    activeCardinals |= cardinal;
            }

            if (!_cardinalHitTestMap.TryGetValue(activeCardinals, out result))
                result = WinUserAPI.HitTest.HTNOWHERE;

            return result;
        }

        private Rect TopRect()
        {
            return new Rect(0, 0, Width, SizeBorderThickness);
        }
        private Rect RightRect()
        {
            return new Rect(Width - SizeBorderThickness, 0, SizeBorderThickness, Height);
        }
        private Rect BottomRect()
        {
            return new Rect(0, Height - SizeBorderThickness, Width, SizeBorderThickness);
        }
        private Rect LeftRect()
        {
            return new Rect(0, 0, SizeBorderThickness, Height);
        }

        private Rect GetRect(WindowCardinals cardinal)
        {
            switch(cardinal)
            {
                case WindowCardinals.Left:
                    return LeftRect();
                case WindowCardinals.Top:
                    return TopRect();
                case WindowCardinals.Right:
                    return RightRect();
                case WindowCardinals.Bottom:
                    return BottomRect();
                default:
                    return new Rect(0, 0, 0, 0);
            }
        }

        private bool MouseInSizingrea(Point position)
        {
            var nonSizeableArea=new Rect(SizeBorderThickness,
                                         SizeBorderThickness,
                                         Width - (SizeBorderThickness * 2),
                                         Height - (SizeBorderThickness * 2));
            return !nonSizeableArea.Contains(position);
        }

        #endregion Window Sizing
    }
}
