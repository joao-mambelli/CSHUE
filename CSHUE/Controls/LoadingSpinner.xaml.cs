using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable InheritdocConsiderUsage
// ReSharper disable UnusedMember.Global

namespace CSHUE.Controls
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class LoadingSpinner
    {
        #region Public Dependency Properties

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

        public Color LoadingColor
        {
            get => (Color)GetValue(LoadingColorProperty);
            set => SetValue(LoadingColorProperty, value);
        }
        public static readonly DependencyProperty LoadingColorProperty =
            DependencyProperty.Register("LoadingColor", typeof(Color), typeof(LoadingSpinner),
                new PropertyMetadata(Colors.White));

        public Color HangingColor
        {
            get => (Color)GetValue(HangingColorProperty);
            set => SetValue(HangingColorProperty, value);
        }
        public static readonly DependencyProperty HangingColorProperty =
            DependencyProperty.Register("HangingColor", typeof(Color), typeof(LoadingSpinner),
                new PropertyMetadata(Colors.Gray));

        public double LoadingRevolutionsPerSecond
        {
            get => (double)GetValue(LoadingRevolutionsPerSecondProperty);
            set => SetValue(LoadingRevolutionsPerSecondProperty, value);
        }
        public static readonly DependencyProperty LoadingRevolutionsPerSecondProperty =
            DependencyProperty.Register("LoadingRevolutionsPerSecond", typeof(double), typeof(LoadingSpinner),
                new PropertyMetadata((double)5 / 7, OnLoadingRevolutionsPerSecondPropertyChanged));

        public double HangingRevolutionsPerSecond
        {
            get => (double)GetValue(HangingRevolutionsPerSecondProperty);
            set => SetValue(HangingRevolutionsPerSecondProperty, value);
        }
        public static readonly DependencyProperty HangingRevolutionsPerSecondProperty =
            DependencyProperty.Register("HangingRevolutionsPerSecond", typeof(double), typeof(LoadingSpinner),
                new PropertyMetadata(0.5, OnHangingRevolutionsPerSecondPropertyChanged));

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(LoadingSpinner),
                new PropertyMetadata((double)25, OnRadiusPropertyChanged));

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LoadingSpinner),
                new PropertyMetadata((double)5));

        #endregion

        #region Private Properties

        private double AngleCanvas
        {
            get => (double)GetValue(AngleCanvasProperty);
            set
            {
                SetValue(AngleCanvasProperty, value);
                SetValue(AngleCanvasHangingProperty, value - 360);
                SetValue(AngleCanvasLoadingProperty, value + 1500);
            }
        }
        private static readonly DependencyProperty AngleCanvasProperty =
            DependencyProperty.Register("AngleCanvas", typeof(double), typeof(LoadingSpinner));

        private Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        private static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(LoadingSpinner),
                new PropertyMetadata(Colors.White));

        private Duration HangingCycleTimeDuration
        {
            get => (Duration)GetValue(HangingCycleTimeDurationProperty);
            set => SetValue(HangingCycleTimeDurationProperty, value);
        }
        private static readonly DependencyProperty HangingCycleTimeDurationProperty =
            DependencyProperty.Register("HangingCycleTimeDuration", typeof(Duration), typeof(LoadingSpinner),
                new PropertyMetadata(new Duration(TimeSpan.FromSeconds(2))));

        private Duration CycleTimeDuration
        {
            get => (Duration)GetValue(CycleTimeDurationProperty);
            set => SetValue(CycleTimeDurationProperty, value);
        }
        private static readonly DependencyProperty CycleTimeDurationProperty =
            DependencyProperty.Register("CycleTimeDuration", typeof(Duration), typeof(LoadingSpinner),
                new PropertyMetadata(new Duration(TimeSpan.FromSeconds(7))));

        private Duration LambdaTimeDuration
        {
            get => (Duration)GetValue(LambdaTimeDurationProperty);
            set => SetValue(LambdaTimeDurationProperty, value);
        }
        private static readonly DependencyProperty LambdaTimeDurationProperty =
            DependencyProperty.Register("LambdaTimeDuration", typeof(Duration), typeof(LoadingSpinner),
                new PropertyMetadata(new Duration(TimeSpan.FromSeconds(.7))));

        private TimeSpan LambdaTime
        {
            get => (TimeSpan)GetValue(LambdaTimeProperty);
            set => SetValue(LambdaTimeProperty, value);
        }
        private static readonly DependencyProperty LambdaTimeProperty =
            DependencyProperty.Register("LambdaTime", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(.7)));

        private TimeSpan LambdaTime2
        {
            get => (TimeSpan)GetValue(LambdaTime2Property);
            set => SetValue(LambdaTime2Property, value);
        }
        private static readonly DependencyProperty LambdaTime2Property =
            DependencyProperty.Register("LambdaTime2", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(1.4)));

        private TimeSpan LambdaTime3
        {
            get => (TimeSpan)GetValue(LambdaTime3Property);
            set => SetValue(LambdaTime3Property, value);
        }
        private static readonly DependencyProperty LambdaTime3Property =
            DependencyProperty.Register("LambdaTime3", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(2.8)));

        private TimeSpan LambdaTime4
        {
            get => (TimeSpan)GetValue(LambdaTime4Property);
            set => SetValue(LambdaTime4Property, value);
        }
        private static readonly DependencyProperty LambdaTime4Property =
            DependencyProperty.Register("LambdaTime4", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(4.2)));

        private TimeSpan LambdaTime5
        {
            get => (TimeSpan)GetValue(LambdaTime5Property);
            set => SetValue(LambdaTime5Property, value);
        }
        private static readonly DependencyProperty LambdaTime5Property =
            DependencyProperty.Register("LambdaTime5", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(5.6)));

        private TimeSpan LambdaTime6
        {
            get => (TimeSpan)GetValue(LambdaTime6Property);
            set => SetValue(LambdaTime6Property, value);
        }
        private static readonly DependencyProperty LambdaTime6Property =
            DependencyProperty.Register("LambdaTime6", typeof(TimeSpan), typeof(LoadingSpinner),
                new PropertyMetadata(TimeSpan.FromSeconds(6.3)));

        private Point EndPoint
        {
            get => (Point)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }
        private static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Point), typeof(LoadingSpinner),
                new PropertyMetadata(new Point((1 - Math.Cos(Math.PI * 86.4 / 180)) * 25, (1 - Math.Sin(Math.PI * 86.4 / 180)) * 25)));

        private double Diameter
        {
            get => (double)GetValue(DiameterProperty);
            set => SetValue(DiameterProperty, value);
        }
        private static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(LoadingSpinner),
                new PropertyMetadata((double)50));

        private Point PointRadiusDiameter
        {
            get => (Point)GetValue(PointRadiusDiameterProperty);
            set => SetValue(PointRadiusDiameterProperty, value);
        }
        private static readonly DependencyProperty PointRadiusDiameterProperty =
            DependencyProperty.Register("PointRadiusDiameter", typeof(Point), typeof(LoadingSpinner),
                new PropertyMetadata(new Point(25, 50)));

        private Point StartPoint2
        {
            get => (Point)GetValue(StartPoint2Property);
            set => SetValue(StartPoint2Property, value);
        }
        private static readonly DependencyProperty StartPoint2Property =
            DependencyProperty.Register("StartPoint2", typeof(Point), typeof(LoadingSpinner),
                new PropertyMetadata(new Point(25, 0)));

        private Point PointDiameterRadius
        {
            get => (Point)GetValue(PointDiameterRadiusProperty);
            set => SetValue(PointDiameterRadiusProperty, value);
        }
        private static readonly DependencyProperty PointDiameterRadiusProperty =
            DependencyProperty.Register("PointDiameterRadius", typeof(Point), typeof(LoadingSpinner),
                new PropertyMetadata(new Point(50, 25)));

        private Size Size
        {
            get => (Size)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }
        private static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size), typeof(LoadingSpinner),
                new PropertyMetadata(new Size(25, 25)));

        private double AngleCanvasHanging
        {
            get => (double)GetValue(AngleCanvasHangingProperty);
            set => SetValue(AngleCanvasHangingProperty, value);
        }
        private static readonly DependencyProperty AngleCanvasHangingProperty =
            DependencyProperty.Register("AngleCanvasHanging", typeof(double), typeof(LoadingSpinner));

        private double AngleCanvasLoading
        {
            get => (double)GetValue(AngleCanvasLoadingProperty);
            set => SetValue(AngleCanvasLoadingProperty, value);
        }
        private static readonly DependencyProperty AngleCanvasLoadingProperty =
            DependencyProperty.Register("AngleCanvasLoading", typeof(double), typeof(LoadingSpinner));

        private double AnglePath1
        {
            get => (double)GetValue(AnglePath1Property);
            set => SetValue(AnglePath1Property, value);
        }
        private static readonly DependencyProperty AnglePath1Property =
            DependencyProperty.Register("AnglePath1", typeof(double), typeof(LoadingSpinner));

        private double AnglePath2
        {
            get => (double)GetValue(AnglePath2Property);
            set => SetValue(AnglePath2Property, value);
        }
        private static readonly DependencyProperty AnglePath2Property =
            DependencyProperty.Register("AnglePath2", typeof(double), typeof(LoadingSpinner));

        private double AnglePath3
        {
            get => (double)GetValue(AnglePath3Property);
            set => SetValue(AnglePath3Property, value);
        }
        private static readonly DependencyProperty AnglePath3Property =
            DependencyProperty.Register("AnglePath3", typeof(double), typeof(LoadingSpinner));

        private double AnglePath4
        {
            get => (double)GetValue(AnglePath4Property);
            set => SetValue(AnglePath4Property, value);
        }
        private static readonly DependencyProperty AnglePath4Property =
            DependencyProperty.Register("AnglePath4", typeof(double), typeof(LoadingSpinner));

        private double AnglePath5
        {
            get => (double)GetValue(AnglePath5Property);
            set => SetValue(AnglePath5Property, value);
        }
        private static readonly DependencyProperty AnglePath5Property =
            DependencyProperty.Register("AnglePath5", typeof(double), typeof(LoadingSpinner));

        private double AnglePath6
        {
            get => (double)GetValue(AnglePath6Property);
            set => SetValue(AnglePath6Property, value);
        }
        private static readonly DependencyProperty AnglePath6Property =
            DependencyProperty.Register("AnglePath6", typeof(double), typeof(LoadingSpinner));

        private Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }
        private static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(LoadingSpinner));

        private SweepDirection SweepDirection
        {
            get => (SweepDirection)GetValue(SweepDirectionProperty);
            set => SetValue(SweepDirectionProperty, value);
        }
        private static readonly DependencyProperty SweepDirectionProperty =
            DependencyProperty.Register("SweepDirection", typeof(SweepDirection), typeof(LoadingSpinner));

        private bool IsLargeArc
        {
            get => (bool)GetValue(IsLargeArcProperty);
            set => SetValue(IsLargeArcProperty, value);
        }
        private static readonly DependencyProperty IsLargeArcProperty =
            DependencyProperty.Register("IsLargeArc", typeof(bool), typeof(LoadingSpinner));

        private KeyTime KeyTime
        {
            get => (KeyTime)GetValue(KeyTimeProperty);
            set => SetValue(KeyTimeProperty, value);
        }
        private static readonly DependencyProperty KeyTimeProperty =
            DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(LoadingSpinner));

        private KeyTime KeyTime2
        {
            get => (KeyTime)GetValue(KeyTime2Property);
            set => SetValue(KeyTime2Property, value);
        }
        private static readonly DependencyProperty KeyTime2Property =
            DependencyProperty.Register("KeyTime2", typeof(KeyTime), typeof(LoadingSpinner),
                new PropertyMetadata((KeyTime) new TimeSpan((long)Math.Round(TimeSpan.FromSeconds(.7).Ticks * ((double)2380950 / 999000 / 7)))));

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

        private static void OnLoadingRevolutionsPerSecondPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var animationPeriod = 5 / (double) e.NewValue;

            ((LoadingSpinner)d).CycleTimeDuration = new Duration(TimeSpan.FromSeconds(animationPeriod));
            ((LoadingSpinner)d).LambdaTimeDuration = new Duration(new TimeSpan(TimeSpan.FromSeconds(animationPeriod).Ticks / 10));
            ((LoadingSpinner)d).LambdaTime = new TimeSpan(TimeSpan.FromSeconds(animationPeriod).Ticks / 10);
            ((LoadingSpinner)d).LambdaTime2 = new TimeSpan(TimeSpan.FromSeconds(animationPeriod).Ticks / 5);
            ((LoadingSpinner)d).LambdaTime3 = new TimeSpan(TimeSpan.FromSeconds(animationPeriod).Ticks * 2 / 5);
            ((LoadingSpinner)d).LambdaTime4 = new TimeSpan(TimeSpan.FromSeconds(animationPeriod).Ticks * 3 / 5);
            ((LoadingSpinner)d).LambdaTime5 = new TimeSpan(TimeSpan.FromSeconds(animationPeriod).Ticks * 4 / 5);
            ((LoadingSpinner)d).LambdaTime6 = new TimeSpan(TimeSpan.FromSeconds(animationPeriod).Ticks - TimeSpan.FromSeconds(animationPeriod).Ticks / 10);
            ((LoadingSpinner)d).KeyTime2 = new TimeSpan((long)Math.Round(((LoadingSpinner)d).LambdaTime.Ticks * ((double)2380950 / 999000 / 7)));
        }

        private static void OnHangingRevolutionsPerSecondPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var animationPeriod = 1 / (double)e.NewValue;

            ((LoadingSpinner)d).HangingCycleTimeDuration = new Duration(TimeSpan.FromSeconds(animationPeriod));
        }

        private static void OnRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LoadingSpinner)d).Diameter = (double)e.NewValue * 2;
            ((LoadingSpinner)d).Size = new Size((double)e.NewValue, (double)e.NewValue);
            ((LoadingSpinner)d).PointRadiusDiameter = new Point((double)e.NewValue, (double)e.NewValue * 2);
            ((LoadingSpinner)d).PointDiameterRadius = new Point((double)e.NewValue * 2, (double)e.NewValue);
            ((LoadingSpinner)d).StartPoint2 = new Point((double)e.NewValue, 0);
            ((LoadingSpinner)d).EndPoint = new Point((1 - Math.Cos(Math.PI * 86.4 / 180)) * (double)e.NewValue, (1 - Math.Sin(Math.PI * 86.4 / 180)) * (double)e.NewValue);
        }

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
                        ((LoadingSpinner)d).IsLargeArc = !(((LoadingSpinner)d).Arc.Point.Y > ((LoadingSpinner)d).Radius);

                        if (((LoadingSpinner)d).Arc.Point.X >= ((LoadingSpinner)d).Radius)
                        {
                            ((LoadingSpinner)d).KeyTime = TimeSpan.FromMilliseconds(0);
                        }
                        else
                        {
                            var degree = 270 - -1 * (Math.Asin(-1 * ((((LoadingSpinner)d).Arc.Point.Y - ((LoadingSpinner)d).Radius) / ((LoadingSpinner)d).Radius)) * 180 / Math.PI - 90);
                            var mili = (int)Math.Round(0.7 / degree * (degree - 90) * 1000);

                            ((LoadingSpinner)d).KeyTime = TimeSpan.FromMilliseconds(mili);
                        }

                        ((LoadingSpinner) d).Color = ((LoadingSpinner) d).LoadingColor;

                        VisualStateManager.GoToState((LoadingSpinner)d, "Loading", true);
                        break;
                    }
                case SpinnerStates.Hanging:
                    {
                        if (((LoadingSpinner)d).Arc.Point.X > ((LoadingSpinner)d).Radius)
                        {
                            ((LoadingSpinner)d).SweepDirection = SweepDirection.Clockwise;
                            ((LoadingSpinner)d).IsLargeArc = true;
                        }
                        else
                        {
                            ((LoadingSpinner)d).SweepDirection = SweepDirection.Counterclockwise;
                            ((LoadingSpinner)d).IsLargeArc = false;
                        }

                        if (((LoadingSpinner)d).StartPoint.Y == ((LoadingSpinner)d).Radius * 2)
                        {
                            ((LoadingSpinner)d).StartPoint = new Point(((LoadingSpinner)d).Radius + .00001, ((LoadingSpinner)d).Radius * 2 - .00001);
                        }

                        ((LoadingSpinner)d).Color = ((LoadingSpinner)d).HangingColor;
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
