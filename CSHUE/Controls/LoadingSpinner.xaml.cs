using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Controls
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class LoadingSpinner
    {
        #region Dependency Properties

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

        public TimeSpan CycleTime
        {
            get => (TimeSpan)GetValue(CycleTimeProperty);
            set
            {
                CycleTimeDuration = new Duration(value);
                LambdaTimeDuration = new Duration(new TimeSpan(value.Ticks / 10));
                LambdaTime = new TimeSpan(value.Ticks / 10);
                LambdaTime2 = new TimeSpan(value.Ticks / 5);
                LambdaTime3 = new TimeSpan(value.Ticks * 2 / 5);
                LambdaTime4 = new TimeSpan(value.Ticks * 3 / 5);
                LambdaTime5 = new TimeSpan(value.Ticks * 4 / 5);
                LambdaTime6 = new TimeSpan(value.Ticks - value.Ticks / 10);
                SetValue(CycleTimeProperty, value);
            }
        }
        public static readonly DependencyProperty CycleTimeProperty =
            DependencyProperty.Register("CycleTime", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(7), OnStatePropertyChanged));

        public Duration CycleTimeDuration
        {
            get => (Duration)GetValue(CycleTimeDurationProperty);
            set => SetValue(CycleTimeDurationProperty, value);
        }
        public static readonly DependencyProperty CycleTimeDurationProperty =
            DependencyProperty.Register("CycleTimeDuration", typeof(Duration), typeof(LoadingSpinner),
                new PropertyMetadata(new Duration(TimeSpan.FromSeconds(7)), OnStatePropertyChanged));

        public Duration LambdaTimeDuration
        {
            get => (Duration)GetValue(LambdaTimeDurationProperty);
            set => SetValue(LambdaTimeDurationProperty, value);
        }
        public static readonly DependencyProperty LambdaTimeDurationProperty =
            DependencyProperty.Register("LambdaTimeDuration", typeof(Duration), typeof(LoadingSpinner),
                new PropertyMetadata(new Duration(TimeSpan.FromSeconds(.7)), OnStatePropertyChanged));

        public TimeSpan LambdaTime
        {
            get => (TimeSpan)GetValue(LambdaTimeProperty);
            set => SetValue(LambdaTimeProperty, value);
        }
        public static readonly DependencyProperty LambdaTimeProperty =
            DependencyProperty.Register("LambdaTime", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(.7), OnStatePropertyChanged));

        public TimeSpan LambdaTime2
        {
            get => (TimeSpan)GetValue(LambdaTime2Property);
            set => SetValue(LambdaTime2Property, value);
        }
        public static readonly DependencyProperty LambdaTime2Property =
            DependencyProperty.Register("LambdaTime2", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(1.4), OnStatePropertyChanged));

        public TimeSpan LambdaTime3
        {
            get => (TimeSpan)GetValue(LambdaTime3Property);
            set => SetValue(LambdaTime3Property, value);
        }
        public static readonly DependencyProperty LambdaTime3Property =
            DependencyProperty.Register("LambdaTime3", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(2.8), OnStatePropertyChanged));

        public TimeSpan LambdaTime4
        {
            get => (TimeSpan)GetValue(LambdaTime4Property);
            set => SetValue(LambdaTime4Property, value);
        }
        public static readonly DependencyProperty LambdaTime4Property =
            DependencyProperty.Register("LambdaTime4", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(4.2), OnStatePropertyChanged));

        public TimeSpan LambdaTime5
        {
            get => (TimeSpan)GetValue(LambdaTime5Property);
            set => SetValue(LambdaTime5Property, value);
        }
        public static readonly DependencyProperty LambdaTime5Property =
            DependencyProperty.Register("LambdaTime5", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(5.6), OnStatePropertyChanged));

        public TimeSpan LambdaTime6
        {
            get => (TimeSpan)GetValue(LambdaTime6Property);
            set => SetValue(LambdaTime6Property, value);
        }
        public static readonly DependencyProperty LambdaTime6Property =
            DependencyProperty.Register("LambdaTime6", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(6.3), OnStatePropertyChanged));

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set
            {
                Diameter = value * 2;
                Size = new Size(value, value);
                PointRadiusDiameter = new Point(value, value * 2);
                PointDiameterRadius = new Point(value * 2, value);
                StartPoint2 = new Point(value, 0);
                PathGeometry = new PathGeometry(new PathFigureCollection
                {
                    new PathFigure
                    {
                        StartPoint = new Point(value * 2, value),
                        Segments =
                        {
                            new ArcSegment
                            {
                                Size = new Size(value, value),
                                RotationAngle = 0,
                                IsLargeArc = true,
                                SweepDirection = SweepDirection.Clockwise,
                                Point = new Point((1 - Math.Cos(86.4)) * value, (1 - Math.Sin(86.4)) * value)
                            }
                        }
                    }
                });
                SetValue(RadiusProperty, value);
            }
        }
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(LoadingSpinner),
                new PropertyMetadata((double)25, OnStatePropertyChanged));

        public PathGeometry PathGeometry
        {
            get => (PathGeometry)GetValue(PathGeometryProperty);
            set => SetValue(PathGeometryProperty, value);
        }
        public static readonly DependencyProperty PathGeometryProperty =
            DependencyProperty.Register("PathGeometry", typeof(PathGeometry), typeof(LoadingSpinner),
                new PropertyMetadata(new PathGeometry(new PathFigureCollection
                {
                    new PathFigure
                    {
                        StartPoint = new Point(50, 25),
                        Segments =
                        {
                            new ArcSegment
                            {
                                Size = new Size(25, 25),
                                RotationAngle = 0,
                                IsLargeArc = true,
                                SweepDirection = SweepDirection.Clockwise,
                                Point = new Point(23.4302370118, 0.04933178929)
                            }
                        }
                    }
                }), OnStatePropertyChanged));

        public double Diameter
        {
            get => (double)GetValue(DiameterProperty);
            set => SetValue(DiameterProperty, value);
        }
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(LoadingSpinner),
                new PropertyMetadata((double)50, OnStatePropertyChanged));

        public Point PointRadiusDiameter
        {
            get => (Point)GetValue(PointRadiusDiameterProperty);
            set => SetValue(PointRadiusDiameterProperty, value);
        }
        public static readonly DependencyProperty PointRadiusDiameterProperty =
            DependencyProperty.Register("PointRadiusDiameter", typeof(Point), typeof(LoadingSpinner),
                new PropertyMetadata(new Point(25, 50), OnStatePropertyChanged));

        public Point StartPoint2
        {
            get => (Point)GetValue(StartPoint2Property);
            set => SetValue(StartPoint2Property, value);
        }
        public static readonly DependencyProperty StartPoint2Property =
            DependencyProperty.Register("StartPoint2", typeof(Point), typeof(LoadingSpinner),
                new PropertyMetadata(new Point(25, 0), OnStatePropertyChanged));

        public Point PointDiameterRadius
        {
            get => (Point)GetValue(PointDiameterRadiusProperty);
            set => SetValue(PointDiameterRadiusProperty, value);
        }
        public static readonly DependencyProperty PointDiameterRadiusProperty =
            DependencyProperty.Register("PointDiameterRadius", typeof(Point), typeof(LoadingSpinner),
                new PropertyMetadata(new Point(50, 25), OnStatePropertyChanged));

        public Size Size
        {
            get => (Size)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size), typeof(LoadingSpinner),
                new PropertyMetadata(new Size(25, 25), OnStatePropertyChanged));

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
            get => (KeyTime)GetValue(KeyTimeProperty);
            set => SetValue(KeyTimeProperty, value);
        }
        public static readonly DependencyProperty KeyTimeProperty =
            DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(LoadingSpinner));

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

        #endregion

        #region Enums

        public enum SpinnerStates
        {
            Loading,
            Hanging,
            Disabled
        }

        #endregion

        #region Events Handlers

        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LoadingSpinner)d).AngleCanvas = ((LoadingSpinner)d).RotateCanvas.Angle;
            ((LoadingSpinner)d).AnglePath1 = ((LoadingSpinner)d).RotatePath.Angle;
            ((LoadingSpinner)d).AnglePath2 = ((LoadingSpinner)d).RotatePath.Angle + 276;
            ((LoadingSpinner)d).AnglePath3 = ((LoadingSpinner)d).RotatePath.Angle + 552;
            ((LoadingSpinner)d).AnglePath4 = ((LoadingSpinner)d).RotatePath.Angle + 828;
            ((LoadingSpinner)d).AnglePath5 = ((LoadingSpinner)d).RotatePath.Angle + 1104;
            ((LoadingSpinner)d).AnglePath6 = ((LoadingSpinner)d).RotatePath.Angle + 1380;
            ((LoadingSpinner)d).StartPoint = ((LoadingSpinner)d).Arc.Point;

            switch ((SpinnerStates)e.NewValue)
            {
                case SpinnerStates.Loading:
                    {
                        ((LoadingSpinner)d).IsLargeArc = !(((LoadingSpinner)d).Arc.Point.Y > 25);

                        if (((LoadingSpinner)d).Arc.Point.X >= 25)
                        {
                            ((LoadingSpinner)d).KeyTime = TimeSpan.FromMilliseconds(0);
                        }
                        else
                        {
                            var degree = 270 - -1 * (Math.Asin(-1 * ((((LoadingSpinner)d).Arc.Point.Y - 25) / 25)) * 180 / Math.PI - 90);
                            var mili = (int)Math.Round(0.7 / degree * (degree - 90) * 1000);

                            ((LoadingSpinner)d).KeyTime = TimeSpan.FromMilliseconds(mili);
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

        #endregion

        #region Initializers

        public LoadingSpinner()
        {
            InitializeComponent();
        }

        #endregion
    }
}
