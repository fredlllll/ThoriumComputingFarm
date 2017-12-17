﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Thorium_Server;

namespace Thorium_Shared.Data.Serializers
{
    public class ClientSerializer : BaseSerializer<string, Client>
    {
        public override IDatabase Database { get; }
        public override string Table => Database.GetTableName("clients");
        public override string KeyColumn => "id";

        public ClientSerializer(IDatabase database)
        {
            Database = database;
        }

        public override Client Load(string key)
        {
            var reader = SelectStarWhereKey(key);
            reader.Read();

            string ip = (string)reader["ip"];

            return new Client(IPAddress.Parse(ip), key);
        }

        public override void Save(string key, Client value)
        {
            string sql = "INSERT INTO " + Table + "(" + KeyColumn + ",ip) VALUES(@0,@1) ON DUPLICATE KEY UPDATE ip=@2";
            Database.ExecuteNonQueryTransaction(sql, key, value.IPAddress.ToString(), value.IPAddress.ToString());
        }

        public override void CreateTable()
        {
            string sql = @"CREATE TABLE IF NOT EXISTS `" + Table + @"` (
  `" + KeyColumn + @"` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ip` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`" + KeyColumn + @"`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";
            Database.ExecuteNonQueryTransaction(sql);
        }

        public override void CreateConstraints()
        {
        }
    }
}
