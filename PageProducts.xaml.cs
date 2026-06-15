using System;
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
using System.IO;
namespace Demex3
{
    /// <summary>
    /// Логика взаимодействия для PageProducts.xaml
    /// </summary>
    public partial class PageProducts : Window
    {
        C_users currentUser;
        dbEntities conn = new dbEntities();
        List<dynamic> allProducts = new List<dynamic>();
        public PageProducts(dbEntities conn, C_users user)
        {
            InitializeComponent();
            currentUser = user;
            lb_UserFIO.Content = currentUser.FIO;
            LoadFilters();
            LoadData();
            SetRoleRights();
        }
        private void SetRoleRights()
        // Исходя из роли выбирает, к каким функциям
        // пользователь будет иметь доступ
        {
            switch(currentUser.role_ID)
            {
                case 2: //Менеджер
                    bt_AddProduct.Visibility = Visibility.Hidden;
                    bt_DeleteProduct.Visibility = Visibility.Hidden;
                    break;
                case 1: // Администратор
                    break;
                default: // Для всех остальных
                    lb_Search.Visibility = Visibility.Hidden;
                    tb_Search.Visibility = Visibility.Hidden;
                    cb_Filter.Visibility = Visibility.Hidden;
                    cb_Sort.Visibility = Visibility.Hidden;
                    bt_DeleteProduct.Visibility = Visibility.Hidden;
                    bt_AddProduct.Visibility = Visibility.Hidden;
                    bt_Orders.Visibility = Visibility.Hidden;
                    break;
            }
        }
        private void LoadFilters()
        // Загрузка производителей для поля фильтрации
        {
            var filters = conn.C_manufacturers.Select(x => x.name).ToList();
            filters.Insert(0, "Выберите производителя");
            cb_Filter.ItemsSource = filters;
            cb_Filter.SelectedIndex = 0;
        }
        private void LoadData()
        // Загрузка актуальных данных из БД
        {
            allProducts = conn.C_products.ToList().Select(x=> new
            {
                //Все поля C_products
                image_path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Images",x.image_path),
                x.article,
                x.C_categories,
                x.name,
                x.description,
                x.C_manufacturers,
                x.C_providers,
                x.price,
                x.count,
                x.discount,
                isDiscountHighEnough = x.discount>15,
                isAvailable = x.count>0,

                total_price = Math.Round(x.price * (1-x.discount/100),2)
            }).Cast<dynamic>().ToList();
            ListProducts.ItemsSource = allProducts;
        }

        public void UpdateSearch()
        // Одновременные поиск, сортировка и фильтрация 
        {
            LoadData();
            string search = tb_Search.Text.ToLower();

            // Поиск по неточному совпадению
            var resultProducts = allProducts.Where(x =>
                (x.name.ToLower().Contains(search)) ||
                (x.C_categories.name.ToLower().Contains(search))
            ).ToList();

            // Сортировка
            if (cb_Sort != null)
            {
                switch (cb_Sort.SelectedIndex)
                {
                    case 1:
                        resultProducts = resultProducts.OrderBy(x => x.price).ToList();
                        break;
                    case 2:
                        resultProducts = resultProducts.OrderBy(x => x.discount).ToList();
                        break;
                    case 3:
                        resultProducts = resultProducts.OrderByDescending(x => x.price).ToList();
                        break;
                    case 4:
                        resultProducts = resultProducts.OrderByDescending(x => x.discount).ToList();
                        break;
                }
            }

            // Фильтрация по производителю
            if (cb_Filter != null && cb_Filter.SelectedIndex > 0)
            {
                string selectedFilter = cb_Filter.SelectedItem.ToString();
                resultProducts = resultProducts.Where(x => x.C_manufacturers.name == selectedFilter).ToList();
            }

            ListProducts.ItemsSource = resultProducts;
        }

        private void SearchChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSearch();
        }

        private void SortChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSearch();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSearch();
        }

        private void click_DeleteProduct(object sender, RoutedEventArgs e)
        // Удаление выбранного в списке товара
        {
            try
            {
                dynamic product = ListProducts.SelectedItem;
                if (product != null)
                {
                    string selectedArticle = product.article;
                    C_products Delete = conn.C_products.FirstOrDefault(x => x.article == selectedArticle);
                    conn.C_products.Remove(Delete);
                    conn.SaveChanges();
                    MessageBox.Show("Товар успешно удалён");
                    UpdateSearch();
                }
                else MessageBox.Show("Выберите товар для удаления");
            }
            catch { MessageBox.Show("Не удалось удалить товар, т.к. он добавлен в заказ"); }
        }

        private void click_Close(object sender, RoutedEventArgs e)
        // Возврат на окно авторизации
        {
            AuthWindow _page = new AuthWindow();
            _page.Show();
            Close();
        }

        private void click_AddProduct(object sender, RoutedEventArgs e)
        // Добавление нового товара
        {
            EditProduct _page = new EditProduct(conn,new C_products());
            Hide();
            _page.ShowDialog();
            UpdateSearch();
            Show();
        }

        private void click_EditProduct(object sender, MouseButtonEventArgs e)
        {
            if (currentUser.role_ID != 1) return;
            dynamic selectedItem = ListProducts.SelectedItem;
            string article = selectedItem.article;
            C_products product = conn.C_products.First(x=>x.article == article);
            EditProduct _page = new EditProduct(conn, product);
            Hide();
            _page.ShowDialog();
            UpdateSearch();
            Show();
        }

        private void click_Orders(object sender, RoutedEventArgs e)
        {
            PageOrders _page = new PageOrders(conn, currentUser);
            _page.Show();
            Close();
        }
    }
}
