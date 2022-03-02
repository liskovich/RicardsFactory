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

namespace RicardsFactory
{
    public partial class Form1 : Form
    {
        // replace with correct local db path
        private const string _pathToDatabase = @"C:\Users\user\Desktop\Export\kd_liskovskis_db\RicardsFactory\RicardsFactory\Factory.mdf";
        private readonly string _connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={_pathToDatabase};Integrated Security=True";

        DataSet DeptsDataSet;
        DataSet ConvsDataSet;
        DataSet DevcsDataSet;


        public Form1()
        {
            InitializeComponent();

            var result = LoadDatabase();

            if (!result)
            {
                CreateDatabase();
            }
        }

        private void CreateDatabase()
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            const string createDepartmentsTable = "create table Departments(" +
            "ID_Dept int identity primary key, " +
            "Dept_Name nvarchar(50), " +
            "Dept_Location nvarchar(100), " +
            "Dept_EmployeeCount int, " +
            "Dept_UnitsProducedMonthly int" +
            ");";

            const string createConveyorsTable = "create table Conveyors(" +
            "ID_Conv int identity primary key, " +
            "Conv_Name nvarchar(50), " +
            "Conv_Operator nvarchar(50), " +
            "Conv_Length int, " +
            "ID_Dept int, " +
            "CONSTRAINT Department_Conveyor FOREIGN KEY (ID_Dept) " +
            "REFERENCES Departments (ID_Dept)" +
            ");";

            const string createDevicesTable = "create table Devices(" +
            "ID_Devc int identity primary key, " +
            "Devc_Name nvarchar(50), " +
            "Devc_Description nvarchar(200), " +
            "Devc_ProductionCost int, " +
            "Devc_PartsCount int, " +
            "ID_Conv int, " +
            "CONSTRAINT Conveyor_Device FOREIGN KEY (ID_Conv) " +
            "REFERENCES Conveyors (ID_Conv)" +
            ");";

            SqlCommand command = new SqlCommand();

            try
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = createDepartmentsTable;
                command.ExecuteNonQuery();

                command.CommandText = createConveyorsTable;
                command.ExecuteNonQuery();

                command.CommandText = createDevicesTable;
                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            RefreshView();
        }

        private bool LoadDatabase()
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            const string getDepts = "SELECT * FROM Departments";
            const string getConvs = "SELECT * FROM Conveyors";
            const string getDevcs = "SELECT * FROM Devices";

            SqlDataAdapter deptsAdapter = new SqlDataAdapter(getDepts, connection);
            SqlDataAdapter convsAdapter = new SqlDataAdapter(getConvs, connection);
            SqlDataAdapter devcsAdapter = new SqlDataAdapter(getDevcs, connection);

            DeptsDataSet = new DataSet();
            ConvsDataSet = new DataSet();
            DevcsDataSet = new DataSet();

            try
            {
                connection.Open();
                deptsAdapter.Fill(DeptsDataSet);
                convsAdapter.Fill(ConvsDataSet);
                devcsAdapter.Fill(DevcsDataSet);
                connection.Close();
            }
            catch
            {                
                return false;
            }            

            BindingSource deptsBindSource = new BindingSource();
            BindingSource convsBindSource = new BindingSource();
            BindingSource devcsBindSource = new BindingSource();

            deptsBindSource.DataSource = DeptsDataSet.Tables[0].DefaultView;
            convsBindSource.DataSource = ConvsDataSet.Tables[0].DefaultView;
            devcsBindSource.DataSource = DevcsDataSet.Tables[0].DefaultView;

            var deptsCount = DeptsDataSet.Tables[0].DefaultView.Count;
            var convsCount = ConvsDataSet.Tables[0].DefaultView.Count;
            var devcsCount = DevcsDataSet.Tables[0].DefaultView.Count;

            DeptBindingNavigator.BindingSource = deptsBindSource;
            ConvBindingNavigator.BindingSource = convsBindSource;
            DevcBindingNavigator.BindingSource = devcsBindSource;

            DeptDataGridView.DataSource = deptsBindSource;
            ConvDataGridView.DataSource = convsBindSource;
            DevcDataGridView.DataSource = devcsBindSource;

            DeptCount.Text = $"{deptsCount} records";
            ConvCount.Text = $"{convsCount} records";
            DevcCount.Text = $"{devcsCount} records";

