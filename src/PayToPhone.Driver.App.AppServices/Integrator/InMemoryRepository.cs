using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayToPhone.Driver.App.AppServices.Integrator {
    internal class InMemoryRepository<TKey, TItem> {
        private readonly ConcurrentDictionary<TKey, TItem> ObjectList = new ConcurrentDictionary<TKey, TItem>();

        public TItem Get(TKey key) {
            ObjectList.TryGetValue(key, out TItem item);
            return item;
        }

        public void AddOrUpdate(TKey key, TItem item) {
            ObjectList.AddOrUpdate(key, item, (k, v) => item);
        }

    }
}
