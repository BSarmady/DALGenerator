using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DALGenerator {

    delegate void LoggerDelegate(string Message, Color? color = null);

    static class Program {

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);

        private const int ATTACH_PARENT_PROCESS = -1;
        private static StringBuilder log = new StringBuilder();
        private static bool save_log = true;

        #region private static int Main(...)
        [STAThread]
        private static int Main(string[] args) {

            if (args.Length > 0) {
                #region App has parameters and either is running from bat file or command line
                string server = "";
                bool oslogin = false;
                string user = "";
                string pass = "";
                string database = "";
                string output = "";
                string name_space = "";
                bool with_suffix = false;
                bool with_model = false;
                bool with_solution = false;
                bool omit_dbo = false;

                try {
                    AttachConsole(ATTACH_PARENT_PROCESS);
                    args = build_argument_list(Environment.CommandLine);

                    // Parse parameters
                    foreach (string param in args) {
                        string to_lower = param.ToLower();
                        if (to_lower.StartsWith("--server=")) {
                            server = param.get_param_value();
                        } else if (to_lower.StartsWith("--user=")) {
                            user = param.get_param_value();
                        } else if (to_lower.StartsWith("--pass=")) {
                            pass = param.get_param_value();
                        } else if (to_lower.StartsWith("--database=")) {
                            database = param.get_param_value();
                        } else if (to_lower.StartsWith("--output=")) {
                            output = param.get_param_value();
                        } else if (to_lower.StartsWith("--namespace=")) {
                            name_space = param.get_param_value();
                        } else if (to_lower == "--oslogin") {
                            oslogin = true;
                        } else if (to_lower == "--withsuffix") {
                            with_suffix = true;
                        } else if (to_lower == "--withmodel") {
                            with_model = true;
                        } else if (to_lower == "--nolog") {
                            save_log = false;
                        } else if (to_lower == "--withsolution") {
                            with_solution = true;
                        } else if (to_lower == "--omitdbo") {
                            omit_dbo = true;
                        }
                    }
                    // validate parameters
                    if (
                        (server == "") ||
                        (!oslogin && (user == "" || pass == "")) ||
                        (database == "")
                    ) {
                        ShowHelp();
                        return 1;
                    }

                    if (name_space == "") {
                        name_space = database;
                        Log("No namespace was specified, using " + name_space + " as namespace", Color.Red);
                    }
                    name_space = name_space.Replace("[", "").Replace("]", "").Replace("-", "_");
                    if (char.IsDigit(name_space[0]) || char.IsSymbol(name_space[0])) {
                        name_space = "_" + name_space;
                    }

                    if (output == "") {
                        output = Path.GetDirectoryName(Application.ExecutablePath).AddTrailingBackSlashes();
                        Log("Output folder was not specified, using " + output, Color.Red);
                    }
                    output = output.AddTrailingBackSlashes();

                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.DataSource = server;
                    if (oslogin) {
                        builder.IntegratedSecurity = true;
                    } else {
                        builder.UserID = user;
                        builder.Password = pass;
                        builder.IntegratedSecurity = false;
                    }
                    builder.InitialCatalog = database;

                    dal_base DAL;
                    if (with_model) {
                        DAL = new dal_with_model(output, name_space, with_suffix, omit_dbo, with_solution, builder.ConnectionString, Log);
                        //CreateDAL();
                    } else {
                        DAL = new dal_simple(output, name_space, with_suffix, omit_dbo, with_solution, builder.ConnectionString, Log);
                    }
                    DAL.build();
                } catch (Exception ex) {
                    Log("An Exception was thrown while building the DAL." + ex.Message, Color.Red);
                    Log("Additional information:", Color.Red);
                    Log(ex.ToString(), Color.Red);
                }
                if (save_log) {
                    File.WriteAllText(output + "DALGenerator.log.txt", log.ToString());
                }
                #endregion
            } else {
                #region Run in winform
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
                #endregion
            }
            return 0;
        }
        #endregion

        #region private void Log(string Message, Color? color)
        private static void Log(string Message, Color? color = null) {
            ConsoleColor prev_color = Console.ForegroundColor;
            if (color == null || color == Color.Black) {
                Console.ForegroundColor = ConsoleColor.White;
            } else {
                Console.ForegroundColor = color.Value.ToConsoleColor();
            }
            Console.WriteLine(Message);
            if (save_log)
                log.AppendLine(Message);

            Console.ForegroundColor = prev_color;
        }
        #endregion

        #region private static string get_param_value(...)
        private static string get_param_value(this string param, string default_value = "") {
            string[] parts = param.Split(new char[] { '=' }, 2);
            if (parts.Length == 2)
                return parts[1].Trim();
            return default_value;
        }
        #endregion

        #region private static string[] build_argument_list(...)
        private static string[] build_argument_list(string commandLine) {
            StringBuilder sb = new StringBuilder(commandLine);

            bool in_quote = false;
            for (int i = 0; i < sb.Length; i++) {
                if (sb[i] == '"')
                    in_quote = !in_quote;
                if (sb[i] == ' ' && !in_quote)
                    sb[i] = '\n';
            }
            string[] array = sb.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < array.Length; j++) {
                array[j] = array[j].Replace("\"", "");
            }
            return array;
        }
        #endregion

        #region private static ConsoleColor ToConsoleColor(...)
        /// <summary>
        /// Converts 24 bit Color to 4 bit console color
        /// </summary>
        /// <param name="color">24 bit color</param>
        /// <returns>4 bit console color</returns>
        private static ConsoleColor ToConsoleColor(this Color color) {
            int index = (color.R > 128 | color.G > 128 | color.B > 128) ? 8 : 0; // Brightness
            index |= (color.R > 64) ? 4 : 0; // Red
            index |= (color.G > 64) ? 2 : 0; // Green
            index |= (color.B > 64) ? 1 : 0; // Blue
            return (ConsoleColor) index;
        }
        #endregion

        #region private static void ShowHelp()
        private static void ShowHelp() {
            Console.WriteLine(@"Console Parameters are:
    --server=[server address]
        Required server address with port if necessary
    --oslogin
        Use Windows authentication
    --user=[username]
        Username, required if oslogin is not used
    --pass=[password]
        Password, required if oslogin is not used
    --database=[database name]
        Required database name
    --output=[output folder]
        Output folder surround in double quotes if has spaces, 
        if left empty current path will be used
    --name_space=[namespace]
        namespace to be used for Data Access Layer. DAL always 
        will be attached to namespace, either as prefix or suffix
    --withsuffix
        attach DAL to namespace as suffix instead of prefix
    --wihtmodel
        Create with model classes too (slower preformance)
    --withsolution
        Create solution file too. if exists, will not be overwritten
    --nolog
        Do not save log file
    --omitdbo
        Do not add dbo schema to namespace");
        }
        #endregion
    }
}