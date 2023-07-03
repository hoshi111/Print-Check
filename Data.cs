using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Print_Check
{
    public class Data
    {
        public string id { get; set; }
        public string RefNumber { get; set; }
        public string PayeeEntityRefFullName { get; set; }
        public string TxnDate { get; set; }
        public string Amount { get; set; }
    }
}
