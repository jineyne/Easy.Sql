﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Easy.Sql.Utility.Extensions {
    // https://github.com/mbdavid/LiteDB/blob/master/LiteDB/Utils/Extensions/DictionaryExtensions.cs
    internal static class DictionaryExtensions {
        public static T GetOrDefault<K, T>(this IDictionary<K, T> dict, K key, T defaultValue = default) {
            if (dict.TryGetValue(key, out var result)) {
                return result;
            }

            return defaultValue;
        }

        public static T GetOrAdd<K, T>(this IDictionary<K, T> dict, K key, Func<K, T> valueFactoy) {
            if (dict.TryGetValue(key, out var value) == false) {
                value = valueFactoy(key);

                dict.Add(key, value);
            }

            return value;
        }

        public static void ParseKeyValue(this IDictionary<string, string> dict, string connectionString) {
            var position = 0;

            while (position < connectionString.Length) {
                EatWhitespace();
                var key = ReadKey();

                EatWhitespace();
                var value = ReadValue();

                dict[key] = value;
            }

            string ReadKey() {
                var sb = new StringBuilder();

                while (position < connectionString.Length) {
                    var current = connectionString[position];

                    if (current == '=') {
                        position++;
                        return sb.ToString().Trim();
                    }

                    sb.Append(current);
                    position++;
                }

                return sb.ToString().Trim();
            }

            string ReadValue() {
                var sb = new StringBuilder();
                var quote =
                    connectionString[position] == '"' ? '"' :
                    connectionString[position] == '\'' ? '\'' : ' ';

                if (quote != ' ') {
                    position++;
                }

                while (position < connectionString.Length) {
                    var current = connectionString[position];

                    if (quote == ' ') {
                        if (current == ';') {
                            position++;
                            return sb.ToString().Trim();
                        }
                    } else if (quote != ' ' && current == quote) {
                        if (connectionString[position - 1] == '\\') {
                            sb.Length--;
                        } else {
                            position++;

                            EatWhitespace();

                            if (connectionString[position] == ';') {
                                position++;
                            }

                            return sb.ToString();
                        }
                    }

                    sb.Append(current);
                    position++;
                }

                return sb.ToString().Trim();
            }

            void EatWhitespace() {
                while (position < connectionString.Length) {
                    if (connectionString[position] == ' ' ||
                        connectionString[position] == '\t' ||
                        connectionString[position] == '\f') {
                        position++;
                        continue;
                    }

                    break;
                }
            }
        }

        /// <summary>
        ///     Get value from dictionary converting datatype T
        /// </summary>
        public static T GetValue<T>(this Dictionary<string, string> dict, string key, T defaultValue = default) {
            try {
                if (dict.TryGetValue(key, out var value) == false) {
                    return defaultValue;
                }

                if (typeof(T) == typeof(TimeSpan)) {
                    // if timespan are numbers only, convert as seconds
                    if (Regex.IsMatch(value, @"^\d+$", RegexOptions.Compiled)) {
                        return (T) (object) TimeSpan.FromSeconds(Convert.ToInt32(value));
                    }

                    return (T) (object) TimeSpan.Parse(value);
                }

                if (typeof(T).GetTypeInfo().IsEnum) {
                    return (T) Enum.Parse(typeof(T), value, true);
                }

                return (T) Convert.ChangeType(value, typeof(T));
            } catch (Exception) {
                //TODO: fix string connection parser
                throw new Exception($"Invalid connection string value type for `{key}`");
            }
        }
    }
}