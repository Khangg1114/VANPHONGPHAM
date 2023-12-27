using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace VANPHONGPHAM
{
    public partial class ConfirmationForm : Form
    {
        private string connectionString = "Data Source=LAPTOP-BJ64IR0O\\ANKHANG;Initial Catalog=VANPHONGPHAM;User ID=sa;Password=123;";
        private string password;
        private string loggedInUsername;
        public ConfirmationForm()
        {
            InitializeComponent();
        }
        public ConfirmationForm(string username) : this()  // Gọi constructor mặc định trước
        {
            loggedInUsername = username;  // Lưu giữ giá trị loggedInUsername
        }
        public string Password
        {
            get { return password; }
            private set { password = value; }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckPasswordFromDatabase(textBox1.Text))
            {
                Password = textBox1.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Mật khẩu không đúng. Vui lòng thử lại.");
                Password = null; // Đặt Password thành null để biết rằng mật khẩu không đúng
                textBox1.Clear();
            }
        }

        private bool CheckPasswordFromDatabase(string enteredPassword)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Truy vấn để lấy mật khẩu từ CSDL
                string query = "SELECT MATKHAU FROM NHAN_VIEN WHERE TENDANGNHAP = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", loggedInUsername);

                    // Lấy mật khẩu từ CSDL
                    object result = command.ExecuteScalar();

                    // Kiểm tra mật khẩu nhập vào với mật khẩu lấy từ CSDL
                    return (result != null && result != DBNull.Value) ? (enteredPassword == result.ToString()) : false;
                }
            }
        }


    }
}
