using System;
using System.Windows;
using RealEstateRentalSystem.Database;

namespace RealEstateRentalSystem.Views
{
    public partial class AddEditContractWindow : Window
    {
        private DatabaseHelper db = new DatabaseHelper();

        public AddEditContractWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция добавления договоров в разработке.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = false;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}