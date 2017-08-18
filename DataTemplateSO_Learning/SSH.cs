using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace SSH
{
    class SSH
    {
        private SshClient _client;
        private SshCommand _res;
        private string[] _results;
        public SSH(string Ip, string login, string passwd)
        {
            _client = new SshClient(Ip, login, passwd);
        }

        public void Connect()
        {
            try
            {
                _client.Connect();
            }
            catch (System.Exception e)
            {
                
                _client.Disconnect();
            }
        }

        public string GetSignal()
        {
            if (_client.IsConnected)
            {
                _res = _client.RunCommand("iwconfig |grep Signal");
                _results = _res.Result.Split(' ');
                _results = _results[14].Split('-');
                //test = int.Parse(results[1]);
                return _results[1];
            }
            return "0";

        }

        public void DisConnect()
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
        }

    }



    
}
