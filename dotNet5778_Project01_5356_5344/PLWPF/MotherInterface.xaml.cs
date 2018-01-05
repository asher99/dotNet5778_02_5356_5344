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
    /// Interaction logic for MoterInterface.xaml
    /// </summary>
    public partial class MoterInterface : Window
    {
        static IBL myBL = BL_Factory.Get_BL;

        Mother thisMother;

        /// <summary>
        /// window constructor.
        /// </summary>
        /// <param name="mother"></param>
        public MoterInterface(Mother mother)
        {
            InitializeComponent();
            thisMother = mother;
            dataGrid.ItemsSource = null;
        }

        /// <summary>
        /// exit window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Event: when user choosing option - open the right window / dataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Options_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Options.SelectedIndex)
            {
                case 0: updateDetails();
                    break;
                case 1:
                    //dataGrid.ItemsSource = myBL.getListOfContractByMother(thisMother); not implement yet
                    break;
                case 2:
                    addChildToMother();
                    break;
                case 3: dataGrid.ItemsSource = myBL.getListOfChildByMother(thisMother);
                    break;
                case 4:
                    break;
            }

        }

        /// <summary>
        /// A window with mother details opend and make it possible to make change.
        /// </summary>
        private void updateDetails()
        {
            Window update = new mother_update_details(thisMother);
            update.ShowDialog();
        }

        /// <summary>
        /// adding child to mother is done through a special window.
        /// </summary>
        private void addChildToMother()
        {
            Window NewChildWindow = new newChildWindow(thisMother.id);
            NewChildWindow.ShowDialog();
        }
    }
}