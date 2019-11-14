using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CSHUE.AttachedBehaviors
{
    public static class ScrollAnimationBehavior
    {
        #region Fields

        public static DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached("VerticalOffset",
            typeof(double), typeof(ScrollAnimationBehavior), new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));
        public static DependencyProperty TimeDurationProperty = DependencyProperty.RegisterAttached("TimeDuration",
            typeof(TimeSpan), typeof(ScrollAnimationBehavior), new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 0)));
        public static DependencyProperty PointsToScrollProperty = DependencyProperty.RegisterAttached("PointsToScroll",
            typeof(double), typeof(ScrollAnimationBehavior), new PropertyMetadata(0.0));
        public static DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled",
            typeof(bool), typeof(ScrollAnimationBehavior), new UIPropertyMetadata(false, OnIsEnabledChanged));
        private static double _currentToValue;
        private static Storyboard _storyboard;

        #endregion

        #region Events Handlers

        private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = sender;

            if (target is ScrollViewer scroller)
                scroller.Loaded += OnScrollerLoaded;
        }

        private static void OnScrollerLoaded(object sender, RoutedEventArgs e)
        {
            var scroller = sender as ScrollViewer;

            SetEventHandlersForScrollViewer(scroller);
        }

        private static void OnScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var tempToValue = _currentToValue;
            double mouseWheelChange = e.Delta;
            var scroller = (ScrollViewer) sender;
            var offset = mouseWheelChange * 2 / 3;

            if (_storyboard == null || _storyboard.GetCurrentState() == ClockState.Filling)
                _currentToValue = scroller.VerticalOffset;

            _currentToValue = offset > _currentToValue
                ? 0
                : _currentToValue - offset > scroller.ScrollableHeight
                    ? scroller.ScrollableHeight
                    : _currentToValue - offset;

            if (Math.Abs(_currentToValue - scroller.VerticalOffset) > 0 && Math.Abs(_currentToValue - tempToValue) > 0)
                AnimateScroll(scroller);

            e.Handled = true;
        }

        private static void OnScrollViewerPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var scroller = (ScrollViewer) sender;
            var offset = GetPointsToScroll(scroller);
            var keyPressed = e.Key;
            var isKeyHandled = false;

            if (_storyboard == null || _storyboard.GetCurrentState() == ClockState.Filling)
                _currentToValue = scroller.VerticalOffset;
            
            switch (keyPressed)
            {
                case Key.Down:
                case Key.PageDown:
                    offset = offset * -1;
                    _currentToValue = offset > _currentToValue
                        ? 0
                        : _currentToValue - offset > scroller.ScrollableHeight
                            ? scroller.ScrollableHeight
                            : _currentToValue - offset;
                    isKeyHandled = true;
                    break;
                case Key.Up:
                case Key.PageUp:
                    _currentToValue = offset > _currentToValue
                        ? 0
                        : _currentToValue - offset > scroller.ScrollableHeight
                            ? scroller.ScrollableHeight
                            : _currentToValue - offset;
                    isKeyHandled = true;
                    break;
            }

            if (Math.Abs(_currentToValue - scroller.VerticalOffset) > 0)
                AnimateScroll(scroller);

            e.Handled = isKeyHandled;
        }

        #endregion

        #region Methods

        public static void SetTimeDuration(FrameworkElement target, TimeSpan value)
        {
            target.SetValue(TimeDurationProperty, value);
        }

        public static TimeSpan GetTimeDuration(FrameworkElement target)
        {
            return (TimeSpan) target.GetValue(TimeDurationProperty);
        }

        public static void SetPointsToScroll(FrameworkElement target, double value)
        {
            target.SetValue(PointsToScrollProperty, value);
        }

        public static double GetPointsToScroll(FrameworkElement target)
        {
            return (double) target.GetValue(PointsToScrollProperty);
        }

        private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is ScrollViewer scrollViewer)
                scrollViewer.ScrollToVerticalOffset((double) e.NewValue);
        }

        public static void SetIsEnabled(FrameworkElement target, bool value)
        {
            target.SetValue(IsEnabledProperty, value);
        }

        private static void AnimateScroll(ScrollViewer scrollViewer)
        {
            var verticalAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(GetTimeDuration(scrollViewer))
            };

            verticalAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(scrollViewer.VerticalOffset,
                KeyTime.FromPercent(0)));
            verticalAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(_currentToValue, KeyTime.FromPercent(1.0),
                new CubicEase
                {
                    EasingMode = EasingMode.EaseOut
                }));

            _storyboard = new Storyboard();

            _storyboard.Children.Add(verticalAnimation);

            Storyboard.SetTarget(verticalAnimation, scrollViewer);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(VerticalOffsetProperty));

            _storyboard.Begin();
        }

        private static void SetEventHandlersForScrollViewer(IInputElement scroller)
        {
            scroller.PreviewMouseWheel += OnScrollViewerPreviewMouseWheel;
            scroller.PreviewKeyDown += OnScrollViewerPreviewKeyDown;
        }

        #endregion
    }
}
