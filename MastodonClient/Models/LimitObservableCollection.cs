using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastodonClient.Models
{
    public class LimitObservableCollection<T> : ObservableCollection<T>
    {
        private int _Limit;

        public LimitObservableCollection(int limit)
        {
            _Limit = limit;
        }

        public new void Add(T item)
        {
            if (this.Count > _Limit)
            {
                this.RemoveAt(this.Count - 1);
            }

            this.Insert(0, item);
        }
    }
}
