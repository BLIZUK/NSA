using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;


namespace track_widths.Core.Behavior
{
    public class PlaceholderBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += OnGotFocus;
            AssociatedObject.LostFocus += OnLostFocus;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;


            if (string.IsNullOrEmpty(AssociatedObject.Text))
            {
                AssociatedObject.Text = "*";
            }
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.LostFocus -= OnLostFocus;
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        }


        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.Text == "*")
            {
                AssociatedObject.Text = string.Empty;
            }
        }


        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AssociatedObject.Text))
            {
                AssociatedObject.Text = "*";
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (AssociatedObject.Text == "*" && e.Key != Key.Tab)
            {
                AssociatedObject.Text = string.Empty;
            }
        }
    }
}