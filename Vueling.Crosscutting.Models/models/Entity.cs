using System;
using System.Runtime.Serialization;
using System.Text;

namespace Vueling.Crosscutting.Models
{
    [DataContract]
    public class Entity
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string Email { get; set; }

        public override string ToString()
        {
            return $"ID: {ID}, Name: {Name}, Address: {Address}, Phone: {PhoneNumber}, Email: {Email}";
        }
    }

}
