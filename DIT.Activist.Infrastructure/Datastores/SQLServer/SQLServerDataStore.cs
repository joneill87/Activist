using DIT.Activist.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Infrastructure.Datastores.SQLServer
{
    internal class SQLServerDataStore : BaseDataStore
    {
        private string tableName;
        
        private SqlConnection GetSqlConnection()
        {
            string server = @"localhost\SQLEXPRESS";
            string db = "Activist";
            string user = "sa";
            string password = "u4Tengos*";
            return new SqlConnection(String.Format("Server={0}; Database={1}; User id={2};password={3}", server, db, user, password));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private object ExecuteScalar(string sql)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                object scalar = cmd.ExecuteScalar();
                return scalar;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private int ExecuteNonQuery(string sql)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected;
            }
        }

        private object[] GetExpandedRow(object[] unexpanded)
        {

            IEnumerable<object> featureVals = unexpanded[1].ToString().Split(',').Select(v => System.Convert.ChangeType(v, dataFormat.FeatureType));
            return unexpanded.Take(1).Concat(featureVals).Concat(unexpanded.Skip(2)).ToArray();
        }

        private bool TableExists(string name)
        {
            string sql = String.Format("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}'", name);
            int tableCount = (int)ExecuteScalar(sql);
            return tableCount > 0;
        }

        private void DropTable(string tableName)
        {
            string sql = String.Format("DROP TABLE {0}", tableName);
            ExecuteScalar(sql);
        }

        private void CreateTable(string tableName)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append(String.Format("CREATE TABLE {0} (ID BIGINT Not NULL PRIMARY KEY,", tableName));
            sb.Append("Features VARCHAR(MAX), ");
            sb.Append("Artifact VARCHAR(MAX), ");
            sb.Append("Label VARCHAR(50)");
            sb.Append(");");
            ExecuteNonQuery(sb.ToString());
        }

        public SQLServerDataStore()
        {
           
        }

        protected override void CreateDatastore(string name)
        {
            if (TableExists(name))
            {
                throw new DatastoreExistsException(name);
            }
            CreateTable(name);
        }

        protected override void CreateOrReplaceDatastore(string name)
        {
            if (TableExists(name))
            {
                DropTable(name);
            }

            CreateDatastore(name);
        }

        public override void Connect(string name)
        {
            if (!TableExists(name))
            {
                throw new NonExistantDatastoreException(name);
            }

            tableName = name;
        }

        public override bool Exists(string name)
        {
            return TableExists(name);
        }

        public override Task AddLabelledRow(object[] labelled)
        {
            long id = dataFormat.GetID(labelled);
            string featureVals = String.Join(",", dataFormat.GetFeatures<object>(labelled));
            string artifactVal = dataFormat.GetArtifact(labelled);
            string labelVal = dataFormat.GetLabel<object>(labelled).ToString();
            string sql = String.Format("INSERT INTO {0} (ID, Features, Artifact, Label) VALUES ('{1}', '{2}', '{3}', '{4}');", tableName, id, featureVals, artifactVal, labelVal);
            ExecuteNonQuery(sql);
            return Task.FromResult<object>(null);
        }

        public override Task AddLabels(IDictionary<long, string> idLabelLookups)
        {
            foreach (var kvp in idLabelLookups)
            {
                string sql = String.Format("UPDATE {0} SET Label='{1}' WHERE ID='{2}';", tableName, kvp.Value, System.Convert.ToInt64(kvp.Key));
                ExecuteNonQuery(sql);
            }
            return Task.FromResult<object>(null);
        }

        public override Task AddUnlabelledRow(object[] unlabelled)
        {
            long id = dataFormat.GetID(unlabelled);
            string featureVals = String.Join(",", dataFormat.GetFeatures<object>(unlabelled));
            string artifactVal = dataFormat.GetArtifact(unlabelled);
            string sql = String.Format("INSERT INTO {0} (ID, Features, Artifact) VALUES ('{1}', '{2}', '{3}');", tableName, id, featureVals, artifactVal);
            return Task.FromResult<object>(null);
        }

        public override void Clear()
        {
            DropTable(tableName);
            CreateTable(tableName);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected override Task<object[]> GetItemById(long id)
        {
            string sql = String.Format("SELECT ID, Features, Artifact, Label FROM {0} WHERE ID='{1}'", tableName, id);
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                object[] row = dt.Rows[0].ItemArray;
                var expanded = GetExpandedRow(row);
                return Task.FromResult(expanded);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected override Task<IEnumerable<object[]>> GetRawLabelledData()
        {
            string sql = String.Format("SELECT ID, Features, Artifact, Label FROM {0} WHERE LABEL IS NOT NULL", tableName);
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return Task.FromResult(dt.AsEnumerable().Select(r => GetExpandedRow(r.ItemArray)).AsEnumerable());
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected override Task<IEnumerable<object[]>> GetRawUnlabelledData()
        {
            string sql = String.Format("SELECT ID, Features, Artifact, Label FROM {0} WHERE LABEL IS NULL", tableName);
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return Task.FromResult(dt.AsEnumerable().Select(r => GetExpandedRow(r.ItemArray)).AsEnumerable());
            }
        }
    }
}
