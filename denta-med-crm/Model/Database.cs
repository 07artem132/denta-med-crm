using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace denta_med_crm.Model
{
    public class Database
    {
        public List<Client> Clients;
        public readonly HistoryCache DoctorsHistory;
        public readonly HistoryCache ProcedureNameHistory;
        private Timer Timer = null;
        private string Path;
        public Database()
        {
            Clients = new List<Client>();
            DoctorsHistory = new HistoryCache();
            ProcedureNameHistory = new HistoryCache();
        }

        private void FillInHistory()
        {
            foreach (var procedure in EnumerateProcedures())
            {
                DoctorsHistory.TryAdd(procedure.Doctor);
                ProcedureNameHistory.TryAdd(procedure.ProcedureName);
            }
        }

        public void Import(string path)
        {
            var serializer = JsonSerializer.Create();
            Clients = JsonConvert.DeserializeObject<List<Client>>(File.ReadAllText(path));

            FillInHistory();
        }
        public void Export(object path)
        {
            var json = JsonConvert.SerializeObject(Clients);
            File.WriteAllText(path.ToString(), json);
        }
        public void Export(string path)
        {
            var json = JsonConvert.SerializeObject(Clients);
            File.WriteAllText(path, json);
            if (Timer != null)
                Timer.Dispose();
            Timer = new Timer(new TimerCallback(Export), path, 0, 1000 * 1);
        }

        public IEnumerable<Procedure> EnumerateProcedures()
        {
            foreach (var item in Clients)
            {
                foreach (var p in item.Procedures)
                {
                    //TODO: filters
                    yield return p;
                }
            }
        }

        public void AddClient(Client client)
        {
            Clients.Add(client);
        }

        public void OnProcedureAdded(Procedure procedure)
        {
            DoctorsHistory.TryAdd(procedure.Doctor);
            ProcedureNameHistory.TryAdd(procedure.ProcedureName);
        }

    }
}
