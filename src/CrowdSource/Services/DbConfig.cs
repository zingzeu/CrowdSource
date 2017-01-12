using CrowdSource.Data;
using CrowdSource.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Services
{
    public class DbConfig : IDbConfig
    {
        private readonly ApplicationDbContext _context;

        public DbConfig(ApplicationDbContext context)
        {
            _context = context;
        }
        public string Get(string key)
        {
            var configurationRow = _context.Configurations.SingleOrDefault(r => r.Key == key);
            return configurationRow?.Value;
        }

        public void Set(string key, string value)
        {
            var configurationRow = _context.Configurations.SingleOrDefault(r => r.Key == key);
            if (configurationRow == null)
            {
                configurationRow = new ConfigurationRow()
                {
                    Key = key,
                    Value = value
                };
                _context.Configurations.Add(configurationRow);
            } else
            {
                configurationRow.Value = value;
            }
            _context.SaveChanges();
        }
    }

    public interface IDbConfig
    {
        string Get(string key);
        void Set(string key, string value);
    }
}
