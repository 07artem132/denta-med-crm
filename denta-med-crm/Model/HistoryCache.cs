using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denta_med_crm.Model
{
    //Список всех строк
    public class HistoryCache : HashSet<Doctor>
    {
        public HistoryCache()
        {
        }

        public void TryAdd(Doctor item)
        {
            if (!Contains(item) && item != null && item.Name != "")
                Add(item);
        }

        public HistoryCache(IEnumerable<Doctor> selection)
        {
            foreach (var item in selection)
            {
                if (!Contains(item))
                    Add(item);
            }
        }

        public IEnumerable<Doctor> Tip(Doctor startsWith)
        {
            foreach (var item in this)
            {
                if (item.Name.StartsWith(startsWith.Name))
                    yield return item;
            }
        }
    }
}
