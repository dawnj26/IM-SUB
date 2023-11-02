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
    public partial class Form2 : Form
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        MySqlDataReader reader;
        public bool edit = false;
        int idno;
        public Form2(bool edit, int idno)
        {
            InitializeComponent();
            this.edit = edit;
            this.idno = idno;
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                string cs = @"server=localhost;userid=root;password=;database=enrollemnt";
                conn = new MySqlConnection(cs);
                conn.Open();
            }
            catch (MySqlException ex)
            {
                MessageBoxButtons b = MessageBoxButtons.OK;
                MessageBox.Show($"Connection error: {ex}", "Error", b, MessageBoxIcon.Error);
            }
            LoadData();
            conn.Close();
        }

        private void LoadData()
        {
            string course = "";
            if (edit)
            {
                cmd = new MySqlCommand($"SELECT * FROM student WHERE student_id = {idno}", conn);
                reader = cmd.ExecuteReader();
                reader.Read();
                surname_txt.Text = reader.GetString(1);
                lastname_txt.Text = reader.GetString(2);
                firstname_txt.Text = reader.GetString(3);
                mobile_txt.Text = reader.GetString(5);
                bday_date.Value = reader.GetDateTime(6);
                address_txt.Text = reader.GetString(7);
                course = reader.GetString(8);
                reader.Close();
            }
            cmd = new MySqlCommand("SELECT * FROM courses", conn);

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                course_combo.Items.Add(reader.GetString(0));
            }
            if (!edit)
                course_combo.SelectedIndex = 0;
            else
                course_combo.SelectedItem = course;
            reader.Close();
        }

        private void reset()
        {
            surname_txt.Clear();
            firstname_txt.Clear();
            lastname_txt.Clear();
            mobile_txt.Clear();
            address_txt.Clear();
            bday_date.ResetText();
            male_radio.Checked = true;
            course_combo.SelectedIndex = 0;
        }

        private void edit_btn_Click(object sender, EventArgs e)
        {
            MessageBoxButtons b = MessageBoxButtons.OK;
            if (string.IsNullOrEmpty(surname_txt.Text) || string.IsNullOrEmpty(firstname_txt.Text) || string.IsNullOrEmpty(lastname_txt.Text) || string.IsNullOrEmpty(mobile_txt.Text) || string.IsNullOrEmpty(address_txt.Text))
            {
                MessageBox.Show("Must complete the form before enrolling!", "Warning", b, MessageBoxIcon.Warning);
                return;
            }

            string surname = surname_txt.Text;
            string firstname = firstname_txt.Text;
            string middlename = lastname_txt.Text;
            string mobile = mobile_txt.Text;
            string address = address_txt.Text;
            string bday = Convert.ToDateTime(bday_date.Text).ToString("yyyy-MM-dd");
            string course = course_combo.Text;
            string sex = male_radio.Checked ? "Male" : "Female";

            string insert = $"INSERT INTO student (surname, middle_name, first_name, sex, mobile_number, date_birth, address, course_code) VALUES ('{surname}', '{middlename}', '{firstname}', '{sex}', '{mobile}', '{bday}', '{address}', '{course}')";
            string update = $"UPDATE student SET surname = '{surname}', middle_name = '{middlename}', first_name = '{firstname}', sex = '{sex}', mobile_number = '{mobile}', date_birth = '{bday}', address = '{address}', course_code = '{course}' WHERE student_id = {idno}";
            string q = edit ? update : insert;
            conn.Open();
            cmd.CommandText = q;
            cmd.ExecuteNonQuery();
            conn.Close();
            string message = edit ? "Updated successfully!" : "Successfully Enrolled!";
            MessageBox.Show(message, "Success", b, MessageBoxIcon.Information);
            if (edit)
            {
                this.Close();
            }
            reset();


        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            reset();
        }
    }
}
