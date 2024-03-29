## What is this?
This is a code generator that builds a light weight Data Access Layer (DAL) to do CRUD operation and also creates an C# interface for calling SQL server stored procedures. 

<p align="center">
<img src="https://github.com/BSarmady/DALGenerator/blob/main/dalgenerator.png" width="320">
</p>


## Why it was created?
Many years ago I noticed most of customer bug reports were due to errors caused by database operations, such as a missing column or parameter, passing a string to a numeric parameter or change in database structure that was not implemented in the UI application.

We needed an application to automatically check our code to make sure its DAL interfaces matches database structure.

Next problem was the absent of Entity Framework from most of our clients' servers. Asking them to install it required writing a justification letter every time and a lengthy process of approval, then sometimes they would install incorrect version which would cause more unrelated errors which in turn would leave behind bad feedback from customer's IT team. I came out with an neat idea. What if I could generate the DAL code based on database and create native language methods.

A few looks around in internals of SQL server and I came out with this application. I migrated one of our company applications to use this DAL during my own free time and presented to my team, they were sceptic at first but when they tried it, almost everyone liked the idea so base on feedback from team I started improving it for our company needs. A few years later majority of feedback from other teams in company was that it makes our job faster, easier and we have less bug reports from customer.

## Why would I want to use this?
-	It runs faster than EF since it does not do the initial checks that are normally done by EF during startup. These checks are not required since assumption is database structure is correct.
-	It creates a smaller dll compared to EF. A typical DAL dll created by this app for a database with 200 tables and 1200 procedures is about 45 KB (Yes! Kilobytes not Megabytes)
-	It allows higher security on database. Application access to database can be limited to tables and procedures included in the DAL, since it doesn't expose every single table and procedure in the database.
-	Since developer will be calling native methods instead of mixing SQL, LINQ and C# commands, chance of making mistakes is lower.
-	Most checks such as length and range checks are not implemented which in turn causes faster running applications. (It is job of business logic to validate data before sending them to DAL)
-	This can be included in CI/CD pipeline to regenerate the DAL automatically which will ensure changes in database will cause compile errors instead of runtime errors.
-	Intellisense will pick up method signature and if this app is used in conjunction with 2 other application in this toolset, will show the column, table and procedure comments as hints for method and parameters.

<p align="center">
<img src="https://github.com/BSarmady/DALGenerator/blob/main/Intellisense%20Support.png" width="320">
</p>



-	Note that using n-tier application architecture (which is recommended way of programming for enterprise applications including cloud) will help protecting the proprietary company code, since no developer is required to have access to entire code base to be able to compile their application. Database (DB) is developed by someone, Business logic (BL) code is generated by someone else and API, Webservices, UI each by different developer.
-	From my observation on projects in my company that use this app toolset, I noticed they complete between 60 to 80 percent faster than when EF used. In one case project with 1 year timeline was completed and deployed in 4 month (enterprise level project with minimum 182 active client applications at any time).

## How the code will look like using this
This app can generate 2 types of DAL, one with model structures and one without them, the one with models has overhead of having to create an instance of each structure and insert data into it which is then slower than directly using the data row itself.


```c#
using (SqlConnection conn = new SqlConnection(Connection_String)) {
    using (test.DAL.Entry dal_entry = new test.DAL.Entry(conn)) {

        // Add Record
        test.Entities.Entry entry = new test.Entities.Entry();
        entry.AccountNo = "4568099023246532";
        int recordID = dal_entry.Add(entry);

        // Edit Record
        int result = dal_entry.Edit(recordID, entry);

        // Get Record by its Id
        test.Entities.Entry rec = dal_entry.Get(11);
        Console.WriteLine(rec.AccountNo);

        // Delete Record
        result = dal_entry.Delete(recordID);

        // Call custom stored procedure
        DataTable table = dal_entry.GetTransactionList("4568099023246532");
    }
}
```

Without model structures it will look like

```c#
using (SqlConnection conn = new SqlConnection(Connection_String)) {
    using (test.DAL.Entry dal_entry = new test.DAL.Entry(conn)) {

        // Add Record
        int recordID = dal_entry.Add("4568099023246532");

        // Edit Record
        int result = dal_entry.Edit(recordID, "4568099023246532");

        // Get Record by its Id
        DataTable table = dal_entry.Get(11);
        if (table!null && table.Rows.Count > 0){
            DataRow row = table.Rows[0];
            Console.WriteLine(row["AccountNo"]);
        } else {
            Console.WriteLine("Record not found");
        }

        // Delete Record
        result = dal_entry.Delete(recordID);

        // Call custom stored procedure
        DataTable table = dal_entry.GetTransactionList("4568099023246532");
    }
}
```

