using System;
using System.Windows;
using System.Windows.Controls;
using RealEstateRentalSystem.Database;
using RealEstateRentalSystem.Models;

namespace RealEstateRentalSystem.Views
{
    public partial class AddEditPropertyWindow : Window
    {
        private DatabaseHelper db = new DatabaseHelper();
        private int? editId = null;

        public AddEditPropertyWindow(int? id = null)
        {
            InitializeComponent();
            editId = id;
            if (id.HasValue)
            {
                Title = "✏️ Редактировать объект";
                LoadData(id.Value);
            }
        }

        private void LoadData(int id)
        {
            var dt = db.ExecuteQuery($"SELECT * FROM Properties WHERE Id={id}");
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                txtAddress.Text = row["Address"].ToString();

                string type = row["PropertyType"].ToString();
                for (int i = 0; i < cmbType.Items.Count; i++)
                {
                    if ((cmbType.Items[i] as ComboBoxItem).Content.ToString() == type)
                    {
                        cmbType.SelectedIndex = i;
                        break;
                    }
                }

                txtRooms.Text = row["Rooms"].ToString();
                txtArea.Text = row["Area"].ToString();
                txtFloor.Text = row["Floor"].ToString();
                txtPrice.Text = row["PricePerMonth"].ToString();
                txtDescription.Text = row["Description"].ToString();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Введите адрес!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var prop = new Property
            {
                Id = editId ?? 0,
                Address = txtAddress.Text,
                PropertyType = (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Rooms = int.TryParse(txtRooms.Text, out int rooms) ? rooms : 0,
                Area = decimal.TryParse(txtArea.Text, out decimal area) ? area : 0,
                Floor = int.TryParse(txtFloor.Text, out int floor) ? (int?)floor : null,
                PricePerMonth = price,
                Description = txtDescription.Text
            };

            bool result = editId.HasValue ? db.UpdateProperty(prop) : db.AddProperty(prop);

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