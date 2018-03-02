using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnmpSharpNet;

namespace SnmpServer
{
public    class Snmp
    {
        private SimpleSnmp _snmp;

        public Snmp()
        {
            string host = "127.0.0.1";
            string community = "public";
            _snmp = new SimpleSnmp(host, community);
        }

        public string[] get(string oid)
        {
            if (!_snmp.Valid)
            {
                return null;
            }
            Dictionary<Oid, AsnType> result = _snmp.Get(SnmpVersion.Ver2,new string[] { oid });

            if (result == null)
            {
                return null;
            }
            else
            {
                return readGetResult(result);
            }
        }

        private string[] readGetResult(Dictionary<Oid, AsnType> result)
        {
            if (result == null)
                return null;
            var resultStrings = new string[3];
            foreach (KeyValuePair<Oid, AsnType> kvp in result)
            {
                resultStrings[0] = kvp.Key.ToString();
                resultStrings[1] = SnmpConstants.GetTypeName(kvp.Value.Type);
                resultStrings[2] = kvp.Value.ToString();
            }
            return resultStrings;
        }

    }
}
