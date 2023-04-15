using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using Dapper;
using MessageControl;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace DataBaseConnection.DataAccess
{
    public static class Helper
    {
        private static QueryFactory db;
        private static IDbConnection Connection => db.Connection;

        private static System.Timers.Timer connectionTimer;

        public static void Init(string connectionString)
        {
            db = new QueryFactory(new SQLiteConnection(connectionString), new MySqlCompiler());
            InitTimer();
        }

        public static void Init(QueryFactory dbFactory)
        {
            db = dbFactory;
            InitTimer();
        }

        private static void InitTimer()
        {
            connectionTimer = new(100);
            connectionTimer.AutoReset = false;
            connectionTimer.Elapsed += ConnectionTimer_Elapsed;
        }

        private static void ConnectionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CloseConnection();
        }

        private static void CloseConnection()
        {
            try
            {
                Connection?.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("exception caught while closing connection");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage($"Database Error: {ex.Message}"));
                });
            }
        }

        private static DynamicParameters CreateDynamicsParameters(this List<object> parameters)
        {
            if (parameters is null || parameters.Count == 0) return null;
            DynamicParameters p = new();
            int index = 0;
            foreach (object param in parameters)
            {
                string name = "@p" + index;
                p.Add(name, param);
                index++;
            }
            return p;
        }

        private static T ExecuteSafely<T>(this Func<T> action)
        {
            T result;

            try
            {
                connectionTimer?.Stop(); // if was running then connection is still open, avoid closing it during execution

                if (Connection.State != ConnectionState.Open)
                {
                    Connection?.Open();
                }

                result = action.Invoke();
            }
            catch(Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage($"Database Error: {ex.Message}"));
                });

                connectionTimer?.Start();
                return default;
            }

            connectionTimer?.Start(); 
            return result;
        }

        private static void ExecuteSafely(this Action action)
        {
            try
            {
                connectionTimer?.Stop(); // if was running then connection was not yet closed

                if (Connection.State != ConnectionState.Open)
                    Connection?.Open();

                action.Invoke();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage($"Database Error: {ex.Message}"));
                });
            }
            finally
            {
                connectionTimer?.Start();
            }
        }

        private static void ExecuteSafely<T>(this Action<T> action, T args)
        {
            try
            {
                connectionTimer?.Stop(); // if was running then connection was not yet closed

                if (Connection.State != ConnectionState.Open)
                    Connection?.Open();

                action.Invoke(args);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage($"Database Error: {ex.Message}"));
                });
            }
            finally
            {
                connectionTimer?.Start();
            }
        }

        private static SqlResult Compile(this Query query)
        {
            return db.Compiler.Compile(query);
        }

        /// <summary>
        /// Execute the insert query and select the id
        /// </summary>
        /// <param name="query"></param>
        /// <returns>the id of the inserted row</returns>
        public static Task<int> InsertAsync(this Query query, bool getId = true)
        {
            SqlResult sqlResult = query.Compile();

            if(getId)
                sqlResult.Sql += " RETURNING Id";

            Func<Task<int>> func = () => Connection.ExecuteScalarAsync<int>(sqlResult.Sql, sqlResult.Bindings.CreateDynamicsParameters());

            return func.ExecuteSafely();
        }

        /// <summary>
        /// Execute the insert query and select the id
        /// </summary>
        /// <param name="query"></param>
        /// <returns>the id of the inserted row</returns>
        public static int Insert(this Query query, bool getId = true)
        {
            SqlResult sqlResult = query.Compile();

            if(getId)
                sqlResult.Sql += " RETURNING Id";

            Func<int> func = () => Connection.ExecuteScalar<int>(sqlResult.Sql, sqlResult.Bindings.CreateDynamicsParameters());

            return func.ExecuteSafely();
        }

        public static void Insert(this List<Query> query)
        {
            List<SqlResult> sqlResults = new List<SqlResult>();
            foreach (Query q in query)
            {
                sqlResults.Add(q.Compile());
            }

            Action<List<SqlResult>> action = (List<SqlResult> results) =>
            {
                foreach (SqlResult q in results)
                {
                    Connection.Execute(q.Sql, q.Bindings.CreateDynamicsParameters());
                }
            };

            action.ExecuteSafely<List<SqlResult>>(sqlResults);
        }

        public static Task<List<T>> GetAsync<T>(this Query query)
        {
            SqlResult sqlResult = query.Compile();
            DynamicParameters p = sqlResult.Bindings.CreateDynamicsParameters();
            Func<Task<List<T>>> func = async () => (await Connection.QueryAsync<T>(sqlResult.Sql, param:p)).ToList();
            return func.ExecuteSafely();
        }

        public static Task<List<int>> GetAsync(this Query query)
        {
            SqlResult sqlResult = query.Compile();
            Func<Task<List<int>>> func = async () => (await Connection.QueryAsync<int>(sqlResult.Sql, sqlResult.Bindings.CreateDynamicsParameters())).ToList();
            return func.ExecuteSafely();
        }

        public static List<T> Get<T>(this Query query)
        {
            SqlResult sqlResult = query.Compile();
            Func<List<T>> func = () => Connection.Query<T>(sqlResult.Sql, sqlResult.Bindings.CreateDynamicsParameters()).ToList();
            return func.ExecuteSafely();
        }

        public static Task<T> FirstAsync<T>(this Query query)
        {
            SqlResult sqlResult = query.Compile();
            Func<Task<T>> func =  async () => await Connection.QueryFirstOrDefaultAsync<T>(sqlResult.Sql, sqlResult.Bindings.CreateDynamicsParameters());
            return func.ExecuteSafely();
        }

        public static T First<T>(this Query query)
        {
            SqlResult sqlResult = query.Compile();
            DynamicParameters p = sqlResult.Bindings.CreateDynamicsParameters();
            Func<T> func = () => Connection.QueryFirstOrDefault<T>(sqlResult.Sql, p);
            return func.ExecuteSafely();
        }


        public static void Delete(this Query query)
        {
            SqlResult sqlResult = query.Compile();
            Action action = () =>  Connection.Execute(sqlResult.Sql, sqlResult.Bindings.CreateDynamicsParameters());
            action.ExecuteSafely();
        }

        public static Task DeleteAsync(this Query query)
        {
            SqlResult sqlResult = query.Compile();
            Func<Task> func = () => Connection.ExecuteAsync(sqlResult.Sql, sqlResult.Bindings.CreateDynamicsParameters());
            return func.ExecuteSafely();
        }

        public static int Count(this Query query)
        {
            SqlResult sqlResult = query.Compile();
            Func<int> func = () => Connection.ExecuteScalar<int>(sqlResult.Sql, sqlResult.Bindings.CreateDynamicsParameters());
            return func.ExecuteSafely();
        }

        public static Task<int> CountAsync(this Query query)
        {
            SqlResult sqlResult = query.Compile();
            Func<Task<int>> func = () => Connection.ExecuteScalarAsync<int>(sqlResult.Sql, sqlResult.Bindings.CreateDynamicsParameters());
            return func.ExecuteSafely();
        }

        public static Task ExecuteAsync(this Query query)
        {
            SqlResult sqlResult = query.Compile();
            Func<Task> func = () => Connection.ExecuteAsync(sqlResult.Sql, sqlResult.Bindings.CreateDynamicsParameters());
            return func.ExecuteSafely();
        }

        public static void Execute(this Query query)
        {
            SqlResult sqlResult = query.Compile();
            Action action = () => Connection.ExecuteAsync(sqlResult.Sql, sqlResult.Bindings.CreateDynamicsParameters());
            action.ExecuteSafely();
        }

        public static Task ExecuteAsync(this List<Query> queries)
        {
            List<SqlResult> sqlResults = new List<SqlResult>();
            foreach (Query q in queries)
            {
                sqlResults.Add(q.Compile());
            }

            Action<List<SqlResult>> action = (List<SqlResult> results) =>
            {
                foreach (SqlResult q in results)
                {
                    Connection.Execute(q.Sql, q.Bindings.CreateDynamicsParameters());
                }
            };

            return Task.Run(() => action.ExecuteSafely<List<SqlResult>>(sqlResults));
        }

        public static string DateTimeToString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string DateTimeToDateOnlyString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// take a timespan and return its string representation
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns> hh:mm:ss or mm:ss if hours are not needed </returns>
        public static string ToFormattedString(this TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
                return timeSpan.ToString(@"d\.hh\:mm\:ss");
            else if (timeSpan.Hours > 0)
                return timeSpan.ToString(@"hh\:mm\:ss");
            else return timeSpan.ToString(@"mm\:ss");
        }

        public static string FormatStringToTime(this string time)
        {
            if (time.Split(":").Length == 2)
            {
                return "00:" + time;
            }
            else return time;
        }
    }
}
