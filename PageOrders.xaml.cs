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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Demex3
{
    /// <summary>
    /// Логика взаимодействия для PageOrders.xaml
    /// </summary>
    public partial class PageOrders : Window
    {
        dbEntities conn;
        C_users user;
        public PageOrders(dbEntities conn,C_users user)
        {
            InitializeComponent();
            this.conn = conn;
            this.user = user;
            LoadData();
        }
        private void LoadData()
        {
            ListBoxOrders.ItemsSource = conn.C_orders.ToList();
        }
        private void click_Back_Click(object sender, RoutedEventArgs e)
        {
            PageProducts _page = new PageProducts(conn,user);
            _page.Show();
            Close();
        }

        private void click_EditOrder(object sender, MouseButtonEventArgs e)
        // Редактирование заказа
        {
            dynamic selectedOrder = ListBoxOrders.SelectedItem;
            try
            {
                if (selectedOrder != null)
                {
                    int ID = selectedOrder.ID_order;
                    C_orders order = conn.C_orders.First(x => x.ID_order == ID);
                    EditOrder _page = new EditOrder(conn, order);
                    Hide();
                    _page.ShowDialog();
                    Show();
                    LoadData();
                }
                else { MessageBox.Show("Выберите заказ для удаления"); }
            }
            catch { MessageBox.Show("Ошибка при открытии заказа"); }
            }

        private void click_DeleteOrder(object sender, RoutedEventArgs e)
        // Удаление заказа
        {
            try
            {
                dynamic selectedOrder = ListBoxOrders.SelectedItem;
                if (selectedOrder == null) 
                {
                    MessageBox.Show("Выберите заказ для удаления");
                    return;
                }
                conn.C_orders.Remove(selectedOrder);
                conn.SaveChanges();
                MessageBox.Show("Заказ удалён успешно");
                LoadData();
            }
            catch { MessageBox.Show("Ошибка удаления заказа. Повторите позже"); }
        }

        private void click_AddOrder(object sender, RoutedEventArgs e)
        {
            C_orders order = new C_orders();
            order.code = null;
            EditOrder _page = new EditOrder(conn, order);
            Hide();
            _page.ShowDialog();
            Show();
            LoadData();
        }
    }
}
