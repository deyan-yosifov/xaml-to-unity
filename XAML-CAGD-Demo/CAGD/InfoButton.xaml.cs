using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CAGD
{
    /// <summary>
    /// Interaction logic for InfoButton.xaml
    /// </summary>
    public partial class InfoButton : UserControl
    {
        public InfoButton()
        {
            InitializeComponent();
        }

        private UIElement GetTooltipContent()
        {
            Border border = new Border();
            border.CornerRadius = new CornerRadius(10);
            border.Margin = new Thickness(0, 0, 5, 0);
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = new SolidColorBrush(Colors.Blue);
            border.Background = new SolidColorBrush(Colors.White);
            border.Padding = new Thickness(10);

            TextBlock textBlock = new TextBlock();
            textBlock.Text = @"You may do the following actions on the viewport:
 - ORBIT => Drag with mouse left or rigth button down.
 - PAN => Drag with middle mouse button down.
 - ZOOM => Scroll with the mouse wheel.
 - ZOOM TO CONTENTS => Double click with middle mouse button.
 - MOVE BEZIER CONTROL POINT => Drag some control point with mouse left or right button down.";
            border.Child = textBlock;

            return border;
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button button = (Button)sender;
            button.CaptureMouse();

            ToolTip tooltip = (button.ToolTip as ToolTip) ?? new ToolTip();
            tooltip.Background = null;
            tooltip.BorderBrush = null;
            tooltip.BorderThickness = new Thickness(0);
            tooltip.Content = this.GetTooltipContent();
            tooltip.PlacementTarget = button;
            tooltip.Placement = System.Windows.Controls.Primitives.PlacementMode.Left;
            button.ToolTip = tooltip;
            tooltip.IsOpen = true;
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Button button = (Button)sender;
            button.ReleaseMouseCapture();

            ToolTip tooltip = (button.ToolTip as ToolTip);

            if (tooltip != null)
            {
                tooltip.IsOpen = false;
                button.ToolTip = null;
            }
        }
    }
}