As you might noticed with structures you will have to pass a model to `Add`, `Edit` models, but without model row data will be passed as parameters to method. Also when retrieving a record you will receive a structure instead of `DataRow` with model based. Each type has their own benefits and drawbacks and from my experience each suits well in different scenario. 


## How do I use it

To create a meaningful DAL few rules should be followed:
1. Name your procedures prefixed with table name followed by an underscore and then method name. For example stored procedure that adds a record to table entry should be named Entry_Add which then generated code will a method called Add in class named Entry.
1. If application cannot find a suitable table name to assign procedure to it, it will create a class using schema name of procedure as class name prefixed with underscore. For example if a procedure is called sp_ambiguous_procedure then a method named sp_ambiguous_procedure will be created within _dbo class (default schema is dbo).
1. Schema name of table, procedure will be used as part of namespace of class. For example if Entry table is in Finance schema, it will be created as Entry class under namespace namespace_prefix.Entry. 
1. A namespace prefix is always required, if it is not specified database name will be used.

### Running from windows explorer

- If you run it from windows, it will show a GUI and allow you to adjust your settings. These settings will be remembered for next use.

### Running from console, shell script, bat file

If you run it from console or shell script, GUI will not be shown, instead command line parameters will be used. Running from console without parameters will 
show the GUI.
Command line parameters are as following:

```
    --help, --?, -?, /?, ?
        Shows this help
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
    --nolog
        Do not save log file
    --withsolution
        Create solution file too. if exists, will not be overwritten
    --omitdbo
        Do not add dbo schema to namespace
```

**Note:** Running the app from console you will see the result of generation. Due to complications of having an app that can run on both console and windows (output cannot be captured easily), a log file is saved which contains output of application. Console capture will not work since Microsoft didn't make it easy to have both GUI and console in same program even though it was asked frequently since about 2000


After completion, a project will be created in selected output folder which will include classes generated for the DAL. You can add the project directly to your project or add its output as reference.

**Note:** if the project file exists in output, it will not be overwritten to avoid issues with customized project content. This will cause classes generated for tables added after project was created to not add to project. Developer is required to add them manually to project or delete project file before running the app.

## Things to consider 
If you are planning to use this app you will need to consider 
1. Generating without model will not create CRUD methods and simply will use existing procedures to create DAL, thus you will need to create required procedures. To create procedures you can use the other 2 tools in this toolset (Procedure Creator and Writer). This is to reduce clutter and increase security of database by creating a class with exactly what is required.
1. Database name, table, column and procedure names that are not valid in C# programming language will cause your generated code not to compile, so use sensible names. For example if name of a column or table is `class` or 'if', it will cause compile error. 
1. If a table doesn't have primary key even if it has usable unique index, will not have an Edit or Delete generated for it.
1. List and Search (e.g. List by Parent Id) are essential methods when accessing data in database, however it is too complex to create methods for them. As a workaround create a procedure with desired filtering parameters which a method will be created for it. Also no model will be created for them as recordset might be different based on the parameters passed to procedure, instead a table (`DataTable`) with rows and columns will be returned.
1. While generating code with model, if you need special Add, Edit, Delete and Get methods, create stored procedures named the same and they will override default generated methods, however you will lose the ability to use entities for passing parameters in input or output.
1. This application uses DeriveParameters method (System.Data.SqlClient or Microsoft.Data.SqlClient). If this method fails to determine output of the stored procedure or throws exception, a throwing method will be created. In such scenario as if you insist in having procedure like that and also need to use that procedure, inherit the class in a secondary class and override that specific method.
1. DeriveParameters fails when a procedure use temp tables, if possible use table variables instead.

## What is not supported
Following features are not supported in this free version.
1. Geography, Geometry, HierarchyId columns are not supported. They do not exist in System.Data.SqlTypes shipped with .NET framework and require a Microsoft feature pack that is shipped with SQL Server. To avoid copyright issues I will not support it with this free version.
1. This free version uses SQL type variables (`SqlString`, `SqlInt32` ...) and nullable variables are not supported (String?,  Int? ...). to pass null to parameters, you can use SqlString.Null, SqlInt32.Null, etc.
1. Only first recordset will be returned from each procedure, multi recordset procedures will also return `DataTable` (no `Dataset`).
1. Transactions are not supported.
1. Only Microsoft SQL Server is supported (There are versions for other databases and other languages too).
