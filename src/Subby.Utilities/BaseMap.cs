using FluentNHibernate.Mapping;

namespace Subby.Utilities
{
    public class BaseMap<T> : ClassMap<T> where T : BaseEntity
    {
        public BaseMap()
        {
            Id(x => x.Id);
        }
    }
}