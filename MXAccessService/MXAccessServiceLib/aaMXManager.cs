using System;
using System.Collections.Generic;
using System.Linq;
using ArchestrA.MxAccess;

namespace Intma.Libraries
{
    public class aaMXManager : MarshalByRefObject, IMXAccessService, IDisposable
    {
        public bool IsSupervisoryConnection { get; set; }

        public int hLMX;
        LMXProxyServerClass LMX_Server;
        Dictionary<int, aaAttribute> hItems;
        readonly string ServerName;

        public aaMXManager(string serverName)
        {
            hLMX = 0;
            hItems = new Dictionary<int, aaAttribute>();
            ServerName = serverName;
        }

        public void Advise(string tagName)
        {
            var item = hItems.FirstOrDefault(a => a.Value.TagName == tagName);

            if (!item.Value.OnAdvise)
            {
                if (IsSupervisoryConnection)
                {
                    LMX_Server.AdviseSupervisory(hLMX, item.Key);
                }
                else
                {
                    LMX_Server.Advise(hLMX, item.Key);
                }
                item.Value.OnAdvise = true;
            }
        }

        public void AdviseAll()
        {
            if (hItems.Count == 0)
            {
                return;
            }
            foreach (var item in hItems)
            {
                if (!item.Value.OnAdvise)
                {
                    if (IsSupervisoryConnection)
                    {
                        LMX_Server.AdviseSupervisory(hLMX, item.Key);
                    }
                    else
                    {
                        LMX_Server.Advise(hLMX, item.Key);
                    }
                    item.Value.OnAdvise = true;
                }
            }
        }

        public void AddItem(aaAttribute item)
        {
            if ((LMX_Server != null) && (hLMX != 0))
            {
                int key = LMX_Server.AddItem(hLMX, item.TagName);
                hItems.Add(key, item);
            }
        }


        public void AddGroupItem(IEnumerable<aaAttribute> items)
        {
            if ((LMX_Server != null) && (hLMX != 0))
            {
                foreach (var item in items)
                {
                    int key = LMX_Server.AddItem(hLMX, item.TagName);
                    hItems.Add(key, item);
                }
            }
        }

        public void UnAdviseAll()
        {
            foreach (var item in hItems)
                Unadvise(item.Value.TagName);
        }

        public void Unadvise(string value)
        {
            var item = hItems.FirstOrDefault(a => a.Value.TagName == value);
            if (item.Value != null && item.Value.OnAdvise)
            {
                LMX_Server.UnAdvise(hLMX, item.Key);
                item.Value.OnAdvise = false;
            }
        }
        public void Unadvise(int index)
        {
            if (hItems[index].OnAdvise)
            {
                LMX_Server.UnAdvise(hLMX, index);
                hItems[index].OnAdvise = false;
            }
        }

        public void RemoveAll()
        {
            UnAdviseAll();

            if (hItems.Count != 0)
            {
                foreach (var item in hItems)
                    RemoveItem(item.Key);
            }
        }

        public void RemoveItem(string tagName)
        {
            var item = hItems.FirstOrDefault(a => a.Value.TagName == tagName);
            if (item.Value == null)
                return;
            if (item.Value.OnAdvise)
            {
                Unadvise(tagName);
            }
            
            if (hItems.Count != 0)
            {
                LMX_Server.RemoveItem(hLMX, item.Key);
                hItems.Remove(item.Key);
            }
        }


        public void RemoveItem(int index)
        {
            if (hItems[index].OnAdvise)
            {
                Unadvise(hItems[index].TagName);
            }

            LMX_Server.RemoveItem(hLMX, index);
            hItems.Remove(index);
        }


        public void Register()
        {
            if (LMX_Server == null)
            {
                LMX_Server = new ArchestrA.MxAccess.LMXProxyServerClass();
            }
            if ((LMX_Server != null) && (hLMX == 0))
            {
                hLMX = LMX_Server.Register(ServerName);
                LMX_Server.OnDataChange += new _ILMXProxyServerEvents_OnDataChangeEventHandler(LMX_OnDataChange);

                hItems = new Dictionary<int, aaAttribute>();
            }
        }

        private void LMX_OnDataChange(int hLMXServerHandle, int phItemHandle, object pvItemValue, int pwItemQuality, object pftItemTimeStamp, ref ArchestrA.MxAccess.MXSTATUS_PROXY[] ItemStatus)
        {
            if (hItems[phItemHandle] != null)
            {
                if (ItemStatus[0].success != 0)
                {
                    hItems[phItemHandle].Quality = (int)pwItemQuality;
                    hItems[phItemHandle].TimeStamp = (DateTime)pftItemTimeStamp;
                    hItems[phItemHandle].Value = pvItemValue;
                }
            }
        }


        public void Poke(string tagName, object value, DateTime? timeStamp = null)
        {
            var item = hItems.FirstOrDefault(a => a.Value.TagName == tagName);
            if (item.Value == null)
            {
                throw new Exception("Item was not found");
            }
            if (item.Value.OnAdvise)
            {
                if (timeStamp != null)
                {
                    LMX_Server.Write2(hLMX, item.Key, value, timeStamp, 1);
                }
                else
                {
                    LMX_Server.Write(hLMX, item.Key, value, 1);
                }
            }
        }

        public void Unregister()
        {
            if ((LMX_Server != null) && (hLMX != 0))
            {
                UnAdviseAll();
                RemoveAll();

                LMX_Server.Unregister(hLMX);
                LMX_Server = null;
                hLMX = 0;
            }
        }

        public void Dispose()
        {
            Unregister();
        }
    }
}
