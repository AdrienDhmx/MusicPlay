using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlay.Database.Models
{
    [Table("ArtistRole")]
    public class ArtistRole : BaseModel
    {
        [Required]
        public int ArtistId { get; set; }
        [Required]
        public int RoleId { get; set; }

        private Role _role;
        public Role Role
        {
            get
            {
                if(_role.IsNull())
                {
                    _role = Role.Get(RoleId);
                }
                return _role;
            }
            set => SetField(ref  _role, value);
        }
        private Artist _artist;
        public Artist Artist
        {
            get
            {
                if (_artist.IsNull())
                {
                    _artist = Artist.GetSync(ArtistId);
                }
                return _artist;
            }
            set => SetField(ref _artist, value);
        }

        public ArtistRole(string role, int artistId, int roleId)
        {
            Role = new(roleId, role);
            ArtistId = artistId;
            RoleId = roleId;
        }

        public ArtistRole(Role role, Artist artist)
        {
            Role = role;
            RoleId = role.Id;
            Artist = artist;
            ArtistId = artist.Id;
        }

        public ArtistRole(string role, int artistId)
        {
            Role = new(role);
            ArtistId = artistId;
        }

        public ArtistRole(string role)
        {
            Role = new(role);
        }

        public ArtistRole(Role role, int artistId)
        {
            Role = role;
            RoleId = role.Id;
            ArtistId = artistId;
        }

        public ArtistRole()
        {
            
        }
    }
}
