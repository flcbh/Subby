using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Subby.Core.Entities;

namespace Subby.Core.Mappings
{
    public class UserMappingOverride : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> mapping)
        {
            mapping.Map(x => x.Bio).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            mapping.HasMany(x => x.UserReviews).KeyColumn("[User_id]");
            mapping.IgnoreProperty(x => x.IsPremium);
        }
    }
}