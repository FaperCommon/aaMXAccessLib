using System;
using System.Collections.Generic;

namespace Intma.Libraries
{
    public interface IMXAccessService
    {
        void Register();
        void Unregister();
        void AddItem(aaAttribute item);
        void AddGroupItem(IEnumerable<aaAttribute> items);
        void Advise(String tagName);
        void AdviseAll();
        void Poke(string tagName, object value, DateTime? timeStamp = null);
        IEnumerable<aaAttribute> GetCollection();
    }
}
