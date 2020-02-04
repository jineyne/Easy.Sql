using System.Data;

namespace Easy.Sql {
    public interface IEasyDatabase {
        void Open(string fileName);

        /// <summary>
        ///     Execute query.
        /// </summary>
        DataTable Execute(string query);

        /// <summary>
        ///     Execute query.
        ///     This method contain try - catch statement
        /// </summary>
        DataTable ExecuteSafe(string query);

        object ExecuteScalar(string query);
    }
}