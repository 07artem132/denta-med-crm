using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using Akavache;
using Akavache.Sqlite3;
using Newtonsoft.Json;
using Registrations = Akavache.Registrations;

namespace denta_med_crm.Model
{
    public class Database
    {
        public List<Client> Clients;
        public HistoryCache DoctorsHistory;
        private readonly Timer Timer = null;
        private readonly object locker = new object();

        private static string CurrentPath
        {
            get
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                return Uri.UnescapeDataString(
                    new Uri(Path.GetDirectoryName(path) ?? throw new InvalidOperationException()).AbsolutePath);
            }
        }

        public Database()
        {
            DoctorsHistory = new HistoryCache();

            if (!Directory.Exists("backups"))
                Directory.CreateDirectory("backups");
            Registrations.Start("DriverControl");

            BlobCache.LocalMachine = new SqlRawPersistentBlobCache(Path.Combine(CurrentPath, "local.db"));
            BlobCache.UserAccount = new SqlRawPersistentBlobCache(Path.Combine(CurrentPath, "user.db"));
            BlobCache.Secure = new SQLiteEncryptedBlobCache(Path.Combine(CurrentPath, "secure.db"));
            try
            {
                Clients = (List<Client>) BlobCache.UserAccount.GetAllObjects<Client>().Wait();
                FillInHistory();
                Export(Path.Combine(Environment.CurrentDirectory,$@"backups\{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json"));
            }
            catch (KeyNotFoundException e)
            {
                MessageBox.Show(
                    "При загрузке основной базы данных произошла ошибка, загрузите резервную... критическая ситуация сообщения об ошибке:" +
                    e.Message + e.StackTrace);
                Application.Current.Shutdown();
            }
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
            try
            {
                var text = File.ReadAllText(path);
                var clients = JsonConvert.DeserializeObject<List<Client>>(text);
                if (clients == null)
                    throw new Exception();
                var id = 0;
                BlobCache.UserAccount.InvalidateAll();
                foreach (var x in clients)
                {
                    x.Id = (++id).ToString();
                    BlobCache.UserAccount.InsertObject(x.Id, x);
                }
                BlobCache.UserAccount.Flush();
            }
            catch (Exception e)
            {
                MessageBox.Show("При загрузке базы данных произошла ошибка, критическая ситуация сообщения об ошибке:" +
                                e.Message + e.StackTrace);
                return;
            }


            Clients = (List<Client>) BlobCache.UserAccount.GetAllObjects<Client>().Wait();
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

        private IEnumerable<Procedure> EnumerateProcedures()
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
            //Timer.Dispose();
        }
    }
}