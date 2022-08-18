using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace VialLogParsing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FileInfo[] Files;
        private List<DataEntry> Data = new List<DataEntry>();
        private readonly List<Presence> Presences = new List<Presence>();

        public MainWindow()
        {
            string[] fileNames = OpenFiles("Select paired sensor log files", "Log file (*.log)|*.log");
            if (fileNames == null)
            {
                Close();
                return;
            }

            Files = fileNames.Select(x => new FileInfo(x)).ToArray();
            if (!LoadData())
            {
                MessageBox.Show("Files contain invalid data. Closing.");
                Close();
                return;
            }
            ParseData();
            InitializeComponent();
        }

        private bool LoadData()
        {
            foreach (FileInfo file in Files)
            {
                string name = file.Name.Replace(file.Extension, "");
                string[] data = File.ReadAllLines(file.FullName);
                foreach (string line in data)
                {
                    DataEntry dataEntry = new DataEntry(name, line);
                    if (dataEntry.Valid)
                        Data.Add(dataEntry);
                    else
                        return false;
                }
            }
            return true;
        }

        private void ParseData()
        {
            Data = Data.OrderBy(x => x.DT).ToList();

            bool present = false;
            string name = string.Empty;
            foreach (DataEntry entry in Data)
            {
                if (entry.State && !present)
                {
                    Presences.Add(new Presence(entry));
                    present = true;
                    name = entry.Name;
                }
                else if (present && !entry.State && entry.Name == name)
                {
                    Presences.Last().End = entry.DT;
                    present = false;
                }
                else if (entry.State && Presences.Count == 0)
                {
                    Presences.Add(new Presence(entry));
                    present = true;
                }
            }

            if (present) Presences.Last().End = Data.Last().DT;
        }

        private string[] OpenFiles(string title, string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                Title = title,
                Filter = filter,
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileNames;
            return null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FlowDocument doc = new FlowDocument();
            Paragraph par = new Paragraph();

            par.Inlines.Add("Index\tStart\t\tStop\t\tDuration (s)\tName\n");

            for (int i = 0; i < Presences.Count; i++)
            {
                par.Inlines.Add(
                    $"{i+1}\t" +
                    $"{Presences[i].Start:HH:mm:ss}\t" +
                    $"{Presences[i].End:HH:mm:ss}\t" +
                    $"{Presences[i].Duration.TotalSeconds}\t\t" +
                    $"{Presences[i].Name}\n");
            }

            doc.Blocks.Add(par);
            RTB.Document = doc;
        }
    }
}
