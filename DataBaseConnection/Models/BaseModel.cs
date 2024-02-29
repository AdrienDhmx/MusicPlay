using System.ComponentModel.DataAnnotations;

namespace MusicPlay.Database.Models
{
    public class BaseModel : ObservableObject
    {
        [Required]
        public int Id { get; set; }

        public BaseModel(int id)
        {
            Id = id;
        }

        public BaseModel() { }

        public override bool Equals(object obj)
        {
            return obj is BaseModel model && model.Id == Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
