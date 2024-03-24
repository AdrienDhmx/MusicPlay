using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessageControl;
using Microsoft.EntityFrameworkCore;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlay.Database.Models
{
    [Table("Folder")]
    public class Folder : BaseModel
    {
        private string _name = string.Empty;
        private bool _isMonitored = true;

        // not mapped
        private bool _scanning = false;
        private bool _deleting = false;
        private int _trackImportedCount = 0;

        [Required]
        [System.ComponentModel.DataAnnotations.Schema.Index(IsUnique = true)]
        public string Path { get; init; } // can only be set once

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public bool IsMonitored
        {
            get => _isMonitored;
            set => SetField(ref _isMonitored, value);
        }

        public int TrackImportedCount
        {
            get => _trackImportedCount;
            set => SetField(ref _trackImportedCount, value);
        }

        [NotMapped]
        public bool Scanning
        {
            get => _scanning;
            set => SetField(ref _scanning, value);
        }

        [NotMapped]
        public bool Deleting
        {
            get => _deleting;
            set => SetField(ref _deleting, value);
        }

        public Folder(string path)
        {
            Path = path;
            Name = Path.Split('\\').Last();
        }

        public Folder()
        {
            
        }

        public static async Task Insert(Folder folder)
        {
            using DatabaseContext context = new();
            await context.Folders.AddAsync(folder);
            await context.SaveChangesAsync();
        }

        public static async Task<List<Folder>> GetAll()
        {
            using DatabaseContext context = new();
            return await context.Folders.ToListAsync();
        }

        public static void Update(Folder folder, string newName, bool isMonitored)
        {
            using DatabaseContext context = new();
            context.Folders.Update(folder);
            folder.Name = newName;
            folder.IsMonitored = isMonitored;
            context.SaveChanges();
        }
    }
}
