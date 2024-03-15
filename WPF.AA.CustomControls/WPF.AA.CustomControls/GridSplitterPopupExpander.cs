using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WPF.AA.CustomControls
{
    /// <summary>An Expander that presents its content in a GridSplitter in a Popup.</summary>
    public class GridSplitterPopupExpander : HeaderedContentControl
    {
        #region Fields

        private Grid contentGrid;
        private GridSplitter gridSplitter;
        private Window popupWindow;
        private double? widthHeight;

        #endregion

        #region Properties

        /// <summary>Gets or sets the corner radius for the expander.</summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GridSplitterPopupExpander), new PropertyMetadata(null));

        /// <summary>Gets or sets the expand direction for the popup.</summary>
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register("ExpandDirection", typeof(ExpandDirection), typeof(GridSplitterPopupExpander), new PropertyMetadata(ExpandDirection.Down));

        /// <summary>Gets or sets the background for the GridSplitter.</summary>
        public Brush GridSplitterBackground
        {
            get { return (Brush)GetValue(GridSplitterBackgroundProperty); }
            set { SetValue(GridSplitterBackgroundProperty, value); }
        }

        public static readonly DependencyProperty GridSplitterBackgroundProperty =
            DependencyProperty.Register("GridSplitterBackground", typeof(Brush), typeof(GridSplitterPopupExpander), new PropertyMetadata(Brushes.Black));

        /// <summary>Gets or sets whether or not the GridSplitter updates the column or row size as the user drags the control. Default is false.</summary>
        /// <see cref="GridSplitter.ShowsPreview" />
        public bool GridSplitterShowsPreview
        {
            get { return (bool)GetValue(GridSplitterShowsPreviewProperty); }
            set { SetValue(GridSplitterShowsPreviewProperty, value); }
        }

        public static readonly DependencyProperty GridSplitterShowsPreviewProperty =
            DependencyProperty.Register("GridSplitterShowsPreview", typeof(bool), typeof(GridSplitterPopupExpander), new PropertyMetadata(false));

        /// <summary>Gets or sets the style to use when showing the preview for the GridSplitter.</summary>
        public Style GridSplitterPreviewStyle
        {
            get { return (Style)GetValue(GridSplitterPreviewStyleProperty); }
            set { SetValue(GridSplitterPreviewStyleProperty, value); }
        }

        public static readonly DependencyProperty GridSplitterPreviewStyleProperty =
            DependencyProperty.Register("GridSplitterPreviewStyle", typeof(Style), typeof(GridSplitterPopupExpander), new PropertyMetadata(null));

        /// <summary>Gets or sets whether or not the expander is open (true) or closed (false).</summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(GridSplitterPopupExpander), new PropertyMetadata(false, IsExpandedChanged));

        private static void IsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridSplitterPopupExpander obj = d as GridSplitterPopupExpander;

            if (obj == null) return;

            if ((bool)e.NewValue)
            {
                obj.BuildAndShowPopupWindow();

                obj.PopupOpened?.Invoke(obj, EventArgs.Empty);
            }
            else
            {
                if (obj.popupWindow == null) return;

                obj.gridSplitter.DragCompleted -= obj.GridSplitter_DragCompleted;
                obj.gridSplitter = null;

                obj.contentGrid.Children.Clear();
                obj.contentGrid = null;

                obj.popupWindow.Close();
                obj.Loaded -= obj.PopupWindow_Loaded;
                obj.popupWindow.Deactivated -= obj.PopupWindow_Deactivated;
                obj.popupWindow = null;

                obj.PopupClosed?.Invoke(obj, EventArgs.Empty);
            }
        }

        /// <summary>Gets or sets the element used to calculate the size and position of the popup.</summary>
        /// <remarks>This should be some kind of root (top most) Grid or Border.</remarks>
        public FrameworkElement PopupBoundsElement
        {
            get { return (FrameworkElement)GetValue(PopupBoundsElementProperty); }
            set { SetValue(PopupBoundsElementProperty, value); }
        }

        public static readonly DependencyProperty PopupBoundsElementProperty =
            DependencyProperty.Register("PopupBoundsElement", typeof(FrameworkElement), typeof(GridSplitterPopupExpander), new PropertyMetadata(null));

        /// <summary>Gets or sets the maximum height for the popup's content. Default is double.PositiveInfinity.</summary>
        public double PopupContentMaximumHeight
        {
            get { return (double)GetValue(PopupContentMaximumHeightProperty); }
            set { SetValue(PopupContentMaximumHeightProperty, value); }
        }

        public static readonly DependencyProperty PopupContentMaximumHeightProperty =
            DependencyProperty.Register("PopupContentMaximumHeight", typeof(double), typeof(GridSplitterPopupExpander),
                new PropertyMetadata(double.PositiveInfinity, null, CoercePopupContentMaximumHeight));

        private static object CoercePopupContentMaximumHeight(DependencyObject d, object baseValue)
        {
            GridSplitterPopupExpander obj = d as GridSplitterPopupExpander;

            if (obj == null) return baseValue;

            double val = (double)baseValue;

            if (val < 0.0) return 0.0; // cannot be less than 0.0
            if (val < obj.PopupContentMinimumHeight) return obj.PopupContentMinimumHeight;

            return baseValue;
        }

        /// <summary>Gets or sets the maximum width for the popup. Default is double.PositiveInfinity.</summary>
        public double PopupContentMaximumWidth
        {
            get { return (double)GetValue(PopupContentMaximumWidthProperty); }
            set { SetValue(PopupContentMaximumWidthProperty, value); }
        }

        public static readonly DependencyProperty PopupContentMaximumWidthProperty =
            DependencyProperty.Register("PopupContentMaximumWidth", typeof(double), typeof(GridSplitterPopupExpander),
                new PropertyMetadata(double.PositiveInfinity, null, CoercePopupContentMaximumWidth));

        private static object CoercePopupContentMaximumWidth(DependencyObject d, object baseValue)
        {
            GridSplitterPopupExpander obj = d as GridSplitterPopupExpander;

            if (obj == null) return baseValue;

            double val = (double)baseValue;

            if (val < 0.0) return 0.0; // cannot be less than 0.0
            if (val < obj.PopupContentMinimumWidth) return obj.PopupContentMinimumWidth;

            return baseValue;
        }

        /// <summary>Gets or sets the minimum height for the popup. Default is 0.0.</summary>
        public double PopupContentMinimumHeight
        {
            get { return (double)GetValue(PopupContentMinimumHeightProperty); }
            set { SetValue(PopupContentMinimumHeightProperty, value); }
        }

        public static readonly DependencyProperty PopupContentMinimumHeightProperty =
            DependencyProperty.Register("PopupContentMinimumHeight", typeof(double), typeof(GridSplitterPopupExpander),
                new PropertyMetadata(0.0, null, CoercePopupContentMinimumHeight));

        private static object CoercePopupContentMinimumHeight(DependencyObject d, object baseValue)
        {
            GridSplitterPopupExpander obj = d as GridSplitterPopupExpander;

            if (obj == null) return baseValue;

            double val = (double)baseValue;

            if (val < 0.0) return 0.0; // cannot be less than 0.0
            if (val > obj.PopupContentMaximumHeight) return obj.PopupContentMaximumHeight;

            return baseValue;
        }

        /// <summary>Gets or sets the minimum width for the popup. Default is 0.0.</summary>
        public double PopupContentMinimumWidth
        {
            get { return (double)GetValue(PopupContentMinimumWidthProperty); }
            set { SetValue(PopupContentMinimumWidthProperty, value); }
        }

        public static readonly DependencyProperty PopupContentMinimumWidthProperty =
            DependencyProperty.Register("PopupContentMinimumWidth", typeof(double), typeof(GridSplitterPopupExpander),
                new PropertyMetadata(0.0, null, CoercePopupMinimumWidth));

        private static object CoercePopupMinimumWidth(DependencyObject d, object baseValue)
        {
            GridSplitterPopupExpander obj = d as GridSplitterPopupExpander;

            if (obj == null) return baseValue;

            double val = (double)baseValue;

            if (val < 0.0) return 0.0; // cannot be less than 0.0
            if (val > obj.PopupContentMaximumWidth) return obj.PopupContentMaximumWidth;

            return baseValue;
        }

        /// <summary>Gets or sets the padding for the content in the popup.</summary>
        public Thickness PopupContentPadding
        {
            get { return (Thickness)GetValue(PopupContentPaddingProperty); }
            set { SetValue(PopupContentPaddingProperty, value); }
        }

        public static readonly DependencyProperty PopupContentPaddingProperty =
            DependencyProperty.Register("PopupContentPadding", typeof(Thickness), typeof(GridSplitterPopupExpander), new PropertyMetadata(new Thickness(0)));

        /// <summary>Gets or sets the preferred height when rendering the popup content.</summary>
        public double PopupContentPreferredHeight
        {
            get { return (double)GetValue(PopupContentPreferredHeightProperty); }
            set { SetValue(PopupContentPreferredHeightProperty, value); }
        }

        public static readonly DependencyProperty PopupContentPreferredHeightProperty =
            DependencyProperty.Register("PopupContentPreferredHeight", typeof(double), typeof(GridSplitterPopupExpander),
                new PropertyMetadata(-1.0, null, CoercePopupContentPreferredHeight));

        private static object CoercePopupContentPreferredHeight(DependencyObject d, object baseValue)
        {
            GridSplitterPopupExpander obj = d as GridSplitterPopupExpander;

            if (obj == null) return baseValue;

            double val = (double)baseValue;

            if (val == -1.0) return baseValue;

            if (val < obj.PopupContentMinimumHeight) return obj.PopupContentMinimumHeight;
            if (val > obj.PopupContentMaximumHeight) return obj.PopupContentMaximumHeight;

            return baseValue;
        }

        /// <summary>Gets or sets the preferred width when rendering the popup content.</summary>
        public double PopupContentPreferredWidth
        {
            get { return (double)GetValue(PopupContentPreferredWidthProperty); }
            set { SetValue(PopupContentPreferredWidthProperty, value); }
        }

        public static readonly DependencyProperty PopupContentPreferredWidthProperty =
            DependencyProperty.Register("PopupContentPreferredWidth", typeof(double), typeof(GridSplitterPopupExpander),
                new PropertyMetadata(-1.0, null, CoercePopupContentPreferredWidth));

        private static object CoercePopupContentPreferredWidth(DependencyObject d, object baseValue)
        {
            GridSplitterPopupExpander obj = d as GridSplitterPopupExpander;

            if (obj == null) return baseValue;

            double val = (double)baseValue;

            if (val == -1.0) return baseValue;

            if (val < obj.PopupContentMinimumWidth) return obj.PopupContentMinimumWidth;
            if (val > obj.PopupContentMaximumWidth) return obj.PopupContentMaximumWidth;

            return baseValue;
        }

        /// <summary>Gets or sets whether or not the popup stays open or not. Default is false.</summary>
        public bool PopupStaysOpen
        {
            get { return (bool)GetValue(PopupStaysOpenProperty); }
            set { SetValue(PopupStaysOpenProperty, value); }
        }

        public static readonly DependencyProperty PopupStaysOpenProperty =
            DependencyProperty.Register("PopupStaysOpen", typeof(bool), typeof(GridSplitterPopupExpander), new PropertyMetadata(false));

        #endregion

        #region Events

        public event EventHandler PopupOpened;
        public event EventHandler PopupClosed;

        #endregion

        #region Constructors

        static GridSplitterPopupExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridSplitterPopupExpander), new FrameworkPropertyMetadata(typeof(GridSplitterPopupExpander)));
        }

        #endregion

        #region Methods

        protected virtual void BuildAndShowPopupWindow()
        {
            contentGrid = new Grid
            {
                Margin = PopupContentPadding
            };
            ContentPresenter cp = new ContentPresenter
            {
                Content = Content
            };
            gridSplitter = new GridSplitter
            {
                Background = GridSplitterBackground,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                PreviewStyle = GridSplitterPreviewStyle,
                ResizeBehavior = GridResizeBehavior.PreviousAndNext,
                ShowsPreview = GridSplitterShowsPreview
            };
            gridSplitter.DragCompleted += GridSplitter_DragCompleted;

            Point topLeft = PopupBoundsElement.PointToScreen(new Point(0, 0));
            double height = 0.0, width = 0.0, left = 0.0, top = 0.0;

            if (ExpandDirection == ExpandDirection.Up)
            {
                height = PopupBoundsElement.ActualHeight - ActualHeight;
                width = PopupBoundsElement.ActualWidth;
                left = topLeft.X;
                top = topLeft.Y;

                contentGrid.RowDefinitions.Add(new RowDefinition 
                { 
                    Height = new GridLength(1, GridUnitType.Star) 
                });
                contentGrid.RowDefinitions.Add(new RowDefinition 
                { 
                    Height = GridLength.Auto 
                });
                contentGrid.RowDefinitions.Add(new RowDefinition 
                { 
                    MinHeight = PopupContentMinimumHeight,
                    MaxHeight = PopupContentMaximumHeight == double.PositiveInfinity 
                        ? PopupBoundsElement.ActualHeight - 3 - ActualHeight - PopupContentPadding.Top
                        : PopupContentMaximumHeight,
                    Height = widthHeight.HasValue 
                        ? new GridLength(widthHeight.Value, GridUnitType.Pixel) 
                        : PopupContentPreferredHeight != -1.0 
                            ? new GridLength(PopupContentPreferredHeight, GridUnitType.Pixel) 
                            : new GridLength(1, GridUnitType.Star)
                });

                gridSplitter.Height = 3;

                Grid.SetRow(gridSplitter, 1);
                Grid.SetRow(cp, 2);

                contentGrid.Children.Add(gridSplitter);
                contentGrid.Children.Add(cp);
            }
            else if (ExpandDirection == ExpandDirection.Down)
            {
                height = PopupBoundsElement.ActualHeight - ActualHeight;
                width = PopupBoundsElement.ActualWidth;
                left = topLeft.X;
                top = topLeft.Y + ActualHeight;

                contentGrid.RowDefinitions.Add(new RowDefinition
                {
                    MinHeight = PopupContentMinimumHeight,
                    MaxHeight = PopupContentMaximumHeight == double.PositiveInfinity
                        ? PopupBoundsElement.ActualHeight - 3 - ActualHeight - PopupContentPadding.Bottom
                        : PopupContentMaximumHeight,
                    Height = widthHeight.HasValue
                        ? new GridLength(widthHeight.Value, GridUnitType.Pixel) 
                        : PopupContentPreferredHeight != -1.0
                            ? new GridLength(PopupContentPreferredHeight, GridUnitType.Pixel)
                            : new GridLength(1, GridUnitType.Star)
                });
                contentGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });
                contentGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });

                gridSplitter.Height = 3;

                Grid.SetRow(cp, 0);
                Grid.SetRow(gridSplitter, 1);

                contentGrid.Children.Add(cp);
                contentGrid.Children.Add(gridSplitter);
            }
            else if (ExpandDirection == ExpandDirection.Left)
            {
                height = PopupBoundsElement.ActualHeight;
                width = PopupBoundsElement.ActualWidth - ActualWidth;
                left = topLeft.X;
                top = topLeft.Y;

                contentGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = GridLength.Auto
                });
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    MinWidth = PopupContentMinimumWidth,
                    MaxWidth = PopupContentMaximumWidth == double.PositiveInfinity
                        ? PopupBoundsElement.ActualWidth - 3 - ActualWidth - PopupContentPadding.Left
                        : PopupContentMaximumWidth,
                    Width = widthHeight.HasValue
                        ? new GridLength(widthHeight.Value, GridUnitType.Pixel) 
                        : PopupContentPreferredWidth != -1.0
                            ? new GridLength(PopupContentPreferredWidth, GridUnitType.Pixel)
                            : new GridLength(1, GridUnitType.Star)
                });

                gridSplitter.Width = 3;

                Grid.SetColumn(gridSplitter, 1);
                Grid.SetColumn(cp, 2);

                contentGrid.Children.Add(gridSplitter);
                contentGrid.Children.Add(cp);
            }
            else if (ExpandDirection == ExpandDirection.Right)
            {
                height = PopupBoundsElement.ActualHeight;
                width = PopupBoundsElement.ActualWidth - ActualWidth;
                left = topLeft.X + ActualWidth;
                top = topLeft.Y;

                contentGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    MinWidth = PopupContentMinimumWidth,
                    MaxWidth = PopupContentMaximumWidth == double.PositiveInfinity
                        ? PopupBoundsElement.ActualWidth - 3 - ActualWidth - PopupContentPadding.Right
                        : PopupContentMaximumWidth,
                    Width = widthHeight.HasValue
                        ? new GridLength(widthHeight.Value, GridUnitType.Pixel) 
                        : PopupContentPreferredWidth != -1.0
                            ? new GridLength(PopupContentPreferredWidth, GridUnitType.Pixel)
                            : new GridLength(1, GridUnitType.Star)
                });
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = GridLength.Auto
                });
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                gridSplitter.Width = 3;

                Grid.SetColumn(cp, 0);
                Grid.SetColumn(gridSplitter, 1);

                contentGrid.Children.Add(cp);
                contentGrid.Children.Add(gridSplitter);
            }

            popupWindow = new Window
            {
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                Content = contentGrid,
                Height = height,
                Left = left,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                Top = top,
                Width = width,
                WindowStyle = WindowStyle.None
            };
            popupWindow.Deactivated += PopupWindow_Deactivated;
            popupWindow.Loaded += PopupWindow_Loaded;
            popupWindow.Show();
        }

        private void GridSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (ExpandDirection == ExpandDirection.Up)
            {
                widthHeight -= e.VerticalChange;

                if (widthHeight > PopupContentMaximumHeight - 3) widthHeight = PopupContentMaximumHeight - 3;
            }
            else if (ExpandDirection == ExpandDirection.Down)
            {
                widthHeight += e.VerticalChange;

                if (widthHeight > PopupContentMaximumHeight - 3) widthHeight = PopupContentMaximumHeight - 3;
            }
            else if (ExpandDirection == ExpandDirection.Left)
            {
                widthHeight -= e.HorizontalChange;

                if (widthHeight > PopupContentMaximumWidth - 3) widthHeight = PopupContentMaximumWidth - 3;
            }
            else if (ExpandDirection == ExpandDirection.Right)
            {
                widthHeight += e.HorizontalChange;

                if (widthHeight > PopupContentMaximumWidth - 3) widthHeight = PopupContentMaximumWidth - 3;
            }

            if (widthHeight < 0) widthHeight = 0;
        }

        private void PopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ExpandDirection == ExpandDirection.Up)
            {
                widthHeight = contentGrid.RowDefinitions[2].ActualHeight;
            }
            else if (ExpandDirection == ExpandDirection.Down)
            {
                widthHeight = contentGrid.RowDefinitions[0].ActualHeight;
            }
            else if (ExpandDirection == ExpandDirection.Left)
            {
                widthHeight = contentGrid.ColumnDefinitions[2].ActualWidth;
            }
            else if (ExpandDirection == ExpandDirection.Right)
            {
                widthHeight = contentGrid.ColumnDefinitions[0].ActualWidth;
            }
        }

        private void PopupWindow_Deactivated(object sender, EventArgs e)
        {
            if (!PopupStaysOpen) IsExpanded = false;
        }

        public override void OnApplyTemplate()
        {
            if (PopupBoundsElement == null)
                throw new InvalidOperationException("PopupBoundsElement property required.");

            base.OnApplyTemplate();
        }

        #endregion
    }
}
