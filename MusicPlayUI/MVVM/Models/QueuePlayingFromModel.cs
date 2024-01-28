using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;

namespace MusicPlayUI.MVVM.Models
{
    public class QueuePlayingFromModel : BaseModel
    {
        public string PlayingFrom { get; set; }

        public ModelTypeEnum ModelType { get; set; }

        public int DataId { get; private set; }

        public QueuePlayingFromModel(string playingFrom, ModelTypeEnum modelType, int dataId)
        {
            PlayingFrom = playingFrom;
            ModelType = modelType;
            DataId = dataId;
        }
    }
}
