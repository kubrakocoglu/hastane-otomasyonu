using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisualProject
{
    public partial class LoginWindow : Form
    {

        private readonly string cnnString = "Data Source=hospital.db;Version=3;";

        private readonly string query = "select count(*) from doctor where  Email = @userName and Password = @Password;";
        public LoginWindow()
        {
            InitializeComponent();
        }

        public SQLiteConnection dbConneciton()
        {
            SQLiteConnection cnn = new SQLiteConnection(cnnString);
            {
                try
                {
                    cnn.Open();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                return cnn;
            }
        }

        
        private void loginButton_Click(object sender, EventArgs e)
        {


            SQLiteConnection cnn = dbConneciton();

            if (userName.Text.Length == 0)
            {
                MessageBox.Show("Kullanıcı adı boş");
            }

            if (password.Text.Length == 0)
            {
                MessageBox.Show("Şifre boş");
            }

            SQLiteCommand cmd = new SQLiteCommand(query, cnn);

            cmd.Parameters.AddWithValue("@userName", userName.Text);
            cmd.Parameters.AddWithValue("@Password", password.Text);

            int userExists = Convert.ToInt32(cmd.ExecuteScalar());
            if (userExists == 1)
            {
                MessageBox.Show("Giriş Başarılı!");
                PatientWindow patientWindow = new PatientWindow();
                patientWindow.Show(); 
                this.Hide();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya parola yanlış!");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            InputBoxForm inputForm = new InputBoxForm("Telefon numaranızı girin:");

            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                string enteredPhoneNumber = inputForm.UserInput;

                if (string.IsNullOrEmpty(enteredPhoneNumber))
                {
                    MessageBox.Show("Telefon numarası boş bırakılamaz.");
                    return;
                }
                
                SQLiteConnection cnn = dbConneciton();

               
                string query = "SELECT Email FROM doctor WHERE PhoneNumber = @phoneNumber;";
                SQLiteCommand cmd = new SQLiteCommand(query, cnn);
                cmd.Parameters.AddWithValue("@phoneNumber", enteredPhoneNumber);

             
                string email = cmd.ExecuteScalar() as string;

                if (string.IsNullOrEmpty(email))
                {
                    MessageBox.Show("Bu telefon numarası ile kayıtlı kullanıcı bulunamadı.");
                }
                else
                {
                    
                    string newPassword = GenerateRandomPassword();

                    
                    string updateQuery = "UPDATE doctor SET Password = @newPassword WHERE PhoneNumber = @phoneNumber;";
                    SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, cnn);
                    updateCmd.Parameters.AddWithValue("@newPassword", newPassword);
                    updateCmd.Parameters.AddWithValue("@phoneNumber", enteredPhoneNumber);

                    try
                    {
                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Yeni şifreniz: {newPassword}");
                            this.Show();
                        }
                        else
                        {
                            MessageBox.Show("Şifre güncellemesi sırasında bir hata oluştu.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Bir hata oluştu: {ex.Message}");
                    }
                    
                    cnn.Close();



                }
            }
        }
        
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}
