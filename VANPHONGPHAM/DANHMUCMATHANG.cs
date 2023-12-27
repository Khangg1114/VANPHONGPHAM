using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace VANPHONGPHAM
{
    public partial class DANHMUCMATHANG : Form
    {
        private string connectionString = "Data Source=LAPTOP-BJ64IR0O\\ANKHANG;Initial Catalog=VANPHONGPHAM;User ID=sa;Password=123;";
        private SqlDataAdapter adapter;
        private DataTable dataTable;
        public DANHMUCMATHANG()
        {
            InitializeComponent();
        }

        private void DANHMUCMATHANG_Load(object sender, EventArgs e)
        {
            LoadMatHangData();
        }
        private void LoadMatHangData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM MAT_HANG";
                adapter = new SqlDataAdapter(query, connection);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridView1.DataSource = dataTable;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // Thêm một dòng mới vào DataTable
            DataRow newRow = dataTable.NewRow();
            dataTable.Rows.Add(newRow);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Xóa dòng được chọn khỏi DataTable
                int selectedIndex = dataGridView1.SelectedRows[0].Index;
                dataTable.Rows[selectedIndex].Delete();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // Lưu thay đổi vào cơ sở dữ liệu
            adapter.Update(dataTable);
            MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem có hàng được chọn không
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Lấy hàng được chọn
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Lấy giá trị từ các cột tương ứng và hiển thị trong TextBox
                cbLMH.Text = selectedRow.Cells["MALOAI"].Value.ToString();
                txtMMH.Text = selectedRow.Cells["MAMH"].Value.ToString();
                txtDVT.Text = selectedRow.Cells["GIABAN"].Value.ToString();
                txtTENMH.Text = selectedRow.Cells["TENMH"].Value.ToString();
                cbDVT.Text = selectedRow.Cells["DVT"].Value.ToString();
                txtMOTA.Text = selectedRow.Cells["MOTA"].Value.ToString();
                object cellValue = selectedRow.Cells["VOHIEUHOA"].Value;

                // Kiểm tra giá trị và đặt trạng thái của checkBox1
                if (cellValue != null && cellValue != DBNull.Value)
                {
                    bool VOHIEUHOAValue = Convert.ToBoolean(cellValue);
                    checkBox1.Checked = VOHIEUHOAValue;
                }
                else
                {
                    // Xử lý khi giá trị là null hoặc DBNull.Value
                    checkBox1.Checked = false; // Hoặc bạn có thể đặt giá trị mặc định khác
                }
                // ... làm tương tự cho các cột khác
            }
        }
    }
}
