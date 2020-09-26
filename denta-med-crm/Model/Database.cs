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
        private readonly Timer Timer = null;
        private readonly string Path;
        private readonly object locker = new object();

        public Database(string permanentPath)
        {
            Path = permanentPath;
            Clients = new List<Client>();
            DoctorsHistory = new HistoryCache();
            Timer = new Timer(new TimerCallback(x => Export(this.Path)), null, 0, 1000 * 1);
        }

        private void FillInHistory()
        {
            foreach (var procedure in EnumerateProcedures())
            {
                DoctorsHistory.TryAdd(procedure.Doctor);
            }
        }

        public void Import(string path)
        {
            var serializer = JsonSerializer.Create();
            Clients = JsonConvert.DeserializeObject<List<Client>>(File.ReadAllText(path));
            FillInHistory();
        }

        public void Export(string path)
        {
            lock (locker)
            {
                var json = JsonConvert.SerializeObject(Clients);
                File.WriteAllText(path, json);
            }
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
        }

        public void Dispose()
        {
            Timer.Dispose();
        }
    }
}
