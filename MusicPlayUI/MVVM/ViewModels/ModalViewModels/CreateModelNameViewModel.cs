using System.Windows.Input;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels.ModalViewModels
{
    public class CreateModelNameViewModel : ModalViewModel
    {
        private string _originalName = string.Empty;

        private CreateEditNameModel _model;
        public CreateEditNameModel Model 
        {
            get => _model;
            set
            {
                SetField(ref _model, value);
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
        public CreateModelNameViewModel(IModalService modalService) : base(modalService)
        {
            CreateTagCommand = new RelayCommand(() =>
            {
                ValidName = !string.IsNullOrWhiteSpace(Model.Name);

                if (ValidName)
                {
                    if(IsEdit)
                    {
                        Model.OnEdit(Model.Name);
                    } 
                    else
                    {
                        Model.OnCreate(Model.Name);
                    }
                    CloseModal();
                }
            });
        }

        public override void CloseModal(bool canceled = false)
        {
            if(canceled)
            {
                Model.Name = _originalName;
            }

            base.CloseModal(canceled);
        }

        public override void Update(BaseModel parameter = null)
        {
            if(parameter is CreateEditNameModel model)
            {
                Model = model;
                IsEdit = Model.Edit;
                _originalName = Model.Name;
            }
            else
            {
                throw new System.Exception($"The parameter type is not supported, {typeof(CreateEditNameModel)} is expected.");
            }
        }
    }
}
