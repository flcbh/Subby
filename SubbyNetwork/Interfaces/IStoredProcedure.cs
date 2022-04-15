using System.Collections.Generic;

namespace SubbyNetwork.Interfaces
{
    public interface IStoredProcedure<T> where T : new()
    {
        string ConnectionString { get; set; }
        void AddParameter(string key, object value);
        void AddOrUpdateParameter(string key, object value);
        void SetParameters(IDictionary<string, object> parameters);
        T GetDesignedSingleResult();
        IList<T> GetDesignedListResult();
        int ExecuteNonQuery();
        void ClearParameters();

        /// <summary>
        /// Gets or sets the stored procedure to use for this stored procedure
        /// This will override the procedure set using the MappedFieldAttribute
        /// </summary>
        string ProcedureName { get; set; }
    }
}