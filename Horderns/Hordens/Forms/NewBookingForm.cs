﻿using System;
using System.Linq;
using System.Windows.Forms;

namespace Hordens
{
    public partial class NewBookingForm : Form
    {
        private Customer customer;
        public DateTime newBookingDate;
        public NewBookingForm()
        {
            InitializeComponent();
        }
        // Initialize list of "Job Types" and "Estimated Time" when loading this form
        private void NewBookingForm_Load(object sender, EventArgs e)
        {
            foreach (var item in Info.jobTypes)
            {
                jobType_Cmb.Items.Add(item.typeName);
            }
            //foreach (int time in Info.estimatedTime)
            //{
            //    estimatedTime_Cmb.Items.Add(time.ToString());
            //}
            
            updateCustomers();
            
            clearFields();
        }

        private void cancel_Btn_Click(object sender, EventArgs e)
        {            
            UIControl.showForm(UIControl.bookingGridForm);
        }

        private void save_Btn_Click(object sender, EventArgs e)
        {            
            // Check if Job NO is not empty
            if (jobNO_Txt.Text == "")
            {
                MessageBox.Show("Job No can not be empty. Please input!");
                jobNO_Txt.Focus();
                return;
            }
            // Check if a job No already exists in the database
            Info.bookings = DatabaseControl.getBookings();
            var list = Info.bookings.Where(b => b.jobNO == jobNO_Txt.Text).ToList();
            if (list.Count > 0)
            {
                MessageBox.Show("The booking with Job No. " + jobNO_Txt.Text + " already exists! Please input another Job No.");
                jobNO_Txt.Focus();
                return;
            }

            // Check if Job NO is not empty
            if (jobType_Cmb.Text == "")
            {
                MessageBox.Show("Job Type can not be empty. Please input!");
                jobType_Cmb.Focus();
                return;
            }
            // Check if a Customer filed is not empty
            if (customerName_Txt.Text == "")
            {
                MessageBox.Show("Customer field can not be empty. Please input!");
                customerName_Txt.Focus();
                return;
            }
            // Check if an Address filed is not empty.
            if (address1_Txt.Text == "")
            {
                MessageBox.Show("Address field can not be empty. Please input!");
                address1_Txt.Focus();
                return;
            }
            if (address2_Txt.Text == "")
            {
                MessageBox.Show("Address field can not be empty. Please input!");
                address2_Txt.Focus();
                return;
            }
            if (address3_Txt.Text == "")
            {
                MessageBox.Show("Address field can not be empty. Please input!");
                address3_Txt.Focus();
                return;
            }
            // Check if an Post Code filed is not empty.
            if (postCode_Txt.Text == "")
            {
                MessageBox.Show("Post Code can not be empty. Please input!");
                postCode_Txt.Focus();
                return;
            }
            // Check if an email filed is not empty.
            if (email_Txt.Text == "")
            {
                MessageBox.Show("Email field can not be empty. Please input!");
                email_Txt.Focus();
                return;
            }
            // Check if an Tel filed is not empty.
            if (tel_Txt.Text == "")
            {
                MessageBox.Show("Tel field can not be empty. Please input!");
                tel_Txt.Focus();
                return;
            }
            // Check if an Vehicle Model filed is not empty.
            if (vehicleModel_Txt.Text == "")
            {
                MessageBox.Show("Vehicle Model field can not be empty. Please input!");
                vehicleModel_Txt.Focus();
                return;
            }
            // Check if an Booked By filed is not empty.
            if (bookedBy_Txt.Text == "")
            {
                MessageBox.Show("Booked By field can not be empty. Please input!");
                bookedBy_Txt.Focus();
                return;
            }
            // Check if an Job Description filed is not empty.
            if (jobDescription_Txt.Text == "")
            {
                MessageBox.Show("Work Title field can not be empty. Please input!");
                jobDescription_Txt.Focus();
                return;
            }
            
            // Compare Time in and Time Out
            if (Convert.ToDouble(timeIn_Cmb.Text) > Convert.ToDouble(timeOut_Cmb.Text))
            {
                MessageBox.Show("Time in should be less than Time Out!");
                timeIn_Cmb.Focus();
                return;
            }

            int customerId = 0;
            customer = new Customer
            {
                honor = honor_Cmb.Text,
                name = customerName_Txt.Text,
                address1 = address1_Txt.Text,
                address2 = address2_Txt.Text,
                address3 = address3_Txt.Text,
                postCode = postCode_Txt.Text,
                email = email_Txt.Text,
                tel = tel_Txt.Text
            };
            if (existingCustomer_Cmb.SelectedIndex == 0)
            {                
                customerId = DatabaseControl.addCustomer(customer);
                updateCustomers();
                UIControl.editBookingForm.updateCustomers();
            }
            else
            {
                customerId = Info.customers[existingCustomer_Cmb.SelectedIndex - 1].id;
                DatabaseControl.updateCustomer(customer, customerId);
            }
            double estimatedTime = Convert.ToDouble(timeOut_Cmb.Text) - Convert.ToDouble(timeIn_Cmb.Text);
            double timeRemaining = DatabaseControl.getBookingDates(DateTime.Now.Date) - estimatedTime;
            if (timeRemaining < 0)
                timeRemaining = 0;
            var newBooking = new Booking
            {
                jobNO = jobNO_Txt.Text,
                jobType = jobType_Cmb.Text,
                customerID = customerId,
                servicePlan = servicePlan_Cmb.Text,
                vehicleMake = vehicleMake_Txt.Text,
                vehicleModel = vehicleModel_Txt.Text,
                vehicleRegNo = vehicleRegNo_Txt.Text,
                mileage = Convert.ToDouble(mileage_Txt.Text),
                loanCar = loanCar_Cmb.Text,
                timeIn = Convert.ToDouble(timeIn_Cmb.Text),
                timeOut = Convert.ToDouble(timeOut_Cmb.Text),
                bookedBy = bookedBy_Txt.Text,
                estimatedTime = estimatedTime,
                timeRemaining = timeRemaining,
                insuranceRequired = insurance_Cmb.Text,
                jobDescription = jobDescription_Txt.Text,
                notes = notes_Txt.Text,
                bookingDate = newBookingDate
            };
            if (DatabaseControl.addBooking(newBooking))
            {
                MessageBox.Show("New booking has been added succesfully!");
                UIControl.bookingGridForm.showBookings();
                clearFields();
            }
        }

       
        private void clearFields()
        {
            jobNO_Txt.Text = "";
            if (jobType_Cmb.Items.Count > 0)
                jobType_Cmb.SelectedIndex = 0;
            existingCustomer_Cmb.SelectedIndex = 0;
            customerName_Txt.Text = "";
            address1_Txt.Text = "";
            address2_Txt.Text = "";
            address3_Txt.Text = "";
            postCode_Txt.Text = "";
            servicePlan_Cmb.SelectedIndex = 0;
            email_Txt.Text = "";
            tel_Txt.Text = "";
            vehicleMake_Txt.Text = "";
            vehicleModel_Txt.Text = "";
            vehicleRegNo_Txt.Text = "";
            mileage_Txt.Text = "0";
            loanCar_Cmb.SelectedIndex = 0;
            bookedBy_Txt.Text = Info.userID;
            timeIn_Cmb.Text = "0";
            timeOut_Cmb.Text = "0";
            estimatedTime_Txt.Text = "0";
            insurance_Cmb.SelectedIndex = 1;
            jobDescription_Txt.Text = "";
            notes_Txt.Text = "";
        }

