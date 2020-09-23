using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denta_med_crm.Model
{
    //Список всех строк
    public class HistoryCache : HashSet<string>
    {
        public HistoryCache()
        {
        }

        public void TryAdd(string item)
        {
            if (!Contains(item))
                Add(item);
        }

        public HistoryCache(IEnumerable<string> selection)
        {
            foreach (var item in selection)
            {
                if (!Contains(item))
                    Add(item);
            }
        }

        public IEnumerable<string> Tip(string startsWith)
        {
            foreach (var item in this)
            {
                if (item.StartsWith(startsWith))
                    yield return item;
            }
        }
    }
}
