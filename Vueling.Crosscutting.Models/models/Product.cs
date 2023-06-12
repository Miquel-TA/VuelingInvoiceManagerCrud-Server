using System;
using System.Runtime.Serialization;
using System.Text;

namespace Vueling.Crosscutting.Models
{
    [DataContract]
    public class Product
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Description { get; set; }

        public override string ToString()
        {
            return $"ID: {ID}, Description: {Description}";
        }
    }

}
