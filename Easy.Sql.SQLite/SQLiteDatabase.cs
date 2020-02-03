﻿using System.ComponentModel.Composition;
using System.Data.SQLite;

namespace Easy.Sql.SQLite {
    [Export(typeof(IDatabase))]
    public class SQLiteDatabase : IDatabase {
        private SQLiteConnection mConn;
        public void Open(string fileName) {
            mConn = new SQLiteConnection($"Data Source={fileName};Version=3;Pooling=False");
            mConn.Open();
        }
    }
}