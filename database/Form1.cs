using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace database
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        readonly string cs = @"server=localhost;userid=root;password=;database=enrollemnt";
        MySqlConnection conn = null;
        MySqlDataReader read = null;
        MySqlCommand cmd = null;
        Form f2, f3;
        int idno;

        public void refreshData()
        {
            conn.Open();
            stud_table.Rows.Clear();
            string query = "SELECT * FROM student";
            cmd = new MySqlCommand(query, conn);

            read = cmd.ExecuteReader();



            while (read.Read())
            {
                string fullname = $"{read[3]} {read[2]} {read[1]}";
                stud_table.Rows.Add(read[0], fullname, read[4], read[5], Convert.ToDateTime(read[6]).ToString("dd/MM/yyyy"), read[7], read[8]);
            }
            stud_table.Refresh();
            stud_table.CurrentCell.Selected = false;
            stud_table.CurrentRow.Selected = false;
            conn.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                conn.Close();
            } catch(MySqlException ex)
            {
                MessageBoxButtons b = MessageBoxButtons.OK;
                MessageBox.Show($"Connection error: {ex}", "Error", b, MessageBoxIcon.Error);
            }

            refreshData();

        }


        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            label_home.BackColor = Color.IndianRed;
            label_subject.BackColor = Color.Transparent;
            label_enrollment.BackColor = Color.Transparent;

            if (main_panel.Controls.Contains(f3) || main_panel.Controls.Contains(f2))
            {
                main_panel.Controls.Remove(f2);
                main_panel.Controls.Remove(f3);
                f3?.Close();
                f2?.Close();
                f3 = null;
                f2 = null;
            }



/*            main_panel.Controls.Add(stud_table);*/
            label3.Text = "DASHBOARD";
            refreshData();


        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            label_home.BackColor = Color.Transparent;
            label_subject.BackColor = Color.IndianRed;
            label_enrollment.BackColor = Color.Transparent;


            if (!main_panel.Controls.Contains(f3))
            {
                f3 = new Form3();
                f2?.Close();
                f3.TopLevel = false;
                main_panel.Controls.Add(f3);
                f3.BringToFront();
                f3.Dock = DockStyle.Fill;
                f3.Show();
            }
            

            label3.Text = "SUBJECT";
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            label_home.BackColor = Color.Transparent;
            label_subject.BackColor = Color.Transparent;
            label_enrollment.BackColor = Color.IndianRed;

            f3?.Close();
            showForm2(false);
        }

        private void showForm2(bool edit)
        {

            if (!main_panel.Controls.Contains(f2))
            {

                f2 = new Form2(edit, idno);
                f2.TopLevel = false;
                main_panel.Controls.Add(f2);
                f2.BringToFront();
                f2.Dock = DockStyle.Fill;

                f2.Show();
            }


            label3.Text = edit ? "EDIT" : "ENROLLMENT";
        }

        private void del_btn_Click(object sender, EventArgs e)
        {
            MessageBoxButtons b = MessageBoxButtons.OK;
            if (stud_table.SelectedRows.Count <= 0 && stud_table.SelectedCells.Count <= 0)
            {
                MessageBox.Show("Select a row or cell!", "Warning", b, MessageBoxIcon.Warning);
                return;
            }
            b = MessageBoxButtons.YesNo;

            if (MessageBox.Show($"Do you want to delete student with ID No. {idno}?", "Warning", b, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            string q = $"DELETE FROM student WHERE student_id = {idno}";
            conn.Open();
            cmd = new MySqlCommand(q, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            refreshData();
            MessageBox.Show("Deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void stud_table_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string temp = stud_table.CurrentRow.Cells[0].Value.ToString();
            idno = int.Parse(temp);
        }

        private void edit_btn_Click(object sender, EventArgs e)
        {
            MessageBoxButtons b = MessageBoxButtons.OK;
            if (stud_table.SelectedRows.Count <= 0 && stud_table.SelectedCells.Count <= 0)
            {
                MessageBox.Show("Select a row or cell!", "Warning", b, MessageBoxIcon.Warning);
                return;
            }
            showForm2(true);
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            refreshData();
        }
    }
}

