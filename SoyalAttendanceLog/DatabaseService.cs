using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace SoyalAttendanceLog
{
    public class DatabaseService
    {
        private readonly string _connectionString =
            "Data Source=attendance.db;Version=3;";

        public void InitializeDatabase()
        {
           
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                CREATE TABLE IF NOT EXISTS AttendanceLogs
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DeviceId TEXT NOT NULL,
                    UserId TEXT NOT NULL,
                    UserName TEXT,
                    EventTime TEXT NOT NULL,
                    VerifyType TEXT,
                    EventType TEXT,
                    Source TEXT,
                    LogIndex TEXT,
                    UNIQUE(DeviceId, LogIndex)
                );";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                     string employeeSql = @"
                    CREATE TABLE IF NOT EXISTS Employees
                    (
                        UserId TEXT PRIMARY KEY,
                        UserName TEXT NOT NULL
                    );";

            using (var employeeCommand = new SQLiteCommand(employeeSql, connection))
            {
                employeeCommand.ExecuteNonQuery();
            }
                }
            }

        }

        public bool InsertLog(AttendanceLog log)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
               INSERT OR IGNORE INTO AttendanceLogs
                (LogIndex, DeviceId, UserId, UserName, EventTime, VerifyType, EventType, Source)
                               VALUES
                (@LogIndex, @DeviceId, @UserId, @UserName, @EventTime, @VerifyType, @EventType, @Source);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@LogIndex", log.LogIndex);
                    command.Parameters.AddWithValue("@DeviceId", log.DeviceId);
                    command.Parameters.AddWithValue("@UserId", log.UserId);
                    command.Parameters.AddWithValue("@UserName", log.UserName);
                    command.Parameters.AddWithValue("@EventTime", log.EventTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@VerifyType", log.VerifyType);
                    command.Parameters.AddWithValue("@EventType", log.EventType);
                    command.Parameters.AddWithValue("@Source", log.Source);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        public void SaveEmployee(string userId, string userName)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                INSERT OR REPLACE INTO Employees
                (UserId, UserName)
                VALUES
                (@UserId, @UserName);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@UserName", userName);

                    command.ExecuteNonQuery();
                }
            }
        }
        public string GetEmployeeName(string userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                SELECT UserName
                FROM Employees
                WHERE UserId = @UserId;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    var result = command.ExecuteScalar();

                    return result == null ? "" : result.ToString();
                }
            }
        }
        public List<AttendanceLog> GetAllLogs()
        {
            var logs = new List<AttendanceLog>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                SELECT Id, DeviceId, UserId, UserName, EventTime, VerifyType, EventType, Source
                FROM AttendanceLogs
                ORDER BY EventTime DESC;";

                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(new AttendanceLog
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            DeviceId = reader["DeviceId"].ToString(),
                            UserId = reader["UserId"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            EventTime = DateTime.Parse(reader["EventTime"].ToString()),
                            VerifyType = reader["VerifyType"].ToString(),
                            EventType = reader["EventType"].ToString(),
                            Source = reader["Source"].ToString()
                        });
                    }
                }
            }

            return logs;
        }
        public void UpdateLogUserName(string userId, string userName)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                UPDATE AttendanceLogs
                SET UserName = @UserName
                WHERE UserId = @UserId;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@UserName", userName);

                    command.ExecuteNonQuery();
                }
            }
        }
        public void ClearLogs()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "DELETE FROM AttendanceLogs;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public int CountEmployees()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM Employees;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        public List<AttendanceLog> SearchLogsByUserId(string userId)
        {
            var logs = new List<AttendanceLog>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                SELECT *
                FROM AttendanceLogs
                WHERE UserId = @UserId
                ORDER BY EventTime DESC;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logs.Add(new AttendanceLog
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                LogIndex = reader["LogIndex"].ToString(),
                                DeviceId = reader["DeviceId"].ToString(),
                                UserId = reader["UserId"].ToString(),
                                UserName = reader["UserName"].ToString(),
                                EventTime = DateTime.Parse(reader["EventTime"].ToString()),
                                VerifyType = reader["VerifyType"].ToString(),
                                EventType = reader["EventType"].ToString(),
                                Source = reader["Source"].ToString()
                            });
                        }
                    }
                }
            }

            return logs;
        }
        public int CountTodayLogs()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
        SELECT COUNT(*)
        FROM AttendanceLogs
        WHERE date(EventTime) = date('now', 'localtime');";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int CountUniqueEmployeesToday()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                SELECT COUNT(DISTINCT UserId)
                FROM AttendanceLogs
                WHERE date(EventTime) = date('now', 'localtime')
                  AND UserId IS NOT NULL
                  AND UserId <> '';";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int CountInvalidCardsToday()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                SELECT COUNT(*)
                FROM AttendanceLogs
                WHERE date(EventTime) = date('now', 'localtime')
                  AND (
                      EventType LIKE '%Invalid card%'
                      OR VerifyType LIKE '%Invalid card%'
                      OR EventType LIKE '%Fingerprint Error%'
                      OR VerifyType LIKE '%Fingerprint Error%'
                  );";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        public List<AttendanceLog> GetLogsByDate(DateTime fromDate, DateTime toDate)
        {
            var logs = new List<AttendanceLog>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                SELECT *
                FROM AttendanceLogs
                WHERE EventTime >= @FromDate
                  AND EventTime <= @ToDate
                ORDER BY EventTime DESC";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logs.Add(new AttendanceLog
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                DeviceId = reader["DeviceId"].ToString(),
                                UserId = reader["UserId"].ToString(),
                                UserName = reader["UserName"].ToString(),
                                EventTime = DateTime.Parse(reader["EventTime"].ToString()),
                                VerifyType = reader["VerifyType"].ToString(),
                                EventType = reader["EventType"].ToString(),
                                Source = reader["Source"].ToString()
                            });
                        }
                    }
                }
            }

            return logs;
        }
        public int CountLogs()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM AttendanceLogs;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

    }
}