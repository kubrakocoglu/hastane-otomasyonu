using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace VisualProject
{
    public partial class PatientWindow : Form
    {
        public PatientWindow()
        {
            InitializeComponent();
        }

        private void PatientWindow_Load(object sender, EventArgs e)
        {
            VeriCek();
            LoadData();
            LoadBloodGroupChart();

        }

        private void VeriCek()
        {
            using (SQLiteConnection cnn = new SQLiteConnection("Data Source=hospital.db;Version=3;"))
            {
                try
                {
                    cnn.Open();
                    string query = "SELECT * FROM Patient"; 
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(query, cnn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.AutoGenerateColumns = true; 
                        dataGridView1.DataSource = null;         
                        dataGridView1.DataSource = dt;          
                        dataGridView1.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null) 
            {
                txtAd.Text = dataGridView1.CurrentRow.Cells["FirstName"].Value.ToString();
                txtSoyad.Text = dataGridView1.CurrentRow.Cells["LastName"].Value.ToString();
                txtCepNo.Text = dataGridView1.CurrentRow.Cells["PhoneNumber"].Value.ToString();
                txtKanGrubu.Text = dataGridView1.CurrentRow.Cells["BloodGroup"].Value.ToString();
                txtHastalik.Text = dataGridView1.CurrentRow.Cells["Sickness"].Value.ToString();
                txtDescription.Text = dataGridView1.CurrentRow.Cells["Description"].Value?.ToString();
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                string id = dataGridView1.CurrentRow.Cells["Id"].Value.ToString(); 
                string query = "UPDATE Patient SET FirstName=@FirstName, LastName=@LastName, PhoneNumber=@PhoneNumber, " +
                               "BloodGroup=@BloodGroup, Sickness=@Sickness, Description=@Description " +
                               "WHERE Id=@Id";

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=hospital.db;Version=3;"))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", txtAd.Text);
                        cmd.Parameters.AddWithValue("@LastName", txtSoyad.Text);
                        cmd.Parameters.AddWithValue("@PhoneNumber", txtCepNo.Text);
                        cmd.Parameters.AddWithValue("@BloodGroup", txtKanGrubu.Text);
                        cmd.Parameters.AddWithValue("@Sickness", txtHastalik.Text);
                        cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                        cmd.Parameters.AddWithValue("@Id", id);

                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                MessageBox.Show("Kayıt başarıyla güncellendi!");
                
                LoadData();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                string id = dataGridView1.CurrentRow.Cells["Id"].Value.ToString();

                DialogResult result = MessageBox.Show("Seçilen hastayı silmek istediğinize emin misiniz?",
                                                      "Silme Onayı", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM Patient WHERE Id=@Id";

                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=hospital.db;Version=3;"))
                    {
                        conn.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                    MessageBox.Show("Kayıt başarıyla silindi!");
                   
                    LoadData();
                }
            }

        }
        private void LoadData()
        {
            string query = "SELECT * FROM Patient";
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=hospital.db;Version=3;"))
            {
                conn.Open();
                SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                conn.Close();
            }
        }

        private void LoadBloodGroupChart()
        {
            
            string connectionString = "Data Source=hospital.db;Version=3;";
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    
                    string query = "SELECT BloodGroup, COUNT(*) AS Total FROM Patient GROUP BY BloodGroup";
                    SQLiteCommand cmd = new SQLiteCommand(query, conn);

                    
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    
                    chartBloodGroup.Series.Clear();
                    chartBloodGroup.Titles.Clear();

                    
                    chartBloodGroup.Titles.Add("Kan Grubu Dağılımı");
                    Series series = chartBloodGroup.Series.Add("Blood Groups");

                    
                    series.ChartType = SeriesChartType.Pie;

                    
                    foreach (DataRow row in dt.Rows)
                    {
                        series.Points.AddXY(row["BloodGroup"].ToString(), row["Total"]);
                    }

                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }
           
        }
}

