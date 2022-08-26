using FiveWordFinderWpf.Commands;
using FiveWordFinderWpf.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Collections;

namespace FiveWordFinderWpf.ViewModel
{
    public class HomeViewModel : ViewModelBase
    {
        private string[] _defaultFiles = new string[]
        {
            @".\Resources\words_alpha.txt",
            @".\Resources\wordle_words.txt"
        };

        ApplicationState _applicationState;

        public string WordsFilePath
        {
            get
            {
                return _applicationState.WordsFilePath;
            }
            set
            {
                _applicationState.WordsFilePath = value;
                OnPropertyChanged();
            }
        }

        
        public RelayCommand BrowseCommand { get; private set; }

        public HomeViewModel(ApplicationState applicationState)
        {
            _applicationState = applicationState;
            WordsFilePath = CheckGetDefaultFilePath();
            BrowseCommand = InitBrowseCommand();
            RegisterValidators();
        }

        private void RegisterValidators()
        {
            RegisterPropertyValidator(nameof(WordsFilePath), () => {
                var valid = File.Exists(WordsFilePath);
                return new ValidationResult(valid, valid ? string.Empty : "File Does Not Exist");
            });
        }

        private RelayCommand InitBrowseCommand()
        {
            var browseCommand = new RelayCommand((o) =>
            {
                Browse_Click(o?.ToString() ?? String.Empty);
            });

            return browseCommand;
        }

        private string CheckGetDefaultFilePath()
        {
            foreach (string file in _defaultFiles)
            {
                if (File.Exists(file))
                {
                    return file;
                }
            }

            return String.Empty;
        }

        private void Browse_Click(string? curFilePath)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Words File",
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            
            if (!string.IsNullOrWhiteSpace(curFilePath) &&
                File.Exists(curFilePath))
            {
                var initialPath = System.IO.Path.GetDirectoryName(curFilePath);
                if (!string.IsNullOrWhiteSpace(initialPath))
                {
                    if (!System.IO.Path.IsPathFullyQualified(initialPath))
                    {
                        initialPath = System.IO.Path.GetFullPath(initialPath);
                    }
                    openFileDialog.InitialDirectory = initialPath;
                }
            }
            bool? dlgResult = openFileDialog.ShowDialog();
            if (dlgResult == true)
            {
                this.WordsFilePath = openFileDialog.FileName;
            }
        }
    }
}
