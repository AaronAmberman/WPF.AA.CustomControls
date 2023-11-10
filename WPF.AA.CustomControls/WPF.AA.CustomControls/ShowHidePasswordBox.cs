using System;
using System.Windows;
using System.Windows.Controls;

namespace WPF.AA.CustomControls
{
    /// <summary>A custom PasswordBox control that can show and hide the plain text entered by the user.</summary>
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_PasswordBox", Type = typeof(PasswordBox))]
    [TemplatePart(Name = "PART_ShowTextButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_HideTextButton", Type = typeof(Button))]
    public class ShowHidePasswordBox : TextBox
    {
        #region Fields

        private Button hideTextButton;
        private PasswordBox passwordBox;
        private Button showTextButton;
        private TextBox textBox;

        #endregion

        #region Properties

        /// <summary>Gets or set whether or not to show the password in plain text. Default is false.</summary>
        public bool ShowPassword
        {
            get { return (bool)GetValue(ShowPasswordProperty); }
            set { SetValue(ShowPasswordProperty, value); }
        }

        public static readonly DependencyProperty ShowPasswordProperty =
            DependencyProperty.Register("ShowPassword", typeof(bool), typeof(ShowHidePasswordBox), new PropertyMetadata(false));

        /// <summary>Gets or sets the watermark.</summary>
        public object Watermark
        {
            get { return (object)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(object), typeof(ShowHidePasswordBox), new PropertyMetadata(null));

        /// <summary>Gets or sets the watermark template.</summary>
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        public static readonly DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(ShowHidePasswordBox), new PropertyMetadata(null));

        #endregion

        #region Constructors

        static ShowHidePasswordBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShowHidePasswordBox), new FrameworkPropertyMetadata(typeof(ShowHidePasswordBox)));
            TextProperty.OverrideMetadata(typeof(ShowHidePasswordBox), new FrameworkPropertyMetadata(string.Empty, TextChangedCallback));
            TextAlignmentProperty.OverrideMetadata(typeof(ShowHidePasswordBox), new FrameworkPropertyMetadata(TextAlignment.Left, NotSupportPropertyChangedCallback));
            TextDecorationsProperty.OverrideMetadata(typeof(ShowHidePasswordBox), new FrameworkPropertyMetadata(new TextDecorationCollection(), NotSupportPropertyChangedCallback));
            TextWrappingProperty.OverrideMetadata(typeof(ShowHidePasswordBox), new FrameworkPropertyMetadata(TextWrapping.NoWrap, NotSupportPropertyChangedCallback));
        }

        #endregion

        #region Methods

        private void HideTextClick(object sender, RoutedEventArgs e)
        {
            ShowPassword = false;

            passwordBox.Focus();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            passwordBox = GetTemplateChild("PART_PasswordBox") as PasswordBox;
            textBox = GetTemplateChild("PART_TextBox") as TextBox;
            hideTextButton = GetTemplateChild("PART_HideTextButton") as Button; 
            showTextButton = GetTemplateChild("PART_ShowTextButton") as Button;

            if (hideTextButton != null)
                hideTextButton.Click += HideTextClick;

            if (showTextButton != null)
                showTextButton.Click += ShowTextClick;

            // if we have text in our Text property prior to having our template applied then we need to set that text on the pasword box
            // this will more than likely occur when their is a Binding to the Text property
            if (!string.IsNullOrEmpty(Text))
                passwordBox.Password = Text;

            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Text = passwordBox.Password;
        }

        private void ShowTextClick(object sender, RoutedEventArgs e)
        {
            ShowPassword = true;

            textBox.Focus();
            textBox.Select(Text.Length, 0);
        }

        private static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShowHidePasswordBox instance = d as ShowHidePasswordBox;

            if (instance == null) return;
            if (instance.passwordBox == null) return;

            if (string.IsNullOrWhiteSpace(instance.passwordBox.Password) || !instance.passwordBox.Password.Equals(instance.Text))
                instance.passwordBox.Password = instance.Text;
        }

        private static void NotSupportPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
