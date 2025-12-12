using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.Text.Json.Serialization;

namespace KoreanRailwayTrackEditor.Models
{
    [JsonDerivedType(typeof(Signal), typeDiscriminator: "Signal")]
    [JsonDerivedType(typeof(Switch), typeDiscriminator: "Switch")]
    [JsonDerivedType(typeof(TrackCircuit), typeDiscriminator: "TrackCircuit")]
    public abstract class TrackComponent : INotifyPropertyChanged
    {
        private string _id;
        private double _x;
        private double _y;
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public double X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
