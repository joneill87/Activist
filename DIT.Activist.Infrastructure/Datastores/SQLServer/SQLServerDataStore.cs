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
    public class SQLServerDataStore : BaseDataStore
    {
        private readonly string server;
        private readonly string dbName;
        private const string tableName = "ADS";

        private static bool shouldRecreate = false;
        
        private SqlConnection GetSqlConnection()
        {
            string server = @"localhost\SQLEXPRESS";
            string db = "Activist";
            string user = "sa";
            string password = "u4Tengos*";
            return new SqlConnection(String.Format("Server={0}; Database={1}; User id={2};password={3}", server, db, user, password));
        }

        private object ExecuteScalar(string sql)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                object scalar = cmd.ExecuteScalar();
                conn.Close();
                return scalar;
            }
        }

        private int ExecuteNonQuery(string sql)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();
                return rowsAffected;
            }
        }

        private object[] GetExpandedRow(object[] unexpanded)
        {

            IEnumerable<object> featureVals = unexpanded[1].ToString().Split(',').Select(v => System.Convert.ChangeType(v, dataFormat.FeatureType));
            return unexpanded.Take(1).Concat(featureVals).Concat(unexpanded.Skip(2)).ToArray();
        }

        private bool TableExists()
        {
            string sql = String.Format("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}'", tableName);
            int tableCount = (int)ExecuteScalar(sql);
            return tableCount > 0;
        }

        private void DropTable()
        {
            string sql = String.Format("DROP TABLE {0}", tableName);
            ExecuteScalar(sql);
        }

        private void CreateInitialTable()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append(String.Format("CREATE TABLE {0} (ID BIGINT Not NULL PRIMARY KEY,", tableName));
            sb.Append("Features VARCHAR(MAX), ");
            sb.Append("Artifact VARCHAR(MAX), ");
            sb.Append("Label VARCHAR(50)");
            sb.Append(");");
            ExecuteNonQuery(sb.ToString());
        }

        public SQLServerDataStore(IDataFormat format) : base(format)
        {
            if (shouldRecreate)
            {
                if (TableExists())
                {
                    DropTable();
                }
                CreateInitialTable();
                shouldRecreate = false;
            }
        }

        public override Task AddLabelledRow(object[] labelled)
        {
            long id = dataFormat.GetID(labelled);
            string featureVals = String.Join(",", dataFormat.GetFeatures<object>(labelled));
            string artifactVal = dataFormat.GetArtifact(labelled);
            string labelVal = dataFormat.GetLabel<object>(labelled).ToString();
            string sql = String.Format("INSERT INTO {0} (ID, Features, Artifact, Label) VALUES ({1}, '{2}', '{3}', '{4}');", tableName, id, featureVals, artifactVal, labelVal);
            ExecuteNonQuery(sql);
            return Task.FromResult<object>(null);
        }

        public override Task AddLabels(IDictionary<long, string> idLabelLookups)
        {
            foreach (var kvp in idLabelLookups)
            {
                string sql = String.Format("UPDATE {0} SET Label={1} WHERE ID={2};", tableName, kvp.Value, System.Convert.ToInt64(kvp.Key));
                ExecuteNonQuery(sql);
            }
            return Task.FromResult<object>(null);
        }

        public override Task AddUnlabelledRow(object[] unlabelled)
        {
            long id = dataFormat.GetID(unlabelled);
            string featureVals = String.Join(",", dataFormat.GetFeatures<object>(unlabelled));
            string artifactVal = dataFormat.GetArtifact(unlabelled);
            string sql = String.Format("INSERT INTO {0} (ID, Features, Artifact) VALUES ({1}, {2}, {3});", tableName, id, featureVals, artifactVal);
            return Task.FromResult<object>(null);
        }

        public override void Clear()
        {
            DropTable();
            CreateInitialTable();
        }

        protected override Task<object[]> GetItemById(long id)
        {
            string sql = String.Format("SELECT ID, Features, Artifact, Label FROM {0} WHERE ID={1}", tableName, id);
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
