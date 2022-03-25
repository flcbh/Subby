using System;

namespace Subby.Utilities.StoredProcedures
{
    public class StoredProcedureAttribute : Attribute
    {
        public string ProcedureName { get; set; }
        public int CommandTimeout { get; set; }
    }
}