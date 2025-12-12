namespace KoreanRailwayTrackEditor.Models
{
    public class TrackCircuit : TrackComponent
    {
        private double _endX;
        private double _endY;
        private bool _isOccupied;

        public TrackCircuit()
        {
            // Default length 100
            _endX = X + 100;
            _endY = Y;
        }

        public double EndX
        {
            get => _endX;
            set
            {
                if (_endX != value)
                {
                    _endX = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        public double EndY
        {
            get => _endY;
            set
            {
                if (_endY != value)
                {
                    _endY = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        public double Length
        {
            get
            {
                double dx = EndX - X;
                double dy = EndY - Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
            set
            {
                // Optional: Implement setter if needed, but for now it's derived
            }
        }

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
        
        // Override X and Y setters to notify Length change if needed, 
        // but base class doesn't have virtual setters. 
        // We might need to listen to PropertyChanged in constructor if we want Length to update when X/Y changes.
        // However, for this specific requirement, we are updating X/Y/EndX/EndY explicitly.
    }
}
