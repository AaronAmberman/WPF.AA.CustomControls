﻿using System.Windows;

namespace Testing
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = new MainWindowViewModel
            {
                Value = 81
            };

            DataContext = viewModel;
        }
    }
}
