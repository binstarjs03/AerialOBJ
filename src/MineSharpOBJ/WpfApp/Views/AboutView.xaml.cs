﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels;

namespace binstarjs03.MineSharpOBJ.WpfApp.Views;
/// <summary>
/// Interaction logic for AboutView.xaml
/// </summary>
public partial class AboutView : Window {
    public AboutView() {
        InitializeComponent();
        DataContext = AboutViewModel.GetInstance;
    }
}
