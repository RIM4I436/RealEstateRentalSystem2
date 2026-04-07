using System;
using System.Windows;
using RealEstateRentalSystem.Database;
using RealEstateRentalSystem.Models;

namespace RealEstateRentalSystem.Views
{
    public partial class AddEditClientWindow : Window
    {
        private DatabaseHelper db = new DatabaseHelper();
        private int? editId = null;

        public AddEditClientWindow(int? id = null)
        {
            InitializeComponent();
            editId = id;
            if (id.HasValue)
            {
                Title = "✏️ Редактировать клиента";
                LoadData(id.Value);
            }
        }

        private void LoadData(int id)
        {
            try
            {
                var dt = db.ExecuteQuery($"SELECT * FROM Clients WHERE Id={id}");
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    txtFullName.Text = row["FullName"].ToString();
                    txtPhone.Text = row["Phone"].ToString();
                    txtEmail.Text = row["Email"].ToString();
                    txtPassport.Text = row["PassportData"].ToString();
                    chkActive.IsChecked = Convert.ToBoolean(row["IsActive"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО клиента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Введите номер телефона!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var client = new Client
            {
                Id = editId ?? 0,
                FullName = txtFullName.Text,
                Phone = txtPhone.Text,
                Email = txtEmail.Text,
                PassportData = txtPassport.Text,
                IsActive = chkActive.IsChecked == true
            };

            bool result = editId.HasValue ? db.UpdateClient(client) : db.AddClient(client);

            if (result)
            {
                MessageBox.Show("Данные сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка сохранения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}