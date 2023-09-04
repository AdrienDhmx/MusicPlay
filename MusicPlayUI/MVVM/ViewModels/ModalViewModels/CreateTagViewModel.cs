using System.Windows.Input;
using DataBaseConnection.DataAccess;
using MusicPlayModels;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;

namespace MusicPlayUI.MVVM.ViewModels.ModalViewModels
{
    public class CreateTagViewModel : ModalViewModel
    {
        private string _originalName = string.Empty;

        private TagModel _tag = new();
        public TagModel Tag 
        {
            get => _tag;
            set
            {
                SetField(ref _tag, value);
            }
        }

        private bool _isEdit;
        public bool IsEdit
        {
            get { return _isEdit; }
            set
            {
                SetField(ref _isEdit, value);
            }
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

        public ICommand CreateTagCommand { get; }
        public CreateTagViewModel(IModalService modalService, INavigationService navigationService) : base(modalService, navigationService)
        {
            CreateTagCommand = new RelayCommand(() =>
            {
                ValidName = !string.IsNullOrWhiteSpace(Tag.Name);

                if (ValidName)
                {
                    if(IsEdit)
                    {
                        DataAccess.Connection.UpdateTag(Tag);
                    } 
                    else
                    {
                        DataAccess.Connection.InserTag(Tag);
                    }
                    CloseModal();
                }
            });
        }

        public override void CloseModal(bool canceled = false)
        {
            if(canceled)
            {
                Tag.Name = _originalName;
            }

            base.CloseModal(canceled);
        }

        public override void Update(BaseModel parameter = null)
        {
            IsEdit = parameter != null;

            if (IsEdit)
            {
                Tag = parameter as TagModel;
                _originalName = Tag.Name;
            }
        }
    }
}
