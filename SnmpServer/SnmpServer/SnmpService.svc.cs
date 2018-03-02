using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using SnmpSharpNet;

namespace SnmpServer {
//    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    // NOTE: In order to launch WCF Test Client for testing this service, please select SnmpService.svc or SnmpService.svc.cs at the Solution Explorer and start debugging.
    public class SnmpService : ISnmpService {
        private SimpleSnmp _snmp;
        
        public SnmpTypeObject Get(SnmpTypeObject snmpObject) {
            Console.WriteLine("Weszlo do geta restowego");
            if (_snmp == null) {
                string host = "127.0.0.1";
                string community = "public";
                _snmp = new SimpleSnmp(host, community);
            }

            string[] value = get(snmpObject.Oid);
            // NIE WIEM CO MA BYC ZWRACANE CZY TYLKO VALUE CZY CALA TABLICA
            snmpObject.Oid = value[0];
            snmpObject.Type = value[1];
            snmpObject.Value = value[2];
            return snmpObject;
        }

        public string GetString(string oid)
        {
            Console.WriteLine("weszlo w GetString");
            if (_snmp == null)
            {
                string host = "127.0.0.1";
                string community = "public";
                _snmp = new SimpleSnmp(host, community);
            }

            string[] value = get(oid);

            string wartosc = value[2];
            //Console.WriteLine(wartosc);
            return wartosc;
        }

        private string[] get(string oid) {
            if (!_snmp.Valid) {
                return null;
            }
            Dictionary<Oid, AsnType> result = _snmp.Get(SnmpVersion.Ver2,
                new string[] {oid});
            if (result == null) {
                //Console.WriteLine("No results received.");
                return null;
            }
            else {
                return readGetResult(result);
            }
        }

        private string[] readGetResult(Dictionary<Oid, AsnType> result) {
            if (result == null)
                return null;
            var rsltStrings = new string[3];
            foreach (KeyValuePair<Oid, AsnType> kvp in result) {
                rsltStrings[0] = kvp.Key.ToString();
                rsltStrings[1] = SnmpConstants.GetTypeName(kvp.Value.Type);
                rsltStrings[2] = kvp.Value.ToString();
            }
            return rsltStrings;
        }


        public SnmpTypeObject GetDataUsingDataContract(SnmpTypeObject snmpObject) {
            if (snmpObject == null) {
                throw new ArgumentNullException("snmp");
            }
            if (snmpObject.Value != null) {
                snmpObject.Value += "Suffix";
            }
            return snmpObject;
        }
    }
}