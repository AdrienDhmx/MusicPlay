using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class ArtistRoleModel : BaseModel
    {
        public readonly int artistId;
        public readonly int roleId;

        public string Role {  get; set; }

        public ArtistRoleModel(string role, int artistId, int roleId) 
        {
            Role = role;
            this.artistId = artistId;
            this.roleId = roleId;
        }

        public override Dictionary<string, object> CreateTable()
        {
            Dictionary<string, object> keyValues = new Dictionary<string, object>
            {
                { DataBaseColumns.ArtistId, artistId },
                { DataBaseColumns.RoleId, roleId },
            };
            return keyValues;
        }
    }
}
