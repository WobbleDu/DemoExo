using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace Demex3
{
    /// <summary>
    /// Логика взаимодействия для EditOrder.xaml
    /// </summary>
    public partial class EditOrder : Window
    {
        dbEntities conn;
        C_orders order;
        bool isNewOrder;
        public EditOrder(dbEntities conn, C_orders order)
        {
            InitializeComponent();
            this.conn = conn;
            this.order = order;
            if (this.order.code == null) isNewOrder = true;
            tb_OrderID.IsEnabled = false;
            lb_OrderID.Visibility = isNewOrder? Visibility.Visible: Visibility.Hidden;
            tb_OrderID.Visibility = isNewOrder? Visibility.Visible : Visibility.Hidden;
            this.Title = isNewOrder ? "Создание заказа" : "Редактирование заказа";
            LoadComboBoxes();
            LoadData();
            
        }
        private void LoadComboBoxes()
        {
            // Загрузить список статусов
            {
                var statusSource = conn.C_statuses.ToList();
                C_statuses placeholder = new C_statuses();
                placeholder.ID_status = -1;
                placeholder.name = "Выберите статус заказа";
                statusSource.Insert(0, placeholder);
                cb_OrderStatus.ItemsSource = statusSource;
                cb_OrderStatus.SelectedIndex = 0;
            }
            // Загрузить список пунктов выдачи заказов
            {
                var pvzSource = conn.C_PVZ.ToList();
                C_PVZ placeholder = new C_PVZ();
                placeholder.ID_PVZ = -1;
                placeholder.info = "Выберите адрес ПВЗ";
                pvzSource.Insert(0, placeholder);
                cb_PVZ.ItemsSource = pvzSource;
                cb_PVZ.SelectedIndex = 0;
            }
        }
        private void LoadData()
        {
            if (!isNewOrder)
            {
                tb_OrderID.Text = order.ID_order.ToString();
                cb_OrderStatus.SelectedValue = order.status_ID;
                cb_PVZ.SelectedValue = order.PVZ_ID;
                dp_DateOrdered.Text = order.date_ordered;
                dp_DateRecieved.Text = order.date_recieved;
            }
        }
        private void click_Back(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void click_SaveChanges(object sender, RoutedEventArgs e)
        {
            if (false)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            try
            {
                
                
                if (isNewOrder)
                {
                    C_orders newOrder = new C_orders
                    {
                        status_ID = Convert.ToInt32(cb_OrderStatus.SelectedValue),
                        PVZ_ID = Convert.ToInt32(cb_PVZ.SelectedValue),
                        date_ordered = dp_DateOrdered.Text,
                        date_recieved = dp_DateRecieved.Text
                    };
                        conn.C_orders.Add(newOrder);
                }
                else
                {
                    order.status_ID = Convert.ToInt32(cb_OrderStatus.SelectedValue);
                    order.PVZ_ID = Convert.ToInt32(cb_PVZ.SelectedValue);
                    order.date_ordered = dp_DateOrdered.Text;
                    order.date_recieved = dp_DateRecieved.Text;
                    conn.Entry(order).State = System.Data.EntityState.Modified;
                }
                conn.SaveChanges();
                MessageBox.Show("Успешно сохранено");
                Close();
            }
            catch { MessageBox.Show("Некорректно заполнены поля"); }
        }
    }
}
