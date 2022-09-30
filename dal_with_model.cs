using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;

namespace DALGenerator {

    internal class dal_with_model: dal_base {

        private static string[] unsupported_column_types = new string[] { "geography", "geometry", "hierarchyid" };
        private static string[] readonly_column_types = new string[] { "timestamp" };
        private string model_name_space;

        #region public dal_simple(...)
        public dal_with_model(string output_folder, string name_space, bool name_space_as_Suffix, bool omit_dbo, bool create_solution, string connection_string, LoggerDelegate Logger) :
            base(output_folder, name_space, name_space_as_Suffix, omit_dbo, create_solution, connection_string, Logger) {
            this.model_name_space = name_space_as_Suffix ? name_space + ".Entities" : "Entities." + name_space;
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

        #region public override void build(...);
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
                    if (!table_names.ContainsKey(row["table_name"].ToString())) {
                        table_names.Add(row["table_name"].ToString(), row["comment"].ToString());
                    }
                }
                Dictionary<string, string> procedure_names = new Dictionary<string, string> { };
                foreach (DataRow row in procedure_list.Rows) {
                    if (row["parameter_name"].ToString() != "")
                        continue;
                    if (!procedure_names.ContainsKey(row["procedure_name"].ToString())) {
                        procedure_names.Add(row["procedure_name"].ToString(), row["comment"].ToString());
                    }
                }
                #endregion

                Logger("Creating models ... ");
                #region Build model classes
                foreach (DataRow row in table_list.Rows) {
                    if (row["column_name"] != DBNull.Value)
                        continue;
                    DataTable columns = new DataView(table_list, "table_name='" + row["table_name"] + "'", "", DataViewRowState.CurrentRows).ToTable();
                    project_items.Append("    <Compile Include=\"" + create_model_class(columns) + "\" />" + CRLF);
                }
                #endregion

                Logger("Creating methods ... ");
                #region Build method body and list of methods
                Dictionary<string, Dictionary<string, Class_Def>> DAL = new Dictionary<string, Dictionary<string, Class_Def>> { };
                foreach (DataRow row in table_list.Rows) {
                    if (row["column_name"].ToString() != "")
                        continue;
                    string[] TableName = row["table_name"].ToString().Split(new char[] { '.' });

                    DataTable table_columns = new DataView(table_list, "table_name='" + row["table_name"] + "'", "", DataViewRowState.CurrentRows).ToTable();

                    if (!DAL.ContainsKey(TableName[0]))
                        DAL.Add(TableName[0], new Dictionary<string, Class_Def> { });
                    if (!DAL[TableName[0]].ContainsKey(TableName[1]))
                        DAL[TableName[0]].Add(TableName[1], new Class_Def(TableName[0], TableName[1], row["comment"].ToString()));

                    /*
                    try {
                        DAL[TableName[0]][TableName[1]].methods.Add("_ColumnDetails",create_column_list_comment(table_columns));
                    } catch (Exception ex) {
                        Logger("    Error: " + ex.Message, Color.Red);
                    }
                    */

                    try {
                        Logger("    " + row["table_name"] + ".Add(...)");
                        DAL[TableName[0]][TableName[1]].methods.Add("Add", create_add_method_body(table_columns));
                    } catch (Exception ex) {
                        Logger("    Error: " + ex.Message, Color.Red);
                    }
                    try {
                        Logger("    " + row["table_name"] + ".Edit(...)");
                        DAL[TableName[0]][TableName[1]].methods.Add("Edit", create_edit_method_body(table_columns));
                    } catch (Exception ex) {
                        Logger("    Error: " + ex.Message, Color.Red);
                    }
                    try {
                        Logger("    " + row["table_name"] + ".Delete(...)");
                        DAL[TableName[0]][TableName[1]].methods.Add("Delete", create_delete_method_body(table_columns));
                    } catch (Exception ex) {
                        Logger("    Error: " + ex.Message, Color.Red);
                    }

                    try {
                        Logger("    " + row["table_name"] + ".Get(...)");
                        DAL[TableName[0]][TableName[1]].methods.Add("Get", create_get_method_body(table_columns));
                    } catch (Exception ex) {
                        Logger("    Error: " + ex.Message, Color.Red);
                    }
                }

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

