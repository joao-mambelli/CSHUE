using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CSHUE.AttachedBehaviors
{
    public static class ScrollAnimationBehavior
    {
        /*#region Private ScrollViewer for ListBox

        private static ScrollViewer _listBoxScroller = new ScrollViewer();

        #endregion*/

        #region VerticalOffset Property

        public static DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset",
                                                typeof(double),
                                                typeof(ScrollAnimationBehavior),
                                                new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }

        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }

        #endregion

        #region TimeDuration Property

        public static DependencyProperty TimeDurationProperty =
            DependencyProperty.RegisterAttached("TimeDuration",
                                                typeof(TimeSpan),
                                                typeof(ScrollAnimationBehavior),
                                                new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 0)));

        public static void SetTimeDuration(FrameworkElement target, TimeSpan value)
        {
            target.SetValue(TimeDurationProperty, value);
        }

        public static TimeSpan GetTimeDuration(FrameworkElement target)
        {
            return (TimeSpan)target.GetValue(TimeDurationProperty);
        }

        #endregion

        #region PointsToScroll Property

        public static DependencyProperty PointsToScrollProperty =
            DependencyProperty.RegisterAttached("PointsToScroll",
                                                typeof(double),
                                                typeof(ScrollAnimationBehavior),
                                                new PropertyMetadata(0.0));

        public static void SetPointsToScroll(FrameworkElement target, double value)
        {
            target.SetValue(PointsToScrollProperty, value);
        }

        public static double GetPointsToScroll(FrameworkElement target)
        {
            return (double)target.GetValue(PointsToScrollProperty);
        }

        #endregion

        #region OnVerticalOffset Changed

        private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }

        #endregion

        #region IsEnabled Property

        public static DependencyProperty IsEnabledProperty =
                                                DependencyProperty.RegisterAttached("IsEnabled",
                                                typeof(bool),
                                                typeof(ScrollAnimationBehavior),
                                                new UIPropertyMetadata(false, OnIsEnabledChanged));

        public static void SetIsEnabled(FrameworkElement target, bool value)
        {
            target.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(FrameworkElement target)
        {
            return (bool)target.GetValue(IsEnabledProperty);
        }

        #endregion

        #region OnIsEnabledChanged Changed

        private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = sender;

            if (target != null && target is ScrollViewer)
            {
                ScrollViewer scroller = target as ScrollViewer;
                scroller.Loaded += new RoutedEventHandler(ScrollerLoaded);
            }

            /*if (target != null && target is ListBox)
            {
                ListBox listbox = target as ListBox;
                listbox.Loaded += new RoutedEventHandler(ListboxLoaded);
            }*/
        }

        #endregion

        #region AnimateScroll Helper

        private static double CurrentToValue = 0;

        private static Storyboard storyboard;

        static void AnimateScroll(ScrollViewer scrollViewer)
        {
            DoubleAnimationUsingKeyFrames verticalAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(GetTimeDuration(scrollViewer))
            };

            verticalAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(scrollViewer.VerticalOffset, KeyTime.FromPercent(0)));
            verticalAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(CurrentToValue, KeyTime.FromPercent(1.0), new SineEase()
            {
                EasingMode = EasingMode.EaseInOut
            }));

            storyboard = new Storyboard();

            storyboard.Children.Add(verticalAnimation);
            Storyboard.SetTarget(verticalAnimation, scrollViewer);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(VerticalOffsetProperty));
            storyboard.Begin();
        }

        #endregion

        /*#region UpdateScrollPosition Helper

        private static void UpdateScrollPosition(object sender)
        {
            if (sender is ListBox listbox)
            {
                double scrollTo = 0;

                for (int i = 0; i < (listbox.SelectedIndex); i++)
                {
                    if (listbox.ItemContainerGenerator.ContainerFromItem(listbox.Items[i]) is ListBoxItem tempItem)
                    {
                        scrollTo += tempItem.ActualHeight;
                    }
                }

                AnimateScroll(_listBoxScroller, scrollTo);
            }
        }

        #endregion*/

        #region SetEventHandlersForScrollViewer Helper

        private static void SetEventHandlersForScrollViewer(ScrollViewer scroller)
        {
            scroller.PreviewMouseWheel += new MouseWheelEventHandler(ScrollViewerPreviewMouseWheel);
            scroller.PreviewKeyDown += new KeyEventHandler(ScrollViewerPreviewKeyDown);
        }

        #endregion

        #region scrollerLoaded Event Handler

        private static void ScrollerLoaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer scroller = sender as ScrollViewer;

            SetEventHandlersForScrollViewer(scroller);
        }

        #endregion

        /*#region listboxLoaded Event Handler

        private static void ListboxLoaded(object sender, RoutedEventArgs e)
        {
            ListBox listbox = sender as ListBox;

            _listBoxScroller = FindVisualChildHelper.GetFirstChildOfType<ScrollViewer>(listbox);
            SetEventHandlersForScrollViewer(_listBoxScroller);

            SetTimeDuration(_listBoxScroller, new TimeSpan(0, 0, 0, 0, 200));
            SetPointsToScroll(_listBoxScroller, 16.0);

            listbox.SelectionChanged += new SelectionChangedEventHandler(ListBoxSelectionChanged);
            listbox.Loaded += new RoutedEventHandler(ListBoxLoaded);
            listbox.LayoutUpdated += new EventHandler(ListBoxLayoutUpdated);
        }

        #endregion*/

        #region ScrollViewerPreviewMouseWheel Event Handler

        private static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var TempToValue = CurrentToValue;

            double mouseWheelChange = (double)e.Delta;
            ScrollViewer scroller = (ScrollViewer)sender;
            var offset = mouseWheelChange * 2 / 3;

            if (storyboard == null || storyboard.GetCurrentState() == ClockState.Filling)
                CurrentToValue = scroller.VerticalOffset;

            CurrentToValue = offset > CurrentToValue ?
                0 :
                (CurrentToValue - offset > scroller.ScrollableHeight ?
                    scroller.ScrollableHeight :
                    CurrentToValue - offset);

            if (CurrentToValue != scroller.VerticalOffset && CurrentToValue != TempToValue)
            {
                AnimateScroll(scroller);
            }

            e.Handled = true;
        }

        #endregion

        #region ScrollViewerPreviewKeyDown Handler

        private static void ScrollViewerPreviewKeyDown(object sender, KeyEventArgs e)
        {
            ScrollViewer scroller = (ScrollViewer)sender;
            var offset = GetPointsToScroll(scroller);

            Key keyPressed = e.Key;
            bool isKeyHandled = false;

            if (storyboard == null || storyboard.GetCurrentState() == ClockState.Filling)
                CurrentToValue = scroller.VerticalOffset;

            if (keyPressed == Key.Down || keyPressed == Key.PageDown)
            {
                offset = offset * -1;

                CurrentToValue = offset > CurrentToValue ?
                    0 :
                    (CurrentToValue - offset > scroller.ScrollableHeight ?
                        scroller.ScrollableHeight :
                        CurrentToValue - offset);

                isKeyHandled = true;
            }
            else if(keyPressed == Key.Up || keyPressed == Key.PageUp)
            {
                CurrentToValue = offset > CurrentToValue ?
                    0 :
                    (CurrentToValue - offset > scroller.ScrollableHeight ?
                        scroller.ScrollableHeight :
                        CurrentToValue - offset);

                isKeyHandled = true;
            }

            if (CurrentToValue != scroller.VerticalOffset)
            {
                AnimateScroll(scroller);
            }

            e.Handled = isKeyHandled;
        }

        #endregion

        /*#region ListBox Event Handlers

        private static void ListBoxLayoutUpdated(object sender, EventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        private static void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        private static void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        #endregion*/
    }
}
