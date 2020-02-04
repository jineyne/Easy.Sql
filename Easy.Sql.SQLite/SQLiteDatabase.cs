using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SQLite;

namespace Easy.Sql.SQLite {
    [Export(typeof(IEasyDatabase))]
    public class SQLiteDatabase : IEasyDatabase {
        private SQLiteConnection mConn;

        public void Open(string fileName) {
            mConn = new SQLiteConnection($"Data Source={fileName};Version=3;Pooling=False");
            mConn.Open();
        }

        public DataTable Execute(string query) {
            var result = new DataTable();

            using (var cmd = new SQLiteCommand(query, mConn)) {
                using (var rdr = cmd.ExecuteReader()) {
                    result.Load(rdr);
                    return result;
                }
            }
        }

        public DataTable ExecuteSafe(string query) {
            var result = new DataTable();

            try {
                using (var cmd = new SQLiteCommand(query, mConn)) {
                    using (var rdr = cmd.ExecuteReader()) {
                        result.Load(rdr);
                        return result;
                    }
                }
            } catch (Exception) { }

            return result;
        }

        public object ExecuteScalar(string query) {
            using (var cmd = new SQLiteCommand(query, mConn)) {
                return cmd.ExecuteScalar();
            }
        }
    }
}