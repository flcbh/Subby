using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Subby.Core.Entities;

namespace Subby.Core.Mappings
{
    public class JobMappingOverride : IAutoMappingOverride<Job>
    {
        public void Override(AutoMapping<Job> mapping)
        {
            mapping.Map(x => x.Description).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            mapping.IgnoreProperty(x => x.ViewCount);
            mapping.IgnoreProperty(x => x.IsApplied);
            mapping.IgnoreProperty(x => x.Distance);
            // mapping.HasMany(x => x.Activities);
            // mapping.HasMany(x => x.Applications);
        }
    }
}