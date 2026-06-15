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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demex3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        dbEntities conn = new dbEntities();
        public AuthWindow()
        {
            InitializeComponent();
            
        }
      
        private void Click_LoginAsUser(object sender, RoutedEventArgs e)
        // Кнопка авторизации пользователя по логину и паролю
        {
                // Поиск пользователя по связке логин + пароль в базе данных.
                C_users user = conn.C_users.FirstOrDefault(c=>c.login == tb_Login.Text && c.password == pb_Password.Password);
                if (user != null)
                {
                    // Инициализация страницы товаров
                    PageProducts _Page = new PageProducts(null, user);
                    _Page.Show();
                    this.Close();
                }
                else MessageBox.Show("Неверный логин или пароль");
        }

        private void Click_LoginAsGuest(object sender, RoutedEventArgs e)
        // Кнопка авторизации в роли гостя
        {
            // Создание пустого пользователя (с ролью гостя и плейсхолдером ФИО)
            C_users guest = new C_users();
            guest.role_ID = -1;
            guest.FIO = "Вы вошли как гость";
                PageProducts _Page = new PageProducts(null, guest);
                _Page.Show();
                this.Close();
        }
    }
}
