using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BottleAutomat
{
    class Bottle
    {

        //We need to have som bottle to use in out Automat, so a bottle class is nessesary

        //Fields
        private string type;
        private int serialNumber;


        //Properties
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public int SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }

        //Constructors
        public Bottle()
        {

        }

        public Bottle(string type, int serialNumber)
        {
            this.type = type;
            this.serialNumber = serialNumber;
        }

    }
}
