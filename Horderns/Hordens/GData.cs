﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hordens
{
    class GData
    {
        // variables for database info
        public static string hostAddress;
        public static string userID;
        public static string password;
        public static string port;
        public static string database;
        public static string conStr;   // Connection string without initial catalog
        public static string conStr1;  // Connection string with initial catalog

        public static List<Booking> bookings = new List<Booking>();
        public static List<JobType> jobTypes = new List<JobType>();
        public static List<Technician> technicians = new List<Technician>();   // define list of techinicians
        public static List<Customer> customers = new List<Customer>();
        public static List<BookingDate> workingHours = new List<BookingDate>();

        public static int[] estimatedTime = { 1, 2, 3, 4, 5, 6, 7, 8 };        // array of estimated time
        //public static int workingHours = 8;                                    // working hours
        public static DateTime[] blackoutDates = { new DateTime(2020, 1, 1), new DateTime(2020, 4, 10),
        new DateTime(2020, 4, 13), new DateTime(2020, 5, 8), new DateTime(2020, 5, 25), new DateTime(2020, 8, 31),
        new DateTime(2020, 12, 25), new DateTime(2020, 12, 28), new DateTime(2021, 1, 1), new DateTime(2021, 4, 2),
        new DateTime(2021, 4, 5), new DateTime(2021, 5, 3), new DateTime(2021, 5, 31), new DateTime(2021, 8, 30),
            new DateTime(2021, 12, 27),new DateTime(2021, 12, 28),};
    }
}
