using System.IO;
using System.Text;

namespace Easy.Sql.Document.Json {
    public class JsonSerializer {
        public static string Serialize(EasyValue value) {
            var sb = new StringBuilder();

            Serialize(value, sb);

            return sb.ToString();
        }

        public static void Serialize(EasyValue value, TextWriter writer) {
            var json = new JsonWriter(writer);

            json.Serialize(value ?? EasyValue.Null);
        }


        public static void Serialize(EasyValue value, StringBuilder sb) {
            using (var writer = new StringWriter(sb)) {
                var w = new JsonWriter(writer);

                w.Serialize(value ?? EasyValue.Null);
            }
        }
    }
}