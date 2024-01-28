using MusicPlay.Database.Models;
using System.Windows.Media;

namespace MusicPlayUI.MVVM.Models
{
    public class ConfirmActionModel : BaseModel
    {
        public string Message { get; set; }
        public string MessageDetail { get; set; }
        public string ConfirmAction { get; set; }
        public Brush ConfirmActionColor { get; set; }
        public Brush ConfirmActionForeground { get; set; }
    }
}
