﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace Hordens
{
    public partial class BookingGridForm : Form
    {
        private class FilteredBooking
        {
            public int id { get; set; }
            public string jobNO { get; set; }
            public string jobType { get; set; }
            public string customer { get; set; }
            public string vehicleModel { get; set; }
            public string regNo { get; set; }
            public string jobDescription { get; set; }
            public string bookedBy { get; set; }
            public string loanCar { get; set; }
            public DateTime timeIn { get; set; }
        }

        DateTime lastDate;

        public BookingGridForm()
        {
            InitializeComponent();
        }
        private void BookingGridForm_Load(object sender, EventArgs e)
        {
            showBookings();
            dataGridView1.Columns[1].HeaderText = "Job NO";
            dataGridView1.Columns[2].HeaderText = "Job Type";
            dataGridView1.Columns[3].HeaderText = "Customer";
            dataGridView1.Columns[4].HeaderText = "Vehicle Model";
            dataGridView1.Columns[5].HeaderText = "Reg No";
            dataGridView1.Columns[6].HeaderText = "Work Title";
            dataGridView1.Columns[7].HeaderText = "Booked By";
            dataGridView1.Columns[8].HeaderText = "Loan Car";
            dataGridView1.Columns[9].HeaderText = "Time In";
            dataGridView1.Columns[9].DefaultCellStyle.Format = "HH:mm";
            // Add Edit button column in datagridview
            DataGridViewButtonColumn editCol = new DataGridViewButtonColumn();
            editCol.Name = "dataGridViewEditButton";
            editCol.HeaderText = "";
            editCol.Text = "Edit";
            editCol.UseColumnTextForButtonValue = true;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns.Add(editCol);
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Width = 50;
            dataGridView1.Columns[2].Width = 70;
            dataGridView1.Columns[3].Width = 300;
            dataGridView1.Columns[6].Width = 300;
            dataGridView1.Columns["dataGridViewEditButton"].Width = 50;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.ReadOnly = true;
            // Set blackout dates from datetimepicker
            lastDate = dateTimePicker1.Value;

        }

        // Show booking grid with booking list from SQL Table
        public void showBookings()
        {
            BindingSource bs = new BindingSource();
            Info.bookings = DatabaseControl.getBookings();
            bs.DataSource = Info.bookings.Select(b => new FilteredBooking()
            {
                id = b.id,
                jobNO = b.jobNO,
                jobType = b.jobType,
                customer = b.customer,
                vehicleModel = b.vehicleModel,
                regNo = b.vehicleRegNo,
                bookedBy = b.bookedBy,
                loanCar = b.loanCar,
                jobDescription = b.jobDescription,
                timeIn = b.timeIn
            }).Where(b => b.timeIn.Date == dateTimePicker1.Value.Date).ToList();
            dataGridView1.DataSource = bs;



        }
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            updateColumnColor();
        }

        public void updateColumnColor()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var jobType = row.Cells["jobType"].Value;
                if (jobType != null)
                {
                    string background = Info.jobTypes.Where(j => j.typeName == jobType.ToString()).ToList()[0].background;
                    row.Cells["jobType"].Style.BackColor = ColorTranslator.FromHtml(background);
                }
            }
        }
        private void add_Btn_Click(object sender, EventArgs e)
        {
            UIControl.showForm(UIControl.newBookingForm);
        }


        private void del_Btn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                int id = (int)dataGridView1.Rows[index].Cells["ID"].Value;
                var result = MessageBox.Show("Are you sure want to deleted this booking?", "Warning!", MessageBoxButtons.YesNo);
                Booking booking = Info.bookings.Where(b => b.id == id).ToList()[0];
                if (result == DialogResult.Yes)
                {
                    if (DatabaseControl.deleteBooking(booking))
                    {
                        MessageBox.Show("Selected booking has been deleted!");
                        showBookings();
                    }
                }
                else
                {
                    return;
                }
            }
        }

        // When double click cell in the datagridview, navigate to the BookingDetailForm
        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.SelectedRows[0].IsNewRow)
                return;

            UIControl.showForm(UIControl.bookingDetailForm);
            Booking booking = Info.bookings[dataGridView1.SelectedRows[0].Index];
            UIControl.bookingDetailForm.getDescription(booking);

        }

        // Edit and Delete bookings by click buttons.
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == dataGridView1.NewRowIndex || e.RowIndex < 0)
                return;

            int id = (int)dataGridView1.Rows[e.RowIndex].Cells["id"].Value;
            if (e.ColumnIndex == dataGridView1.Columns["dataGridViewEditButton"].Index)
            {
                UIControl.showForm(UIControl.editBookingForm);                
                UIControl.editBookingForm.bookingToEdit = Info.bookings.Where(b => b.id == id).ToList()[0];
                UIControl.editBookingForm.getDescription();
            }
            if (e.ColumnIndex == dataGridView1.Columns["customer"].Index)
            {
                UIControl.showForm(UIControl.bookingDetailForm);
                Booking booking = Info.bookings[dataGridView1.SelectedRows[0].Index];
                UIControl.bookingDetailForm.getDescription(booking);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {            
            foreach (DateTime dt in Info.blackoutDates)
            {
                if (dt.Date == dateTimePicker1.Value.Date)
                {
                    MessageBox.Show("Can not select holidays!");
                    dateTimePicker1.Value = lastDate;
                    return;
                }
            }
            if (dateTimePicker1.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Can not select Sundays!");
                dateTimePicker1.Value = lastDate;
                return;
            }
            lastDate = dateTimePicker1.Value;
            showBookings();
        }
        private void print_Btn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF Files | *.pdf";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string fileName = sfd.FileName;
                Document document = new Document(PageSize.A3, 40, 40, 100, 100);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));
                document.Open();

                PdfPTable table = new PdfPTable(dataGridView1.Columns.Count - 2);
                table.WidthPercentage = 100;

                //Set columns names in the pdf file
                for (int k = 0; k < dataGridView1.Columns.Count; k++)
                {
                    if (dataGridView1.Columns[k].Name != "dataGridViewEditButton" && dataGridView1.Columns[k].HeaderText != "ID")
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dataGridView1.Columns[k].HeaderText));

                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                        cell.BackgroundColor = new iTextSharp.text.BaseColor(51, 102, 102);

                        table.AddCell(cell);
                    }
                }
                //Add values of DataTable in pdf file
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        if (dataGridView1.Rows[i].Cells[j].Value != "Edit" && dataGridView1.Columns[j].HeaderText != "ID")
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(dataGridView1.Rows[i].Cells[j].Value.ToString()));

                            //Align the cell in the center
                            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;

                            table.AddCell(cell);
                        }
                    }
                    //table.AddCell(new PdfPCell(new Phrase("\n")));
                }
                iTextSharp.text.Font font = FontFactory.GetFont("Arial", 20);
                document.Add(new Phrase("Carry forward work for " + dateTimePicker1.Value.ToString("dd/MM/yyyy"), font ));
                document.Add(table);
                document.Close();
                System.Diagnostics.Process.Start(fileName);
            }
        }

       
    }
}
