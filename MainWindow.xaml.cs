using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KoreanRailwayTrackEditor.ViewModels;
using KoreanRailwayTrackEditor.Models;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace KoreanRailwayTrackEditor
{
    public partial class MainWindow : Window
    {
        private bool _isPanning;
        private Point _panStartPoint;
        private bool _isDragging;
        private Point _startPoint;
        private Dictionary<TrackComponent, (double X, double Y, double EndX, double EndY)> _originalPositions = new Dictionary<TrackComponent, (double, double, double, double)>();
        private Dictionary<Switch, (double AX, double AY, double SX, double SY, double RX, double RY, double NX, double NY)> _originalSwitchPositions = new Dictionary<Switch, (double, double, double, double, double, double, double, double)>();
        private TrackComponent _selectedComponent;
        private string _dragMode; // "Whole", "Start", "End", "A", "S", "R", "N"
        private double _zoomLevel = 1.0;
        private const double ZoomMin = 0.1;
        private const double ZoomMax = 5.0;
        private const double ZoomStep = 0.1;
        
        // Rubber-band selection
        private bool _isSelecting;
        private Point _selectionStartPoint;

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += OnWindowKeyDown;
            this.PreviewMouseWheel += OnWindowPreviewMouseWheel;
        }

        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                // Ctrl+C for Copy
                if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    viewModel.Copy();
                    e.Handled = true;
                }
                // Ctrl+V for Paste
                else if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    viewModel.Paste();
                    e.Handled = true;
                }
                // Delete key for Delete
                else if (e.Key == Key.Delete)
                {
                    viewModel.DeleteSelected();
                    e.Handled = true;
                }
            }
        }

        private void OnWindowPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Ctrl + Wheel for zoom
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                double delta = e.Delta > 0 ? ZoomStep : -ZoomStep;
                _zoomLevel = Math.Clamp(_zoomLevel + delta, ZoomMin, ZoomMax);
                
                CanvasScaleTransform.ScaleX = _zoomLevel;
                CanvasScaleTransform.ScaleY = _zoomLevel;
                
                e.Handled = true;
            }
        }

        private void OnCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging)
            {
                _isSelecting = true;
                _selectionStartPoint = e.GetPosition(sender as UIElement);
                
                SelectionBox.Visibility = Visibility.Visible;
                Canvas.SetLeft(SelectionBox, _selectionStartPoint.X);
                Canvas.SetTop(SelectionBox, _selectionStartPoint.Y);
                SelectionBox.Width = 0;
                SelectionBox.Height = 0;
                
                // Clear selection if not holding Ctrl
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
                {
                    if (DataContext is MainViewModel viewModel)
                    {
                        viewModel.ClearSelection();
                    }
                }
                
                ((UIElement)sender).CaptureMouse();
            }
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _isPanning = true;
                _panStartPoint = e.GetPosition(this);
                ((UIElement)sender).CaptureMouse();
                e.Handled = true;
            }
        }

        private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _isPanning = false;
                ((UIElement)sender).ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                Point currentPoint = e.GetPosition(this);
                double diffX = currentPoint.X - _panStartPoint.X;
                double diffY = currentPoint.Y - _panStartPoint.Y;

                double gridSize = 10.0;

                if (Math.Abs(diffX) >= gridSize)
                {
                    double stepX = Math.Truncate(diffX / gridSize) * gridSize;
                    CanvasTranslateTransform.X += stepX;
                    _panStartPoint.X += stepX;
                }

                if (Math.Abs(diffY) >= gridSize)
                {
                    double stepY = Math.Truncate(diffY / gridSize) * gridSize;
                    CanvasTranslateTransform.Y += stepY;
                    _panStartPoint.Y += stepY;
                }

                e.Handled = true;
                return;
            }

            if (_isSelecting)
            {
                Point currentPoint = e.GetPosition(sender as UIElement);
                double x = Math.Min(currentPoint.X, _selectionStartPoint.X);
                double y = Math.Min(currentPoint.Y, _selectionStartPoint.Y);
                double w = Math.Abs(currentPoint.X - _selectionStartPoint.X);
                double h = Math.Abs(currentPoint.Y - _selectionStartPoint.Y);

                Canvas.SetLeft(SelectionBox, x);
                Canvas.SetTop(SelectionBox, y);
                SelectionBox.Width = w;
                SelectionBox.Height = h;
            }
        }

        private void OnCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isSelecting)
            {
                _isSelecting = false;
                SelectionBox.Visibility = Visibility.Collapsed;
                ((UIElement)sender).ReleaseMouseCapture();

                // Select items within the box
                double x = Canvas.GetLeft(SelectionBox);
                double y = Canvas.GetTop(SelectionBox);
                double w = SelectionBox.Width;
                double h = SelectionBox.Height;
                Rect selectionRect = new Rect(x, y, w, h);

                if (DataContext is MainViewModel viewModel)
                {
                    foreach (var component in viewModel.TrackComponents)
                    {
                        Rect itemRect = new Rect(component.X, component.Y, 20, 20); 
                        if (component is TrackCircuit tc) 
                        {
                            double minX = Math.Min(tc.X, tc.EndX);
                            double minY = Math.Min(tc.Y, tc.EndY);
                            double maxX = Math.Max(tc.X, tc.EndX);
                            double maxY = Math.Max(tc.Y, tc.EndY);
                            itemRect = new Rect(minX, minY, maxX - minX + 20, maxY - minY + 20); 
                        }
                        
                        if (selectionRect.IntersectsWith(itemRect))
                        {
                            component.IsSelected = true;
                        }
                    }
                }
            }
        }

        private void OnComponentMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ContentPresenter contentPresenter && contentPresenter.DataContext is TrackComponent component)
            {
                _isDragging = true;
                _startPoint = e.GetPosition(contentPresenter.Parent as UIElement);
                _selectedComponent = component;
                
                // Selection Logic
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
                {
                    if (!component.IsSelected)
                    {
                        if (DataContext is MainViewModel vm) vm.ClearSelection();
                        component.IsSelected = true;
                    }
                    // Update SelectedComponent for property panel
                    if (DataContext is MainViewModel vm2) vm2.SelectedComponent = component;
                }
                else
                {
                    component.IsSelected = !component.IsSelected;
                    // Update SelectedComponent if this component is now selected
                    if (component.IsSelected && DataContext is MainViewModel vm3)
                    {
                        vm3.SelectedComponent = component;
                    }
                }

                // Store original positions for ALL selected components
                _originalPositions.Clear();
                _originalSwitchPositions.Clear();
                if (DataContext is MainViewModel viewModel)
                {
                    foreach (var item in viewModel.TrackComponents)
                    {
                        if (item.IsSelected)
                        {
                            double endX = 0, endY = 0;
                            if (item is TrackCircuit tc) { endX = tc.EndX; endY = tc.EndY; }
                            _originalPositions[item] = (item.X, item.Y, endX, endY);
                            
                            // Store Switch anchor positions
                            if (item is Switch sw)
                            {
                                _originalSwitchPositions[sw] = (sw.AX, sw.AY, sw.SX, sw.SY, sw.RX, sw.RY, sw.NX, sw.NY);
                            }
                        }
                    }
                    
                    if (!component.IsSelected && !_originalPositions.ContainsKey(component))
                    {
                        double endX = 0, endY = 0;
                        if (component is TrackCircuit tc) { endX = tc.EndX; endY = tc.EndY; }
                        _originalPositions[component] = (component.X, component.Y, endX, endY);
                        
                        if (component is Switch sw)
                        {
                            _originalSwitchPositions[sw] = (sw.AX, sw.AY, sw.SX, sw.SY, sw.RX, sw.RY, sw.NX, sw.NY);
                        }
                    }
                }

                // Determine drag mode
                if (component is TrackCircuit circuit)
                {
                    if (e.OriginalSource is FrameworkElement element && (element.Tag as string) == "Start")
                    {
                        _dragMode = "Start";
                    }
                    else if (e.OriginalSource is FrameworkElement element2 && (element2.Tag as string) == "End")
                    {
                        _dragMode = "End";
                    }
                    else
                    {
                        _dragMode = "Whole";
                    }
                }
                else if (component is Switch switchComponent)
                {
                    // Check which anchor point was clicked
                    if (e.OriginalSource is FrameworkElement element)
                    {
                        string tag = element.Tag as string;
                        if (tag == "A" || tag == "S" || tag == "R" || tag == "N")
                        {
                            _dragMode = tag; // "A", "S", "R", or "N"
                        }
                        else
                        {
                            _dragMode = "Whole";
                        }
                    }
                    else
                    {
                        _dragMode = "Whole";
                    }
                }
                else
                {
                    _dragMode = "Whole";
                }

                contentPresenter.CaptureMouse();
                e.Handled = true;
            }
        }

        private void OnComponentMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _selectedComponent != null && sender is ContentPresenter contentPresenter)
            {
                Point currentPoint = e.GetPosition(contentPresenter.Parent as UIElement);
                double offsetX = (currentPoint.X - _startPoint.X) / _zoomLevel;
                double offsetY = (currentPoint.Y - _startPoint.Y) / _zoomLevel;

                if (!_originalPositions.ContainsKey(_selectedComponent)) return;
                var origPrimary = _originalPositions[_selectedComponent];
                
                double newPrimaryX = origPrimary.X + offsetX;
                double newPrimaryY = origPrimary.Y + offsetY;
                
                // Snap primary
                newPrimaryX = Math.Round(newPrimaryX / 10.0) * 10.0;
                newPrimaryY = Math.Round(newPrimaryY / 10.0) * 10.0;
                
                double snappedDeltaX = newPrimaryX - origPrimary.X;
                double snappedDeltaY = newPrimaryY - origPrimary.Y;

                if (_dragMode == "Whole")
                {
                    foreach (var kvp in _originalPositions)
                    {
                        var item = kvp.Key;
                        var orig = kvp.Value;
                        
                        item.X = orig.X + snappedDeltaX;
                        item.Y = orig.Y + snappedDeltaY;
                        
                        if (item is TrackCircuit tc)
                        {
                            tc.EndX = orig.EndX + snappedDeltaX;
                            tc.EndY = orig.EndY + snappedDeltaY;
                        }
                        
                        // Move Switch anchors
                        if (item is Switch sw && _originalSwitchPositions.ContainsKey(sw))
                        {
                            var origSw = _originalSwitchPositions[sw];
                            sw.AX = origSw.AX + snappedDeltaX;
                            sw.AY = origSw.AY + snappedDeltaY;
                            sw.SX = origSw.SX + snappedDeltaX;
                            sw.SY = origSw.SY + snappedDeltaY;
                            sw.RX = origSw.RX + snappedDeltaX;
                            sw.RY = origSw.RY + snappedDeltaY;
                            sw.NX = origSw.NX + snappedDeltaX;
                            sw.NY = origSw.NY + snappedDeltaY;
                        }
                    }
                }
                else if (_selectedComponent is TrackCircuit circuit)
                {
                    if (_dragMode == "Start")
                    {
                        double newX = origPrimary.X + offsetX;
                        double newY = origPrimary.Y + offsetY;
                        newX = Math.Round(newX / 10.0) * 10.0;
                        newY = Math.Round(newY / 10.0) * 10.0;
                        
                        if (newX != circuit.X || newY != circuit.Y)
                        {
                            circuit.X = newX;
                            circuit.Y = newY;
                        }
                    }
                    else if (_dragMode == "End")
                    {
                        double newEndX = origPrimary.EndX + offsetX;
                        double newEndY = origPrimary.EndY + offsetY;
                        newEndX = Math.Round(newEndX / 10.0) * 10.0;
                        newEndY = Math.Round(newEndY / 10.0) * 10.0;
                        
                        if (newEndX != circuit.EndX || newEndY != circuit.EndY)
                        {
                            circuit.EndX = newEndX;
                            circuit.EndY = newEndY;
                        }
                    }
                }
                else if (_selectedComponent is Switch switchComp && _originalSwitchPositions.ContainsKey(switchComp))
                {
                    var origSw = _originalSwitchPositions[switchComp];
                    
                    // Drag individual anchor points
                    if (_dragMode == "A")
                    {
                        double newAX = origSw.AX + offsetX;
                        double newAY = origSw.AY + offsetY;
                        newAX = Math.Round(newAX / 10.0) * 10.0;
                        newAY = Math.Round(newAY / 10.0) * 10.0;
                        switchComp.AX = newAX;
                        switchComp.AY = newAY;
                    }
                    else if (_dragMode == "S")
                    {
                        double newSX = origSw.SX + offsetX;
                        double newSY = origSw.SY + offsetY;
                        newSX = Math.Round(newSX / 10.0) * 10.0;
                        newSY = Math.Round(newSY / 10.0) * 10.0;
                        switchComp.SX = newSX;
                        switchComp.SY = newSY;
                    }
                    else if (_dragMode == "R")
                    {
                        double newRX = origSw.RX + offsetX;
                        double newRY = origSw.RY + offsetY;
                        newRX = Math.Round(newRX / 10.0) * 10.0;
                        newRY = Math.Round(newRY / 10.0) * 10.0;
                        switchComp.RX = newRX;
                        switchComp.RY = newRY;
                    }
                    else if (_dragMode == "N")
                    {
                        double newNX = origSw.NX + offsetX;
                        double newNY = origSw.NY + offsetY;
                        newNX = Math.Round(newNX / 10.0) * 10.0;
                        newNY = Math.Round(newNY / 10.0) * 10.0;
                        switchComp.NX = newNX;
                        switchComp.NY = newNY;
                    }
                }
            }
        }

        private void OnComponentMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging && sender is ContentPresenter contentPresenter)
            {
                _isDragging = false;
                _selectedComponent = null;
                _originalPositions.Clear();
                _originalSwitchPositions.Clear();
                contentPresenter.ReleaseMouseCapture();
            }
        }
    }
}