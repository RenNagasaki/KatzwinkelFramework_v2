using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    [DataContract]
    public class AppointmentObject
    {
        [DataMember]
        public int id = -1;

        [DataMember]
        public string userName;

        [DataMember]
        public string name;

        [DataMember]
        public string description;

        [DataMember]
        public int day;

        [DataMember]
        public int month;

        [DataMember]
        public int year;

        [DataMember]
        public int fromHour;

        [DataMember]
        public int fromMinute;

        [DataMember]
        public int toHour;

        [DataMember]
        public int toMinute;
    }
}
