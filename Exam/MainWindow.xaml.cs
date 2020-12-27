using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Exam
{
    public partial class MainWindow : Window
    {

        SQLiteConnection sqlConnection = new SQLiteConnection("Data Source=data.db");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Head_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void HeadBtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HeadBtnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Aut.Visibility = Visibility.Visible;
        }

        private void AutBtnNoAcc_Click(object sender, RoutedEventArgs e)
        {
            Aut.Visibility = Visibility.Hidden;
            Reg.Visibility = Visibility.Visible;
        }

        private void RegBtnYesAcc_Click(object sender, RoutedEventArgs e)
        {
            Reg.Visibility = Visibility.Hidden;
            Aut.Visibility = Visibility.Visible;
        }

        private void RegBtnReg_Click(object sender, RoutedEventArgs e)
        {
            string login = RegTextLog.Text;
            string passone = RegTextPassOne.Password;
            string passtwo = RegTextPassTwo.Password;
            string email = RegTextEmail.Text;
            string name = RegTextName.Text;

            if(login.Length < 4)
            {
                RegTextLog.BorderBrush = Brushes.Red;
                RegTextLog.Focus();
                MessageBox.Show("Логин должен бысть больше трех символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                RegTextLog.BorderBrush = Brushes.White;
            }

            if (passone.Length < 4)
            {
                RegTextPassOne.BorderBrush = Brushes.Red;
                RegTextPassOne.Focus();
                MessageBox.Show("Пароль должен бысть больше трех символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                RegTextPassOne.BorderBrush = Brushes.White;
            }

            if (passone != passtwo)
            {
                RegTextPassOne.BorderBrush = Brushes.Red;
                RegTextPassTwo.BorderBrush = Brushes.Red;
                RegTextPassOne.Focus();
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                RegTextPassOne.BorderBrush = Brushes.White;
                RegTextPassTwo.BorderBrush = Brushes.White;
            }

            if (email.Contains("@") && email.Contains("."))
            {
                RegTextEmail.BorderBrush = Brushes.White;
            }
            else
            {
                RegTextEmail.BorderBrush = Brushes.Red;
                RegTextEmail.Focus();
                MessageBox.Show("Введите корректный Email", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (name.Contains(" ") && name.Contains(" "))
            {
                RegTextName.BorderBrush = Brushes.White;

                sqlConnection.Open();
                SQLiteCommand command = new SQLiteCommand("SELECT * FROM Users", sqlConnection);
                SQLiteDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    if(dr["login"].ToString() == login)
                    {
                        MessageBox.Show($"Логин {login} уже зарегестрирован", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    else if(dr["email"].ToString() == email)
                    {
                        MessageBox.Show($"Почта {email} уже зарегестрирован", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        SQLiteCommand command1 = new SQLiteCommand("insert into Users(login, pass, email, name) VALUES (@login,@pass,@email,@name)", sqlConnection);
                        command1.Parameters.AddWithValue("login", login);
                        command1.Parameters.AddWithValue("pass", passone);
                        command1.Parameters.AddWithValue("email", email);
                        command1.Parameters.AddWithValue("name", name);
                        command1.ExecuteNonQuery();

                        Reg.Visibility = Visibility.Hidden;
                        Aut.Visibility = Visibility.Visible;

                        MessageBox.Show("Регистрация успешно заврешена", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                sqlConnection.Close();
            }
            else
            {
                RegTextName.BorderBrush = Brushes.Red;
                RegTextName.Focus();
                MessageBox.Show("Введите корректные данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void AutBtnAut_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;

            sqlConnection.Open();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Users", sqlConnection);
            SQLiteDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                if (dr["login"].ToString() == AutTextLogin.Text && dr["pass"].ToString() == AutTextPassword.Password)
                {
                    result = true;

                    AccTextId.Text = dr["id"].ToString();
                    AccTextLogin.Text = dr["login"].ToString();
                    AccTextPassword.Password = dr["pass"].ToString();
                    AccTextEmail.Text = dr["email"].ToString();
                    AccTextName.Text = dr["name"].ToString();
                }
            }
            sqlConnection.Close();
          

            if(result == true)
            {
                Aut.Visibility = Visibility.Hidden;
                Calc.Visibility = Visibility.Visible;
            }
            else
            {
                AutTextLogin.Clear();
                AutTextPassword.Clear();
                AutTextLogin.Focus();

                MessageBox.Show("Логин или пароль введены неверно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AccBtnExit_Click(object sender, RoutedEventArgs e)
        {
            Acc.Visibility = Visibility.Hidden;
            Aut.Visibility = Visibility.Visible;
        }

        private void AccBtnNew_Click(object sender, RoutedEventArgs e)
        {

            sqlConnection.Open();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Users", sqlConnection);
            SQLiteDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                if (dr["id"].ToString() == AccTextId.Text)
                {
                    if (MessageBox.Show("Вы действительно хотите изменить данные?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        SQLiteCommand command1 = new SQLiteCommand("UPDATE Users set login=@login, pass=@pass, email=@email, name=@name", sqlConnection);
                        command1.Parameters.AddWithValue("login", AccTextLogin.Text);
                        command1.Parameters.AddWithValue("pass", AccTextPassword.Password);
                        command1.Parameters.AddWithValue("email", AccTextEmail.Text);
                        command1.Parameters.AddWithValue("name", AccTextName.Text);
                        command1.ExecuteNonQuery();

                        MessageBox.Show("Данные успешно изменены", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    else
                    {
                        AccTextId.Text = dr["id"].ToString();
                        AccTextLogin.Text = dr["login"].ToString();
                        AccTextPassword.Password = dr["pass"].ToString();
                        AccTextEmail.Text = dr["email"].ToString();
                        AccTextName.Text = dr["name"].ToString();
                    }
                }
            }
            sqlConnection.Close();
        }

        private void AccBtnDel_Click(object sender, RoutedEventArgs e)
        {
            sqlConnection.Open();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Users", sqlConnection);
            SQLiteDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                if (dr["id"].ToString() == AccTextId.Text)
                {
                    if (MessageBox.Show("Вы действительно хотите удалить аккаунт?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        SQLiteCommand command1 = new SQLiteCommand("DELETE FROM Users WHERE id=@id", sqlConnection);
                        command1.Parameters.AddWithValue("id", AccTextId.Text);
                        command1.ExecuteNonQuery();

                        Acc.Visibility = Visibility.Hidden;
                        Aut.Visibility = Visibility.Visible;

                        MessageBox.Show("Аккаунт успешно удален", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            sqlConnection.Close();
        }

        private void AccBtnCalc_Click(object sender, RoutedEventArgs e)
        {
            Acc.Visibility = Visibility.Hidden;
            Calc.Visibility = Visibility.Visible;
        }

        private void CalcBtnAcc_Click(object sender, RoutedEventArgs e)
        {
            Calc.Visibility = Visibility.Hidden;
            Acc.Visibility = Visibility.Visible;
        }

        private void CalcBtnCalc_Click(object sender, RoutedEventArgs e)
        {
            if(CalcTextKom.Text == "")
            {
                CalcTextKom.BorderBrush = Brushes.Red;
                MessageBox.Show("Введите команду", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                CalcTextKom.Focus();
                return;
            }
            else
            {
                CalcTextKom.BorderBrush = Brushes.White;
            }

            if (CalcTextOne.Text == "")
            {
                CalcTextOne.BorderBrush = Brushes.Red;
                MessageBox.Show("Введите первое число", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                CalcTextOne.Focus();
                return;
            }
            else
            {
                CalcTextOne.BorderBrush = Brushes.White;
            }

            if (CalcTextTwo.Text == "")
            {
                CalcTextTwo.BorderBrush = Brushes.Red;
                MessageBox.Show("Введите второе число", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                CalcTextTwo.Focus();
                return;
            }
            else
            {
                CalcTextTwo.BorderBrush = Brushes.White;
            }

            int a = Convert.ToInt32(CalcTextOne.Text);
            int b = Convert.ToInt32(CalcTextTwo.Text);
            int result;
            string time;

            if(CalcTextKom.Text == "Add" || CalcTextKom.Text == "add")
            {
                result = a + b;

                if (CalcCheckBinary.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 2);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 2);
                    }
                }

                else if(CalcCheckOctal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 8);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 8);
                    }
                }

                else if(CalcCheckDecimal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 10);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 10);
                    }
                }

                else if(CalcCheckHexadecimal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 16);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 16);
                    }
                }
            }

            if (CalcTextKom.Text == "Sub" || CalcTextKom.Text == "sub")
            {
                result = a - b;

                if (CalcCheckBinary.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 2);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 2);
                    }
                }

                else if (CalcCheckOctal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 8);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 8);
                    }
                }

                else if (CalcCheckDecimal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 10);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 10);
                    }
                }

                else if (CalcCheckHexadecimal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 16);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 16);
                    }
                }
            }

            if (CalcTextKom.Text == "Mul" || CalcTextKom.Text == "mul")
            {
                result = a * b;

                if (CalcCheckBinary.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 2);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 2);
                    }
                }

                else if (CalcCheckOctal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 8);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 8);
                    }
                }

                else if (CalcCheckDecimal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 10);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 10);
                    }
                }

                else if (CalcCheckHexadecimal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 16);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 16);
                    }
                }
            }

            if (CalcTextKom.Text == "Div" || CalcTextKom.Text == "div")
            {
                result = a / b;

                if (CalcCheckBinary.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 2);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 2);
                    }
                }

                else if (CalcCheckOctal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 8);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 8);
                    }
                }

                else if (CalcCheckDecimal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 10);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 10);
                    }
                }

                else if (CalcCheckHexadecimal.IsChecked == true)
                {
                    if (result < 0)
                    {
                        result *= -1;
                        time = Convert.ToString(result, 16);
                        result = -1 * Convert.ToInt32(time);

                        CalcTextEnd.Text = Convert.ToString(result);
                    }
                    else
                    {
                        CalcTextEnd.Text = Convert.ToString(result, 16);
                    }
                }
            }
        }
    }
}