                        // if namespace does not exist, add it to list
                        if (!DAL.ContainsKey(name_space))
                            DAL.Add(name_space, new Dictionary<string, Class_Def> { });
                        // if class does not exist, add it to list
                        if (!DAL[name_space].ContainsKey(class_name))
                            DAL[name_space].Add(class_name, new Class_Def(name_space, class_name, class_doc));

                        // if method does not exist, add it to list otherwise overwrite it
                        if (DAL[name_space][class_name].methods.ContainsKey(method_name)) {
                            DAL[name_space][class_name].methods[method_name] = new Method_Def(method_name, create_method_body(method_name, row["procedure_name"].ToString()));
                        } else {
                            DAL[name_space][class_name].methods.Add(method_name, new Method_Def(method_name, create_method_body(method_name, row["procedure_name"].ToString())));
                        }
                    } catch (Exception ex) {
                        Logger("\tException while creating method for " + row["parameter_name"].ToString() + CRLF + ex.ToString(), Color.Red);
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
                Logger("Error: " + ex.Message, Color.Red);
            }
        }
        #endregion

        #region private string create_model_class(...)
        private string create_model_class(DataTable column_defs) {
            StringBuilder sb = new StringBuilder();
            string TableName = "";
            string class_namespace = "";

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Data.SqlTypes;");
            sb.AppendLine();

            #region build model class header
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value) {
                    TableName = row["table_name"].ToString();
                    class_namespace = TableName.Split('.')[0];
                    if (class_namespace.ToLower() == "dbo" && omit_dbo)
                        class_namespace = "";
                    TableName = TableName.Split('.')[1];

                    Logger("    " + model_name_space + "." + TableName);
                    sb.AppendLine("namespace " + model_name_space + (class_namespace == "" ? "" : "." + class_namespace) + " {");
                    sb.AppendLine();
                    if (row["comment"] != DBNull.Value) {
                        sb.AppendLine("    /// <summary>");
                        sb.AppendLine("    /// " + row["comment"].ToString().PadStringAtNewLines());
                        sb.AppendLine("    /// </summary>");
                    }
                    sb.AppendLine("    public class " + TableName + " {");
                    break;
                }
            }
            sb.AppendLine();
            #endregion

            #region build propery list
            sb.AppendLine("        #region Properties");
            bool starting = true;
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;
                string comments = "";
                if (row["is_primary_key"].ToBoolean())
                    comments += "Primary key, ";
                else if (row["is_unique"].ToBoolean())
                    comments += "Unique, ";
                if (row["is_identity"].ToBoolean())
                    comments += "Identity, ";
                if (row["is_computed"].ToBoolean())
                    comments += "Computed, ";
                if (comments != "")
                    comments = "(" + comments.Trim(new char[] { ' ', ',' }) + ")";
                if (row["comment"] != DBNull.Value)
                    comments = row["comment"].ToString().PadStringAtNewLines() + " " + comments;
                if (comments != "") {
                    if (!starting)
                        sb.AppendLine();
                    starting = false;
                    sb.AppendLine("        /// <summary>");
                    sb.AppendLine("        /// " + comments.Trim());
                    sb.AppendLine("        /// </summary>");
                }
                if (row["is_identity"].ToBoolean() || row["is_computed"].ToBoolean() || row["type"].ToString() == "timestamp") {
                    sb.AppendLine("        public " + row["type"].to_csharp_sql_type_name() + " " + row["column_name"] + " { get; }");
                } else {
                    sb.AppendLine("        public " + row["type"].to_csharp_sql_type_name() + " " + row["column_name"] + " { get; set; }");
                }
            }
            sb.AppendLine("        #endregion");
            #endregion

            #region build constructors
            sb.AppendLine();
            sb.AppendLine("        #region public " + TableName + "()");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// public constructor without parameters");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        public " + TableName + "() {");
            sb.AppendLine("        }");
            sb.AppendLine("        #endregion");

            sb.AppendLine();
            sb.AppendLine("        #region public " + TableName + "(...)");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// public constructor");
            sb.AppendLine("        /// </summary>");

            List<string> public_param_list = new List<string> { };
            List<string> internal_param_list = new List<string> { };
            List<string> readonly_list = new List<string> { };
            List<string> public_assign = new List<string> { };
            List<string> internal_assign = new List<string> { };
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;
                if (row["is_identity"].ToBoolean() || row["is_computed"].ToBoolean()) {
                    readonly_list.Add(row["type"].to_csharp_sql_type_name() + " " + row["column_name"]);
                    internal_assign.Add(" this." + row["column_name"] + " = " + row["column_name"] + ";");
                    internal_param_list.Add(row["type"].to_csharp_sql_type_name() + " " + row["column_name"]);

                    // TODO: fix the value assigned, if cannot be null
                    public_assign.Add(" this." + row["column_name"] + " = " + row["type"].to_csharp_sql_type_name() + ".Null;");
                } else if (row["type"].ToString() == "timestamp") {
                    internal_assign.Add(" this." + row["column_name"] + " = " + row["column_name"] + ";");
                    internal_param_list.Add(row["type"].to_csharp_sql_type_name() + " " + row["column_name"]);
                } else {
                    public_param_list.Add(row["type"].to_csharp_sql_type_name() + " " + row["column_name"]);
                    internal_param_list.Add(row["type"].to_csharp_sql_type_name() + " " + row["column_name"]);
                    internal_assign.Add(" this." + row["column_name"] + " = " + row["column_name"] + ";");

                    public_assign.Add(" this." + row["column_name"] + " = " + row["column_name"] + ";");
                }
            }

            sb.AppendLine("        public " + TableName + "(" + string.Join(", ", public_param_list.ToArray()) + ") {");
            sb.AppendLine("            " + string.Join(CRLF + "            ", public_assign.ToArray()));
            sb.AppendLine("        }");
            sb.AppendLine("        #endregion");

            if (readonly_list.Count > 0) {
                sb.AppendLine();
                sb.AppendLine("        #region internal " + TableName + "(...)");
                sb.AppendLine("        /// <summary>");
                sb.AppendLine("        /// internal constructor");
                sb.AppendLine("        /// </summary>");

                sb.AppendLine("        internal " + TableName + "(" + string.Join(", ", internal_param_list.ToArray()) + ") {");
                sb.AppendLine("            " + string.Join(CRLF + "            ", internal_assign.ToArray()));
                sb.AppendLine("        }");
                sb.AppendLine("        #endregion");
            }
            #endregion

            sb.AppendLine("    }");
            sb.AppendLine("}");

            #region write model class to file
            string file_name = output_folder + ("Entities." + class_namespace).Replace(".", "\\").AddTrailingBackSlashes() + TableName + ".cs";
            //string file_name = output_folder + "Entities\\" + TableName.Replace(".", "\\") + ".cs";
            string Folder = Path.GetDirectoryName(file_name);
            if (!Directory.Exists(Folder)) {
                Directory.CreateDirectory(Folder);
            }
            File.WriteAllText(file_name, sb.ToString().Replace(CRLF + CRLF + CRLF, CRLF + CRLF));
            #endregion

            return file_name;
        }
        #endregion

        #region protected string create_add_method_body(...)
        private Method_Def create_add_method_body(DataTable column_defs) {
            StringBuilder sb = new StringBuilder();
            string table_name = column_defs.Rows[0]["table_name"].ToString();
            string entity_name = table_name.Split('.')[1];
            if (table_name.StartsWith("dbo.") && omit_dbo)
                table_name = table_name.Replace("dbo.", "");

            bool has_identity = false;
            //List<string> sql_params = new List<string>{ };
            List<string> sql_columns = new List<string> { };
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;

                if (Array.IndexOf(unsupported_column_types, row["type"].ToString().ToLower()) > -1) {
                    Logger("Error: " + table_name + "." + row["type"] + " column type require microsoft proprietary libraries and are not supported.", Color.OrangeRed);
                    continue;
                }
                if (Array.IndexOf(readonly_column_types, row["type"].ToString().ToLower()) > -1) {
                    Logger("    Warning: " + table_name + "." + row["type"] + " column is readonly, Add method will not attempt to write to it.", Color.OrangeRed);
                    continue;
                }

                if (row["is_identity"].ToBoolean()) {
                    has_identity = true;
                    continue;
                }
                if (row["is_computed"].ToBoolean())
                    continue;
                sql_columns.Add(row["column_name"].ToString());
            }

            #region method documentation
            sb.AppendLine("        #region public int Add(...)");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Adds a record to " + table_name + " table");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <param name=\"" + entity_name + "\">Entity record to be added</param>");
            sb.AppendLine("        /// <returns>" + (has_identity ? "Identity of record if succeeded, 0 if failed" : "1 if record is inserted, 0 if failed") + "</returns>");
            #endregion

            #region method signature
            sb.AppendLine("        public int Add(" + model_name_space + "." + table_name + " " + entity_name + ") {");
            #endregion

            sb.AppendLine("            string sql = @\"INSERT INTO " + table_name + "(");
            sb.AppendLine("                    " + string.Join(", ", sql_columns).SplitAtWords().PadStringAtNewLines("                    ").Trim(new char[] { ' ', '\r', '\n' }));
            sb.AppendLine("                ) VALUES (");
            sb.AppendLine("                    @" + string.Join(", @", sql_columns).SplitAtWords().PadStringAtNewLines("                    ").Trim(new char[] { ' ', '\r', '\n' }));
            sb.AppendLine("                );");
            sb.AppendLine("                SELECT " + (has_identity ? "SCOPE_IDENTITY()" : "@@ROWCOUNT") + ";\";");

            sb.AppendLine("            using (SqlCommand cmd = new SqlCommand(sql, dbConnection)) {");
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value || row["is_identity"].ToBoolean() || row["is_computed"].ToBoolean())
                    continue;
                if (Array.IndexOf(unsupported_column_types, row["type"].ToString().ToLower()) > -1)
                    continue;
                if (Array.IndexOf(readonly_column_types, row["type"].ToString().ToLower()) > -1)
                    continue;
                sb.AppendLine("                cmd.Parameters.AddWithValue(\"@" + row["column_name"] + "\", " + entity_name + "." + row["column_name"] + ");");
            }
            sb.AppendLine();
            sb.AppendLine("                if (dbConnection.State != ConnectionState.Open)");
            sb.AppendLine("                    dbConnection.Open();");
            sb.AppendLine("                int result = cmd.ExecuteScalar().ToInt32(0);");
            sb.AppendLine("                if (dbConnection.State != ConnectionState.Closed)");
            sb.AppendLine("                    dbConnection.Close();");
            sb.AppendLine("                return result;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("        #endregion");

            return new Method_Def("Add", sb.ToString());
        }
        #endregion

        #region protected string create_edit_method_body(...)
        private Method_Def create_edit_method_body(DataTable column_defs) {
            StringBuilder sb = new StringBuilder();
            string table_name = column_defs.Rows[0]["table_name"].ToString();
            string entity_name = table_name.Split('.')[1];
            if (table_name.StartsWith("dbo.") && omit_dbo)
                table_name = table_name.Replace("dbo.", "");

            List<string> key_list = new List<string> { };
            List<string> param_key_list = new List<string> { };
            List<string> sql_columns = new List<string> { };
            int field_len = 0;
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;
                if (row["is_primary_key"].ToBoolean()) {
                    key_list.Add(row["column_name"].ToString());
                    param_key_list.Add(row["type"].to_csharp_type_name(row["is_nullable"].ToBoolean()) + " " + row["column_name"].ToString());
                    if (field_len < row["column_name"].ToString().Length)
                        field_len = row["column_name"].ToString().Length;
                }
                if (row["is_identity"].ToBoolean() || row["is_computed"].ToBoolean())
                    continue;
                if (Array.IndexOf(unsupported_column_types, row["type"].ToString().ToLower()) > -1) {
                    Logger("    Error: " + table_name + "." + row["type"] + " column type require microsoft proprietary libraries and are not supported.", Color.OrangeRed);
                    continue;
                }
                if (Array.IndexOf(readonly_column_types, row["type"].ToString().ToLower()) > -1) {
                    Logger("    Warning: " + table_name + "." + row["type"] + " column is readonly, Edit method will not attempt to write to it.", Color.OrangeRed);
                    continue;
                }

                sql_columns.Add(row["column_name"].ToString());
                if (field_len < row["column_name"].ToString().Length)
                    field_len = row["column_name"].ToString().Length;
            }
            for (int i = 0; i < sql_columns.Count; i++) {
                sql_columns[i] = sql_columns[i].PadRight(field_len) + " = @" + sql_columns[i];
            }
            for (int i = 0; i < param_key_list.Count; i++) {
                key_list[i] = key_list[i].PadRight(field_len) + " = @" + key_list[i];
            }


            if (key_list.Count < 1)
                throw new Exception(table_name + " does not have primary key, will not create edit method");
            #region method documentation
            sb.AppendLine("        #region public int Edit(...)");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Edits a record in " + table_name + " table");
            sb.AppendLine("        /// </summary>");
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;
                if (!row["is_primary_key"].ToBoolean())
                    continue;
                sb.AppendLine("        /// <param name=\"" + row["column_name"] + "\">" + (row["comment"].ToString() == "" ? row["column_name"] : row["comment"]) + "</param>");
            }

            sb.AppendLine("        /// <param name=\"" + entity_name + "\">Entity record that will replace old record</param>");
            sb.AppendLine("        /// <returns>number of records affected</returns>");
            #endregion

            #region method signature
            sb.AppendLine("        public int Edit(" + string.Join(", ", param_key_list) + ", " + model_name_space + "." + table_name + " " + entity_name + ") {");
            #endregion

            sb.AppendLine("            string sql = @\"UPDATE " + table_name + " SET ");
            sb.AppendLine("                    " + string.Join("," + CRLF, sql_columns).PadStringAtNewLines("                    ").Trim(new char[] { ' ', '\r', '\n' }));
            sb.AppendLine("                WHERE ");
            sb.AppendLine("                    " + string.Join("AND " + CRLF + "@", key_list).PadStringAtNewLines("                    ").Trim(new char[] { ' ', '\r', '\n' }) + ";");
            sb.AppendLine("                SELECT @@ROWCOUNT;\";");

            sb.AppendLine("            using (SqlCommand cmd = new SqlCommand(sql, dbConnection)) {");
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;
                if (!row["is_primary_key"].ToBoolean())
                    continue;
                if (Array.IndexOf(unsupported_column_types, row["type"].ToString().ToLower()) > -1)
                    continue;
                if (Array.IndexOf(readonly_column_types, row["type"].ToString().ToLower()) > -1)
                    continue;

                sb.AppendLine("                cmd.Parameters.AddWithValue(\"@" + row["column_name"] + "\", " + row["column_name"] + ");");
            }

            sb.AppendLine();
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value || row["is_identity"].ToBoolean() || row["is_computed"].ToBoolean())
                    continue;
                if (Array.IndexOf(unsupported_column_types, row["type"].ToString().ToLower()) > -1)
                    continue;
                if (Array.IndexOf(readonly_column_types, row["type"].ToString().ToLower()) > -1)
                    continue;

                sb.AppendLine("                cmd.Parameters.AddWithValue(\"@" + row["column_name"] + "\", " + entity_name + "." + row["column_name"] + ");");
            }
            sb.AppendLine();
            sb.AppendLine("                if (dbConnection.State != ConnectionState.Open)");
            sb.AppendLine("                    dbConnection.Open();");
            sb.AppendLine("                int result = cmd.ExecuteScalar().ToInt32(0);");
            sb.AppendLine("                if (dbConnection.State != ConnectionState.Closed)");
            sb.AppendLine("                    dbConnection.Close();");
            sb.AppendLine("                return result;");
            sb.AppendLine("            }");

            sb.AppendLine("        }");
            sb.AppendLine("        #endregion");

            return new Method_Def("Add", sb.ToString());
        }
        #endregion

        #region protected string create_column_list_comment(...)
        private Method_Def create_column_list_comment(DataTable column_defs) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("        /*");
            foreach (DataColumn col in column_defs.Columns) {
                sb.Append(col.ColumnName + ";");
            }
            sb.AppendLine();
            foreach (DataRow row in column_defs.Rows) {
                foreach (DataColumn col in column_defs.Columns) {
                    sb.Append(row[col.ColumnName] + ";");
                }
                sb.AppendLine();
            }
            sb.AppendLine("*" + "/");
            return new Method_Def("_Comments", sb.ToString().PadStringAtNewLines("        "));
        }
        #endregion

        #region protected string create_delete_method_body(...)
        private Method_Def create_delete_method_body(DataTable column_defs) {
            StringBuilder sb = new StringBuilder();
            string table_name = column_defs.Rows[0]["table_name"].ToString();
            string entity_name = table_name.Split('.')[1];
            if (table_name.StartsWith("dbo.") && omit_dbo)
                table_name = table_name.Replace("dbo.", "");

            List<string> key_list = new List<string> { };
            List<string> param_key_list = new List<string> { };
            List<string> sql_columns = new List<string> { };
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;
                if (row["is_primary_key"].ToBoolean()) {
                    key_list.Add(row["column_name"].ToString() + " = @" + row["column_name"].ToString());
                    param_key_list.Add(row["type"].to_csharp_type_name(row["is_nullable"].ToBoolean()) + " " + row["column_name"].ToString());
                }
            }

            if (key_list.Count < 1)
                throw new Exception(table_name + " does not have primary key, will not create delete method");
            #region method documentation
            sb.AppendLine("        #region public int Delete(...)");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Deletes a record from " + table_name + " table");
            sb.AppendLine("        /// </summary>");
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;
                if (!row["is_primary_key"].ToBoolean())
                    continue;
                sb.AppendLine("        /// <param name=\"" + row["column_name"] + "\">" + (row["comment"].ToString() == "" ? row["column_name"] : row["comment"]) + "</param>");
            }
            sb.AppendLine("        /// <returns>number of records affected</returns>");
            #endregion

            #region method signature
            sb.AppendLine("        public int Delete(" + string.Join(", ", param_key_list) + ") {");
            #endregion

            sb.AppendLine("            string sql = @\"DELETE FROM " + table_name + " WHERE " + string.Join("AND " + CRLF + "@", key_list) + ";SELECT @@ROWCOUNT;\";");
            sb.AppendLine("            using (SqlCommand cmd = new SqlCommand(sql, dbConnection)) {");
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;
                if (!row["is_primary_key"].ToBoolean())
                    continue;
                if (Array.IndexOf(unsupported_column_types, row["type"].ToString().ToLower()) > -1)
                    continue;
                if (Array.IndexOf(readonly_column_types, row["type"].ToString().ToLower()) > -1)
                    continue;

                sb.AppendLine("                cmd.Parameters.AddWithValue(\"@" + row["column_name"] + "\", " + row["column_name"] + ");");
            }
            sb.AppendLine();
            sb.AppendLine("                if (dbConnection.State != ConnectionState.Open)");
            sb.AppendLine("                    dbConnection.Open();");
            sb.AppendLine("                int result = cmd.ExecuteScalar().ToInt32(0);");
            sb.AppendLine("                if (dbConnection.State != ConnectionState.Closed)");
            sb.AppendLine("                    dbConnection.Close();");
            sb.AppendLine("                return result;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("        #endregion");
            return new Method_Def("Delete", sb.ToString());
        }
        #endregion

        #region protected string create_get_method_body(...)
        private Method_Def create_get_method_body(DataTable column_defs) {
            StringBuilder sb = new StringBuilder();
            string table_name = column_defs.Rows[0]["table_name"].ToString();
            string entity_name = table_name.Split('.')[1];
            if (table_name.StartsWith("dbo.") && omit_dbo)
                table_name = table_name.Replace("dbo.", "");

            List<string> key_list = new List<string> { };
            List<string> param_key_list = new List<string> { };
            List<string> assign_list = new List<string> { };
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;
                if (row["is_primary_key"].ToBoolean()) {
                    key_list.Add(row["column_name"].ToString() + " = @" + row["column_name"].ToString());
                    param_key_list.Add(row["type"].to_csharp_sql_type_name() + " " + row["column_name"].ToString());
                }
                assign_list.Add("row[\"" + row["column_name"] + "\"].To" + row["type"].to_csharp_sql_type_name() + "()");
            }

            if (key_list.Count < 1)
                throw new Exception(table_name + " does not have primary key, will not create delete method");
            #region method documentation
            sb.AppendLine("        #region public " + model_name_space + "." + table_name + " Get(...)");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Get a record from " + table_name + " table");
            sb.AppendLine("        /// </summary>");
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;
                if (!row["is_primary_key"].ToBoolean())
                    continue;
                sb.AppendLine("        /// <param name=\"" + row["column_name"] + "\">" + (row["comment"].ToString() == "" ? row["column_name"] : row["comment"]) + "</param>");
            }
            sb.AppendLine("        /// <returns>Return a " + entity_name + " class or null if no matching record was found</returns>");
            #endregion

            #region method signature
            sb.AppendLine("        public " + model_name_space + "." + table_name + " Get(" + string.Join(", ", param_key_list) + ") {");
            #endregion

            sb.AppendLine("            string sql = @\"SELECT * FROM " + table_name + " WHERE " + string.Join("AND " + CRLF + "@", key_list) + ";SELECT @@ROWCOUNT;\";");
            sb.AppendLine("            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, dbConnection)) {");
            foreach (DataRow row in column_defs.Rows) {
                if (row["column_name"] == DBNull.Value)
                    continue;
                if (!row["is_primary_key"].ToBoolean())
                    continue;
                if (Array.IndexOf(unsupported_column_types, row["type"].ToString().ToLower()) > -1)
                    continue;
                if (Array.IndexOf(readonly_column_types, row["type"].ToString().ToLower()) > -1)
                    continue;

                sb.AppendLine("                adapter.SelectCommand.Parameters.AddWithValue(\"@" + row["column_name"] + "\", " + row["column_name"] + ");");
            }
            sb.AppendLine();
            sb.AppendLine("                if (dbConnection.State != ConnectionState.Open)");
            sb.AppendLine("                    dbConnection.Open();");
            sb.AppendLine("                DataTable table = new DataTable();");
            sb.AppendLine("                adapter.Fill(table);");

            sb.AppendLine("                if (dbConnection.State != ConnectionState.Closed)");
            sb.AppendLine("                    dbConnection.Close();");
            sb.AppendLine("                if (table == null || table.Rows.Count < 1)");
            sb.AppendLine("                    return null;");
            sb.AppendLine("                DataRow row = table.Rows[0];");
            sb.AppendLine("                return new " + model_name_space + "." + table_name + "(");
            sb.AppendLine("                    " + string.Join("," + CRLF, assign_list).PadStringAtNewLines("                    "));
            sb.AppendLine("                );");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("        #endregion");
            return new Method_Def("Delete", sb.ToString());
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
                    sb.AppendLine("            throw new Exception(\"" + output_type.Replace(CRLF, "").Replace("\"", "\\\"") + "\");");
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

            Logger("\t" + name_space + "." + class_def.name_space + "." + class_def.class_name, Color.Green);

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
            string file_name = output_folder + "DAL\\" + class_def.name_space + "\\" + class_def.class_name + ".cs";
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