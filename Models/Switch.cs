namespace KoreanRailwayTrackEditor.Models
{
    public class Switch : TrackComponent
    {
        private bool _isReverse;
        private int _switchNumber;
        
        // Point A (left end)
        private double _aX;
        private double _aY;
        
        // Point S (center junction)
        private double _sX;
        private double _sY;
        
        // Point R (reverse/diverging end)
        private double _rX;
        private double _rY;
        
        // Point N (normal/straight end)
        private double _nX;
        private double _nY;

        public int SwitchNumber
        {
            get => _switchNumber;
            set
            {
                if (_switchNumber != value)
                {
                    _switchNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsReverse
        {
            get => _isReverse;
            set
            {
                if (_isReverse != value)
                {
                    _isReverse = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _trackId = "";
        public string TrackId
        {
            get => _trackId;
            set
            {
                if (_trackId != value)
                {
                    _trackId = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isOccupied;
        public bool IsOccupied
        {
            get => _isOccupied;
            set
            {
                if (_isOccupied != value)
                {
                    _isOccupied = value;
                    OnPropertyChanged();
                }
            }
        }

        // Point A properties
        public double AX
        {
            get => _aX;
            set
            {
                if (_aX != value)
                {
                    _aX = value;
                    OnPropertyChanged();
                }
            }
        }

        public double AY
        {
            get => _aY;
            set
            {
                if (_aY != value)
                {
                    _aY = value;
                    OnPropertyChanged();
                }
            }
        }

        // Point S properties
        public double SX
        {
            get => _sX;
            set
            {
                if (_sX != value)
                {
                    _sX = value;
                    OnPropertyChanged();
                }
            }
        }

        public double SY
        {
            get => _sY;
            set
            {
                if (_sY != value)
                {
                    _sY = value;
                    OnPropertyChanged();
                }
            }
        }

        // Point R properties
        public double RX
        {
            get => _rX;
            set
            {
                if (_rX != value)
                {
                    _rX = value;
                    OnPropertyChanged();
                }
            }
        }

        public double RY
        {
            get => _rY;
            set
            {
                if (_rY != value)
                {
                    _rY = value;
                    OnPropertyChanged();
                }
            }
        }

        // Point N properties
        public double NX
        {
            get => _nX;
            set
            {
                if (_nX != value)
                {
                    _nX = value;
                    OnPropertyChanged();
                }
            }
        }

        public double NY
        {
            get => _nY;
            set
            {
                if (_nY != value)
                {
                    _nY = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
