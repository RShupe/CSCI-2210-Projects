﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         Driver.cs
//	Description:       The driver class is where the main method is stored.
//
//	Course:            CSCI 2210 - Data Structures
//	Author:            Ryan Shupe, shuper@etsu.edu, East Tennessee State University
//	Created:           Tuesday, Apr 09 2019
//
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Project4
{
    class Customer
    {
        String name { get; set; }
        Random rand = new Random();



        public Customer()
        {
            name = rand.Next(500).ToString();

        }

        public Customer(int n)
        {
            name = n.ToString();
        }

        public override string ToString()
        {
            return name;
        }

    }
}
