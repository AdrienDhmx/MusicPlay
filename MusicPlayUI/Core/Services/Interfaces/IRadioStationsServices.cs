using MusicPlay.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IRadioStationsService
    {
        int ArtistInfluence { get; set; }
        int GenreInfluence { get; set; }
        int RadioStationTrackNumber { get; set; }
        List<Playlist> TodayRadioStations { get; set; }

        Task<Playlist> CreateRadioStation();
        Task<Playlist> CreateRadioStation(Track basedOn);
        Task<List<Playlist>> CreateRadioStations(int number);
    }
}