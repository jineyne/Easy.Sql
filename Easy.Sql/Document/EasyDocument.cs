using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Easy.Sql.Utility.Extensions;

namespace Easy.Sql.Document {
    // base on https://github.com/mbdavid/LiteDB/blob/master/LiteDB/Document/BsonDocument.cs
    public class EasyDocument : EasyValue, IDictionary<string, EasyValue> {
        private int _length = 0;

        public EasyDocument()
            : base(EasyDataType.Document, new Dictionary<string, EasyValue>(StringComparer.OrdinalIgnoreCase)) { }

        public EasyDocument(ConcurrentDictionary<string, EasyValue> dict)
            : this() {
            if (dict == null) {
                throw new ArgumentNullException(nameof(dict));
            }

            foreach (var element in dict) {
                Add(element);
            }
        }

        public EasyDocument(IDictionary<string, EasyValue> dict)
            : this() {
            if (dict == null) {
                throw new ArgumentNullException(nameof(dict));
            }

            foreach (var element in dict) {
                Add(element);
            }
        }

        internal new Dictionary<string, EasyValue> RawValue => base.RawValue as Dictionary<string, EasyValue>;

        ///// <summary>
        /////     Get/Set position of this document inside database. It's filled when used in Find operation.
        ///// </summary>
        //internal PageAddress RawId { get; set; } = PageAddress.Empty;

        /// <summary>
        ///     Get/Set a field for document. Fields are case sensitive
        /// </summary>
        public override EasyValue this[string key] {
            get => RawValue.GetOrDefault(key, Null);
            set => RawValue[key] = value ?? Null;
        }

        #region CompareTo

        public override int CompareTo(EasyValue other) {
            // if types are different, returns sort type order
            if (other.Type != EasyDataType.Document) {
                return Type.CompareTo(other.Type);
            }

            var thisKeys = Keys.ToArray();
            var thisLength = thisKeys.Length;

            var otherDoc = other.AsDocument;
            var otherKeys = otherDoc.Keys.ToArray();
            var otherLength = otherKeys.Length;

            var result = 0;
            var i = 0;
            var stop = Math.Min(thisLength, otherLength);

            for (; 0 == result && i < stop; i++) {
                result = this[thisKeys[i]].CompareTo(otherDoc[thisKeys[i]]);
            }

            // are different
            if (result != 0) {
                return result;
            }

            // test keys length to check which is bigger
            if (i == thisLength) {
                return i == otherLength ? 0 : -1;
            }

            return 1;
        }

        #endregion

        #region IDictionary

        public ICollection<string> Keys => RawValue.Keys;

        public ICollection<EasyValue> Values => RawValue.Values;

        public int Count => RawValue.Count;

        public bool IsReadOnly => false;

        public bool ContainsKey(string key) {
            return RawValue.ContainsKey(key);
        }

        /// <summary>
        ///     Get all document elements - Return "_id" as first of all (if exists)
        /// </summary>
        public IEnumerable<KeyValuePair<string, EasyValue>> GetElements() {
            if (RawValue.TryGetValue("_id", out var id)) {
                yield return new KeyValuePair<string, EasyValue>("_id", id);
            }

            foreach (var item in RawValue.Where(x => x.Key != "_id")) {
                yield return item;
            }
        }

        public void Add(string key, EasyValue value) {
            RawValue.Add(key, value ?? Null);
        }

        public bool Remove(string key) {
            return RawValue.Remove(key);
        }

        public void Clear() {
            RawValue.Clear();
        }

        public bool TryGetValue(string key, out EasyValue value) {
            return RawValue.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, EasyValue> item) {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<string, EasyValue> item) {
            return RawValue.Contains(item);
        }

        public bool Remove(KeyValuePair<string, EasyValue> item) {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, EasyValue>> GetEnumerator() {
            return RawValue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return RawValue.GetEnumerator();
        }

        public void CopyTo(KeyValuePair<string, EasyValue>[] array, int arrayIndex) {
            ((ICollection<KeyValuePair<string, EasyValue>>) RawValue).CopyTo(array, arrayIndex);
        }

        public void CopyTo(EasyDocument other) {
            foreach (var element in this) {
                other[element.Key] = element.Value;
            }
        }

        #endregion
    }
}