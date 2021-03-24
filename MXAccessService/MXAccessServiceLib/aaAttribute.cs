using System;

namespace Intma.Libraries
{
    public  class aaAttribute 
    {

        virtual public string TagName { get; set; }
        virtual public DateTime TimeStamp { get; set; }
        virtual public object Value { get; set; }
        virtual public int Quality { get; set; }
        virtual public bool OnAdvise { get; set; }
    }
}
