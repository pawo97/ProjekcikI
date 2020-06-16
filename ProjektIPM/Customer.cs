using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektIPM
{
    //backing data source in MyViewModel
    public class Customer
    {
        public String FirstName { get; set; }
        public double LastName { get; set; }

        public Customer(String firstName, double lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        
    }
}