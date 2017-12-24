﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Mother
    {
        public int id { set; get; }

        public string familyName { set; get; }

        public string firstName { set; get; }

        public int phoneNumber { set; get; }

        public string address { set; get; }

        public string addressRadius { set; get; }   // not clear

        public bool[] daysOfNanny = new bool[6]; // 
        public Day[] hoursByNanny = new Day[6]; 



        public string comments { set; get; }
        // more options if needed

        public override string ToString()
        {
            return familyName + ' ' + firstName + " - MOTHER\n" + address + "\n id:\t" + id +'\n';
        }

        public Mother(string my_familyName, string my_firstName, string my_address, int my_id, int my_phone)
        {
            familyName = my_familyName;
            firstName = my_firstName;
            address = my_address;
            id = my_id;
            phoneNumber = my_phone;
        }
    }
}