            return true;
        }

        private void DeptSearchBtn_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand();

            try
            {
                var input = int.Parse(DeptSearch.Text);
                string getDept = $"SELECT * FROM Departments WHERE Departments.ID_Dept = {input}";
                
                connection.Open();
                command.Connection = connection;

                command.CommandText = getDept;
                SqlDataReader dataReader = command.ExecuteReader();

                if (!dataReader.HasRows)
                {
                    MessageBox.Show("Department not found");
                    DeptSearch.Text = string.Empty;

                    return;
                }

                string output = "Department:\n";
                while (dataReader.Read())
                {
                    output += "Name: " + dataReader[1] + "\n";
                    output += "Location: " + dataReader[2] + "\n";
                    output += "Employee Count: " + dataReader[3] + "\n";
                    output += "Units Produced Monthly: " + dataReader[4] + "\n";
                }
                MessageBox.Show(output);

                dataReader.Close();
                connection.Close();
            }
            catch
            {
                MessageBox.Show("Failed to get department");
            }

            DeptSearch.Text = string.Empty;
        }

        private void ConvSearchBtn_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand();

            try
            {
                var input = int.Parse(ConvSearch.Text);
                string getConv = $"SELECT * FROM Conveyors WHERE Conveyors.ID_Conv = {input}";

                connection.Open();
                command.Connection = connection;

                command.CommandText = getConv;
                SqlDataReader dataReader = command.ExecuteReader();

                if (!dataReader.HasRows)
                {
                    MessageBox.Show("Conveyor not found");
                    ConvSearch.Text = string.Empty;

                    return;
                }

                string output = "Conveyor:\n";
                while (dataReader.Read())
                {
                    output += "Name: " + dataReader[1] + "\n";
                    output += "Operator: " + dataReader[2] + "\n";
                    output += "Length: " + dataReader[3] + "\n";
                    output += "Department ID: " + dataReader[4] + "\n";
                }
                MessageBox.Show(output);

                dataReader.Close();
                connection.Close();
            }
            catch
            {
                MessageBox.Show("Failed to get Conveyor");
            }

            ConvSearch.Text = string.Empty;
        }

        private void DevcSearchBtn_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand();

            try
            {
                var input = int.Parse(DevcSearch.Text);
                string getDevc = $"SELECT * FROM Devices WHERE Devices.ID_Devc = {input}";

                connection.Open();
                command.Connection = connection;

                command.CommandText = getDevc;
                SqlDataReader dataReader = command.ExecuteReader();

                if (!dataReader.HasRows)
                {
                    MessageBox.Show("Device not found");
                    DevcSearch.Text = string.Empty;

                    return;
                }

                string output = "Device:\n";
                while (dataReader.Read())
                {
                    output += "Name: " + dataReader[1] + "\n";
                    output += "Description: " + dataReader[2] + "\n";
                    output += "Production Cost: " + dataReader[3] + "\n";
                    output += "Parts Count: " + dataReader[4] + "\n";
                    output += "Conveyor ID: " + dataReader[4] + "\n";
                }
                MessageBox.Show(output);

                dataReader.Close();
                connection.Close();
            }
            catch
            {
                MessageBox.Show("Failed to get Device");
            }

            DevcSearch.Text = string.Empty;
        }

        private void DeleteAllBtn_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            const string dropConstraintDeptConv = "ALTER TABLE Conveyors DROP CONSTRAINT Department_Conveyor;";
            const string dropConstraintConvDevc = "ALTER TABLE Devices DROP CONSTRAINT Conveyor_Device;";
            const string dropDepts = "DROP TABLE Departments;";
            const string dropConvs = "DROP TABLE Conveyors;";
            const string dropDevcs = "DROP TABLE Devices;";

            SqlCommand command = new SqlCommand();

            try
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = dropConstraintDeptConv;
                command.ExecuteNonQuery();

                command.CommandText = dropConstraintConvDevc;
                command.ExecuteNonQuery();

                command.CommandText = dropDevcs;
                command.ExecuteNonQuery();

                command.CommandText = dropConvs;
                command.ExecuteNonQuery();

                command.CommandText = dropDepts;
                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            var result = LoadDatabase();

            if (!result)
            {
                CreateDatabase();
            }

            RefreshView();
        }

        private void PopulateBtn_Click(object sender, EventArgs e)
        {
            var result = LoadDatabase();

            if (!result)
            {
                CreateDatabase();
            }

            SqlConnection connection = new SqlConnection(_connectionString);

            const string fillDeptsTable = "insert into Departments" +
            "(Dept_Name, Dept_Location, Dept_EmployeeCount, Dept_UnitsProducedMonthly)" +
            "values('Electronics', 'Riga', 45, 5000); " +
            "insert into Departments(Dept_Name, Dept_Location, Dept_EmployeeCount, Dept_UnitsProducedMonthly)" +
            "values('Hardware', 'Oslo', 200, 15000); " +
            "insert into Departments(Dept_Name, Dept_Location, Dept_EmployeeCount, Dept_UnitsProducedMonthly)" +
            "values('Robots', 'Madrid', 150, 4300); ";

            const string fillConvsTable = "insert into Conveyors" +
            "(Conv_Name, Conv_Operator, Conv_Length, ID_Dept)" +
            "values('ALI7855', 'Janis', 500, 1); " +
            "insert into Conveyors(Conv_Name, Conv_Operator, Conv_Length, ID_Dept)" +
            "values('RSL5544', 'Karl', 300, 2); " +
            "insert into Conveyors(Conv_Name, Conv_Operator, Conv_Length, ID_Dept)" +
            "values('IYJ1435', 'Martin', 1500, 3); ";

            const string fillDevcsTable = "insert into Devices" +
            "(Devc_Name, Devc_Description, Devc_ProductionCost, Devc_PartsCount, ID_Conv)" +
            "values('Switch', 'Control current flow', 13, 35, 1); " +
            "insert into Devices(Devc_Name, Devc_Description, Devc_ProductionCost, Devc_PartsCount, ID_Conv)" +
            "values('GPU', 'Graphics card', 25, 240000, 2); " +
            "insert into Devices(Devc_Name, Devc_Description, Devc_ProductionCost, Devc_PartsCount, ID_Conv)" +
            "values('Kawasaki RL5.0', 'Idustrial factory robot', 1300, 78000, 3); ";

            SqlCommand command = new SqlCommand();

            try
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = fillDeptsTable;
                command.ExecuteNonQuery();

                command.CommandText = fillConvsTable;
                command.ExecuteNonQuery();

                command.CommandText = fillDevcsTable;
                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            RefreshView();
        }

        private void RefreshView()
        {
            DeptBindingNavigator.BindingSource = null;
            ConvBindingNavigator.BindingSource = null;
            DevcBindingNavigator.BindingSource = null;

            DeptDataGridView.DataSource = null;
            ConvDataGridView.DataSource = null;
            DevcDataGridView.DataSource = null;

            DeptCount.Text = "0 records";
            ConvCount.Text = "0 records";
            DevcCount.Text = "0 records";

            LoadDatabase();
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void DeleteDeptBtn_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand();

            try
            {
                var idInput = int.Parse(ChangeIDText.Text);

                string deleteDepartment = $"DELETE FROM Departments WHERE Departments.ID_Dept = {idInput}";

                connection.Open();
                command.Connection = connection;

                command.CommandText = deleteDepartment;
                command.ExecuteNonQuery();

                connection.Close();

                ChangeIDText.Text = string.Empty;
                ChangeNameText.Text = string.Empty;
                ChangeLocationText.Text = string.Empty;
                ChangeEmployeeText.Text = string.Empty;
                ChangeUPMText.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete data: {ex.Message}");
            }

            RefreshView();
        }

        private void ChangeDeptBtn_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand();

            try
            {
                var idInput = int.Parse(ChangeIDText.Text);
                var nameInput = ChangeNameText.Text;
                var locationText = ChangeLocationText.Text;
                var employeeText = int.Parse(ChangeEmployeeText.Text);
                var upmText = int.Parse(ChangeUPMText.Text);

                if (idInput == 0 || nameInput == null || locationText == null
                    || employeeText == 0 || upmText == 0)
                {
                    MessageBox.Show("There are errors in input fields");
                }

                string updateDepartment = $"UPDATE Departments SET Dept_Name = '{nameInput}', " +
                $"Dept_Location = '{locationText}', Dept_EmployeeCount = {employeeText}, " +
                $"Dept_UnitsProducedMonthly = {upmText} WHERE Departments.ID_Dept = {idInput}";

                connection.Open();
                command.Connection = connection;

                command.CommandText = updateDepartment;
                command.ExecuteNonQuery();

                connection.Close();

                ChangeIDText.Text = string.Empty;
                ChangeNameText.Text = string.Empty;
                ChangeLocationText.Text = string.Empty;
                ChangeEmployeeText.Text = string.Empty;
                ChangeUPMText.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to modify data: {ex.Message}");
            }

            RefreshView();
        }

        private void DeptSaveBtn_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand();

            try
            {
                var nameInput = SaveNameText.Text;
                var locationText = SaveLocationText.Text;
                var employeeText = int.Parse(SaveEmployeeText.Text);
                var upmText = int.Parse(SaveUPMText.Text);

                if (nameInput == null || locationText == null
                    || employeeText == 0 || upmText == 0)
                {
                    MessageBox.Show("There are errors in input fields");
                }

                string insertDepartment = $"insert into Departments (Dept_Name, Dept_Location, Dept_EmployeeCount, Dept_UnitsProducedMonthly)" +
                $" VALUES ('{nameInput}', '{locationText}', {employeeText}, {upmText});";

                connection.Open();
                command.Connection = connection;

                command.CommandText = insertDepartment;
                command.ExecuteNonQuery();

                connection.Close();
               
                SaveNameText.Text = string.Empty;
                SaveLocationText.Text = string.Empty;
                SaveEmployeeText.Text = string.Empty;
                SaveUPMText.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save data: {ex.Message}");
            }

            RefreshView();
        }

        // Add data

        // Remove data

        // Edit data

        // Search data (text input field)

        // Get records count

        // Delete all data

        // Populate with test data
    }
}
