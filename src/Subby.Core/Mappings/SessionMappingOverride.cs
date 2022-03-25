using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Subby.Core.Entities;

namespace Subby.Core.Mappings
{
    public class SessionMappingOverride : IAutoMappingOverride<Session>
    {
        public void Override(AutoMapping<Session> mapping)
        {
            
        }
    }
}