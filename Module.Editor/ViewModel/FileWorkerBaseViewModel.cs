using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FomodInfrastructure.Interface;
using FomodModel.Base.ModuleCofiguration;
using MahApps.Metro.Controls.Dialogs;

namespace Module.Editor.ViewModel
{
    public class FileWorkerBaseViewModel<T> : BaseViewModel<T> where T : class
    {
        public FileWorkerBaseViewModel(IFileBrowserDialog fileBrowserDialog, IFolderBrowserDialog folderBrowserDialog, IDialogCoordinator dialogCoordinator)
        {
            _fileBrowserDialog = fileBrowserDialog;
            _folderBrowserDialog = folderBrowserDialog;
            _dialogCoordinator = dialogCoordinator;
        }

        #region Services

        private readonly IFileBrowserDialog _fileBrowserDialog;

        private readonly IFolderBrowserDialog _folderBrowserDialog;

        private readonly IDialogCoordinator _dialogCoordinator;

        #endregion

        #region Methods

        protected async void AddFile(ObservableCollection<SystemItem> itemSource, List<string> paths)
        {
            var files = paths;
            if (files == null)
            {
                _fileBrowserDialog.Multiselect = true;
                _fileBrowserDialog.CheckFileExists = true;
                _fileBrowserDialog.StartFolder = FolderPath;
                _fileBrowserDialog.ShowDialog();
                if (_fileBrowserDialog.SelectedPaths != null)
                    files = _fileBrowserDialog.SelectedPaths.ToList();
                _fileBrowserDialog.Reset();
            }
            if (files == null)
                return;
            if (files.All(fileName => fileName.StartsWith(FolderPath)))
            {
                foreach (var file in files)
                {
                    var source = file.Substring(FolderPath.Length + 1);
                    var destination = file.Substring(FolderPath.Length + 1);
                    itemSource.Add(FileSystemItem.Create(source, destination));
                }
            }
            else
                await _dialogCoordinator.ShowMessageAsync(this, "������", "����������� ��������� ����� ������ �� ���������� �������");
        }

        protected async void AddFolders(ObservableCollection<SystemItem> itemSource, List<string> paths)
        {
            var folders = paths;
            if (folders == null)
            {
                _folderBrowserDialog.CheckFolderExists = true;
                _folderBrowserDialog.ShowDialog();
                if (!string.IsNullOrWhiteSpace(_folderBrowserDialog.SelectedPath))
                    folders = new List<string> { _folderBrowserDialog.SelectedPath };
                _folderBrowserDialog.Reset();
            }
            if (folders == null)
                return;
            if (folders.All(folderName => folderName.StartsWith(FolderPath)))
            {
                foreach (var folder in folders)
                {
                    var directory = folder.Substring(FolderPath.Length + 1);
                    var destination = folder.Substring(FolderPath.Length + 1);
                    itemSource.Add(FolderSystemItem.Create(directory, destination));
                }
            }
            else
                await _dialogCoordinator.ShowMessageAsync(this, "������", "����������� ��������� ����� ������ �� ���������� �������");
        }

        protected bool TryGetImage(out string result)
        {
            _fileBrowserDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            _fileBrowserDialog.CheckFileExists = true;
            if (!_fileBrowserDialog.ShowDialog())
            {
                result = string.Empty;
                _fileBrowserDialog.Reset();
                return false;
            }
            var selectedPath = _fileBrowserDialog.SelectedPath;
            _fileBrowserDialog.Reset();
            var projectPath = FolderPath + Path.DirectorySeparatorChar;
            string relativePath;
            if (!selectedPath.StartsWith(projectPath))
            {
                var directoryName = Path.Combine(projectPath, "fomod", "Images");
                Directory.CreateDirectory(directoryName);
                var fileNameOnly = Path.GetFileNameWithoutExtension(selectedPath);
                var extension = Path.GetExtension(selectedPath);
                var newPath = Path.Combine(directoryName, fileNameOnly + extension);
                var count = 1;
                while (File.Exists(newPath))
                    newPath = Path.Combine(directoryName, $"{fileNameOnly}({count++})" + extension);
                File.Copy(selectedPath, newPath);
                relativePath = @"fomod\Images\" + Path.GetFileName(newPath);
            }
            else
                relativePath = selectedPath.Substring(projectPath.Length);
            result = relativePath;
            return true;
        }

        #endregion
    }
}