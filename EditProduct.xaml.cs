using System;
using System.Collections.Generic;
using System.Data.Common.CommandTrees;
using System.IO;
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

namespace Demex3
{
    /// <summary>
    /// Логика взаимодействия для EditProduct.xaml
    /// </summary>
    public partial class EditProduct : Window
    {
        dbEntities conn;
        C_products product;
        bool isNewProduct = false;
        public EditProduct(dbEntities conn, C_products product)
        {
            InitializeComponent();
            this.conn = conn;
            this.product = product;
            if (this.product.article == null) isNewProduct = true;
            this.Title = isNewProduct ? "Создание товара" : "Редактирование товара";
            tb_article.IsEnabled = isNewProduct ? true: false;
            LoadComboBoxes();
            LoadData();
        }
        private void LoadData()
        {
            if (!isNewProduct)
            {
                tb_article.Text = product.article;
                tb_name.Text = product.name;
                tb_description.Text = product.description;
                tb_count.Text = product.count.ToString();
                tb_price.Text = product.price.ToString();
                tb_discount.Text = product.discount.ToString();
                string formatPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", product.image_path);
                _image.Source = new BitmapImage(new Uri(formatPath, UriKind.Absolute));
                cb_category.SelectedValue = product.category_ID;
                cb_manufacturer.SelectedValue = product.manufacturer_ID;
                cb_provider.SelectedValue = product.provider_ID;
            }
        }
        private void LoadComboBoxes()
        {
            //Загрузка списка категорий
            {
                var categorySource = conn.C_categories.ToList();
                C_categories placeholder = new C_categories();
                placeholder.ID_category = -1;
                placeholder.name = "Выберите категорию";
                categorySource.Insert(0, placeholder);
                cb_category.ItemsSource = categorySource;
                cb_category.SelectedIndex = 0;
            }

            //Загрузка списка производителей
            {
                var manufacturerSource = conn.C_manufacturers.ToList();
                C_manufacturers placeholder = new C_manufacturers();
                placeholder.ID_manufacturer = -1;
                placeholder.name = "Выберите производителя";
                manufacturerSource.Insert(0, placeholder);
                cb_manufacturer.ItemsSource = manufacturerSource;
                cb_manufacturer.SelectedIndex = 0;
            }

            //Загрузка списка поставщиков
            {
                var providerSource = conn.C_providers.ToList();
                C_providers placeholder = new C_providers();
                placeholder.ID_provider = -1;
                placeholder.name = "Выберите поставщика";
                providerSource.Insert(0, placeholder);
                cb_provider.ItemsSource = providerSource;
                cb_provider.SelectedIndex = 0;
            }
        }

        private void click_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void click_SaveChanges(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_article.Text)
                || string.IsNullOrWhiteSpace(tb_price.Text)
                || string.IsNullOrWhiteSpace(tb_name.Text)
                || string.IsNullOrWhiteSpace(tb_description.Text)
                || string.IsNullOrWhiteSpace(tb_discount.Text)
                || string.IsNullOrWhiteSpace(tb_count.Text)
                || cb_category.SelectedIndex <= 0
                || cb_manufacturer.SelectedIndex <= 0
                || cb_provider.SelectedIndex <= 0)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            try
            {
                product.article = tb_article.Text;
                product.price = Convert.ToDouble(tb_price.Text);
                product.name = tb_name.Text;
                product.description = tb_description.Text;
                product.discount = Convert.ToDouble(tb_discount.Text);
                product.count = Convert.ToInt32(tb_count.Text);
                product.category_ID = (int)cb_category.SelectedValue;
                product.manufacturer_ID = (int)cb_manufacturer.SelectedValue;
                product.provider_ID = (int)cb_provider.SelectedValue;
                if (isNewProduct)
                {
                    conn.C_products.Add(product);
                }
                else
                {
                    conn.Entry(product).State = System.Data.EntityState.Modified;
                }
                conn.SaveChanges();
                MessageBox.Show("Успешно сохранено");
                Close();
            }
            catch { MessageBox.Show("Некорректно заполнены поля"); }
        }
    }
}
