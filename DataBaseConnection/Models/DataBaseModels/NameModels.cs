using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MusicPlay.Database.DatabaseAccess;

namespace MusicPlay.Database.Models.DataBaseModels
{
    public abstract class NameModel : BaseModel
    {
        public string Name { get; set; }

        public NameModel(int id, string name) : base(id)
        {
            Name = name;
        }

        public NameModel(string name)
        {
            Name = name;
        }

        public NameModel() { }

        public override Dictionary<string, object> InsertTable()
        {
            return new()
            {
                { nameof(Name), Name },
            };
        }

        public override Dictionary<string, object> UpdateTable()
        {
            return new()
            {
                { nameof(Name), Name },
            };
        }
    }

    [Table("Role")]
    public class Role : NameModel
    {
        public Role(int id, string name) : base(id, name) { }
        public Role(string name) : base(name) { }
        public Role() { }

        public static Role Get(int roleId)
        {
            using DatabaseContext context = new();
            return context.Roles.Find(roleId);
        }

        public static List<Role> GetAll()
        {
            using DatabaseContext context = new();
            return [.. context.Roles];
        }

        public static void Insert(Role role)
        {
            using DatabaseContext context = new();
            context.Roles.Add(role);
            context.SaveChanges();
        }
    }

    [Table("Label")]
    public class Label : NameModel
    {
        public Label(int id, string name) : base(id, name) { }
        public Label(string name) : base(name) { }
        public Label() { }
    }

    [Table("Country")]
    public class Country : NameModel
    {
        public Country(int id, string name) : base(id, name) { }
        public Country(string name) : base(name) { }
        public Country() { }
    }
}
