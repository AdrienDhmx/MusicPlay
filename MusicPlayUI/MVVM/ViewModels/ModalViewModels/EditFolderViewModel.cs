using System.Windows.Input;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;

using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels.ModalViewModels
{
    public class EditFolderViewModel : ModalViewModel
    {
        private Folder _folder;
        public Folder Folder
        {
            get => _folder;
            set => SetField(ref _folder, value);
        }

        private string _folderName = string.Empty;
        public string FolderName
        {
            get => _folderName;
            set => SetField(ref _folderName, value);
        }

        private bool _monitored;
        public bool Monitored
        {
            get => _monitored;
            set => SetField(ref _monitored, value);
        }


        private bool _validName = true;
        public bool ValidName
        {
            get => _validName;
            set
            {
                SetField(ref _validName, value);
            }
        }

        public ICommand EditFolderCommand { get; }
        public EditFolderViewModel(IModalService modalService) : base(modalService)
        {
            EditFolderCommand = new RelayCommand(async () =>
            {
                ValidName = !FolderName.IsNullOrWhiteSpace();

                if (ValidName)
                {
                    Folder.Name = FolderName;
                    await StorageService.Instance.UpdateFolder(Folder, Monitored);
                    CloseModal();
                }
            });
        }

        public override void Update(BaseModel parameter = null)
        {
            if (parameter is Folder model)
            {
                Folder = model;
                FolderName = Folder.Name;
                Monitored = Folder.IsMonitored;
            }
            else
            {
                throw new System.Exception($"The parameter type is not supported, {typeof(CreateEditNameModel)} is expected.");
            }
        }
    }
}
