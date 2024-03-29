﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Subby.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Subby.Data
{
    public partial class SubbynetworkContext
    {
        private SubbynetworkContextProcedures _procedures;

        public virtual SubbynetworkContextProcedures Procedures
        {
            get
            {
                if (_procedures is null) _procedures = new SubbynetworkContextProcedures(this);
                return _procedures;
            }
            set
            {
                _procedures = value;
            }
        }

        public SubbynetworkContextProcedures GetProcedures()
        {
            return Procedures;
        }

        protected void OnModelCreatingGeneratedProcedures(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClearOldJobsSubbynetworkResult>().HasNoKey().ToView(null);
            modelBuilder.Entity<ClearOldJobsDboResult>().HasNoKey().ToView(null);
            modelBuilder.Entity<getAddsByPostCodeResult>().HasNoKey().ToView(null);
        }
    }

    public interface ISubbynetworkContextProceduresContract
    {
        Task<int> ClearOldJobsSubbynetworkAsync(CancellationToken cancellationToken = default);
        Task<int> ClearOldJobsDboAsync(CancellationToken cancellationToken = default);
        Task<List<getAddsByPostCodeResult>> getAddsByPostCodeAsync(int? mile, string postcode, CancellationToken cancellationToken = default);
    }

    public partial class SubbynetworkContextProcedures
    {
        private readonly SubbynetworkContext _context;

        public SubbynetworkContextProcedures(SubbynetworkContext context)
        {
            _context = context;
        }

        public virtual async Task<int> ClearOldJobsSubbynetworkAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = System.Data.ParameterDirection.Output,
                SqlDbType = System.Data.SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                parameterreturnValue,
            };
            var _ = await _context.Database.ExecuteSqlRawAsync("EXEC @returnValue = [subbynetwork].[ClearOldJobs]", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }

        public virtual async Task<int> ClearOldJobsDboAsync(OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = System.Data.ParameterDirection.Output,
                SqlDbType = System.Data.SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                parameterreturnValue,
            };
            var _ = await _context.Database.ExecuteSqlRawAsync("EXEC @returnValue = [dbo].[ClearOldJobs]", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }

        public virtual async Task<List<getAddsByPostCodeResult>> getAddsByPostCodeAsync(int? mile, string postcode, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = System.Data.ParameterDirection.Output,
                SqlDbType = System.Data.SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                new SqlParameter
                {
                    ParameterName = "mile",
                    Value = mile ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.Int,
                },
                new SqlParameter
                {
                    ParameterName = "postcode",
                    Size = 1,
                    Value = postcode ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                parameterreturnValue,
            };
            var _ = await _context.SqlQueryAsync<getAddsByPostCodeResult>("EXEC @returnValue = [subbynetwork].[getAddsByPostCode] @mile, @postcode", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }
    }
}
