using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;

namespace DALGenerator {

    internal class dal_simple: dal_base {

        #region public dal_simple(...)
        public dal_simple(string output_folder, string name_space, bool name_space_as_Suffix, bool omit_dbo, bool create_solution, string connection_string, LoggerDelegate Logger) :
            base(output_folder, name_space, name_space_as_Suffix, omit_dbo, create_solution, connection_string, Logger) {
        }
        #endregion

        #region private string examine_proc
        private string examine_proc(string ProcName) {
            try {
                if (dbConn.State == ConnectionState.Open)
                    dbConn.Close();
                using (SqlCommand command = new SqlCommand(ProcName, dbConn) { CommandType = CommandType.StoredProcedure }) {
                    dbConn.Open();
                    SqlCommandBuilder.DeriveParameters(command);
                    dbConn.Close();

                    DataTable table = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.FillSchema(table, SchemaType.Source);
                    if (table.Columns.Count > 0)
                        return "table";
                    else
                        return "scalar";
                }
            } catch (Exception ex) {
                return ProcName + " Returned Error: " + exception_message_helper(ex.Message);
            }
        }
        #endregion

        #region public override void build()(...);
        public override void build() {
            try {
                Logger("Reading Database Information ...");
                #region Read database metadata and build lists of table and procedures
                using (SqlDataAdapter adapter = new SqlDataAdapter(Tables, dbConn)) {
                    adapter.Fill(table_list);
                }
                using (SqlDataAdapter adapter = new SqlDataAdapter(Procedures, dbConn)) {
                    adapter.Fill(procedure_list);
                }
                Dictionary<string, string> table_names = new Dictionary<string, string> { };
                Logger("Building Class list ...");
                foreach (DataRow row in table_list.Rows) {
                    if (row["column_name"].ToString() != "")
                        continue;
                    string table_name = row["table_name"].ToString();
                    if (!table_names.ContainsKey(table_name)) {
                        table_names.Add(table_name, row["comment"].ToString());
                    }
                }
                #endregion

                Logger("Creating methods ... ");
                #region Build method body and list of methods
                Dictionary<string, Dictionary<string, Class_Def>> DAL = new Dictionary<string, Dictionary<string, Class_Def>> { };
                foreach (DataRow row in procedure_list.Rows) {
                    try {
                        if (row["parameter_name"].ToString() != "")
                            continue;
                        string method_name = row["procedure_name"].ToString();
                        int dot_pos = method_name.IndexOf(".");
                        string name_space = method_name.Substring(0, dot_pos);
                        string class_name = "_" + name_space;
                        string class_doc = "";
                        foreach (string table_name in table_names.Keys) {
                            if (method_name.StartsWith(table_name + "_")) {
                                class_name = table_name.Replace(name_space + ".", "");
                                method_name = method_name.Replace(table_name + "_", "");
                                class_doc = table_names[table_name];
                                break;
                            }
                        }

                        if (method_name.StartsWith(name_space + "."))
                            method_name = method_name.Replace(name_space + ".", "");

                        Logger("\t" + name_space + "." + class_name + "." + method_name + "(...)", Color.Blue);

                        if (!DAL.ContainsKey(name_space))
                            DAL.Add(name_space, new Dictionary<string, Class_Def> { });
                        if (!DAL[name_space].ContainsKey(class_name))
                            DAL[name_space].Add(class_name, new Class_Def(name_space, class_name, class_doc));

                        DAL[name_space][class_name].methods.Add(method_name, new Method_Def(method_name, create_method_body(method_name, row["procedure_name"].ToString())));
                    } catch (Exception ex) {
                        Logger("Error: Exception occured while creating method for " + row["parameter_name"].ToString() + CRLF + ex.ToString(), Color.Red);
                    }

                }
                #endregion

                Logger("Creating Classes ... ");
                #region Write Class Files and add their name to list of project files
                foreach (string name_space in DAL.Keys) {
                    foreach (string class_name in DAL[name_space].Keys) {
                        string src_filename = (create_class_file(DAL[name_space][class_name]));
                        project_items.Append("    <Compile Include=\"" + src_filename + "\" />" + CRLF);
                    }
                }
                #endregion

                Logger("Creating Project Files ... ");
                #region write project base files
                Dictionary<string, string> base_files = new Dictionary<string, string> {
                    { "AssemblyInfo.cs", files.Resources.AssemblyInfo_cs},
                    { "DALBase.cs", files.Resources.DALBase_cs },
                    { "DALException.cs", files.Resources.DALException_cs},
                    { "Extensions.cs", files.Resources.Extensions_cs},
                    { name_space + ".csproj", files.Resources.DAL_csproj },

                };

                if (create_solution) {
                    base_files.Add(name_space + ".sln", files.Resources.DAL_sln);
                }

                foreach (KeyValuePair<string, string> base_file in base_files) {
                    string file_name = output_folder + base_file.Key;
                    if (!File.Exists(file_name)) {
                        Logger("\t" + base_file.Key, Color.Blue);
                        File.WriteAllText(file_name, replace_place_holders(base_file.Value));
                    } else {
                        Logger("\t" + base_file.Key + " exists, skipping", Color.Orange);
                    }
                }
                #endregion

            } catch (Exception ex) {
                if (ex is ThreadAbortException)
                    return;
                Logger(ex.Message, Color.Red);
            }
        }
        #endregion

