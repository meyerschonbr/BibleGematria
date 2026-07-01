using BibleGematria.Core;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BibleGematria.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(HebrewInputTextBox, HebrewInputTextBox_Pasting);
            var vm = new MainWindowViewModel();
            DataContext = vm;

            System.Diagnostics.Debug.WriteLine($"DataContext set to {vm.GetType().Name}");
        }
        private void HebrewInputTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = e.Text.Any(ch => !IsAllowedInputCharacter(ch));
        }
        private void HebrewInputTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if(!e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true))
            {
                e.CancelCommand();
                return;
            }
            string pastedText = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string ?? string.Empty;
            string sanitized = new string(pastedText.Where(IsAllowedInputCharacter).ToArray());
            if (sender is not TextBox textBox)
            {
                e.CancelCommand();
                return;
            }
            e.CancelCommand();

            int selectionStart = textBox.SelectionStart;
            string currentText = textBox.Text.Remove(selectionStart, textBox.SelectionLength);
            textBox.Text = currentText.Insert(selectionStart, sanitized);
            textBox.SelectionStart = selectionStart + sanitized.Length;
        }

        private void VerseTextBlock_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RenderVerseText(sender as TextBlock);
        }

        private void RenderVerseText(TextBlock? textBlock)
        {
            if (textBlock == null)
                return;

            if (textBlock.DataContext is not MatchResult match)
                return;

            textBlock.Inlines.Clear();

            string[] parts = System.Text.RegularExpressions.Regex.Split(match.VerseText, @"([ \u05BE-]+)");
            int wordIndex = 0;

            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;

                bool isSeparator = System.Text.RegularExpressions.Regex.IsMatch(part, @"^[ \u05BE-]+$");

                if (isSeparator)
                {
                    textBlock.Inlines.Add(new Run(part));
                }
                else
                {
                    bool isMatchedWord =
                        wordIndex >= match.StartWordIndex &&
                        wordIndex < match.StartWordIndex + match.WordCount;

                    Inline inline = isMatchedWord
                        ? new Bold(new Run(part))
                        : new Run(part);

                    textBlock.Inlines.Add(inline);
                    wordIndex++;
                }
            }
        }
        private static bool IsAllowedInputCharacter(char ch)
        {
            return (ch >= '\u05D0' && ch <= '\u05EA')
                || ch == ' ' || ch == '\u05BE' || ch == '-';
        }
    }
}