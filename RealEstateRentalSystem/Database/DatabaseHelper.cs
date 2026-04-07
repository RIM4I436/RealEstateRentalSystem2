using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Windows;
using RealEstateRentalSystem.Models;

namespace RealEstateRentalSystem.Database
{
    public class DatabaseHelper
    {
        // ИЗМЕНИТЕ строку подключения под вашу систему!
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=RealEstateRental;Integrated Security=True";

        // Альтернативные строки подключения (раскомментируйте нужную):
        // Для SQL Server Express: @"Data Source=.\SQLEXPRESS;Initial Catalog=RealEstateRental;Integrated Security=True"
        // Для SQL Server: @"Data Source=localhost;Initial Catalog=RealEstateRental;Integrated Security=True"

        /// <summary>
        /// Выполняет SELECT запрос и возвращает DataTable
        /// </summary>
        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }

        /// <summary>
        /// Выполняет INSERT/UPDATE/DELETE запрос
        /// </summary>
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        conn.Open();
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }

        // ========== РАБОТА С ОБЪЕКТАМИ ==========

        public DataTable GetAllProperties(string searchText = "", string filterType = "")
        {
            string query = @"SELECT Id, Address, PropertyType, Rooms, Area, Floor, PricePerMonth, 
                                    CASE WHEN IsRented = 1 THEN 'Арендована' ELSE 'Свободна' END as Status
                             FROM Properties WHERE 1=1";

            if (!string.IsNullOrEmpty(searchText))
                query += $" AND (Address LIKE '%{searchText.Replace("'", "''")}%' OR Description LIKE '%{searchText.Replace("'", "''")}%')";
            if (!string.IsNullOrEmpty(filterType) && filterType != "Все")
                query += $" AND PropertyType = '{filterType.Replace("'", "''")}'";

            query += " ORDER BY Id";
            return ExecuteQuery(query);
        }

        public bool AddProperty(Property prop)
        {
            string query = @"INSERT INTO Properties (Address, PropertyType, Rooms, Area, Floor, PricePerMonth, IsRented, Description, CreatedAt) 
                             VALUES (@Address, @Type, @Rooms, @Area, @Floor, @Price, 0, @Desc, GETDATE())";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Address", prop.Address),
                new SqlParameter("@Type", prop.PropertyType),
                new SqlParameter("@Rooms", prop.Rooms),
                new SqlParameter("@Area", prop.Area),
                new SqlParameter("@Floor", (object)prop.Floor ?? DBNull.Value),
                new SqlParameter("@Price", prop.PricePerMonth),
                new SqlParameter("@Desc", (object)prop.Description ?? DBNull.Value)
            };
            return ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateProperty(Property prop)
        {
            string query = @"UPDATE Properties SET Address=@Address, PropertyType=@Type, Rooms=@Rooms, 
                             Area=@Area, Floor=@Floor, PricePerMonth=@Price, Description=@Desc WHERE Id=@Id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", prop.Id),
                new SqlParameter("@Address", prop.Address),
                new SqlParameter("@Type", prop.PropertyType),
                new SqlParameter("@Rooms", prop.Rooms),
                new SqlParameter("@Area", prop.Area),
                new SqlParameter("@Floor", (object)prop.Floor ?? DBNull.Value),
                new SqlParameter("@Price", prop.PricePerMonth),
                new SqlParameter("@Desc", (object)prop.Description ?? DBNull.Value)
            };
            return ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteProperty(int id)
        {
            // Проверяем, есть ли активные договоры
            string checkQuery = "SELECT COUNT(*) FROM RentalContracts WHERE PropertyId=@Id AND Status='Активен'";
            var count = Convert.ToInt32(ExecuteQuery(checkQuery, new[] { new SqlParameter("@Id", id) }).Rows[0][0]);
            if (count > 0)
            {
                MessageBox.Show("Нельзя удалить объект с активными договорами аренды!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            string query = "DELETE FROM Properties WHERE Id=@Id";
            return ExecuteNonQuery(query, new[] { new SqlParameter("@Id", id) }) > 0;
        }

        // ========== РАБОТА С КЛИЕНТАМИ ==========

        public DataTable GetAllClients(string searchText = "")
        {
            string query = @"SELECT Id, FullName, Phone, Email, PassportData, 
                                    CASE WHEN IsActive = 1 THEN 'Активен' ELSE 'Неактивен' END as Status
                             FROM Clients";
            if (!string.IsNullOrEmpty(searchText))
                query += $" WHERE FullName LIKE '%{searchText.Replace("'", "''")}%' OR Phone LIKE '%{searchText.Replace("'", "''")}%'";
            query += " ORDER BY Id";
            return ExecuteQuery(query);
        }

        public bool AddClient(Client client)
        {
            string query = @"INSERT INTO Clients (FullName, Phone, Email, PassportData, IsActive, RegisteredAt) 
                             VALUES (@Name, @Phone, @Email, @Passport, 1, GETDATE())";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Name", client.FullName),
                new SqlParameter("@Phone", client.Phone),
                new SqlParameter("@Email", (object)client.Email ?? DBNull.Value),
                new SqlParameter("@Passport", (object)client.PassportData ?? DBNull.Value)
            };
            return ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateClient(Client client)
        {
            string query = @"UPDATE Clients SET FullName=@Name, Phone=@Phone, Email=@Email, 
                             PassportData=@Passport, IsActive=@Active WHERE Id=@Id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", client.Id),
                new SqlParameter("@Name", client.FullName),
                new SqlParameter("@Phone", client.Phone),
                new SqlParameter("@Email", (object)client.Email ?? DBNull.Value),
                new SqlParameter("@Passport", (object)client.PassportData ?? DBNull.Value),
                new SqlParameter("@Active", client.IsActive ? 1 : 0)
            };
            return ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteClient(int id)
        {
            string checkQuery = "SELECT COUNT(*) FROM RentalContracts WHERE ClientId=@Id AND Status='Активен'";
            var count = Convert.ToInt32(ExecuteQuery(checkQuery, new[] { new SqlParameter("@Id", id) }).Rows[0][0]);
            if (count > 0)
            {
                MessageBox.Show("Нельзя удалить клиента с активными договорами!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            string query = "DELETE FROM Clients WHERE Id=@Id";
            return ExecuteNonQuery(query, new[] { new SqlParameter("@Id", id) }) > 0;
        }

        // ========== РАБОТА С ДОГОВОРАМИ ==========

        public DataTable GetAllContracts(string searchText = "")
        {
            string query = @"SELECT rc.Id, p.Address as PropertyAddress, c.FullName as ClientName, 
                                    rc.StartDate, rc.EndDate, rc.MonthlyPayment, rc.Deposit, rc.Status
                             FROM RentalContracts rc
                             JOIN Properties p ON rc.PropertyId = p.Id
                             JOIN Clients c ON rc.ClientId = c.Id";
            if (!string.IsNullOrEmpty(searchText))
                query += $" WHERE p.Address LIKE '%{searchText.Replace("'", "''")}%' OR c.FullName LIKE '%{searchText.Replace("'", "''")}%'";
            query += " ORDER BY rc.Id";
            return ExecuteQuery(query);
        }
    }
}