using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppDop2Homework3Ado
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection connection;
        private IConfiguration _configuration;

        public List<Product> Products { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            InitializeConfiguration();
            Products = new List<Product>
            {
                new Product { ProductId = 1, Name = "Карандаши", Type = "Рисовальные", Quantity = 100, Cost = 50.00M },
                new Product { ProductId = 2, Name = "Бумага", Type = "Офисная", Quantity = 200, Cost = 120.00M },
                new Product { ProductId = 3, Name = "Ручки", Type = "Шариковые", Quantity = 150, Cost = 30.00M },
                new Product { ProductId = 4, Name = "Тетради", Type = "Обычные", Quantity = 120, Cost = 80.00M },
                new Product { ProductId = 5, Name = "Степлеры", Type = "Офисные", Quantity = 50, Cost = 150.00M },
                new Product { ProductId = 6, Name = "Ластик", Type = "Школьные", Quantity = 80, Cost = 10.00M },
                new Product { ProductId = 7, Name = "Маркеры", Type = "Перманентные", Quantity = 70, Cost = 170.00M }
            };
            DataContext = this;
        }
        private void InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            _configuration = builder.Build();
        }
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = GetConnectionString();
                connection = new SqlConnection(connectionString);
                connection.Open();

                MessageBox.Show("Connection successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed. Error: {ex.Message}");
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    MessageBox.Show("Disconnected from the database.");
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during disconnection: {ex.Message}");
            }
        }
        private string GetConnectionString()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"ConnectionString: {connectionString}");
            return connectionString;
        }
        // Отображение всей информации о канцтоварах
        private void DisplayAllGoodsButton_Click(object sender, RoutedEventArgs e)
        {
            Products.Add(new Product { Name = "Линейка" });
            Products.Add(new Product { Name = "Блокнот" });
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT * FROM Products";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Product> products = new List<Product>();

                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Type = reader.GetString(2),
                                    Quantity = reader.GetInt32(3),
                                    Cost = reader.GetDecimal(4)
                                };
                                products.Add(product);
                            }

                            productsListBox.ItemsSource = null;
                            productsListBox.ItemsSource = products;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying goods: {ex.Message}");
            }
        }
        // Отображение всех типов канцтоваров
        private void DisplayAllProductTypes()
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT DISTINCT Type FROM Products";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<string> productTypes = new List<string>();

                            while (reader.Read())
                            {
                                productTypes.Add(reader.GetString(0));
                            }
                            productTypesListBox.ItemsSource = productTypes;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying product types: {ex.Message}");
            }
        }
        // Отображение всех менеджеров по продажам
        private void DisplayAllManagers()
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT DISTINCT ManagerName FROM Managers";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<string> managerNames = new List<string>();

                            while (reader.Read())
                            {
                                managerNames.Add(reader.GetString(0));
                            }

                            managersListBox.ItemsSource = managerNames;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying managers: {ex.Message}");
            }
        }
        // Показать канцтовары с максимальным количеством единиц
        private void DisplayProductsWithMaxQuantity()
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT * FROM Products WHERE Quantity = (SELECT MAX(Quantity) FROM Products)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Product> productsWithMaxQuantity = new List<Product>();

                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Type = reader.GetString(2),
                                    Quantity = reader.GetInt32(3),
                                    Cost = reader.GetDecimal(4)
                                };

                                productsWithMaxQuantity.Add(product);
                            }

                            productsListBox.ItemsSource = productsWithMaxQuantity;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying products with max quantity: {ex.Message}");
            }
        }
        // Показать канцтовары с минимальным количеством единиц
        private void DisplayProductsWithMinQuantity()
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT * FROM Products WHERE Quantity = (SELECT MIN(Quantity) FROM Products)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Product> productsWithMinQuantity = new List<Product>();

                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Type = reader.GetString(2),
                                    Quantity = reader.GetInt32(3),
                                    Cost = reader.GetDecimal(4)
                                };

                                productsWithMinQuantity.Add(product);
                            }

                            productsListBox.ItemsSource = productsWithMinQuantity;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying products with min quantity: {ex.Message}");
            }
        }
        // Показать канцтовары с минимальной себестоимостью единицы
        private void DisplayProductsWithMinCostPerUnit()
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT * FROM Products WHERE Cost = (SELECT MIN(Cost) FROM Products)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Product> productsWithMinCostPerUnit = new List<Product>();

                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Type = reader.GetString(2),
                                    Quantity = reader.GetInt32(3),
                                    Cost = reader.GetDecimal(4)
                                };

                                productsWithMinCostPerUnit.Add(product);
                            }
                            productsListBox.ItemsSource = productsWithMinCostPerUnit;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying products with min cost per unit: {ex.Message}");
            }
        }
        // Показать канцтовары с максимальной себестоимостью единицы
        private void DisplayProductsWithMaxCostPerUnit()
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT * FROM Products WHERE Cost = (SELECT MAX(Cost) FROM Products)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Product> productsWithMaxCostPerUnit = new List<Product>();

                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Type = reader.GetString(2),
                                    Quantity = reader.GetInt32(3),
                                    Cost = reader.GetDecimal(4)
                                };

                                productsWithMaxCostPerUnit.Add(product);
                            }
                            productsListBox.ItemsSource = productsWithMaxCostPerUnit;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying products with max cost per unit: {ex.Message}");
            }
        }
        // Показать канцтовары заданного типа
        private void DisplayProductsByType(string type)
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = $"SELECT * FROM Products WHERE Type = '{type}'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Product> productsByType = new List<Product>();

                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Type = reader.GetString(2),
                                    Quantity = reader.GetInt32(3),
                                    Cost = reader.GetDecimal(4)
                                };

                                productsByType.Add(product);
                            }

                            productsListBox.ItemsSource = productsByType;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying products by type: {ex.Message}");
            }
        }
        // Показать канцтовары, которые продал конкретный менеджер
        private void DisplayProductsSoldByManager(string managerName)
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = $@"SELECT Products.* 
                              FROM Products
                              JOIN Sales ON Products.ProductId = Sales.ProductId
                              JOIN Managers ON Sales.ManagerId = Managers.ManagerId
                              WHERE Managers.ManagerName = '{managerName}'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Product> productsSoldByManager = new List<Product>();

                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Type = reader.GetString(2),
                                    Quantity = reader.GetInt32(3),
                                    Cost = reader.GetDecimal(4)
                                };

                                productsSoldByManager.Add(product);
                            }

                            productsListBox.ItemsSource = productsSoldByManager;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying products sold by manager: {ex.Message}");
            }
        }
        // Показать канцтовары, которые купила конкретная фирма покупатель
        private void DisplayProductsBoughtByCompany(string companyName)
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = $@"SELECT Products.* 
                              FROM Products
                              JOIN Sales ON Products.ProductId = Sales.ProductId
                              WHERE Sales.CustomerCompany = '{companyName}'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Product> productsBoughtByCompany = new List<Product>();

                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Type = reader.GetString(2),
                                    Quantity = reader.GetInt32(3),
                                    Cost = reader.GetDecimal(4)
                                };

                                productsBoughtByCompany.Add(product);
                            }
                            productsListBox.ItemsSource = productsBoughtByCompany;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying products bought by company: {ex.Message}");
            }
        }
        // Показать информацию о самой недавней продаже
        private void DisplayLatestSale()
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT TOP 1 * FROM Sales ORDER BY SaleDate DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Sale latestSale = new Sale();

                            while (reader.Read())
                            {
                                latestSale.SaleId = reader.GetInt32(0);
                                latestSale.ProductId = reader.GetInt32(1);
                                latestSale.ManagerId = reader.GetInt32(2);
                                latestSale.CustomerCompany = reader.GetString(3);
                                latestSale.QuantitySold = reader.GetInt32(4);
                                latestSale.UnitPrice = reader.GetDecimal(5);
                                latestSale.SaleDate = reader.GetDateTime(6);
                            }

                            MessageBox.Show($"Latest Sale:\nSaleId: {latestSale.SaleId}\nProduct: {latestSale.ProductId}\nManager: {latestSale.ManagerId}\n" +
                                $"Customer Company: {latestSale.CustomerCompany}\nQuantity Sold: {latestSale.QuantitySold}\n" +
                                $"Unit Price: {latestSale.UnitPrice}\nSale Date: {latestSale.SaleDate}");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying latest sale: {ex.Message}");
            }
        }
        // Показать среднее количество товаров по каждому типу канцтоваров
        private void DisplayAverageQuantityByType()
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT Type, AVG(CAST(Quantity AS DECIMAL(18,2))) AS AvgQuantity FROM Products GROUP BY Type";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<string> averageQuantities = new List<string>();

                            while (reader.Read())
                            {
                                string type = reader.GetString(0);
                                decimal averageQuantityDecimal = reader.GetDecimal(1);
                                int averageQuantityInt = Convert.ToInt32(averageQuantityDecimal);
                                string result = $"{type}: {averageQuantityInt}";
                                averageQuantities.Add(result);
                            }
                            MessageBox.Show($"Average Quantity by Type:\n{string.Join("\n", averageQuantities)}");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Not connected to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while displaying average quantity by type: {ex.Message}");
            }
        }
        private void DisplayAllProductTypesButton_Click(object sender, RoutedEventArgs e)
    {
        DisplayAllProductTypes();
    }
        private void DisplayAllManagersButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayAllManagers();
        }
        private void DisplayProductsWithMaxQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayProductsWithMaxQuantity();
        }
        private void DisplayProductsWithMinQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayProductsWithMinQuantity();
        }
        private void DisplayProductsWithMinCostPerUnitButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayProductsWithMinCostPerUnit();
        }
        private void DisplayProductsWithMaxCostPerUnitButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayProductsWithMaxCostPerUnit();
        }
        private void DisplayProductsByTypeButton_Click(object sender, RoutedEventArgs e)
        {
            string productType = txtProductType.Text;
            DisplayProductsByType(productType);
        }
        private void DisplayProductsSoldByManagerButton_Click(object sender, RoutedEventArgs e)
        {
            string managerName = txtManagerName.Text;
            DisplayProductsSoldByManager(managerName);
        }
        private void DisplayProductsBoughtByCompanyButton_Click(object sender, RoutedEventArgs e)
        {
            string companyName = txtCustomerCompany.Text;
            DisplayProductsBoughtByCompany(companyName);
        }
        private void DisplayLatestSaleButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayLatestSale();
        }
        private void DisplayAverageQuantityByTypeButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayAverageQuantityByType();
        }
        public class Product
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public int Quantity { get; set; }
            public decimal Cost { get; set; }
        }
        public class Sale
        {
            public int SaleId { get; set; }
            public int ProductId { get; set; }
            public int ManagerId { get; set; }
            public string CustomerCompany { get; set; }
            public int QuantitySold { get; set; }
            public decimal UnitPrice { get; set; }
            public DateTime SaleDate { get; set; }
        }
    }
}
