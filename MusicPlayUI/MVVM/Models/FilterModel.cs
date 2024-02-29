using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Documents;
using MessageControl;
using MusicPlay.Database.Models;

using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using Newtonsoft.Json;

namespace MusicPlayUI.MVVM.Models
{
    public class LibraryFilters : ObservableObject
    {
        private ObservableCollection<FilterModel> _filters = [];
        public ObservableCollection<FilterModel> Filters
        {
            get => _filters;
            set
            {
                SetField(ref _filters, value);
                DispatchFilters();
            }
        }

        [JsonIgnore]
        public ObservableCollection<FilterModel> TagFilters { get; private set; } = [];
        [JsonIgnore]
        public ObservableCollection<FilterModel> ArtistRoleFilters { get; private set; } = [];
        [JsonIgnore]
        public ObservableCollection<FilterModel> AlbumTypeFilters { get; private set; } = [];
        [JsonIgnore]
        public ObservableCollection<FilterModel> PrimaryArtistFilters { get; private set; } = [];

        public void AddFilter(FilterModel filter)
        {
            Filters.Add(filter);
            DispatchFilter(filter);
        }

        public void AddFilters(List<FilterModel> filters)
        {
            foreach (FilterModel filter in filters)
            {
                Filters.Add(filter);
                DispatchFilter(filter);
            }
        }

        public void RemoveFilter(FilterModel filter)
        {
            Filters.Remove(filter);
            switch (filter.Type)
            {
                case FilterEnum.Tag:
                    TagFilters.Remove(filter);
                    return;
                case FilterEnum.ArtistType:
                    ArtistRoleFilters.Remove(filter);
                    return;
                case FilterEnum.AlbumType:
                    AlbumTypeFilters.Remove(filter);
                    return;
                case FilterEnum.Artist:
                    PrimaryArtistFilters.Remove(filter);
                    return;
                default:
                    "This filter can't be removed, it's of unknown type".CreateWarningMessage().PublishWithAppDispatcher();
                    return;
            }
        }

        private void DispatchFilter(FilterModel filter)
        {
            int index = 0;
            switch (filter.Type)
            {
                case FilterEnum.Tag:
                    index = FindInsertIndex([..TagFilters], filter);
                    TagFilters.Insert(index, filter);
                    return;
                case FilterEnum.ArtistType:
                    index = FindInsertIndex([.. ArtistRoleFilters], filter);
                    ArtistRoleFilters.Add(filter);
                    return;
                case FilterEnum.AlbumType:
                    index = FindInsertIndex([.. AlbumTypeFilters], filter);
                    AlbumTypeFilters.Add(filter);
                    return;
                case FilterEnum.Artist:
                    index = FindInsertIndex([.. PrimaryArtistFilters], filter);
                    PrimaryArtistFilters.Add(filter);
                    return;
                default:
                    "This filter can't be added, it's of unknown type".CreateWarningMessage().PublishWithAppDispatcher();
                    return;
            }
        }

        private void DispatchFilters()
        {
            TagFilters = [];
            ArtistRoleFilters = [];
            AlbumTypeFilters = [];
            PrimaryArtistFilters = [];

            foreach (FilterModel filter in _filters)
            {
                DispatchFilter(filter);
            }
        }

        private static int FindInsertIndex(ICollection<FilterModel> filters, FilterModel filter)
        {
            int insertIndex = 0;

            while (insertIndex < filters.Count &&
                   Comparer<string>.Default.Compare(filter.Name, filters.ElementAt(insertIndex).Name) > 0)
            {
                insertIndex++;
            }

            return insertIndex;
        }
    }

    public class FilterModel : BaseModel
    {
        private string _name = string.Empty;
        private bool _isNegative = false;

        /// <summary>
        /// The name of the filter to display on the Chip
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get => _name;
            set
            {
                SetField(ref _name, value);
                OnPropertyChanged(nameof(FullName));
            }
        }

        /// <summary>
        /// Determines if the filter is applied positively (accept) of negatively (except)
        /// </summary>
        public bool IsNegative
        {
            get => _isNegative;
            set
            {
                SetField(ref _isNegative, value);
                OnPropertyChanged(nameof(FullName));
            }
        }

        /// <summary>
        /// The type of the filter
        /// </summary>
        public FilterEnum Type { get; set; }

        public virtual string FullName
        {
            get
            {
                string prefix = "";
                if(IsNegative)
                {
                    prefix = "- ";
                }
                return prefix + Name;
            }
        }

        public FilterModel(int id, string name, FilterEnum type, bool isNegative = false)
        {
            Id = id;
            Name = name;
            IsNegative = isNegative;
            Type = type;
        }

        public FilterModel(int id, FilterEnum type)
        {
            Id = id;
            Type = type;
        }

        public virtual bool CompareTo(int value) 
        {
            if (IsNegative && Id == value)
            {
                return false;
            }
            return Id == value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not FilterModel) return false;

            FilterModel model = obj as FilterModel;
            return Type == model.Type && Id == model.Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class GreaterSmallerThanFilter : FilterModel
    {
        public override string FullName {
            get
            {
                string prefix = "≥ ";
                if(IsNegative)
                {
                    prefix = "≤ ";
                }

                return prefix + Name;
            }
        }


        public GreaterSmallerThanFilter(int id, string name, FilterEnum type, bool isNegative = false) : base(id, name, type, isNegative)
        {
        }

        public override bool CompareTo(int value)
        {
            if (IsNegative)
                return Id <= value;
            return Id >= value;
        }
    }
}
