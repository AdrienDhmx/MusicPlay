using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayModels;

namespace MusicPlayUI.MVVM.Models
{
    public class CreateEditNameModel : BaseModel
    {
        public string Name { get; set; }
        public string NamePlaceholder { get; set; }
        public bool Edit { get; }
        public Action<string> OnCreate { get; }
        public Action<string> OnEdit { get; }

        public CreateEditNameModel(string name, string namePlaceholder, bool edit, Action<string> onCreate, Action<string> onEdit)
        {
            Name = name;
            NamePlaceholder = namePlaceholder;
            Edit = edit;
            OnCreate = onCreate;
            OnEdit = onEdit;
        }
    }
}
