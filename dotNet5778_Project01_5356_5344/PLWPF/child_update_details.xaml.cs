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
using BL;
using BE;

namespace PLWPF
{
    /// <summary>
    /// Interaction logic for child_update_details.xaml
    /// </summary>
    public partial class child_update_details : Window
    {

        static IBL myBL = BL_Factory.Get_BL;
        Child child;

        public child_update_details(Child thisChild)
        {
            InitializeComponent();
            child = thisChild;
            ChildDetailsGrid.DataContext = child;
        }


        /// <summary>
        /// close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            myBL.updateChild(child);
            MessageBox.Show("Child details were updated!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

    }
}