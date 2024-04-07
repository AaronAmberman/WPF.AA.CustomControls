using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace WPF.AA.CustomControls
{
    /// <summary>A control that allows for numeric input.</summary>
    [TemplatePart(Name = "PART_NumericDisplay", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_UpIncrement", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_DownIncrement", Type = typeof(RepeatButton))]
    public class NumericUpDown : Control
    {
        #region Fields

        private bool isMinLessThanZero = false;

        private RepeatButton partDownIncrement;
        private TextBox partNumericDisplay;
        private RepeatButton partUpIncrement;

        #endregion

        #region Properties

        /// <summary>Gets or sets whether or not the user can modify the content (this affects typing only).</summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(false));

        /// <summary>Gets or sets the maximum value for the numeric up down. Default is 100.</summary>
        public object MaxValue
        {
            get { return (object)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(object), typeof(NumericUpDown), new PropertyMetadata(100, MaxValueChanged, CoerceMaxValue));

        private static void MaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //NumericUpDown numericUpDown = (NumericUpDown)d;
        }

        private static object CoerceMaxValue(DependencyObject d, object baseValue)
        {
            NumericUpDown numericUpDown = (NumericUpDown)d;

            if (numericUpDown.ValueType == NumericUpDownType.Integer)
            {
                if (!int.TryParse(baseValue.ToString(), out int result))
                    return 0;

                if (!int.TryParse(numericUpDown.MinValue.ToString(), out int minValue))
                    return 0;

                if (result < minValue) return minValue;
                else return result;
            }
            else if (numericUpDown.ValueType == NumericUpDownType.Decimal)
            {
                if (!decimal.TryParse(baseValue.ToString(), out decimal result))
                    return 0;

                if (!decimal.TryParse(numericUpDown.MinValue.ToString(), out decimal minValue))
                    return 0;

                if (result < minValue) return minValue;
                else return result;
            }
            else if (numericUpDown.ValueType == NumericUpDownType.Double)
            {
                if (!double.TryParse(baseValue.ToString(), out double result))
                    return 0;

                if (!double.TryParse(numericUpDown.MinValue.ToString(), out double minValue))
                    return 0;

                if (result < minValue) return minValue;
                else return result;
            }

            return 0;
        }

        /// <summary>Gets or sets the minimum value for the numeric up down. Default is 0.</summary>
        public object MinValue
        {
            get { return (object)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(object), typeof(NumericUpDown), new PropertyMetadata(0, MinValueChanged, CoerceMinValue));

        private static void MinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)d;

            if (numericUpDown.ValueType == NumericUpDownType.Integer)
            {
                if (!int.TryParse(numericUpDown.MinValue.ToString(), out int minValue))
                    return;

                if (minValue < 0) numericUpDown.isMinLessThanZero = true;
                else numericUpDown.isMinLessThanZero = false;
            }
            else if (numericUpDown.ValueType == NumericUpDownType.Decimal)
            {
                if (!decimal.TryParse(numericUpDown.MinValue.ToString(), out decimal minValue))
                    return;

                if (minValue < 0) numericUpDown.isMinLessThanZero = true;
                else numericUpDown.isMinLessThanZero = false;
            }
            else if (numericUpDown.ValueType == NumericUpDownType.Double)
            {
                if (!double.TryParse(numericUpDown.MinValue.ToString(), out double minValue))
                    return;

                if (minValue < 0) numericUpDown.isMinLessThanZero = true;
                else numericUpDown.isMinLessThanZero = false;
            }
        }

        private static object CoerceMinValue(DependencyObject d, object baseValue)
        {
            NumericUpDown numericUpDown = (NumericUpDown)d;

            if (numericUpDown.ValueType == NumericUpDownType.Integer)
            {
                if (!int.TryParse(baseValue.ToString(), out int result))
                    return 0;

                if (!int.TryParse(numericUpDown.MaxValue.ToString(), out int maxValue))
                    return 0;

                if (result > maxValue) return maxValue;
                else return result;
            }
            else if (numericUpDown.ValueType == NumericUpDownType.Decimal)
            {
                if (!decimal.TryParse(baseValue.ToString(), out decimal result))
                    return 0;

                if (!decimal.TryParse(numericUpDown.MaxValue.ToString(), out decimal maxValue))
                    return 0;

                if (result > maxValue) return maxValue;
                else return result;
            }
            else if (numericUpDown.ValueType == NumericUpDownType.Double)
            {
                if (!double.TryParse(baseValue.ToString(), out double result))
                    return 0;

                if (!double.TryParse(numericUpDown.MaxValue.ToString(), out double maxValue))
                    return 0;

                if (result > maxValue) return maxValue;
                else return result;
            }

            return 0;
        }

        /// <summary>Gets or sets the step to use when incrementing or decrementing the value. Default is 1.</summary>
        public object Step
        {
            get { return (object)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(object), typeof(NumericUpDown), new PropertyMetadata(1));

        /// <summary>Gets or sets the value for the numeric up down. Default is 0.</summary>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(NumericUpDown), 
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValueChangedCallback, CoerceValue));

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)d;
            numericUpDown.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            NumericUpDown numericUpDown = (NumericUpDown)d;

            if (numericUpDown.ValueType == NumericUpDownType.Integer)
            {
                if (!int.TryParse(baseValue.ToString(), out int result))
                    return 0;

                if (!int.TryParse(numericUpDown.MinValue.ToString(), out int minValue))
                    return 0;

                if (!int.TryParse(numericUpDown.MaxValue.ToString(), out int maxValue))
                    return 0;

                if (result < minValue) return minValue;
                else if (result > maxValue) return maxValue;
                else return result;
            }
            else if (numericUpDown.ValueType == NumericUpDownType.Decimal)
            {
                if (!decimal.TryParse(baseValue.ToString(), out decimal result))
                    return 0;

                if (!decimal.TryParse(numericUpDown.MinValue.ToString(), out decimal minValue))
                    return 0;

                if (!decimal.TryParse(numericUpDown.MaxValue.ToString(), out decimal maxValue))
                    return 0;

                if (result < minValue) return minValue;
                else if (result > maxValue) return maxValue;
                else return result;
            }
            else if (numericUpDown.ValueType == NumericUpDownType.Double)
            {
                if (!double.TryParse(baseValue.ToString(), out double result))
                    return 0;

                if (!double.TryParse(numericUpDown.MinValue.ToString(), out double minValue))
                    return 0;

                if (!double.TryParse(numericUpDown.MaxValue.ToString(), out double maxValue))
                    return 0;

                if (result < minValue) return minValue;
                else if (result > maxValue) return maxValue;
                else return result;
            }

            return 0;
        }

        /// <summary>Gets or sets the value string format. This is for display purposes only. See ValueType for value calculation.</summary>
        public string ValueFormat
        {
            get { return (string)GetValue(ValueFormatProperty); }
            set { SetValue(ValueFormatProperty, value); }
        }

        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register("ValueFormat", typeof(string), typeof(NumericUpDown), new PropertyMetadata(string.Empty, ValueFormatChanged));

        private static void ValueFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)d;
            numericUpDown.SetTextBinding();
        }

        /// <summary>Gets or sets the value type. Default is Integer. This is for value calculation only. See ValueFormat for value output.</summary>
        public NumericUpDownType ValueType
        {
            get { return (NumericUpDownType)GetValue(ValueTypeProperty); }
            set { SetValue(ValueTypeProperty, value); }
        }

        public static readonly DependencyProperty ValueTypeProperty =
            DependencyProperty.Register("ValueType", typeof(NumericUpDownType), typeof(NumericUpDown), new PropertyMetadata(NumericUpDownType.Integer));

        #endregion

        #region Events

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));

        /// <summary>Occurs when the value changes.</summary>
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        #endregion

        #region Constructors

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            partNumericDisplay = GetTemplateChild("PART_NumericDisplay") as TextBox;
            partNumericDisplay.PreviewKeyDown += PartNumericDisplay_PreviewKeyDown;

            SetTextBinding();

            partDownIncrement = GetTemplateChild("PART_DownIncrement") as RepeatButton;
            partDownIncrement.Click += PartDownIncrement_Click;

            partUpIncrement = GetTemplateChild("PART_UpIncrement") as RepeatButton;
            partUpIncrement.Click += PartUpIncrement_Click;
        }

        private void PartNumericDisplay_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                partNumericDisplay.SelectAll();

                return;
            }

            if (ValueType == NumericUpDownType.Integer)
            {
                if (isMinLessThanZero)
                {
                    if (!(e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 ||
                        e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 ||
                        e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 ||
                        e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 ||
                        e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5 ||
                        e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 ||
                        e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key == Key.Back || e.Key == Key.Delete ||
                        e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab || e.Key == Key.End ||
                        (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && e.Key == Key.Tab) ||
                        (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.V) /* allow paste */))
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    if (!(e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 ||
                        e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 ||
                        e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 ||
                        e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 ||
                        e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5 ||
                        e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || 
                        e.Key == Key.NumPad9  || e.Key == Key.Back || e.Key == Key.Delete ||
                        e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab || e.Key == Key.End ||
                        (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && e.Key == Key.Tab) ||
                        (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.V) /* allow paste */))
                    {
                        e.Handled = true;
                    }
                }
            }
            else if (ValueType == NumericUpDownType.Decimal || ValueType == NumericUpDownType.Double)
            {
                if (isMinLessThanZero)
                {
                    if (!(e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 ||
                        e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 ||
                        e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 ||
                        e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 ||
                        e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5 ||
                        e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 ||
                        e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key == Key.OemPeriod || e.Key == Key.Decimal || 
                        e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Left || e.Key == Key.Right || 
                        e.Key == Key.Tab || e.Key == Key.End ||
                        (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && e.Key == Key.Tab) ||
                        (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.V) /* allow paste */))
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    if (!(e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 ||
                        e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 ||
                        e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 ||
                        e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 ||
                        e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5 ||
                        e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 || 
                        e.Key == Key.OemPeriod || e.Key == Key.Decimal || e.Key == Key.Back || e.Key == Key.Delete || 
                        e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab || e.Key == Key.End ||
                        (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && e.Key == Key.Tab) ||
                        (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.V) /* allow paste */))
                    {
                        e.Handled = true;
                    }
                }
            }

            if (e.Key == Key.Up) PartUpIncrement_Click(this, new RoutedEventArgs());
            else if (e.Key == Key.Down) PartDownIncrement_Click(this, new RoutedEventArgs());
        }

        private void PartDownIncrement_Click(object sender, RoutedEventArgs e)
        {
            if (ValueType == NumericUpDownType.Integer)
            {
                if (int.TryParse(Value.ToString(), out int value) &&
                    int.TryParse(MinValue.ToString(), out int minValue) &&
                    int.TryParse(Step.ToString(), out int increment))
                {
                    if (value - increment < minValue)
                        Value = minValue;
                    else
                        Value = value - increment;
                }
            }
            else if (ValueType == NumericUpDownType.Decimal)
            {
                if (decimal.TryParse(Value.ToString(), out decimal value) &&
                    decimal.TryParse(MinValue.ToString(), out decimal minValue) &&
                    decimal.TryParse(Step.ToString(), out decimal increment))
                {
                    if (value - increment < minValue)
                        Value = minValue;
                    else
                        Value = value - increment;
                }
            }
            else if (ValueType == NumericUpDownType.Double)
            {
                if (double.TryParse(Value.ToString(), out double value) &&
                    double.TryParse(MinValue.ToString(), out double minValue) &&
                    double.TryParse(Step.ToString(), out double increment))
                {
                    if (value - increment < minValue)
                        Value = minValue;
                    else
                        Value = value - increment;
                }
            }

            partNumericDisplay.CaretIndex = partNumericDisplay.Text.Length;
        }

        private void PartUpIncrement_Click(object sender, RoutedEventArgs e)
        {
            if (ValueType == NumericUpDownType.Integer)
            {
                if (int.TryParse(Value.ToString(), out int value) &&
                    int.TryParse(MaxValue.ToString(), out int maxValue) &&
                    int.TryParse(Step.ToString(), out int increment))
                {
                    if (value + increment > maxValue)
                        Value = maxValue;
                    else
                        Value = value + increment;
                }
            }
            else if (ValueType == NumericUpDownType.Decimal)
            {
                if (decimal.TryParse(Value.ToString(), out decimal value) &&
                    decimal.TryParse(MaxValue.ToString(), out decimal maxValue) &&
                    decimal.TryParse(Step.ToString(), out decimal increment))
                {
                    if (value + increment > maxValue)
                        Value = maxValue;
                    else
                        Value = value + increment;
                }
            }
            else if (ValueType == NumericUpDownType.Double)
            {
                if (double.TryParse(Value.ToString(), out double value) &&
                    double.TryParse(MaxValue.ToString(), out double maxValue) &&
                    double.TryParse(Step.ToString(), out double increment))
                {
                    if (value + increment > maxValue)
                        Value = maxValue;
                    else
                        Value = value + increment;
                }
            }

            partNumericDisplay.CaretIndex = partNumericDisplay.Text.Length;
        }

        private void SetTextBinding()
        {
            if (partNumericDisplay == null) return;

            BindingOperations.ClearBinding(partNumericDisplay, TextBox.TextProperty);

            BindingOperations.SetBinding(partNumericDisplay, TextBox.TextProperty, new Binding("Value")
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                StringFormat = ValueFormat,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
        }

        #endregion
    }
}
