using System;
using System.Diagnostics;
using Easy.Sql.Document.Json;
using Easy.Sql.Utility.Extensions;
using static System.String;

namespace Easy.Sql.Document {
    // base on https://github.com/mbdavid/LiteDB/blob/master/LiteDB/Document/BsonValue.cs
    public class EasyValue : IComparable<EasyValue>, IEquatable<EasyValue> {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        ///     Represent a Null bson type
        /// </summary>
        public static EasyValue Null = new EasyValue(EasyDataType.Null, null);

        /// <summary>
        ///     Represent a MinValue bson type
        /// </summary>
        public static EasyValue MinValue = new EasyValue(EasyDataType.MinValue, "-oo");

        /// <summary>
        ///     Represent a MaxValue bson type
        /// </summary>
        public static EasyValue MaxValue = new EasyValue(EasyDataType.MaxValue, "+oo");

        public EasyValue() {
            Type = EasyDataType.Null;
            RawValue = null;
        }

        public EasyValue(int value) {
            Type = EasyDataType.Int32;
            RawValue = value;
        }

        public EasyValue(long value) {
            Type = EasyDataType.Int64;
            RawValue = value;
        }

        public EasyValue(double value) {
            Type = EasyDataType.Double;
            RawValue = value;
        }

        public EasyValue(decimal value) {
            Type = EasyDataType.Decimal;
            RawValue = value;
        }

        public EasyValue(string value) {
            Type = value == null ? EasyDataType.Null : EasyDataType.String;
            RawValue = value;
        }

        public EasyValue(byte[] value) {
            Type = value == null ? EasyDataType.Null : EasyDataType.Binary;
            RawValue = value;
        }

        public EasyValue(ObjectId value) {
            Type = value == null ? EasyDataType.Null : EasyDataType.ObjectId;
            RawValue = value;
        }

        public EasyValue(Guid value) {
            Type = EasyDataType.Guid;
            RawValue = value;
        }

        public EasyValue(bool value) {
            Type = EasyDataType.Boolean;
            RawValue = value;
        }

        public EasyValue(DateTime value) {
            Type = EasyDataType.DateTime;
            RawValue = value.Truncate();
        }

        protected EasyValue(EasyDataType type, object rawValue) {
            Type = type;
            RawValue = rawValue;
        }

        public EasyDataType Type { get; }

        /// <summary>
        ///     Get internal .NET value object
        /// </summary>
        internal virtual object RawValue { get; }

        /// <summary>
        ///     Get/Set a field for document. Fields are case sensitive - Works only when value are document
        /// </summary>
        public virtual EasyValue this[string name] {
            get => throw new InvalidOperationException("Cannot access non-document type value on " + RawValue);
            set => throw new InvalidOperationException("Cannot access non-document type value on " + RawValue);
        }

        /// <summary>
        ///     Get/Set value in array position. Works only when value are array
        /// </summary>
        public virtual EasyValue this[int index] {
            get => throw new InvalidOperationException("Cannot access non-array type value on " + RawValue);
            set => throw new InvalidOperationException("Cannot access non-array type value on " + RawValue);
        }

        // as types
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public EasyDocument AsDocument => this as EasyDocument;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public byte[] AsBinary => RawValue as byte[];

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool AsBoolean => (bool) RawValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string AsString => (string) RawValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int AsInt32 => Convert.ToInt32(RawValue);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public long AsInt64 => Convert.ToInt64(RawValue);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public double AsDouble => Convert.ToDouble(RawValue);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public decimal AsDecimal => Convert.ToDecimal(RawValue);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public DateTime AsDateTime => (DateTime) RawValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ObjectId AsObjectId => (ObjectId) RawValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Guid AsGuid => (Guid) RawValue;

        // is types

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsNull => Type == EasyDataType.Null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsArray => Type == EasyDataType.Array;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsDocument => Type == EasyDataType.Document;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsInt32 => Type == EasyDataType.Int32;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsInt64 => Type == EasyDataType.Int64;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsDouble => Type == EasyDataType.Double;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsDecimal => Type == EasyDataType.Decimal;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsNumber => IsInt32 || IsInt64 || IsDouble || IsDecimal;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsBinary => Type == EasyDataType.Binary;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsBoolean => Type == EasyDataType.Boolean;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsString => Type == EasyDataType.String;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsObjectId => Type == EasyDataType.ObjectId;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsGuid => Type == EasyDataType.Guid;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsDateTime => Type == EasyDataType.DateTime;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsMinValue => Type == EasyDataType.MinValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsMaxValue => Type == EasyDataType.MaxValue;

