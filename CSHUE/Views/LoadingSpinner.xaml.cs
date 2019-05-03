﻿using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class LoadingSpinner
    {
        public LoadingSpinner()
        {
            InitializeComponent();
        }

        public SpinnerStates State
        {
            get => (SpinnerStates)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                "State",
                typeof(SpinnerStates),
                typeof(LoadingSpinner),
                new PropertyMetadata(SpinnerStates.Disabled, OnStatePropertyChanged));

        public enum SpinnerStates
        {
            Loading,
            Hanging,
            Disabled
        }

        public double AngleCanvas
        {
            get => (double)GetValue(AngleCanvasProperty);
            set
            {
                SetValue(AngleCanvasProperty, value);
                SetValue(AngleCanvasHangingProperty, value - 360);
                SetValue(AngleCanvasLoadingProperty, value + 1500);
            }
        }

        public static readonly DependencyProperty AngleCanvasProperty =
            DependencyProperty.Register("AngleCanvas", typeof(double), typeof(LoadingSpinner));

        public double AngleCanvasHanging
        {
            get => (double)GetValue(AngleCanvasHangingProperty);
            set => SetValue(AngleCanvasHangingProperty, value);
        }
        public static readonly DependencyProperty AngleCanvasHangingProperty =
            DependencyProperty.Register("AngleCanvasHanging", typeof(double), typeof(LoadingSpinner));

        public double AngleCanvasLoading
        {
            get => (double)GetValue(AngleCanvasLoadingProperty);
            set => SetValue(AngleCanvasLoadingProperty, value);
        }
        public static readonly DependencyProperty AngleCanvasLoadingProperty =
            DependencyProperty.Register("AngleCanvasLoading", typeof(double), typeof(LoadingSpinner));

        public double AnglePath1
        {
            get => (double)GetValue(AnglePath1Property);
            set => SetValue(AnglePath1Property, value);
        }
        public static readonly DependencyProperty AnglePath1Property =
            DependencyProperty.Register("AnglePath1", typeof(double), typeof(LoadingSpinner));

        public double AnglePath2
        {
            get => (double)GetValue(AnglePath2Property);
            set => SetValue(AnglePath2Property, value);
        }
        public static readonly DependencyProperty AnglePath2Property =
            DependencyProperty.Register("AnglePath2", typeof(double), typeof(LoadingSpinner));

        public double AnglePath3
        {
            get => (double)GetValue(AnglePath3Property);
            set => SetValue(AnglePath3Property, value);
        }
        public static readonly DependencyProperty AnglePath3Property =
            DependencyProperty.Register("AnglePath3", typeof(double), typeof(LoadingSpinner));

        public double AnglePath4
        {
            get => (double)GetValue(AnglePath4Property);
            set => SetValue(AnglePath4Property, value);
        }
        public static readonly DependencyProperty AnglePath4Property =
            DependencyProperty.Register("AnglePath4", typeof(double), typeof(LoadingSpinner));

        public double AnglePath5
        {
            get => (double)GetValue(AnglePath5Property);
            set => SetValue(AnglePath5Property, value);
        }
        public static readonly DependencyProperty AnglePath5Property =
            DependencyProperty.Register("AnglePath5", typeof(double), typeof(LoadingSpinner));

        public double AnglePath6
        {
            get => (double)GetValue(AnglePath6Property);
            set => SetValue(AnglePath6Property, value);
        }
        public static readonly DependencyProperty AnglePath6Property =
            DependencyProperty.Register("AnglePath6", typeof(double), typeof(LoadingSpinner));

        public Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(LoadingSpinner));

        public SweepDirection SweepDirection
        {
            get => (SweepDirection)GetValue(SweepDirectionProperty);
            set => SetValue(SweepDirectionProperty, value);
        }
        public static readonly DependencyProperty SweepDirectionProperty =
            DependencyProperty.Register("SweepDirection", typeof(SweepDirection), typeof(LoadingSpinner));

        public bool IsLargeArc
        {
            get => (bool)GetValue(IsLargeArcProperty);
            set => SetValue(IsLargeArcProperty, value);
        }
        public static readonly DependencyProperty IsLargeArcProperty =
            DependencyProperty.Register("IsLargeArc", typeof(bool), typeof(LoadingSpinner));

        public KeyTime KeyTime
        {
            get => (KeyTime) GetValue(KeyTimeProperty);
            set => SetValue(KeyTimeProperty, value);
        }
        public static readonly DependencyProperty KeyTimeProperty =
            DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(LoadingSpinner));

        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LoadingSpinner)d).AngleCanvas = ((LoadingSpinner) d).RotateCanvas.Angle;
            ((LoadingSpinner)d).AnglePath1 = ((LoadingSpinner)d).RotatePath.Angle;
            ((LoadingSpinner)d).AnglePath2 = ((LoadingSpinner)d).RotatePath.Angle + 276;
            ((LoadingSpinner)d).AnglePath3 = ((LoadingSpinner)d).RotatePath.Angle + 552;
            ((LoadingSpinner)d).AnglePath4 = ((LoadingSpinner)d).RotatePath.Angle + 828;
            ((LoadingSpinner)d).AnglePath5 = ((LoadingSpinner)d).RotatePath.Angle + 1104;
            ((LoadingSpinner)d).AnglePath6 = ((LoadingSpinner)d).RotatePath.Angle + 1380;
            ((LoadingSpinner) d).StartPoint = ((LoadingSpinner) d).Arc.Point;

            switch ((SpinnerStates)e.NewValue)
            {
                case SpinnerStates.Loading:
                {
                    ((LoadingSpinner)d).IsLargeArc = !(((LoadingSpinner)d).Arc.Point.Y > 25);

                    if (((LoadingSpinner)d).Arc.Point.X >= 25)
                    {
                        ((LoadingSpinner) d).KeyTime = TimeSpan.FromMilliseconds(0);
                    }
                    else
                    {
                        var degree = 270 - -1 * (Math.Asin(-1 * ((((LoadingSpinner) d).Arc.Point.Y - 25) / 25)) * 180 / Math.PI - 90);
                        var mili = (int)Math.Round(0.7 / degree * (degree - 90) * 1000);

                        ((LoadingSpinner) d).KeyTime = TimeSpan.FromMilliseconds(mili);
                    }

                    ((LoadingSpinner)d).Path.SetResourceReference(Shape.StrokeProperty, "SystemBaseHighColorBrush");

                    VisualStateManager.GoToState((LoadingSpinner)d, "Loading", true);
                    break;
                }
                case SpinnerStates.Hanging:
                {
                    if (((LoadingSpinner)d).Arc.Point.X > 25)
                    {
                        ((LoadingSpinner)d).SweepDirection = SweepDirection.Clockwise;
                        ((LoadingSpinner)d).IsLargeArc = true;
                    }
                    else
                    {
                        ((LoadingSpinner)d).SweepDirection = SweepDirection.Counterclockwise;
                        ((LoadingSpinner)d).IsLargeArc = false;
                    }

                    if (((LoadingSpinner)d).StartPoint.Y == 50)
                    {
                        ((LoadingSpinner)d).StartPoint = new Point(25.00001, 49.99999);
                    }

                    ((LoadingSpinner)d).Path.SetResourceReference(Shape.StrokeProperty, "SystemBaseMediumColorBrush");
                    ((LoadingSpinner)d).RotatePath.Angle = ((LoadingSpinner)d).RotatePath.Angle;

                    VisualStateManager.GoToState((LoadingSpinner)d, "Hanging", true);
                    break;
                }
                case SpinnerStates.Disabled:
                    VisualStateManager.GoToState((LoadingSpinner)d, "Disabled", true);
                    break;
                default:
                    VisualStateManager.GoToState((LoadingSpinner)d, "Disabled", true);
                    break;
            }
        }
    }
}
