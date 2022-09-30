using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

namespace test {
    class Program {

        static void Main(string[] args) {

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"])) {

                using (DAL.TableWithKey dal_tableWithKey = new DAL.TableWithKey(conn)) {

                    Entities.TableWithKey tableWithKey = new Entities.TableWithKey(
                        "Test Data",
                        "Description for test data"
                    );

                    Console.WriteLine("Adding record (\"" + tableWithKey.Name + "\", \"" + tableWithKey.Description + "\"), ");
                    int recordID = dal_tableWithKey.Add(tableWithKey);
                    Console.WriteLine("\trecord added, Id is " + recordID);
                    Console.WriteLine();

                    tableWithKey.Description = "Description for test data edited";
                    Console.WriteLine("Editing record " + recordID + ", ");
                    int result = dal_tableWithKey.Edit(recordID, tableWithKey);
                    Console.WriteLine("\t" + result + " record edited.");
                    Console.WriteLine();

                    Console.WriteLine("Getting record " + recordID);
                    Entities.TableWithKey get_tableWithKey = dal_tableWithKey.Get(recordID);
                    Console.WriteLine("\tId\t\t: " + get_tableWithKey.Id);
                    Console.WriteLine("\tName\t\t: " + get_tableWithKey.Name);
                    Console.WriteLine("\tDescription\t: " + get_tableWithKey.Description);
                    Console.WriteLine();

                    Console.WriteLine("Deleting record " + recordID + ", ");
                    result = dal_tableWithKey.Delete(recordID);
                    Console.WriteLine("\t" + result + " record deleted.");
                    Console.WriteLine();


                    Console.WriteLine("Adding record (\"" + tableWithKey.Name + "\", \"" + tableWithKey.Description + "\") with stored procedure");
                    recordID = dal_tableWithKey.AddWithProc("Test Data", "Description for test data");
                    Console.WriteLine("\trecord added, Id is " + recordID);
                    Console.WriteLine();

                    tableWithKey.Description = "Description for test data edited";
                    Console.WriteLine("Editing record " + recordID + "  with stored procedure");
                    result = dal_tableWithKey.Edit(recordID, tableWithKey);
                    Console.WriteLine("\t" + result + " record edited.");
                    Console.WriteLine();

                    Console.WriteLine("Getting record " + recordID + "  with stored procedure");
                    DataTable table = dal_tableWithKey.GetWithProc(recordID);
                    if (table != null && table.Rows.Count > 1) {
                        Console.WriteLine("Get Result: ");
                        Console.WriteLine("\tId\t\t: " + get_tableWithKey.Id);
                        Console.WriteLine("\tName\t\t: " + get_tableWithKey.Name);
                        Console.WriteLine("\tDescription\t: " + get_tableWithKey.Description);
                    } else {
                        Console.WriteLine("\tRecord not found");
                    }
                    Console.WriteLine();

                    Console.WriteLine("Deleting record " + recordID + "  with stored procedure");
                    result = dal_tableWithKey.DeleteWithProc(recordID);
                    Console.WriteLine("\t" + result + " record deleted.");
                    Console.WriteLine();
                }

                using (DAL._dbo dal_dbo = new DAL._dbo(conn)) {

                    Console.WriteLine("Calling Method_Without_Table (a procedure without table)");
                    DataTable table = dal_dbo.Method_Without_Table("1");
                    if (table != null && table.Rows.Count > 1) {
                        StringBuilder sb = new StringBuilder();

                        foreach (DataColumn col in table.Columns) {
                            Console.Write(col.ColumnName.PadRight(14, ' '));
                            sb.Append("=".PadRight(14, '='));
                        }
                        Console.WriteLine();
                        Console.WriteLine(sb.ToString()); ;
                        foreach (DataRow row in table.Rows) {
                            foreach (DataColumn col in table.Columns) {
                                Console.Write(row[col.ColumnName].ToString().PadRight(14, ' '));
                            }
                            Console.WriteLine("");
                        }
                    } else {
                        Console.WriteLine("\tNo Records found");
                    }

                    Console.WriteLine();
                }

                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
            }
        }
    }
}
