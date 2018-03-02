using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ServerProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISnmpService" in both code and config file together.
    [ServiceContract]
    public interface ISnmpService
    {

        [OperationContract]
        SnmpTypeObject Get(SnmpTypeObject snmpObject);

        [OperationContract]
        string GetString(string oid);

        [OperationContract]
        SnmpTypeObject GetDataUsingDataContract(SnmpTypeObject snmp);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add snmp types to service operations.
    [DataContract]
    public class SnmpTypeObject
    {
        [DataMember]
        public string Oid { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string Type { get; set; }

        public SnmpTypeObject(string Oid, string Value, string Type)
        {
            this.Oid = Oid;
            this.Value = Value;
            this.Type = Type;
        }
    }
}
