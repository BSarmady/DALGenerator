using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DALGenerator {
    internal abstract class dal_base {

        protected static readonly string CRLF = Environment.NewLine;
        protected string output_folder;
        protected string name_space;
        protected bool create_solution;
        protected bool omit_dbo;
        protected SqlConnection dbConn;
        protected LoggerDelegate Logger;

        protected DataTable table_list;
        protected DataTable procedure_list;
        protected StringBuilder project_items;

        #region protected const string Tables
        protected const string Tables = @"SELECT 
                so.object_id,
                table_name = CONCAT(SCHEMA_NAME(so.schema_id) , '.', OBJECT_NAME(so.OBJECT_ID)),
                column_name = null, 
                column_id = 0,
                type = null, 
                max_length = null, 
                is_nullable = null,
                is_identity = null,
                is_computed = null, 
                is_primary_key = null,
                is_unique = null,
                index_NAME = null,
                Comment = se.value,
                so.create_date,
                so.modify_date
            FROM 
	            sys.objects so
                LEFT JOIN sys.extended_properties se ON se.name = 'MS_Description' AND se.minor_id = 0 AND so.object_id = se.major_id
            WHERE
                so.type  in ('U') AND
                so.is_ms_shipped=0 AND
                OBJECT_NAME(so.object_id) NOT IN ('sysdiagrams', 'sp_alterdiagram','sp_creatediagram','sp_dropdiagram','sp_helpdiagramdefinition','sp_helpdiagrams','sp_renamediagram','sp_upgraddiagrams')
            UNION
            SELECT 
                so.object_id,
                table_name = CONCAT(SCHEMA_NAME(so.schema_id) , '.', OBJECT_NAME(so.OBJECT_ID)),
                column_name = sc.name, 
                sc.column_id,
                type = (SELECT name from sys.types st WHERE st.system_type_id = sc.system_type_id AND st.user_type_id = sc.user_type_id ), 
                max_length, 
                is_nullable,
                is_identity,
                is_computed, 
                is_primary_key = ISNULL(sic.is_primary_key,0),
                is_unique = ISNULL(sic.is_unique,0),
                index_NAME = sic.name,
                Comment = se.value,
                so.create_date,
                so.modify_date
            FROM 
	            sys.objects so
                LEFT JOIN sys.columns sc ON so.object_id = sc.object_id
                LEFT JOIN sys.extended_properties se ON se.name = 'MS_Description' AND se.minor_id = sc.column_id AND sc.object_id = se.major_id
                LEFT JOIN (SELECT si.object_id, sic.column_id, name, is_primary_key, is_unique FROM sys.indexes si LEFT JOIN sys.index_columns sic ON si.object_id = sic.object_id and si.index_id = sic.index_id WHERE is_disabled = 0) sic ON sic.object_id = sc.object_id AND sic.column_id = sc.column_id
            WHERE
                so.type  in ('U') AND
                so.is_ms_shipped=0 AND
                OBJECT_NAME(sc.object_id) NOT IN ('sysdiagrams', 'sp_alterdiagram','sp_creatediagram','sp_dropdiagram','sp_helpdiagramdefinition','sp_helpdiagrams','sp_renamediagram','sp_upgraddiagrams')
            ORDER BY 
                table_name DESC,
                column_id";
        #endregion

        #region protected const string Procedures
        protected const string Procedures = @"SELECT 
                so.object_id,
                procedure_name = CONCAT(SCHEMA_NAME(so.SCHEMA_ID), '.', OBJECT_NAME(so.object_id)),
                parameter_name = null, 
                parameter_id = 0, 
                type = null, 
                max_length = null,
                precision = null,
                scale = null,
                is_output = null,
                has_default_value = null,
                default_value = null,
                is_nullable = null,
                is_readonly = null,
                create_date,
                modify_date,
                Comment = se.value
            FROM 
	            sys.objects so
                LEFT JOIN sys.extended_properties se ON se.name = 'MS_Description' AND se.minor_id = 0 AND so.object_id = se.major_id
            WHERE
                so.type  in ('P') AND
                so.is_ms_shipped=0 AND
                OBJECT_NAME(so.object_id) NOT IN ('sysdiagrams', 'sp_alterdiagram','sp_creatediagram','sp_dropdiagram','sp_helpdiagramdefinition','sp_helpdiagrams','sp_renamediagram','sp_upgraddiagrams')
            UNION
            SELECT 
                sp.object_id,
                procedure_name = CONCAT(SCHEMA_NAME(so.SCHEMA_ID), '.', OBJECT_NAME(so.object_id)),
                parameter_name = sp.name, 
                sp.parameter_id, 
                type = (SELECT name from sys.types st WHERE st.system_type_id = sp.system_type_id AND st.user_type_id = sp.user_type_id ), 
                sp.max_length, 
                sp.precision, 
                sp.scale, 
                sp.is_output, 
                sp.has_default_value, 
                sp.default_value,
                is_nullable,
                is_readonly,
                create_date,
                modify_date,
                Comment = se.value
            FROM 
                sys.objects so
                LEFT JOIN sys.parameters sp ON sp.object_id = so.object_id
                LEFT JOIN sys.extended_properties se ON se.name = 'MS_Description' AND so.object_id = se.major_id AND se.minor_id = sp.parameter_id
            WHERE
                so.type in ('P') AND
                sp.parameter_id <>0 AND
                OBJECT_NAME(sp.object_id) NOT IN ('sysdiagrams', 'sp_alterdiagram','sp_creatediagram','sp_dropdiagram','sp_helpdiagramdefinition','sp_helpdiagrams','sp_renamediagram','sp_upgraddiagrams')
            ORDER BY 
                procedure_name DESC,
                parameter_id";
        #endregion

        #region protected class Method_Def
        protected class Method_Def {
            public string method_name;
            public string method_body;

            public Method_Def(string method_name, string method_body) {
                this.method_name = method_name;
                this.method_body = method_body;
            }
        }
        #endregion

        #region protected class Class_Def
        protected class Class_Def {
            public string name_space;
            public string class_name;
            public string class_doc;
            public Dictionary<string, Method_Def> methods;

            public Class_Def(string name_space, string class_name, string class_doc) {
                this.name_space = name_space;
                this.class_name = class_name;
                this.class_doc = class_doc;
                this.methods = new Dictionary<string, Method_Def> { };
            }
        }
        #endregion

        #region public CreateDALBase(...)
        public dal_base(string output_folder, string name_space, bool name_space_as_Suffix, bool omit_dbo, bool create_solution, string connection_string, LoggerDelegate Logger) {
            this.output_folder = output_folder.Trim();
            this.name_space = name_space_as_Suffix ? name_space + ".DAL" : "DAL." + name_space;
            this.create_solution = create_solution;
            this.dbConn = new SqlConnection(connection_string);
            this.Logger = Logger;
            this.omit_dbo = omit_dbo;

            this.table_list = new DataTable();
            this.procedure_list = new DataTable();
            this.project_items = new StringBuilder();
        }
        #endregion

        #region protected string exception_message_helper(...)
        protected string exception_message_helper(string message) {
            if (message.StartsWith("DataReader.GetFieldType(")) {
                message = message + CRLF + "Column Types geography, geometry, hierarchyid require microsoft proprietary libraries and are not supported, You can use other sql types to store the value." + CRLF +
                    "refer to https://docs.microsoft.com/en-us/sql/relational-databases/clr-integration-database-objects-types-net-framework/mapping-clr-parameter-data for more info";
            } else if (message.StartsWith("cannot translate ")) {
                message = message + CRLF + "ColBumn Types geography, geometry, hierarchyid require microsoft proprietary libraries and are not supported, You can use other sql types to store the value." + CRLF +
                    "refer to https://docs.microsoft.com/en-us/sql/relational-databases/clr-integration-database-objects-types-net-framework/mapping-clr-parameter-data for more info";
            }
            return message;
        }
        #endregion

        #region protected string replace_place_holders
        protected string replace_place_holders(string inText) {
            return inText.Replace("%%SOLUTION_NAME%%", name_space)
                .Replace("%%PROJECT_NAME%%", name_space)
                .Replace("%%PROJECT_GUID%%", Guid.NewGuid().ToString())
                .Replace("%%NAMESPACE%%", name_space)
                .Replace("%%COM_GUID%%", Guid.NewGuid().ToString())
                .Replace("%%PROJECT_FILES%%", project_items.ToString())
                .Replace("%%YEAR%%", DateTime.Now.ToString("yyyy"))
                .Replace("%%ASSEMBLY_NAME%%", name_space)
                .Replace("%%ASSEMBLY_TITLE%%", name_space);
        }
        #endregion

        public abstract void build();

        protected abstract string create_method_body(string method_name, string procedure_name);

        protected abstract string create_class_file(Class_Def class_def);
    }
}