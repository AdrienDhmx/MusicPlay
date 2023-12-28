using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class ArtistRoleModel : BaseModel
    {
        public string Role {  get; set; }

        public ArtistRoleModel(string role) 
        {
            Role = role;
        }
    }
}
