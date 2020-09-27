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
        public HistoryCache DoctorsHistory;
        private readonly Timer Timer = null;
        private readonly string Path;
        private readonly string PathBackup;
        private readonly object locker = new object();

        public Database(string permanentPath)
        {
            Path = permanentPath;
            PathBackup = permanentPath + ".backup";
            Clients = new List<Client>();
            DoctorsHistory = new HistoryCache();
            if (File.Exists(Path))
            {
                try
                {
                    Import(Path);
                }
                catch
                {
                    Import(PathBackup);
                }
            }
            Timer = new Timer(
                new TimerCallback
                (x =>
                {
                    Export(Path);
                    Export(PathBackup);
                }), null, 0, 1000 * 1);
        }

        private void FillInHistory()
        {
            DoctorsHistory.Clear();
            foreach (var procedure in EnumerateProcedures())
            {
                DoctorsHistory.TryAdd(procedure.Doctor);
            }
            foreach (var inspection in EnumerateInspection())
            {
                DoctorsHistory.TryAdd(inspection.Doctor);
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
        public IEnumerable<Inspection> EnumerateInspection()
        {
            foreach (var item in Clients)
            {
                foreach (var p in item.Inspections)
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

        public void OnInspectionAdded(Inspection procedure)
        {
            DoctorsHistory.TryAdd(procedure.Doctor);
        }

        public void OnInspectionRemoved(Inspection procedure)
        {
            DoctorsHistory.TryRemove(procedure.Doctor);

        }

        public void OnProcedureRemoved(Procedure procedure)
        {
            DoctorsHistory.TryRemove(procedure.Doctor);
        }

        public void OnDoctorRename(string oldName, string newName)
        {
            DoctorsHistory.TryRemove(oldName);
            DoctorsHistory.TryAdd(newName);
        }

        public void Dispose()
        {
            Timer.Dispose();
        }
    }
}
