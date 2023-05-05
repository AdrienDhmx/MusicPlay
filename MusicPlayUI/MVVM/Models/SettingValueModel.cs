using MusicPlayModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.MVVM.Models
{
    public class SettingValueModel<T> : BaseModel
    {
        private bool _isSelected = false;

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        /// <summary>
        /// Is an id used for handling preferences (avoid languages issues). It often is an Enum and is stored as an int in the config file
        /// </summary>
        public T Value { get; set; }

        public SettingValueModel(string name, string description, T value, bool isSelected = false)
        {
            Name = name;
            Description = description;
            Value = value;
            IsSelected = isSelected;
        }
    }
}
