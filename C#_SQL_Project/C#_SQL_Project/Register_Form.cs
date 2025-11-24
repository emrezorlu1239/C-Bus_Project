using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace C__SQL_Project
{
    public partial class Register_Form : Form
    {
        // Database connection string
        private SqlConnection connection;


        // Control flag to distinguish between View Mode (true) and Register Mode (false)
        bool control;
        string gender;

        // List to hold all seat buttons
        List<Button> buttons = new List<Button>();

        // ------------------------------------------------------------------
        // Constructor and Initialization
        // ------------------------------------------------------------------

        public Register_Form()
        {
            InitializeComponent();

            string appPath = AppDomain.CurrentDomain.BaseDirectory;
           
            string rootPath = Path.Combine(appPath, @"..\..\");
            string dbPath = Path.GetFullPath(Path.Combine(rootPath, "Database1.mdf"));

            
            connection = new SqlConnection(
            $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True");

            control = ControlClass.control;

            // Add all buttons to the list
            buttons.Add(button2);
            buttons.Add(button3);
            buttons.Add(button4);
            buttons.Add(button5);
            buttons.Add(button6);
            buttons.Add(button7);
            buttons.Add(button8);
            buttons.Add(button9);
            buttons.Add(button10);
            buttons.Add(button11);
            buttons.Add(button12);
            buttons.Add(button13);
            buttons.Add(button14);
            buttons.Add(button15);
            buttons.Add(button16);
            buttons.Add(button17);
            buttons.Add(button18);
            buttons.Add(button19);
            buttons.Add(button20);
            buttons.Add(button21);
            buttons.Add(button22);
            buttons.Add(button23);
            buttons.Add(button24);
            buttons.Add(button25);

            foreach (Button btn in buttons)
            {
                btn.Click += Button_Click;
                // Enable all buttons by default, LoadSeatStatus will disable occupied ones
                btn.Enabled = true;
            }

            // UI adjustments based on mode
            if (control) // View Mode
            {
                button29.Text = "View Registration";
                button28.Visible = true;
                dataGridView1.Visible = true;
                dataGridView1.Enabled = true;
                
                button32.Visible = false; // Delete button hidden
                // No need to call LoadSeatStatus here, as SearchRecord/UpdateSeatColor handles coloring in view mode.
            }
            else // Register Mode
            {
                button29.Text = "Register";
                button28.Visible = false;
                dataGridView1.Visible = false;
                dataGridView1.Enabled = false;
                
                button32.Visible = true; // Delete button visible

                // Load seat statuses only in Register Mode (so user knows which seats are available)
                LoadSeatStatus();
            }
        }

        // ------------------------------------------------------------------
        // Helper Methods
        // ------------------------------------------------------------------

        public void WriteButtonTextToTextBox(object sender)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                textBox5.Text = btn.Text; // Write seat number to textbox
                // Optional: Update color immediately upon clicking a seat (e.g., to highlight the selection)
                // UpdateSeatColor(btn); 
            }
        }

        public void Button_Click(object sender, EventArgs e)
        {
            WriteButtonTextToTextBox(sender);
        }

        private void ClearInputFields()
        {
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            if (radioButton1.Checked) radioButton1.Checked = false;
            if (radioButton2.Checked) radioButton2.Checked = false;
            gender = null;
        }

        // ------------------------------------------------------------------
        // CRUD Operations
        // ------------------------------------------------------------------

        private void SearchRecord()
        {
            try
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Table] WHERE name = @name AND surname = @surname AND no = @no AND tc = @tc AND gender = @gender", connection);

                // NOTE: If any textbox is empty, parsing will throw an exception. Consider TryParse or checking for empty fields.
                cmd.Parameters.AddWithValue("@name", textBox2.Text);
                cmd.Parameters.AddWithValue("@surname", textBox3.Text);
                cmd.Parameters.AddWithValue("@tc", long.Parse(textBox4.Text));
                cmd.Parameters.AddWithValue("@no", int.Parse(textBox5.Text));
                cmd.Parameters.AddWithValue("@gender", gender);


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                dataGridView1.DataSource = table;

                // Coloring the found seat blue in View Mode
                if (textBox5.Text != "")
                {
                    Button foundButton = buttons.FirstOrDefault(b => b.Text == textBox5.Text);
                    if (foundButton != null)
                    {
                        foundButton.BackColor = Color.Blue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Revert seat color to white (or another neutral color) if an error occurs during search
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (buttons[i].Text == textBox5.Text)
                    {
                        buttons[i].BackColor = Color.White;
                    }
                }
                ClearInputFields();
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void RegisterRecord()
        {
            // Input Validation
            if (string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(gender))
            {
                MessageBox.Show("Please fill in all fields and select a seat.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBox4.Text.Length != 11)
            {
                MessageBox.Show("TC Kimlik Numarası tam olarak 11 hane olmalıdır.", "Uzunluk Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!long.TryParse(textBox4.Text, out long tc) || !int.TryParse(textBox5.Text, out int seatNo))
            {
                MessageBox.Show("TC and Seat Number must be valid number formats.", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the seat is already occupied
            string checkQuery = "SELECT COUNT(*) FROM [Table] WHERE no = @seatNo";

            try
            {
                connection.Open();

                // 1. Check Seat Availability
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@seatNo", seatNo);

                int count = (int)checkCommand.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show($"Seat number {seatNo} is already occupied. Please select another seat.", "Seat Occupied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 2. Insert New Record
                string insertQuery = "INSERT INTO [Table] (name, surname, tc, no, gender) VALUES (@name, @surname, @tc, @no, @gender)";
                SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

                insertCommand.Parameters.AddWithValue("@name", textBox2.Text);
                insertCommand.Parameters.AddWithValue("@surname", textBox3.Text);
                insertCommand.Parameters.AddWithValue("@tc", tc);
                insertCommand.Parameters.AddWithValue("@no", seatNo);
                insertCommand.Parameters.AddWithValue("@gender", gender);

                int rowsAffected = insertCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Registration successfully added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    for (int i = 0; i < buttons.Count; i++)
                    {
                        if (buttons[i].Text == seatNo.ToString())
                        {
                            buttons[i].BackColor = Color.Red; // Mark seat as occupied
                            
                            break;
                        }
                    }
                    ClearInputFields();
                }



                else
                {
                    MessageBox.Show("An issue occurred while adding the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error during registration: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        private void DeleteRecord()
        {
            // NO MODE RESTRICTION! Deletion is now allowed in both View Mode and Register Mode.

            // Check if the required fields (TC and Seat No) are filled.
            if (string.IsNullOrWhiteSpace(textBox4.Text) || string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Please enter the TC and Seat Number of the record to be deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get TC and Seat No of the record to be deleted
            if (!long.TryParse(textBox4.Text, out long tc) || !int.TryParse(textBox5.Text, out int seatNo))
            {
                MessageBox.Show("A valid TC and Seat Number must be entered for deletion.", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the corresponding seat button
            Button selectedButton = buttons.FirstOrDefault(b => b.Text == textBox5.Text);

            if (selectedButton == null)
            {
                MessageBox.Show("Selected seat button not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // Get user confirmation for deletion
            DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete the record with TC Number {tc} and Seat No {seatNo}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                // Delete query based on seat number and TC
                string deleteQuery = "DELETE FROM [Table] WHERE no = @seatNo AND tc = @tc";

                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(deleteQuery, connection);

                    command.Parameters.AddWithValue("@seatNo", seatNo);
                    command.Parameters.AddWithValue("@tc", tc);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Record successfully deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // --- GUI Update after successful deletion ---

                        // 1. Clear input fields
                        ClearInputFields();

                        // 2. Clear DataGridView (If visible)
                        if (dataGridView1.Visible)
                        {
                            dataGridView1.DataSource = null;
                        }

                        // 3. Set the deleted seat button to GREEN (LimeGreen) and enable it.
                        // This fulfills the requirement for Register Mode.
                        selectedButton.BackColor = Color.LimeGreen;
                        selectedButton.Enabled = true;

                        // 4. If in View Mode (control is true), reload all seat statuses to ensure 
                        // other seats maintain their White/Red colors, as the manual setting above 
                        // only affects the deleted seat.
                        if (control)
                        {
                            LoadSeatStatus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("No record found with the specified information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error during deletion: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Always close the connection.
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        // Seat Status and Color Logic
        // ------------------------------------------------------------------

        private void LoadSeatStatus()
        {
            // First, set all seats to Green (Available)
            foreach (Button btn in buttons)
            {
                btn.BackColor = Color.LimeGreen;
                btn.Enabled = true; // Ensure they are all enabled initially
            }

            try
            {
                connection.Open();

                string query = "SELECT no, tc FROM [Table]";
                SqlCommand command = new SqlCommand(query, connection);

                SqlDataReader reader = command.ExecuteReader();

                long viewerTC = -1;
                // NOTE: If in Register Mode, this block is mostly irrelevant unless textBox5 holds the viewer's TC. 
                // We'll keep it here as it was in the original logic, assuming the viewing TC is passed somehow.
                if (long.TryParse(textBox5.Text, out long tc))
                {
                    viewerTC = tc;
                }

                while (reader.Read())
                {
                    int seatNo = reader.GetInt32(reader.GetOrdinal("no"));
                    long registeredTC = reader.GetInt64(reader.GetOrdinal("tc"));

                    Button seatButton = buttons.FirstOrDefault(b => b.Text == seatNo.ToString());

                    if (seatButton != null)
                    {
                        if (control) // View Mode: Special coloring if viewer's own seat
                        {
                            if (registeredTC == viewerTC)
                            {
                                // Viewer's own seat is BLUE
                                seatButton.BackColor = Color.Blue;
                            }
                            else
                            {
                                // Other occupied seats are RED
                                seatButton.BackColor = Color.Red;
                            }
                            
                        }
                        else // Register Mode: Simple Red for occupied, Green for available
                        {
                            // Occupied seats are RED and disabled (cannot register for them)
                            seatButton.BackColor = Color.Red;
                            
                        }
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading seat status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        

        // ------------------------------------------------------------------
        // Button Click Handlers
        // ------------------------------------------------------------------

        private void button29_Click(object sender, EventArgs e)
        {
            if (control) // View Mode
            {
                SearchRecord();
            }
            else // Register Mode
            {
                RegisterRecord();
            }
        }

        private void button32_Click(object sender, EventArgs e) // Delete Button
        {
            DeleteRecord();
        }

        private void button30_Click(object sender, EventArgs e) // Back Button
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                gender = "Male";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                gender = "Female";
            }
        }


    }
}