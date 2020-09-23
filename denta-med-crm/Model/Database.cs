using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denta_med_crm.Model
{
    public class Database
    {
        public readonly List<Client> Clients;
        public readonly HistoryCache DoctorsHistory;
        public readonly HistoryCache ProcedureNameHistory;

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
            //!

            FillInHistory();
        }

        public void Export(string path)
        {

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