        public virtual int CompareTo(EasyValue other) {
            // first, test if types are different
            if (Type != other.Type) {
                // if both values are number, convert them to Decimal (128 bits) to compare
                // it's the slowest way, but more secure
                if (IsNumber && other.IsNumber) {
                    return Convert.ToDecimal(RawValue).CompareTo(Convert.ToDecimal(other.RawValue));
                }
                // if not, order by sort type order

                return Type.CompareTo(other.Type);
            }

            // for both values with same data type just compare
            switch (Type) {
                case EasyDataType.Null:
                case EasyDataType.MinValue:
                case EasyDataType.MaxValue:
                    return 0;

                case EasyDataType.Int32:
                    return AsInt32.CompareTo(other.AsInt32);

                case EasyDataType.Int64:
                    return AsInt64.CompareTo(other.AsInt64);

                case EasyDataType.Double:
                    return AsDouble.CompareTo(other.AsDouble);

                case EasyDataType.Decimal:
                    return AsDecimal.CompareTo(other.AsDecimal);

                case EasyDataType.String:
                    return Compare(AsString, other.AsString, StringComparison.Ordinal);

                case EasyDataType.Document:
                    return AsDocument.CompareTo(other);

                case EasyDataType.ObjectId:
                    return AsObjectId.CompareTo(other.AsObjectId);

                case EasyDataType.Guid:
                    return AsGuid.CompareTo(other.AsGuid);

                case EasyDataType.Boolean:
                    return AsBoolean.CompareTo(other.AsBoolean);

                case EasyDataType.DateTime:
                    var d0 = AsDateTime;
                    var d1 = other.AsDateTime;
                    if (d0.Kind != DateTimeKind.Utc) {
                        d0 = d0.ToUniversalTime();
                    }

                    if (d1.Kind != DateTimeKind.Utc) {
                        d1 = d1.ToUniversalTime();
                    }

                    return d0.CompareTo(d1);

                default:
                    throw new NotImplementedException();
            }
        }

        public bool Equals(EasyValue other) {
            return CompareTo(other) == 0;
        }

        // +
        public static EasyValue operator +(EasyValue left, EasyValue right) {
            if (!left.IsNumber || !right.IsNumber) {
                return Null;
            }

            if (left.IsInt32 && right.IsInt32) {
                return left.AsInt32 + right.AsInt32;
            }

            if (left.IsInt64 && right.IsInt64) {
                return left.AsInt64 + right.AsInt64;
            }

            if (left.IsDouble && right.IsDouble) {
                return left.AsDouble + right.AsDouble;
            }

            if (left.IsDecimal && right.IsDecimal) {
                return left.AsDecimal + right.AsDecimal;
            }

            var result = left.AsDecimal + right.AsDecimal;
            var type = (EasyDataType) Math.Max((int) left.Type, (int) right.Type);

            return
                type == EasyDataType.Int64 ? new EasyValue((long) result) :
                type == EasyDataType.Double ? new EasyValue((double) result) :
                new EasyValue(result);
        }

        // -
        public static EasyValue operator -(EasyValue left, EasyValue right) {
            if (!left.IsNumber || !right.IsNumber) {
                return Null;
            }

            if (left.IsInt32 && right.IsInt32) {
                return left.AsInt32 - right.AsInt32;
            }

            if (left.IsInt64 && right.IsInt64) {
                return left.AsInt64 - right.AsInt64;
            }

            if (left.IsDouble && right.IsDouble) {
                return left.AsDouble - right.AsDouble;
            }

            if (left.IsDecimal && right.IsDecimal) {
                return left.AsDecimal - right.AsDecimal;
            }

            var result = left.AsDecimal - right.AsDecimal;
            var type = (EasyDataType) Math.Max((int) left.Type, (int) right.Type);

            return
                type == EasyDataType.Int64 ? new EasyValue((long) result) :
                type == EasyDataType.Double ? new EasyValue((double) result) :
                new EasyValue(result);
        }

