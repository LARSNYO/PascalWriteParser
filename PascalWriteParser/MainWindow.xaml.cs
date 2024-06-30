using Microsoft.Win32;
using PascalWriteParser.Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;

namespace PascalWriteParser
{
    public partial class MainWindow : Window
    {
        private string _currentFilePath;

        public MainWindow()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, SaveAs_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, Undo_Click));
        }

        private void New_Click(object sender, ExecutedRoutedEventArgs e)
        {
            EditorTextBox.Clear();
            _currentFilePath = null;
        }

        private void Open_Click(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _currentFilePath = openFileDialog.FileName;
                EditorTextBox.Text = File.ReadAllText(_currentFilePath);
            }
        }

        private void Save_Click(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                SaveAs_Click(sender, e);
            }
            else
            {
                File.WriteAllText(_currentFilePath, EditorTextBox.Text);
            }
        }

        private void SaveAs_Click(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                _currentFilePath = saveFileDialog.FileName;
                File.WriteAllText(_currentFilePath, EditorTextBox.Text);
            }
        }

        private void Copy_Click(object sender, ExecutedRoutedEventArgs e)
        {
            EditorTextBox.Copy();
        }

        private void Paste_Click(object sender, ExecutedRoutedEventArgs e)
        {
            EditorTextBox.Paste();
        }

        private void Cut_Click(object sender, ExecutedRoutedEventArgs e)
        {
            EditorTextBox.Cut();
        }

        private void Undo_Click(object sender, ExecutedRoutedEventArgs e)
        {
            EditorTextBox.Undo();
        }

        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var lexer = new LexicalAnalyzer(EditorTextBox.Text);
                var parser = new SyntacticAnalyzer(lexer);
                var syntaxTree = parser.Parse();
                MessagesTreeView.Items.Clear();
                DisplaySyntaxTree(syntaxTree, MessagesTreeView.Items);
            }
            catch (Exception ex)
            {
                MessagesTreeView.Items.Clear();
                MessagesTreeView.Items.Add($"Parsing failed: {ex.Message}");
            }
        }

        private void DisplaySyntaxTree(SyntaxNode node, ItemCollection items)
        {
            TreeViewItem treeViewItem = new TreeViewItem
            {
                Header = node.Name
            };
            items.Add(treeViewItem);

            foreach (var child in node.Children)
            {
                DisplaySyntaxTree(child, treeViewItem.Items);
            }
        }
    }
}
