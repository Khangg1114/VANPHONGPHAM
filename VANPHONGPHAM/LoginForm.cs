using System;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace VANPHONGPHAM
{
    public partial class LoginForm : Form
    {
        private string connectionString = "Data Source=LAPTOP-BJ64IR0O\\ANKHANG;Initial Catalog=VANPHONGPHAM;User ID=sa;Password=123;";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Lấy thông tin từ TextBox
                string username = textBox1.Text;
                string password = textBox2.Text;

                // Debug: Hiển thị thông tin đăng nhập
                //MessageBox.Show($"Username: {username}\nPassword: {password}");

                // Kiểm tra đăng nhập
                if (CheckLogin(username, password))
                {
                    // Kiểm tra và mở form chính hoặc form phân quyền
                    if (IsAdmin(username))
                    {
  

                        MessageBox.Show("Đăng nhập thành công với vai trò Admin!");

                        PHANQUYENNGUOIDUNG phanQuyenForm = new PHANQUYENNGUOIDUNG();
                        phanQuyenForm.UserRole = GetUserRole(username);
                        phanQuyenForm.LoggedInUsername = username;
                        // Hiển thị form phân quyền người dùng
                        phanQuyenForm.Show();
                    }
                    else
                    {

                        MessageBox.Show("Đăng nhập thành công với vai trò User!");
                        // Mở form chính
                        PHANQUYENNGUOIDUNG phanQuyenForm = new PHANQUYENNGUOIDUNG();
                        phanQuyenForm.UserRole = GetUserRole(username);
                        // Hiển thị form phân quyền người dùng
                        phanQuyenForm.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }
        }
        private string GetUserRole(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Câu truy vấn để lấy giá trị vai trò từ CSDL
                string query = "SELECT LAQUANLY FROM NHAN_VIEN WHERE TENDANGNHAP = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    // Thực hiện truy vấn và lấy giá trị vai trò
                    object result = command.ExecuteScalar();

                    // Nếu kết quả là true, trả về "Admin"; ngược lại trả về "User"
                    return (result != null && result != DBNull.Value && (bool)result) ? "Admin" : "User";
                }
            }
        }
        private bool CheckLogin(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Thực hiện truy vấn kiểm tra đăng nhập
                string query = "SELECT COUNT(*) FROM NHAN_VIEN WHERE TENDANGNHAP = @Username AND MATKHAU = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    int result = (int)command.ExecuteScalar();

                    return result > 0;
                }
            }
        }
        private bool IsAdmin(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM NHAN_VIEN WHERE TENDANGNHAP = @Username AND LAQUANLY = 1";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    int result = (int)command.ExecuteScalar();

                    return result > 0;
                }
            }
        }

    }
}
