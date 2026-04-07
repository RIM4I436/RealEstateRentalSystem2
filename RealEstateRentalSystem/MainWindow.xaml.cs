using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using RealEstateRentalSystem.Database;
using RealEstateRentalSystem.Views;

namespace RealEstateRentalSystem
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper db = new DatabaseHelper();
        private string currentTable = "Properties";

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (currentTable == "Properties")
                {
                    // Безопасное получение фильтра
                    string filter = "";
                    if (cmbFilter.SelectedItem != null && cmbFilter.SelectedItem is ComboBoxItem selectedItem)
                    {
                        filter = selectedItem.Content.ToString();
                        if (filter == "Все") filter = "";
                    }

                    DataTable data = db.GetAllProperties(txtSearch.Text, filter);
                    dgvData.ItemsSource = data?.DefaultView;
                    cmbFilter.IsEnabled = true;
                }
                else if (currentTable == "Clients")
                {
                    DataTable data = db.GetAllClients(txtSearch.Text);
                    dgvData.ItemsSource = data?.DefaultView;
                    cmbFilter.IsEnabled = false;
                }
                else if (currentTable == "Contracts")
                {
                    DataTable data = db.GetAllContracts(txtSearch.Text);
                    dgvData.ItemsSource = data?.DefaultView;
                    cmbFilter.IsEnabled = false;
                }

                dgvData.AutoGenerateColumns = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Навигация
        private void BtnProperties_Click(object sender, RoutedEventArgs e)
        {
            currentTable = "Properties";
            LoadData();
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            currentTable = "Clients";
            LoadData();
        }

        private void BtnContracts_Click(object sender, RoutedEventArgs e)
        {
            currentTable = "Contracts";
            LoadData();
        }

        // Поиск и фильтрация
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }

        private void CmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentTable == "Properties")
                LoadData();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        // Добавление
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentTable == "Properties")
                {
                    var window = new AddEditPropertyWindow();
                    if (window.ShowDialog() == true)
                        LoadData();
                }
                else if (currentTable == "Clients")
                {
                    var window = new AddEditClientWindow();
                    if (window.ShowDialog() == true)
                        LoadData();
                }
                else if (currentTable == "Contracts")
                {
                    var window = new AddEditContractWindow();
                    if (window.ShowDialog() == true)
                        LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Редактирование
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgvData.SelectedItem == null)
                {
                    MessageBox.Show("Выберите запись для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DataRowView selectedRow = (DataRowView)dgvData.SelectedItem;
                int id = Convert.ToInt32(selectedRow["Id"]);

                if (currentTable == "Properties")
                {
                    var window = new AddEditPropertyWindow(id);
                    if (window.ShowDialog() == true)
                        LoadData();
                }
                else if (currentTable == "Clients")
                {
                    var window = new AddEditClientWindow(id);
                    if (window.ShowDialog() == true)
                        LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Удаление
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgvData.SelectedItem == null)
                {
                    MessageBox.Show("Выберите запись для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DataRowView selectedRow = (DataRowView)dgvData.SelectedItem;
                int id = Convert.ToInt32(selectedRow["Id"]);

                if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    bool result = false;
                    if (currentTable == "Properties")
                        result = db.DeleteProperty(id);
                    else if (currentTable == "Clients")
                        result = db.DeleteClient(id);

                    if (result)
                    {
                        MessageBox.Show("Запись удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}