using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denta_med_crm.Model
{



    //Список всех строк
    public class HistoryCache : Dictionary<string, int>, IEnumerable<string>
    {
        public HistoryCache()
        {
        }

        public void TryAdd(string item)
        {
            if (string.IsNullOrEmpty(item))
                return;

            if (TryGetValue(item, out var t))
                this[item] = t + 1;
            else Add(item, 1);
        }

        public void TryRemove(string item)
        {
            if (TryGetValue(item, out var t) && t > 1)
                this[item] = t - 1;
            else Remove(item);
        }

        public HistoryCache(IEnumerable<string> selection)
        {
            foreach (var item in selection)
                TryAdd(item);
        }

        public IEnumerable<string> Tip(string startsWith)
        {
            foreach (var item in this)
            {
                if (item.Key.StartsWith(startsWith))
                    yield return item.Key;
            }
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => this.Keys.GetEnumerator();
    }
}
