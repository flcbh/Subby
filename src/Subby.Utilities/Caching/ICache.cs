using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Subby.Utilities.Caching
{
    public interface ICache
    {
        //void Add<T>(T item, CacheKey key, DateTimeOffset expireOffset);
        //void Add(string item, CacheKey key, DateTimeOffset expireOffset);
        //void Add(bool item, CacheKey key, DateTimeOffset expireOffset);
        //void Add(int item, CacheKey key, DateTimeOffset expireOffset);
        //bool AddNotExists(string item, CacheKey key, DateTimeOffset expireOffset);

        //void Remove(CacheKey key);

        Task<bool> Exists<T>(string key);

        //string GetString(string key);
        //int GetInt(CacheKey key, bool localStorage = false);
        //bool GetBool(CacheKey key, bool localStorage = false);
        T Get<T>(string key);
        Task<T> GetAsync<T>(string key);
        //IList<T> GetMany<T>(IList<CacheKey> keys);

        //T AddOrGetExisting<T>(string key, Func<T> ifMissing, DateTimeOffset expiration);
        T GetOrAdd<T>(string key, Func<T> ifMissing, DateTimeOffset expiration);
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> ifMissing, DateTimeOffset expiration);
        Task<T> UpdateAsync<T>(string key, T value, DateTimeOffset expiration);

        //void UpdateTtl(CacheKey key, DateTimeOffset expireOffset);

        //void ClearCache(string section);
        //void ClearCache(string section, int id);
        //void ClearCache(string section, string id);
    }
}
