using DiagramGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace DiagramGeneratorUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CSharpObjectCollection collection;
        private string analysisBasePath;
        private string collectionFileName;
        private string diagramFileName;
        private string startClass;

        public MainWindow()
        {
            InitializeComponent();
            ReadSettings();
        }

        private void ReadSettings()
        {
            analysisBasePath = Properties.Settings.Default.AnalysisBasePath;
            collectionFileName = Properties.Settings.Default.CollectionFileName;
            diagramFileName = Properties.Settings.Default.DiagramFileName;
            startClass = Properties.Settings.Default.StartClass;
            PublicAssociationsCheckBox.IsChecked = Properties.Settings.Default.IncludePublicAssociations;
            ProtectedAssociationsCheckBox.IsChecked = Properties.Settings.Default.IncludeProtectedAssociations;
            InternalAssociationsCheckBox.IsChecked = Properties.Settings.Default.IncludeInternalAssociations;
            PrivateAssociationsCheckBox.IsChecked = Properties.Settings.Default.IncludePrivateAssociations;
            InheritanceCheckBox.IsChecked = Properties.Settings.Default.IncludeInheritance;
            DepthSlider.Value = Properties.Settings.Default.AnalysisDepth;
        }

        private void WriteSettings()
        {
            Properties.Settings.Default.AnalysisBasePath = analysisBasePath;
            Properties.Settings.Default.CollectionFileName = collectionFileName;
            Properties.Settings.Default.DiagramFileName = diagramFileName;
            Properties.Settings.Default.StartClass = StartClassComboBox.Text;
            Properties.Settings.Default.IncludePublicAssociations = PublicAssociationsCheckBox.IsChecked.GetValueOrDefault();
            Properties.Settings.Default.IncludeProtectedAssociations = ProtectedAssociationsCheckBox.IsChecked.GetValueOrDefault();
            Properties.Settings.Default.IncludeInternalAssociations = InternalAssociationsCheckBox.IsChecked.GetValueOrDefault();
            Properties.Settings.Default.IncludePrivateAssociations = PrivateAssociationsCheckBox.IsChecked.GetValueOrDefault();
            Properties.Settings.Default.IncludeInheritance = InheritanceCheckBox.IsChecked.GetValueOrDefault();
            Properties.Settings.Default.AnalysisDepth = (int)DepthSlider.Value;
            Properties.Settings.Default.Save();
        }

        private async void PerformAnalysisButtonClick(object sender, RoutedEventArgs e)
        {
            using FolderBrowserDialog dialog = new();
            analysisBasePath = @"C:\Users\JAH\Documents\Git\xgsos";
            dialog.SelectedPath = analysisBasePath;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                analysisBasePath = dialog.SelectedPath;
                CSharpAnalyzer analyzer = new(Console.WriteLine);
                IEnumerable<string> files = Directory.EnumerateFiles(analysisBasePath, "*.cs", SearchOption.AllDirectories);
                DisableRectangle.Visibility = Visibility.Visible;
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                collection = await analyzer.AnalyzeFilesAsync(files);
                Mouse.OverrideCursor = null;
                DisableRectangle.Visibility = Visibility.Hidden;
                CollectionLabel.Content = "Unnamed collection";
                CollectionLabel.Foreground = Brushes.Orange;
                PopulateStartClassComboBox(collection);
                DiagramControlsGrid.IsEnabled = true;
                SaveAnalysisButton.IsEnabled = true;
            }
        }

        private void SaveAnalysisButtonClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new() { FileName = collectionFileName };
            if (saveFileDialog.ShowDialog() == true)
            {
                collectionFileName = saveFileDialog.FileName;
                collection.Serialize(collectionFileName);
                CollectionLabel.Content = $"Collection \"{collectionFileName}\"";
                CollectionLabel.Foreground = Brushes.Green;
                SaveAnalysisButton.IsEnabled = false;
            }
        }

        private void LoadAnalysisButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new() { FileName = collectionFileName };
            if (openFileDialog.ShowDialog() == true)
            {
                collectionFileName = openFileDialog.FileName;
                collection = CSharpObjectCollection.Deserialize(collectionFileName);
                CollectionLabel.Content = $"Collection \"{collectionFileName}\"";
                CollectionLabel.Foreground = Brushes.Green;
                PopulateStartClassComboBox(collection);
                DiagramControlsGrid.IsEnabled = true;
                SaveAnalysisButton.IsEnabled = false;
            }
        }

        private void GenerateDiagramButtonClick(object sender, RoutedEventArgs e)
        {
            if (collection != null)
            {
                Settings settings = new()
                {
                    StartClass = StartClassComboBox.Text,
                    IncludePublicAssociations = PublicAssociationsCheckBox.IsChecked.GetValueOrDefault(),
                    IncludeProtectedAssociations = ProtectedAssociationsCheckBox.IsChecked.GetValueOrDefault(),
                    IncludeInternalAssociations = InternalAssociationsCheckBox.IsChecked.GetValueOrDefault(),
                    IncludePrivateAssociations = PrivateAssociationsCheckBox.IsChecked.GetValueOrDefault(),
                    IncludeInheritance = InheritanceCheckBox.IsChecked.GetValueOrDefault()
                };
                CSharpObjectCollection coll = settings.StartClass == "" ? collection : collection.Clone(settings.StartClass, settings, (int)DepthSlider.Value);

                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Plant Uml (*.plantuml)|*.plantuml",
                    FileName = diagramFileName,
                    AddExtension = true,
                    DefaultExt = "plantuml",
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    diagramFileName = saveFileDialog.FileName;
                    using TextWriter tw = new StreamWriter(diagramFileName);
                    foreach (string s in UmlGenerator.GeneratePlantUml(coll))
                    {
                        tw.WriteLine(s);
                    }
                }
            }
        }

        private void PopulateStartClassComboBox(CSharpObjectCollection collection)
        {
            string oldStartClass = StartClassComboBox.Text;
            StartClassComboBox.ItemsSource = collection.Classes.OrderBy(c => c.Name).Select(c => c.Name);
            int index = StartClassComboBox.Items.IndexOf(oldStartClass);
            if (index != -1)
            {
                StartClassComboBox.SelectedIndex = index;
            }
            else
            {
                index = StartClassComboBox.Items.IndexOf(startClass);
                if (index != -1)
                {
                    StartClassComboBox.SelectedIndex = index;
                }
            }
        }

        private void DepthSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DepthSlider.ToolTip = DepthSlider.Value;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WriteSettings();
        }
    }
}