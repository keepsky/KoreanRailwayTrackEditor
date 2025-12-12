using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using KoreanRailwayTrackEditor.Models;

namespace KoreanRailwayTrackEditor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private TrackComponent _selectedComponent;

        public ObservableCollection<TrackComponent> TrackComponents { get; } = new ObservableCollection<TrackComponent>();

        public TrackComponent SelectedComponent
        {
            get => _selectedComponent;
            set
            {
                if (_selectedComponent != value)
                {
                    _selectedComponent = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _viewportCenterX = 400;
        public double ViewportCenterX
        {
            get => _viewportCenterX;
            set
            {
                if (_viewportCenterX != value)
                {
                    _viewportCenterX = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _viewportCenterY = 300;
        public double ViewportCenterY
        {
            get => _viewportCenterY;
            set
            {
                if (_viewportCenterY != value)
                {
                    _viewportCenterY = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AddSignalCommand { get; }
        public ICommand AddSwitchCommand { get; }
        public ICommand AddTrackCircuitCommand { get; }
        public ICommand SelectCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand DeleteCommand { get; }

        public ObservableCollection<SignalToolItem> SignalTools { get; } = new ObservableCollection<SignalToolItem>();

        public MainViewModel()
        {
            // Initialize Signal Tools
            SignalTools.Add(new SignalToolItem
            {
                Type = SignalType.Main3,
                IconPath = null, // Icon will be handled by DataTemplate resource
                Command = new RelayCommand(_ => AddComponent(new Signal { Id = "S-01", Type = SignalType.Main3, Direction = "Up", HasGuide = true, HasRoute = true }))
            });

            AddSignalCommand = new RelayCommand(_ => AddComponent(new Signal { Id = "S-01", Type = SignalType.Main3, Direction = "Up", HasGuide = true, HasRoute = true }));
            AddSwitchCommand = new RelayCommand(_ => AddComponent(new Switch 
            { 
                Id = "SW-01", 
                SwitchNumber = 1, 
                IsReverse = false,
                // Initialize with ㅗ shape: A on left, S in center, R above, N on right
                AX = 0, AY = 0,      // Point A (left)
                SX = 40, SY = 0,     // Point S (center)
                RX = 40, RY = -30,   // Point R (reverse/up)
                NX = 80, NY = 0      // Point N (normal/right)
            }));
            AddTrackCircuitCommand = new RelayCommand(_ => AddComponent(new TrackCircuit { Id = "TC-01", Length = 100, IsOccupied = false }));
            SelectCommand = new RelayCommand(param => SelectedComponent = param as TrackComponent);
            SaveCommand = new RelayCommand(_ => SaveTrack());
            LoadCommand = new RelayCommand(_ => LoadTrack());
            CopyCommand = new RelayCommand(_ => Copy());
            PasteCommand = new RelayCommand(_ => Paste());
            DeleteCommand = new RelayCommand(_ => DeleteSelected());
        }

        private void AddComponent(TrackComponent component)
        {
            // Set position to viewport center
            component.X = ViewportCenterX;
            component.Y = ViewportCenterY;

            if (component is Switch sw)
            {
                // Initialize Switch anchor points relative to center
                sw.AX = ViewportCenterX;
                sw.AY = ViewportCenterY;
                sw.SX = ViewportCenterX + 40;
                sw.SY = ViewportCenterY;
                sw.RX = ViewportCenterX + 40;
                sw.RY = ViewportCenterY - 30;
                sw.NX = ViewportCenterX + 80;
                sw.NY = ViewportCenterY;
            }
            else if (component is TrackCircuit tc)
            {
                // Initialize TrackCircuit horizontally
                tc.EndX = ViewportCenterX + 100;
                tc.EndY = ViewportCenterY;
            }

            TrackComponents.Add(component);
            SelectedComponent = component;
        }

        private void SaveTrack()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                    string json = System.Text.Json.JsonSerializer.Serialize(TrackComponents, options);
                    System.IO.File.WriteAllText(dialog.FileName, json);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error saving file: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void LoadTrack()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string json = System.IO.File.ReadAllText(dialog.FileName);
                    var components = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<TrackComponent>>(json);
                    
                    if (components != null)
                    {
                        TrackComponents.Clear();
                        foreach (var component in components)
                        {
                            TrackComponents.Add(component);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error loading file: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        public void ClearSelection()
        {
            foreach (var component in TrackComponents)
            {
                component.IsSelected = false;
            }
            SelectedComponent = null;
        }

        public void DeleteSelected()
        {
            // Create a list of selected components to delete
            var toDelete = TrackComponents.Where(c => c.IsSelected).ToList();
            
            // Remove them from the collection
            foreach (var component in toDelete)
            {
                TrackComponents.Remove(component);
            }
            
            // Clear SelectedComponent if it was deleted
            if (SelectedComponent != null && toDelete.Contains(SelectedComponent))
            {
                SelectedComponent = null;
            }
        }

        private List<TrackComponent> _clipboard = new List<TrackComponent>();

        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }

        public void Copy()
        {
            _clipboard.Clear();
            foreach (var component in TrackComponents)
            {
                if (component.IsSelected)
                {
                    _clipboard.Add(component);
                }
            }
        }

        public void Paste()
        {
            if (_clipboard.Count == 0) return;

            // Clear current selection
            ClearSelection();

            // Clone and add components with offset
            foreach (var original in _clipboard)
            {
                TrackComponent clone = null;

                if (original is Signal signal)
                {
                    clone = new Signal
                    {
                        Id = GenerateUniqueId(signal.Id),
                        Type = signal.Type,
                        Direction = signal.Direction,
                        X = signal.X + 20,
                        Y = signal.Y + 20
                    };
                }
                else if (original is Switch sw)
                {
                    clone = new Switch
                    {
                        Id = GenerateUniqueId(sw.Id),
                        SwitchNumber = sw.SwitchNumber,
                        IsReverse = sw.IsReverse,
                        X = sw.X + 20,
                        Y = sw.Y + 20,
                        AX = sw.AX + 20,
                        AY = sw.AY + 20,
                        SX = sw.SX + 20,
                        SY = sw.SY + 20,
                        RX = sw.RX + 20,
                        RY = sw.RY + 20,
                        NX = sw.NX + 20,
                        NY = sw.NY + 20
                    };
                }
                else if (original is TrackCircuit tc)
                {
                    clone = new TrackCircuit
                    {
                        Id = GenerateUniqueId(tc.Id),
                        IsOccupied = tc.IsOccupied,
                        X = tc.X + 20,
                        Y = tc.Y + 20,
                        EndX = tc.EndX + 20,
                        EndY = tc.EndY + 20
                    };
                }

                if (clone != null)
                {
                    clone.IsSelected = true;
                    TrackComponents.Add(clone);
                }
            }
        }

        private string GenerateUniqueId(string baseId)
        {
            // Extract prefix and number
            string prefix = baseId;
            int number = 1;

            int dashIndex = baseId.LastIndexOf('-');
            if (dashIndex >= 0 && dashIndex < baseId.Length - 1)
            {
                prefix = baseId.Substring(0, dashIndex + 1);
                if (int.TryParse(baseId.Substring(dashIndex + 1), out int parsed))
                {
                    number = parsed;
                }
            }

            // Find unique ID
            string newId;
            do
            {
                number++;
                newId = $"{prefix}{number:D2}";
            }
            while (TrackComponents.Any(c => c.Id == newId));

            return newId;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class SignalToolItem
    {
        public SignalType Type { get; set; }
        public string IconPath { get; set; }
        public ICommand Command { get; set; }
    }
}
