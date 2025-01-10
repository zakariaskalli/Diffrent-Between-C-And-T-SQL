using BizDataLayerGen.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ConsoleApp1
{
    public class Program
    {
        public static DataTable ReturnDataFormTable()
        {
            DataTable dataTable = new DataTable();
            string query = @"
Use C21_DB1;
select * from Employees2";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log the error)
                Debug.WriteLine(ex.Message);
            }

            return dataTable;
        }

        public static void UpdateDatabase(DataTable dataTable)
        {
            string query = @"
Use C21_DB1;
UPDATE Employees2 SET Salary = @Salary WHERE Name = @Name"; // Modify query as needed

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    connection.Open();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (row.RowState == DataRowState.Modified)
                        {
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                // Add parameters to match the columns in your database
                                command.Parameters.AddWithValue("@Salary", row["Salary"]); // Replace "ColumnName" with actual column name
                                command.Parameters.AddWithValue("@Name", row["Name"]); // Replace "ID" with the primary key column name

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log the error)
                Debug.WriteLine(ex.Message);
            }
        }

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            DataTable DT = ReturnDataFormTable();

            foreach (DataRow row in DT.Rows)
            {
                if (row["Salary"] != DBNull.Value && row["PerformanceRating"] != DBNull.Value)
                {
                    decimal salary = Convert.ToDecimal(row["Salary"]);
                    decimal performanceRating = Convert.ToDecimal(row["PerformanceRating"]);

                    if (performanceRating > 90)
                    {
                        row["Salary"] = salary * 1.15m;
                    }
                    else if (performanceRating >= 75 && performanceRating <= 90)
                    {
                        row["Salary"] = salary * 1.10m;
                    }
                    else if (performanceRating >= 50 && performanceRating < 75)
                    {
                        row["Salary"] = salary * 1.05m;
                    }
                }
            }

            UpdateDatabase(DT);

            stopwatch.Stop();
            Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds} ms");

            Console.WriteLine("Perfect");

            Console.ReadKey();
        }
    }
}




//Query


/*
 
 USE C21_DB1;

-- Declare variables to capture execution time
DECLARE @StartTime DATETIME2(7), @EndTime DATETIME2(7);

-- Capture the start time
SET @StartTime = SYSDATETIME();

-- Optimized query to update salaries
UPDATE Employees2
SET Salary = Salary * 
    CASE 
        WHEN PerformanceRating > 90 THEN 1.15
        WHEN PerformanceRating BETWEEN 75 AND 90 THEN 1.10
        WHEN PerformanceRating BETWEEN 50 AND 74 THEN 1.05
        ELSE 1.00
    END
WHERE PerformanceRating IS NOT NULL; -- Add a filter to exclude unnecessary rows

-- Capture the end time
SET @EndTime = SYSDATETIME();

-- Output the execution time in milliseconds
SELECT DATEDIFF(MILLISECOND, @StartTime, @EndTime) AS ExecutionTimeMilliseconds;

 */



// Coclusion

/*
 Using C# in ADo.Net  the StopWatch give : 
  - minimum: 131 ms
  - maximum: 286 ms
  - Average: 188 ms
Using T-SQL:
  - minimum: 5 ms
  - maximum: 7 ms
  - average: 6 ms
This mean in Total T-SQL more better and Speed for C# (optimse) by 31 time 
 */