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

        public virtual Dictionary<string, object> InsertTable()
        {
            throw new NotSupportedException("CreateTable not supported for the base class.");
        }

        public virtual Dictionary<string, object> UpdateTable()
        {
            throw new NotSupportedException("UpdateTable not supported for the base class.");
        }

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