        #region protected override string create_method_body(...)
        protected override string create_method_body(string method_name, string procedure_name) {
            StringBuilder sb = new StringBuilder();
            DataTable procedure = new DataView(procedure_list, "procedure_name='" + procedure_name + "'", "parameter_id", DataViewRowState.CurrentRows).ToTable();

            #region find procedure document from database
            string method_description = "";
            foreach (DataRow row in procedure.Rows) {
                if (row["parameter_name"].ToString() != "")
                    continue;
                method_description = row["comment"].ToString();
            }
            #endregion

            // Find return type of procedure (a table, an int value, or procedure has error)
            string output_type = examine_proc(procedure_name);

            #region Create parameter list and document
            StringBuilder var_list = new StringBuilder();
            StringBuilder param_docs = new StringBuilder();
            foreach (DataRow row in procedure.Rows) {
                if (row["parameter_name"].ToString() == "")
                    continue;
                string parameter_name = row["parameter_name"].ToString().Substring(1);
                param_docs.AppendLine("        /// <param name=\"" + parameter_name + "\">" + row["Comment"].PadStringAtNewLines() + "</param>");
                if (row["is_output"].ToBoolean()) {
                    var_list.Append("out ");
                }
                var_list.Append(row["type"].to_csharp_sql_type_name() + " " + parameter_name + ", ");
                //if (row["is_output"].ToBoolean()) {
                //    VarList.Append("out ");
                //}
            }
            if (var_list.Length > 0)
                var_list.Remove(var_list.Length - 2, 2); // remove last comma and space
            #endregion

            #region Create Method Documentation
            switch (output_type) {
                case "table":
                    sb.AppendLine("        #region public DataTable " + method_name + "(...)");
                    break;
                case "scalar":
                    sb.AppendLine("        #region public int " + method_name + "(...)");
                    break;
                default:
                    sb.AppendLine("        #region public void " + method_name + "(...)");
                    Logger("\t" + output_type.Replace(CRLF, CRLF + "\t\t"), Color.Red);
                    break;
            }
            if (method_description != "") {
                sb.AppendLine("        /// <summary>");
                sb.AppendLine("        /// " + method_description.PadStringAtNewLines());
                sb.AppendLine("        /// </summary>");
            }
            if (param_docs.Length > 0)
                sb.Append(param_docs);
            #endregion

            #region Create Method Name
            switch (output_type) {
                case "table":
                    sb.AppendLine("        /// <returns>Datatable</returns>");
                    sb.AppendLine("        public DataTable " + method_name + "(" + var_list.ToString() + ") {");
                    break;
                case "scalar":
                    sb.AppendLine("        /// <returns>int</returns>");
                    sb.AppendLine("        public int " + method_name + "(" + var_list.ToString() + ") {");
                    break;
                default:
                    sb.AppendLine("        /// <returns>nothing</returns>");
                    sb.AppendLine("        public void " + method_name + "(" + var_list.ToString() + ") {");
                    break;
            }
            #endregion

            switch (output_type) {
                case "table":
                    #region Create Method Body for return type = Table
                    sb.AppendLine("            SqlDataAdapter adapter = new SqlDataAdapter(PrepareCommand(\"" + procedure_name + "\"));");
                    foreach (DataRow row in procedure.Rows) {
                        if (row["parameter_name"].ToString() == "")
                            continue;
                        string parameter_name = row["parameter_name"].ToString().Substring(1);
                        if (!row["is_output"].ToBoolean()) {
                            if (row["type"].to_csharp_sql_type_name() == "SqlString") {
                                sb.AppendLine("            adapter.SelectCommand.Parameters.AddWithValue(\"" + row["parameter_name"] + "\", (" + parameter_name + " == \"\" ? SqlString.Null : " + parameter_name + "));");
                            } else {
                                sb.AppendLine("            adapter.SelectCommand.Parameters.AddWithValue(\"" + row["parameter_name"] + "\", " + parameter_name + ");");
                            }

                        } else {
                            sb.AppendLine("            adapter.SelectCommand.Parameters.AddWithValue(\"" + row["parameter_name"] + "\", " + row["type"].to_csharp_sql_type_name() + ".Null);");
                            sb.AppendLine("            adapter.SelectCommand.Parameters[\"" + row["parameter_name"] + "\"].Direction = ParameterDirection.Output;");
                        }
                    }
                    sb.AppendLine("            adapter.SelectCommand.Parameters.Add(\"@ReturnValue\", SqlDbType.Int);");
                    sb.AppendLine("            adapter.SelectCommand.Parameters[\"@ReturnValue\"].Direction = ParameterDirection.ReturnValue;");

                    sb.AppendLine("            DataTable table = new DataTable();");
                    sb.AppendLine("            adapter.Fill(table);");

                    foreach (DataRow row in procedure.Rows) {
                        if (row["is_output"].ToBoolean()) {
                            string parameter_name = row["parameter_name"].ToString().Substring(1);
                            sb.AppendLine("            " + parameter_name + " = (" + row["type"].to_csharp_sql_type_name() + ") adapter.SelectCommand.Parameters[\"" + row["parameter_name"] + "\"].Value;");
                        }
                    }
                    sb.AppendLine("");
                    sb.AppendLine("            return table;");
                    #endregion
                    break;
                case "scalar":
                    #region Create Method Body for return type = Scalar (int)
                    sb.AppendLine("            SqlCommand command = PrepareCommand(\"" + procedure_name + "\");");
                    foreach (DataRow row in procedure.Rows) {
                        if (row["parameter_name"].ToString() == "")
                            continue;
                        string parameter_name = row["parameter_name"].ToString().Substring(1);
                        if (!row["is_output"].ToBoolean()) {
                            if (row["type"].to_csharp_sql_type_name() == "SqlString") {
                                sb.AppendLine("            command.Parameters.AddWithValue(\"" + row["parameter_name"] + "\", (" + parameter_name + " == \"\" ? SqlString.Null : " + parameter_name + "));");
                            } else {
                                sb.AppendLine("            command.Parameters.AddWithValue(\"" + row["parameter_name"] + "\", " + parameter_name + ");");
                            }
                        } else {
                            sb.AppendLine("            command.Parameters.AddWithValue(\"" + row["parameter_name"] + "\", " + row["type"].to_csharp_sql_type_name() + ".Null);");
                            sb.AppendLine("            command.Parameters[\"" + row["parameter_name"] + "\"].Direction = ParameterDirection.Output;");
                            //TODO: Check if setting out direction is required
                        }
                    }

                    sb.AppendLine("            command.Parameters.Add(\"@RETURN_VALUE\", SqlDbType.Int);");
                    sb.AppendLine("            command.Parameters[\"@RETURN_VALUE\"].Direction = ParameterDirection.ReturnValue;");
                    sb.AppendLine("            command.ExecuteNonQuery();");

                    foreach (DataRow row in procedure.Rows) {
                        if (row["is_output"].ToBoolean()) {
                            string parameter_name = row["parameter_name"].ToString().Substring(1);
                            sb.AppendLine("            " + parameter_name + " = (" + row["type"].to_csharp_sql_type_name() + ") command.Parameters[\"" + row["parameter_name"] + "\"].Value;");
                        }
                    }
                    sb.AppendLine("");
                    sb.AppendLine("            return (int) command.Parameters[\"@RETURN_VALUE\"].Value;");
                    #endregion
                    break;
                default:
                    #region Method throw exception while running DeriveParameters (means is not supported)
                    sb.AppendLine("            throw new DALException(\"" + output_type.Replace(CRLF, "").Replace("\"", "\\\"") + "\");");
                    #endregion
                    break;
            }
            sb.AppendLine("        }");
            sb.AppendLine("        #endregion");
            return sb.ToString();
        }
        #endregion