        // *
        public static EasyValue operator *(EasyValue left, EasyValue right) {
            if (!left.IsNumber || !right.IsNumber) {
                return Null;
            }

            if (left.IsInt32 && right.IsInt32) {
                return left.AsInt32 * right.AsInt32;
            }

            if (left.IsInt64 && right.IsInt64) {
                return left.AsInt64 * right.AsInt64;
            }

            if (left.IsDouble && right.IsDouble) {
                return left.AsDouble * right.AsDouble;
            }

            if (left.IsDecimal && right.IsDecimal) {
                return left.AsDecimal * right.AsDecimal;
            }

            var result = left.AsDecimal * right.AsDecimal;
            var type = (EasyDataType) Math.Max((int) left.Type, (int) right.Type);

            return
                type == EasyDataType.Int64 ? new EasyValue((long) result) :
                type == EasyDataType.Double ? new EasyValue((double) result) :
                new EasyValue(result);
        }

        // /
        public static EasyValue operator /(EasyValue left, EasyValue right) {
            if (!left.IsNumber || !right.IsNumber) {
                return Null;
            }

            if (left.IsDecimal || right.IsDecimal) {
                return left.AsDecimal / right.AsDecimal;
            }

            return left.AsDouble / right.AsDouble;
        }

        public override string ToString() {
            return JsonSerializer.Serialize(this);
        }

        public static implicit operator int(EasyValue value) {
            return (int) value.RawValue;
        }

        // Int32
        public static implicit operator EasyValue(int value) {
            return new EasyValue(value);
        }

        // Int64
        public static implicit operator long(EasyValue value) {
            return (long) value.RawValue;
        }

        // Int64
        public static implicit operator EasyValue(long value) {
            return new EasyValue(value);
        }

        // Double
        public static implicit operator double(EasyValue value) {
            return (double) value.RawValue;
        }

        // Double
        public static implicit operator EasyValue(double value) {
            return new EasyValue(value);
        }

        // Decimal
        public static implicit operator decimal(EasyValue value) {
            return (decimal) value.RawValue;
        }

        // Decimal
        public static implicit operator EasyValue(decimal value) {
            return new EasyValue(value);
        }

        // UInt64 (to avoid ambigous between Double-Decimal)
        public static implicit operator ulong(EasyValue value) {
            return (ulong) value.RawValue;
        }

        // Decimal
        public static implicit operator EasyValue(ulong value) {
            return new EasyValue((double) value);
        }

        // String
        public static implicit operator string(EasyValue value) {
            return (string) value.RawValue;
        }

        // String
        public static implicit operator EasyValue(string value) {
            return new EasyValue(value);
        }

        // Binary
        public static implicit operator byte[](EasyValue value) {
            return (byte[]) value.RawValue;
        }

        // Binary
        public static implicit operator EasyValue(byte[] value) {
            return new EasyValue(value);
        }

        // ObjectId
        public static implicit operator ObjectId(EasyValue value) {
            return (ObjectId) value.RawValue;
        }

        // ObjectId
        public static implicit operator EasyValue(ObjectId value) {
            return new EasyValue(value);
        }

        // Guid
        public static implicit operator Guid(EasyValue value) {
            return (Guid) value.RawValue;
        }

        // Guid
        public static implicit operator EasyValue(Guid value) {
            return new EasyValue(value);
        }

        // Boolean
        public static implicit operator bool(EasyValue value) {
            return (bool) value.RawValue;
        }

        // Boolean
        public static implicit operator EasyValue(bool value) {
            return new EasyValue(value);
        }

        // DateTime
        public static implicit operator DateTime(EasyValue value) {
            return (DateTime) value.RawValue;
        }

        // DateTime
        public static implicit operator EasyValue(DateTime value) {
            return new EasyValue(value);
        }

        public static bool operator ==(EasyValue lhs, EasyValue rhs) {
            if (ReferenceEquals(lhs, null)) {
                return ReferenceEquals(rhs, null);
            }

            if (ReferenceEquals(rhs, null)) {
                return false; // don't check type because sometimes different types can be ==
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(EasyValue lhs, EasyValue rhs) {
            return !(lhs == rhs);
        }

        public static bool operator >=(EasyValue lhs, EasyValue rhs) {
            return lhs.CompareTo(rhs) >= 0;
        }

        public static bool operator >(EasyValue lhs, EasyValue rhs) {
            return lhs.CompareTo(rhs) > 0;
        }

        public static bool operator <(EasyValue lhs, EasyValue rhs) {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool operator <=(EasyValue lhs, EasyValue rhs) {
            return lhs.CompareTo(rhs) <= 0;
        }

        public override bool Equals(object obj) {
            if (obj is EasyValue other) {
                return Equals(other);
            }

            return false;
        }

        public override int GetHashCode() {
            var hash = 17;
            hash = 37 * hash + Type.GetHashCode();
            hash = 37 * hash + RawValue.GetHashCode();
            return hash;
        }
    }
}