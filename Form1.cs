using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace bd_lab
{
    public partial class Form1 : Form
    {
        private string connectionstring;

        private DataSet dataSet = new DataSet();

        private SqlCommandBuilder tourBuilder;
        private SqlCommandBuilder agencyBuilder;
        private SqlCommandBuilder contractBuilder;
        private SqlCommandBuilder employeeBuilder;
        private SqlCommandBuilder countryBuilder;
        private SqlCommandBuilder clientBuilder;

        private SqlDataAdapter tourAdapter;
        private SqlDataAdapter agencyAdapter;
        private SqlDataAdapter contractAdapter;
        private SqlDataAdapter countryAdapter;
        private SqlDataAdapter employeeAdapter;
        private SqlDataAdapter clientAdapter;
        public Form1()
        {
            InitializeComponent();
            connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //tourAdapter = new SqlDataAdapter("Select * from \"tour\"", connectionstring);
            //agencyAdapter = new SqlDataAdapter("Select * from agency", connectionstring);
            contractAdapter = new SqlDataAdapter("Select * from contract", connectionstring);
            //countryAdapter = new SqlDataAdapter("Select * from country", connectionstring);
            employeeAdapter = new SqlDataAdapter("Select * from employee", connectionstring);
            clientAdapter = new SqlDataAdapter("Select * from \"client\"", connectionstring);

            //tourBuilder = new SqlCommandBuilder(tourAdapter);
            //agencyBuilder = new SqlCommandBuilder(agencyAdapter);
            contractBuilder = new SqlCommandBuilder(contractAdapter);
            //countryBuilder = new SqlCommandBuilder(countryAdapter);
            employeeBuilder = new SqlCommandBuilder(employeeAdapter);
            clientBuilder = new SqlCommandBuilder(clientAdapter);

            //tourAdapter.Fill(dataSet, "tour");
            //agencyAdapter.Fill(dataSet, "agency");
            contractAdapter.Fill(dataSet, "contract");
            //countryAdapter.Fill(dataSet, "country");
            employeeAdapter.Fill(dataSet, "employee");
            clientAdapter.Fill(dataSet, "client");

            dataGridView1.DataSource = dataSet.Tables["client"];
            dataGridView2.DataSource = dataSet.Tables["contract"];
            FillCombobox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                clientAdapter.Update(dataSet, "client");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при обновлении таблицы");
            }
        }

        private void FillCombobox()
        {
            ((DataGridViewComboBoxColumn)dataGridView2.Columns["id_employee"]).DataSource =
                dataSet.Tables["employee"];
            ((DataGridViewComboBoxColumn)dataGridView2.Columns["id_employee"]).DisplayMember =
                "fullname";
            ((DataGridViewComboBoxColumn)dataGridView2.Columns["id_employee"]).ValueMember =
                "id";

            ((DataGridViewComboBoxColumn)dataGridView2.Columns["id_client"]).DataSource =
                dataSet.Tables["client"];
            ((DataGridViewComboBoxColumn)dataGridView2.Columns["id_client"]).DisplayMember =
                "fullname";
            ((DataGridViewComboBoxColumn)dataGridView2.Columns["id_client"]).ValueMember =
                "id";

        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                contractAdapter.Update(dataSet, "contract");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при обновлении таблицы");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
                string query = @"USE travel_bd;
                SELECT employee.fullname, COUNT(*) AS 'Количество' 
                FROM employee 
                INNER JOIN contract ON contract.id_employee = employee.id 
                INNER JOIN tour ON tour.id = contract.id_tour 
                INNER JOIN country ON country.id = tour.id_country 
                WHERE contract.sign_date BETWEEN '" + dateTimePicker1.Value + "' AND '" + dateTimePicker2.Value + @"' 
                GROUP BY employee.fullname 
                ORDER BY COUNT(*) DESC;";
            using (SqlConnection sqlConnection = new SqlConnection(connectionstring))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable("report1");
                dt.Columns.Add("Name");
                dt.Columns.Add("Quantity");
                while (reader.Read())
                {
                    DataRow row = dt.NewRow();
                    row["Name"] = reader["fullname"];
                    row["Quantity"] = reader["Количество"];
                    dt.Rows.Add(row);
                }
                reader.Close();
                dataGridView3.DataSource = dt;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionstring))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("CountTour", sqlConnection);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.Parameters.Add(new SqlParameter("@country_name", SqlDbType.VarChar, 50));
                adapter.SelectCommand.Parameters["@country_name"].Value = textBox1.Text;
                DataSet ds = new DataSet();
                adapter.Fill(ds, "report2");
                dataGridView4.DataSource = ds.Tables["report2"];
            }
        }
    }
}
