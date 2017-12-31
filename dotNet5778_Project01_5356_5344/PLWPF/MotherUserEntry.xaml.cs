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

namespace PLWPF
{
    /// <summary>
    /// Interaction logic for MotherUserEntry.xaml
    /// </summary>
    public partial class MotherUserEntry : Window
    {
        public MotherUserEntry()
        {
            InitializeComponent();
        }
        private void MotherEnter_Click(object sender, RoutedEventArgs e)
        {
            // if mother in list

            Window motherInfo = new MoterInfoWindow();
            motherInfo.ShowDialog();
        }
    }
}