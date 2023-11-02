using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace database
{
    public partial class Form3 : Form
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        MySqlDataReader reader;
        string subcode;
        public Form3()
        {
            InitializeComponent();
        }

        private void save_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(subcode_txt.Text) || string.IsNullOrEmpty(subdesc_txt.Text) || string.IsNullOrEmpty(units_txt.Text))
            {
                MessageBoxButtons b = MessageBoxButtons.OK;
                MessageBox.Show("Must complete the form before adding!", "Warning", b, MessageBoxIcon.Warning);
                return;
            }

            string subcode = subcode_txt.Text;
            string subdesc = subdesc_txt.Text;
            int units = int.Parse(units_txt.Text);

            conn.Open();
            string q = $"INSERT INTO subjects (code, sub_desc, units) VALUES ('{subcode}', '{subdesc}', {units})";
            cmd = new MySqlCommand(q, conn);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Saved Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            conn.Close();
            refreshData();
            subcode_txt.Clear();
            subdesc_txt.Clear();
            units_txt.Clear();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            try
            {
                string cs = @"server=localhost;userid=root;password=;database=enrollemnt";
                conn = new MySqlConnection(cs);
                conn.Open();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBoxButtons b = MessageBoxButtons.OK;
                MessageBox.Show($"Connection error: {ex}", "Error", b, MessageBoxIcon.Error);
            }
            refreshData();

        }

        private void refreshData()
        {
            conn.Open();
            dataGridView1.Rows.Clear();
            string q = "SELECT * FROM subjects";
            cmd = new MySqlCommand(q, conn);
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetInt32(2));
            }
            reader.Close();
            conn.Close();
            dataGridView1.CurrentCell.Selected = false;
            dataGridView1.CurrentRow.Selected = false;
        }

        private void units_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void searchbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(searchbox.Text))
            {
                refreshData();
                return;
            }

            conn.Open();
            dataGridView1.Rows.Clear();
            string key = searchbox.Text;
            string q = $"SELECT * FROM subjects WHERE code COLLATE utf8mb4_unicode_520_ci LIKE '%{key}%' OR sub_desc COLLATE utf8mb4_unicode_520_ci LIKE '%{key}%'";
            cmd = new MySqlCommand(q, conn);
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                dataGridView1.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetInt32(2));
            }
            reader.Close();
            conn.Close();
        }

        private void deletebtn_Click(object sender, EventArgs e)
        {
            MessageBoxButtons b = MessageBoxButtons.OK;
            if (dataGridView1.SelectedRows.Count <= 0 && dataGridView1.SelectedCells.Count <= 0)
            {
                MessageBox.Show("Select a row or cell!", "Warning", b, MessageBoxIcon.Warning);
                return;
            }
            b = MessageBoxButtons.YesNo;

            if (MessageBox.Show($"Do you want to delete subject with code {subcode}?", "Warning", b, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            string q = $"DELETE FROM subjects WHERE code = '{subcode}'";
            conn.Open();
            cmd = new MySqlCommand(q, conn);
            cmd.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            refreshData();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string temp = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            subcode = temp;
        }
    }
}
