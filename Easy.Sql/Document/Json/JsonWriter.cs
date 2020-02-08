using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Easy.Sql.Document.Json {
    public class JsonWriter {
        private static readonly IFormatProvider NumberFormat = CultureInfo.InvariantCulture.NumberFormat;
        private int mIndent;
        private string mSpacer = "";

        private TextWriter mWriter;

        public JsonWriter(TextWriter writer) {
            mWriter = writer;
        }

        /// <summary>
        ///     Get/Set indent size
        /// </summary>
        public int Indent { get; set; } = 4;

        /// <summary>
        ///     Get/Set if writer must print pretty (with new line/indent)
        /// </summary>
        public bool Pretty { get; set; } = false;

        /// <summary>
        ///     Serialize value into text writer
        /// </summary>
        public void Serialize(EasyValue value) {
            mIndent = 0;
            mSpacer = Pretty ? " " : "";

            WriteValue(value ?? EasyValue.Null);
        }

        private void WriteValue(EasyValue value) {
            // use direct cast to better performance
            switch (value.Type) {
                case EasyDataType.Null:
                    mWriter.Write("null");
                    break;

                case EasyDataType.Document:
                    this.WriteObject(value.AsDocument);
                    break;

                case EasyDataType.Boolean:
                    mWriter.Write(value.AsBoolean.ToString().ToLower());
                    break;

                case EasyDataType.String:
                    WriteString(value.AsString);
                    break;

                case EasyDataType.Int32:
                    mWriter.Write(value.AsInt32.ToString(NumberFormat));
                    break;

                case EasyDataType.Double:
                    mWriter.Write(value.AsDouble.ToString("0.0########", NumberFormat));
                    break;

                case EasyDataType.Binary:
                    var bytes = value.AsBinary;
                    WriteExtendDataType("$binary", Convert.ToBase64String(bytes, 0, bytes.Length));
                    break;

                case EasyDataType.ObjectId:
                    WriteExtendDataType("$oid", value.AsObjectId.ToString());
                    break;

                case EasyDataType.Guid:
                    WriteExtendDataType("$guid", value.AsGuid.ToString());
                    break;

                case EasyDataType.DateTime:
                    WriteExtendDataType("$date", value.AsDateTime.ToUniversalTime().ToString("o"));
                    break;

                case EasyDataType.Int64:
                    WriteExtendDataType("$numberLong", value.AsInt64.ToString(NumberFormat));
                    break;

                case EasyDataType.Decimal:
                    WriteExtendDataType("$numberDecimal", value.AsDecimal.ToString(NumberFormat));
                    break;

                case EasyDataType.MinValue:
                    WriteExtendDataType("$minValue", "1");
                    break;

                case EasyDataType.MaxValue:
                    WriteExtendDataType("$maxValue", "1");
                    break;
            }
        }

        private void WriteObject(EasyDocument obj) {
            var length = obj.Keys.Count();
            var hasData = length > 0;

            WriteStartBlock("{", hasData);

            var index = 0;

            foreach (var el in obj.GetElements()) {
                WriteKeyValue(el.Key, el.Value, index++ < length - 1);
            }

            WriteEndBlock("}", hasData);
        }

        private void WriteString(string s) {
            mWriter.Write('\"');
            var l = s.Length;
            for (var index = 0; index < l; index++) {
                var c = s[index];
                switch (c) {
                    case '\"':
                        mWriter.Write("\\\"");
                        break;

                    case '\\':
                        mWriter.Write("\\\\");
                        break;

                    case '\b':
                        mWriter.Write("\\b");
                        break;

                    case '\f':
                        mWriter.Write("\\f");
                        break;

                    case '\n':
                        mWriter.Write("\\n");
                        break;

                    case '\r':
                        mWriter.Write("\\r");
                        break;

                    case '\t':
                        mWriter.Write("\\t");
                        break;

                    default:
                        var i = c;
                        if (i < 32 || i > 127) {
                            mWriter.Write("\\u");
                            mWriter.Write(i.ToString()); //x04
                        } else {
                            mWriter.Write(c);
                        }

                        break;
                }
            }

            mWriter.Write('\"');
        }

        private void WriteExtendDataType(string type, string value) {
            // format: { "$type": "string-value" }
            // no string.Format to better performance
            mWriter.Write("{\"");
            mWriter.Write(type);
            mWriter.Write("\":");
            mWriter.Write(mSpacer);
            mWriter.Write("\"");
            mWriter.Write(value);
            mWriter.Write("\"}");
        }

        private void WriteKeyValue(string key, EasyValue value, bool comma) {
            WriteIndent();

            mWriter.Write('\"');
            mWriter.Write(key);
            mWriter.Write("\":");

            // do not do this tests if is not pretty format - to better performance
            if (Pretty) {
                mWriter.Write(' ');

                if (value.IsDocument && value.AsDocument.Keys.Any()) {
                    WriteNewLine();
                }
            }

            WriteValue(value ?? EasyValue.Null);

            if (comma) {
                mWriter.Write(',');
            }

            WriteNewLine();
        }

        private void WriteStartBlock(string str, bool hasData) {
            if (hasData) {
                WriteIndent();
                mWriter.Write(str);
                WriteNewLine();
                mIndent++;
            } else {
                mWriter.Write(str);
            }
        }

        private void WriteEndBlock(string str, bool hasData) {
            if (hasData) {
                mIndent--;
                WriteIndent();
                mWriter.Write(str);
            } else {
                mWriter.Write(str);
            }
        }

        private void WriteNewLine() {
            if (Pretty) {
                mWriter.WriteLine();
            }
        }

        private void WriteIndent() {
            if (Pretty) {
                mWriter.Write("".PadRight(mIndent * Indent, ' '));
            }
        }
    }
}