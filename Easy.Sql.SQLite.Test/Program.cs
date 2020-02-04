namespace Easy.Sql.SQLite.Test {
    class Program {
        static void Main(string[] args) {
            var importer = new Importer();
            importer.Initialize();

            var db = IoC.Get<IEasyDatabase>();
            db.Open("database.db");
        }
    }
}
