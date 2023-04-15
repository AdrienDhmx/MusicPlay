using MusicPlayModels.MusicModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IRadioStationsService
    {
        int ArtistInfluence { get; set; }
        int GenreInfluence { get; set; }
        int RadioStationTrackNumber { get; set; }
        List<PlaylistModel> TodayRadioStations { get; set; }

        Task<PlaylistModel> CreateRadioStation();
        Task<PlaylistModel> CreateRadioStation(TrackModel basedOn);
        Task<List<PlaylistModel>> CreateRadioStations(int number);
    }
}