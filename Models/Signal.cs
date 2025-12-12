namespace KoreanRailwayTrackEditor.Models
{
    public enum SignalType
    {
        Main3,
        Main45,
        Shunt
    }

    public class Signal : TrackComponent
    {
        private SignalType _type = SignalType.Main3;
        private string _direction = "Up"; // Up (상행), Down (하행)
        private bool _hasGuide;
        private bool _hasRoute;
        private bool _hasNoGuide;
        private bool _isSiso;

        public SignalType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged();
                    // Reset incompatible flags when type changes
                    if (_type == SignalType.Shunt)
                    {
                        HasGuide = false;
                        HasRoute = false;
                        HasNoGuide = true; // Default enable for Shunt
                    }
                    else
                    {
                        HasNoGuide = false;
                        HasGuide = true; // Default enable for Main
                        HasRoute = true; // Default enable for Main
                    }
                }
            }
        }

        public string Direction
        {
            get => _direction;
            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasGuide
        {
            get => _hasGuide;
            set
            {
                if (_hasGuide != value)
                {
                    _hasGuide = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasRoute
        {
            get => _hasRoute;
            set
            {
                if (_hasRoute != value)
                {
                    _hasRoute = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasNoGuide
        {
            get => _hasNoGuide;
            set
            {
                if (_hasNoGuide != value)
                {
                    _hasNoGuide = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSiso
        {
            get => _isSiso;
            set
            {
                if (_isSiso != value)
                {
                    _isSiso = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _hasTtb;
        public bool HasTtb
        {
            get => _hasTtb;
            set
            {
                if (_hasTtb != value)
                {
                    _hasTtb = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