        #region protected override string create_class_file(...)
        protected override string create_class_file(Class_Def class_def) {
            if (omit_dbo && class_def.name_space == "dbo")
                class_def.name_space = "";

            Logger("\t" + name_space + (class_def.name_space == "" ? "" : "." + class_def.name_space) + "." + class_def.class_name, Color.Green);

            #region Create class code
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Data.SqlTypes;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("");
            sb.AppendLine("namespace " + name_space + (class_def.name_space == "" ? "" : "." + class_def.name_space) + " {");
            sb.AppendLine("");

            #region Constructor
            //sb.AppendLine("    #region public class " + class_def.class_name);
            if (class_def.class_doc != "") {
                sb.AppendLine("    /// <summary>");
                sb.AppendLine("    /// " + class_def.class_doc.PadStringAtNewLines());
                sb.AppendLine("    /// </summary>");
            }
            sb.AppendLine("    public class " + class_def.class_name + ": DALBase {");

            sb.AppendLine("");
            sb.AppendLine("        #region Constructor");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Constructor");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <param name=\"DbConnection\">Database Connection</param>");
            sb.AppendLine("        public " + class_def.class_name + "(SqlConnection DbConnection)");
            sb.AppendLine("            : base(DbConnection) {");
            sb.AppendLine("        }");
            sb.AppendLine("        #endregion");
            sb.AppendLine("");
            #endregion

            // Sort methods
            /*
            class_def.methods.Sort((Method_Def a, Method_Def b) => {
                return a.method_name.CompareTo(b.method_name);
            });
            */

            // Add methods
            foreach (Method_Def method in class_def.methods.Values) {
                sb.AppendLine(method.method_body);
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            #endregion

            #region Save class to file
            string file_name = output_folder + (class_def.name_space == "" ? "" : class_def.name_space + "\\") + class_def.class_name + ".cs";
            string Folder = Path.GetDirectoryName(file_name);
            if (!Directory.Exists(Folder)) {
                Directory.CreateDirectory(Folder);
            }
            File.WriteAllText(file_name, sb.ToString());
            #endregion

            return file_name.Replace(output_folder, "");
        }
        #endregion
    }
}