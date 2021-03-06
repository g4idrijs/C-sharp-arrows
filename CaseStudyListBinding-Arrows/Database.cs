﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ArrowDataBinding.Bindings;

namespace CaseStudyListBinding_Arrows
{
    public class Order
    {
        public string Product { get; set; }
        public Supplier Supplier { get; set; }
        public int Volume { get; set; }
        public Customer Customer { get; set; }

        public Order(string product, Supplier supplier, int volume, Customer customer)
        {
            this.Product = product;
            this.Supplier = supplier;
            this.Volume = volume;
            this.Customer = customer;
        }

        public override string ToString()
        {
            return String.Format("Order from {0} in {1} for {2} {3}s from {4}", Customer.Name, Customer.Location, Volume, Product, Supplier.Name);
        }
    }

    public class Customer
    {
        public string Name { get; set; }
        public string Location { get; set; }

        public Customer(string name, string location)
        {
            this.Name = name;
            this.Location = location;
        }
    }

    public class Supplier
    {
        public string Name { get; set; }
        public string Location { get; set; }

        public Supplier(string name, string location)
        {
            this.Name = name;
            this.Location = location;
        }
    }


    public class Database : Bindable
    {
        [Bindable]
        public ObservableCollection<Order> Orders { get; set; }

        public Database()
        {
            Initialise();
        }

        public void Initialise()
        {
            Supplier ms = new Supplier("Microsoft", "Redmond");
            Supplier google = new Supplier("Google", "Mountain View");
            Supplier mcDonalds = new Supplier("McDonald's", "Everywhere");
            Supplier ibm = new Supplier("IBM", "Everywhere");
            Customer jack = new Customer("A. Name", "Cambridge");
            Customer david = new Customer("D. Bindings", "Haskelsinki");
            Customer james = new Customer("Joe Coder", "Cambridge");
            Customer brenda = new Customer("Brenda", "Aberdeen");
            Customer morag = new Customer("Morag", "Aberdeen");
            Customer will = new Customer("Mister Newname", "Glasgow");

            Orders = new ObservableCollection<Order> {
                new Order("VS2012", ms, 57, jack),
                new Order("Happy meal", mcDonalds, 1, morag),
                new Order("Google car", google, 3, james),
                new Order("MS Word", ms, 5, david),
                new Order("Nexus 7", google, 1, brenda),
                new Order("Server", ibm, 50, will)
            };
        }
    }
}
