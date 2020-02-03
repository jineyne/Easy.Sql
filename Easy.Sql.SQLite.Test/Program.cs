namespace Easy.Sql.SQLite.Test {
    class Program {
        static void Main(string[] args) {
            var importer = new Importer();
            importer.Initialize();

            var db = IoC.Get<IDatabase>();
            db.Open("database.db");
        }
    }
}