        public void updateJobTypes()
        {
            Info.jobTypes = DatabaseControl.getJobTypes();
            jobType_Cmb.Items.Clear();
            foreach(JobType job in Info.jobTypes)
            {
                jobType_Cmb.Items.Add(job.typeName);
            }
            jobType_Cmb.Text = "";
        }
        public void updateCustomers()
        {
            Info.customers = DatabaseControl.getCustomers();
            existingCustomer_Cmb.Items.Clear();
            existingCustomer_Cmb.Items.Add("(New)");
            foreach (Customer customer in Info.customers)
            {
                existingCustomer_Cmb.Items.Add(customer.name);
            }
            if (customerName_Txt.Text != "")
            {
                existingCustomer_Cmb.Text = customerName_Txt.Text;
            }
            else
            {
                existingCustomer_Cmb.SelectedIndex = 0;
            }
        }
        private void loanCar_Cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If selected "Loan Car"
            if (loanCar_Cmb.SelectedIndex == 3)
            {
                insurance_Cmb.SelectedIndex = 0;
            }
            else
            {
                insurance_Cmb.SelectedIndex = 1;
            }
        }

        private void existingCustomer_Cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (existingCustomer_Cmb.SelectedIndex == 0)
            {
                honor_Cmb.SelectedIndex = 0;
                customerName_Txt.Text = "";
                address1_Txt.Text = "";
                address2_Txt.Text = "";
                address3_Txt.Text = "";
                postCode_Txt.Text = "";
                tel_Txt.Text = "";
                email_Txt.Text = "";
            }
            else
            {
                customer = Info.customers[existingCustomer_Cmb.SelectedIndex - 1];
                honor_Cmb.Text = customer.honor;
                customerName_Txt.Text = customer.name;
                address1_Txt.Text = customer.address1;
                address2_Txt.Text = customer.address2;
                address3_Txt.Text = customer.address3;
                postCode_Txt.Text = customer.postCode;
                tel_Txt.Text = customer.tel;
                email_Txt.Text = customer.email;
            }
        }

        private void setTimeRemaining()
        {
            if (timeOut_Cmb.Text != "" && timeIn_Cmb.Text != "")
                estimatedTime_Txt.Text = (Convert.ToDouble(timeOut_Cmb.Text) - Convert.ToDouble(timeIn_Cmb.Text)).ToString();
        }

        private void mileage_Txt_Leave(object sender, EventArgs e)
        {
            try
            {
                mileage_Txt.Text = string.Format("{0:###,###}", double.Parse(mileage_Txt.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid number format!");
                mileage_Txt.Text = "0";
            }
        }

        private void timeIn_Cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            setTimeRemaining();
        }

        private void timeOut_Cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            setTimeRemaining();
        }
    }
}
