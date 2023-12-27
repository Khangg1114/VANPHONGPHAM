using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace VANPHONGPHAM
{
    public partial class PHANQUYENNGUOIDUNG : Form
    {
        private string connectionString = "Data Source=LAPTOP-BJ64IR0O\\ANKHANG;Initial Catalog=VANPHONGPHAM;User ID=sa;Password=123;";
        private bool isConfirming = false;

        public PHANQUYENNGUOIDUNG()
        {
            InitializeComponent();
        }

        private string userRole;

        public string UserRole
        {
            get { return userRole; }
            set { userRole = value; }
        }
        private DataTable dataTable;
        private SqlDataAdapter adapter;
        private void LoadEmployeeData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM NHAN_VIEN";
                adapter = new SqlDataAdapter(query, connection);

                // Khởi tạo SqlCommandBuilder để tạo các lệnh Update, Insert, Delete tự động
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Gán DataTable làm nguồn dữ liệu cho DataGridView
                dataGridView1.DataSource = dataTable;
            }
        }

        private void PHANQUYENNGUOIDUNG_Load_1(object sender, EventArgs e)
        {
            LoadEmployeeData();
            if (!IsAdmin())
            {
                HideAdminColumns();
            }
        }

        private bool IsAdmin()
        {
            return (UserRole == "Admin");
        }

        private void HideAdminColumns()
        {
            dataGridView1.Columns["LAQUANLY"].Visible = false;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng có phải là admin không
            if (IsAdmin())
            {
                // Lấy giá trị mới từ ô đã chỉnh sửa
                object newValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                // Lấy tên cột đã chỉnh sửa
                string columnName = dataGridView1.Columns[e.ColumnIndex].Name;

                // Thực hiện cập nhật trực tiếp vào cơ sở dữ liệu mà không cần xác nhận mật khẩu
                UpdateDatabase(columnName, newValue, e.RowIndex);
            }
        }

        private object tempValue;

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            tempValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        }

        private void UpdateDatabase(string columnName, object newValue, int rowIndex)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    // Thực hiện câu truy vấn UPDATE
                    string query = $"UPDATE NHAN_VIEN SET {columnName} = @NewValue WHERE [MANHANVIEN] = @PrimaryKeyValue";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Lấy giá trị khóa chính của dòng được chỉnh sửa
                        object primaryKeyValue = dataGridView1.Rows[rowIndex].Cells["MANHANVIEN"].Value;

                        command.Parameters.AddWithValue("@NewValue", newValue);
                        command.Parameters.AddWithValue("@PrimaryKeyValue", primaryKeyValue);

                        // Thực hiện câu truy vấn UPDATE
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // In thông báo lỗi nếu có
                    Console.WriteLine($"Lỗi SQL: {ex.Message}");
                }
            }
        }

        

        private string loggedInUsername;

        public string LoggedInUsername
        {
            get { return loggedInUsername; }
            set { loggedInUsername = value; }
        }

        private bool CheckPasswordFromDatabase(string enteredPassword)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT MATKHAU FROM NHAN_VIEN WHERE TENDANGNHAP = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", loggedInUsername);

                    object result = command.ExecuteScalar();

                    return (result != null && result != DBNull.Value && result.ToString() == enteredPassword);
                }
            }
        }

        private void dSNHANVIENToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PHANQUYENNGUOIDUNG phanQuyenForm = new PHANQUYENNGUOIDUNG();

            // Gọi phương thức ShowDialog để hiển thị Form và chờ đến khi Form đó được đóng
            phanQuyenForm.ShowDialog();
        }
        private void LoadAndFilterData(bool showAll)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query;
                if (showAll)
                {
                    query = "SELECT * FROM NHAN_VIEN";
                }
                else
                {
                    query = "SELECT * FROM NHAN_VIEN WHERE LAQUANLY = 1"; // Lấy chỉ những nhân viên có LAQUANLY là true
                }

                adapter = new SqlDataAdapter(query, connection);

                // Khởi tạo SqlCommandBuilder để tạo các lệnh Update, Insert, Delete tự động
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Sắp xếp dữ liệu theo ý muốn (ví dụ: theo cột MANHANVIEN)
                dataTable.DefaultView.Sort = "MANHANVIEN ASC";

                // Gán DataTable.DefaultView làm nguồn dữ liệu cho DataGridView
                dataGridView1.DataSource = dataTable.DefaultView;
            }
        }

        private void nHANVIENQUANLYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadAndFilterData(false);
        }
        private void LoadAndFilterData1(bool showOnlyAdmin)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query;
                if (showOnlyAdmin)
                {
                    query = "SELECT * FROM NHAN_VIEN WHERE LAQUANLY = 0"; // Lấy chỉ những nhân viên có LAQUANLY là false
                }
                else
                {
                    query = "SELECT * FROM NHAN_VIEN";
                }

                adapter = new SqlDataAdapter(query, connection);

                // Khởi tạo SqlCommandBuilder để tạo các lệnh Update, Insert, Delete tự động
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Sắp xếp dữ liệu theo ý muốn (ví dụ: theo cột MANHANVIEN)
                dataTable.DefaultView.Sort = "MANHANVIEN ASC";

                // Gán DataTable.DefaultView làm nguồn dữ liệu cho DataGridView
                dataGridView1.DataSource = dataTable.DefaultView;
            }
        }
        private void nHANVIENBANHANGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadAndFilterData1(true);
        }

        private void dANHMUCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DANHMUCMATHANG DMMH = new DANHMUCMATHANG();

            // Gọi phương thức ShowDialog để hiển thị Form và chờ đến khi Form đó được đóng
            DMMH.ShowDialog();
        }
    }
}
