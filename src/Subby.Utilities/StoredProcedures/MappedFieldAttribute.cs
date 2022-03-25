using System;

namespace Subby.Utilities.StoredProcedures
{
    public class MappedFieldAttribute : Attribute
    {
        public MappedFieldAttribute()
        {
            
        }

        public MappedFieldAttribute(int index)
        {
            Index = index;
        }

        public int Index { get; set; }
    }
}