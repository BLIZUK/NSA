using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Xaml.Behaviors;


namespace track_widths.Core.Behavior
{
    public class NumericInputBehavior : Behavior<TextBox>
    {
        private static readonly Regex _regex = new Regex(@"^-?\d*[.,]?\d*$");

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            DataObject.AddPastingHandler(AssociatedObject, OnPasting);
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            DataObject.RemovePastingHandler(AssociatedObject, OnPasting);
        }


        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
            bool allowNegative = textBox.Name == "ambientTempTextBox";

            e.Handled = !IsValid(newText, allowNegative);
        }


        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = (TextBox)sender;
            bool allowNegative = textBox.Name == "ambientTempTextBox";

            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                if (!IsValid(text, allowNegative))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }


        private bool IsValid(string text, bool allowNegative)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            if (allowNegative && text.StartsWith("-"))
            {
                string numberPart = text.Substring(1);
                return _regex.IsMatch(numberPart);
            }

            return _regex.IsMatch(text);
        }
    }
}