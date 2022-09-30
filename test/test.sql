DROP TABLE IF EXISTS GenericTreeTable
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Stores Generic Tree data
  Created     : 2022-09-27 Bob
------------------------------------------------------------------------------------------------------ */
CREATE TABLE GenericTreeTable (
    Id          int IDENTITY(1,1) NOT NULL,
    ParentId    int,
    Name        varchar(50),
    Description varchar(50),
    CONSTRAINT  PK_GenericTreeTable PRIMARY KEY (Id ASC)
)
GO

-- Table Comments ---------------------------------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'GenericTreeTable', @value=N'Stores Generic Tree data'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'GenericTreeTable', @level2type=N'COLUMN', @level2name=N'Id', @value=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'GenericTreeTable', @level2type=N'COLUMN', @level2name=N'ParentId', @value=N'Id of Parent Record'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'GenericTreeTable', @level2type=N'COLUMN', @level2name=N'Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'GenericTreeTable', @level2type=N'COLUMN', @level2name=N'Description', @value=N'Description'
GO

INSERT INTO GenericTreeTable (ParentId,Name,Description)
SELECT 1, 'Name1', 'Description1' UNION
SELECT 1, 'Name2', 'Description2' UNION
SELECT 2, 'Name3', 'Description3' UNION
SELECT 2, 'Name4', 'Description4' UNION
SELECT 3, 'Name5', 'Description5' UNION
SELECT 3, 'Name6', 'Description6' UNION
SELECT 4, 'Name7', 'Description7' UNION
SELECT 5, 'Name8', 'Description8' UNION
SELECT 6, 'Name9', 'Description9'


DROP PROCEDURE IF EXISTS GenericTreeTable_Add
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Adds a record to GenericTreeTable table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE GenericTreeTable_Add (
    @ParentId    int,
    @Name        varchar(50),
    @Description varchar(50)
) AS

    INSERT INTO GenericTreeTable (
        ParentId, Name, Description
    ) VALUES (
        @ParentId, @Name, @Description
    )

    RETURN SCOPE_IDENTITY()

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Add', @value=N'Adds a record to GenericTreeTable table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Add', @level2type=N'PARAMETER', @level2name=N'@ParentId', @value=N'Id of Parent Record'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Add', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Add', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS GenericTreeTable_Delete
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Deletes a record from GenericTreeTable table. Optionally a trigger will move 
                record to History table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE GenericTreeTable_Delete (
    @Id int
) AS

    DELETE FROM
        GenericTreeTable
    WHERE
        Id = @Id

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Delete', @value=N'Deletes a record from GenericTreeTable table. Optionally a trigger will move record to History table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Delete', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
GO

DROP PROCEDURE IF EXISTS GenericTreeTable_Edit
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Edits a record in GenericTreeTable table. Optionally a trigger will copy old 
                record to History table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE GenericTreeTable_Edit (
    @Id          int,
    @ParentId    int,
    @Name        varchar(50),
    @Description varchar(50)
) AS

    UPDATE GenericTreeTable SET
        ParentId    = @ParentId,
        Name        = @Name,
        Description = @Description
    WHERE
        Id = @Id

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Edit', @value=N'Edits a record in GenericTreeTable table. Optionally a trigger will copy old record to History table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Edit', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Edit', @level2type=N'PARAMETER', @level2name=N'@ParentId', @value=N'Id of Parent Record'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Edit', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Edit', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS GenericTreeTable_Get
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Gets a record from GenericTreeTable table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE GenericTreeTable_Get (
    @Id int
) AS

    SELECT * FROM GenericTreeTable WHERE Id = @Id 

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Get', @value=N'Gets a record from GenericTreeTable table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Get', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
GO

DROP PROCEDURE IF EXISTS GenericTreeTable_ListByParentId
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Lists GenericTreeTable records by their parent ID
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE GenericTreeTable_ListByParentId (
    @ParentId int
) AS

    SELECT *
    FROM GenericTreeTable
    WHERE 
        ParentId = @ParentId
    -- ORDER BY

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_ListByParentId', @value=N'Lists GenericTreeTable records by their parent ID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_ListByParentId', @level2type=N'PARAMETER', @level2name=N'@ParentId', @value=N''
GO

DROP PROCEDURE IF EXISTS GenericTreeTable_Search
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Lists GenericTreeTable records by their parent ID
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE GenericTreeTable_Search (
    @Name        varchar(50),
    @Description varchar(50)
) AS

    SELECT *
    FROM GenericTreeTable
    WHERE 
        (@Name        IS NULL OR Name         Like @Name) AND 
        (@Description IS NULL OR @Description Like @Name)
    -- ORDER BY

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Search', @value=N'Lists GenericTreeTable records by their parent ID'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Search', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N''
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'GenericTreeTable_Search', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N''
GO

DROP TABLE IF EXISTS TableWithDoubleKey
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Stores Generic Lookup data with dual key
  Created     : 2022-09-27 Bob
------------------------------------------------------------------------------------------------------ */
CREATE TABLE TableWithDoubleKey (
    PkId1       int IDENTITY(1,1) NOT NULL,
    PkId2       int NOT NULL,
    Name        varchar(50),
    Description varchar(250),
    CONSTRAINT  PK_TableWithDoubleKey PRIMARY KEY (PkId1 ASC, PkId2 ASC)
)
GO

-- Table Comments ---------------------------------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithDoubleKey', @value=N'Stores Generic Lookup data with dual key'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithDoubleKey', @level2type=N'COLUMN', @level2name=N'PkId1', @value=N'First Primary key'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithDoubleKey', @level2type=N'COLUMN', @level2name=N'PkId2', @value=N'Second Primary key'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithDoubleKey', @level2type=N'COLUMN', @level2name=N'Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithDoubleKey', @level2type=N'COLUMN', @level2name=N'Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS TableWithDoubleKey_Add
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Adds a record to TableWithDoubleKey table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithDoubleKey_Add (
    @PkId2       int,
    @Name        varchar(50),
    @Description varchar(250)
) AS

    INSERT INTO TableWithDoubleKey (
        PkId2, Name, Description
    ) VALUES (
        @PkId2, @Name, @Description
    )

    RETURN SCOPE_IDENTITY()

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Add', @value=N'Adds a record to TableWithDoubleKey table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Add', @level2type=N'PARAMETER', @level2name=N'@PkId2', @value=N''
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Add', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N''
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Add', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N''
GO

DROP PROCEDURE IF EXISTS TableWithDoubleKey_Delete
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Deletes a record from TableWithDoubleKey table. Optionally a trigger will move 
                record to History table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithDoubleKey_Delete (
    @PkId1 int,
    @PkId2 int
) AS

    DELETE FROM
        TableWithDoubleKey
    WHERE
        PkId1 = @PkId1 AND
        PkId2 = @PkId2

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Delete', @value=N'Deletes a record from TableWithDoubleKey table. Optionally a trigger will move record to History table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Delete', @level2type=N'PARAMETER', @level2name=N'@PkId1', @value=N'This is pkid of the row'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Delete', @level2type=N'PARAMETER', @level2name=N'@PkId2', @value=N''
GO

DROP PROCEDURE IF EXISTS TableWithDoubleKey_Edit
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Edits a record in TableWithDoubleKey table. Optionally a trigger will copy old 
                record to History table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithDoubleKey_Edit (
    @PkId1       int,
    @PkId2       int,
    @Name        varchar(50),
    @Description varchar(250)
) AS

    UPDATE TableWithDoubleKey SET
        Name        = @Name,
        Description = @Description
    WHERE
        PkId1 = @PkId1 AND
        PkId2 = @PkId2

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Edit', @value=N'Edits a record in TableWithDoubleKey table. Optionally a trigger will copy old record to History table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Edit', @level2type=N'PARAMETER', @level2name=N'@PkId1', @value=N'This is pkid of the row'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Edit', @level2type=N'PARAMETER', @level2name=N'@PkId2', @value=N''
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Edit', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N''
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Edit', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N''
GO

DROP PROCEDURE IF EXISTS TableWithDoubleKey_Get
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Gets a record from TableWithDoubleKey table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithDoubleKey_Get (
    @PkId1 int,
    @PkId2 int
) AS

    SELECT * FROM TableWithDoubleKey WHERE PkId1 = @PkId1 AND PkId2 = @PkId2 

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Get', @value=N'Gets a record from TableWithDoubleKey table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Get', @level2type=N'PARAMETER', @level2name=N'@PkId1', @value=N'This is pkid of the row'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_Get', @level2type=N'PARAMETER', @level2name=N'@PkId2', @value=N''
GO

DROP PROCEDURE IF EXISTS TableWithDoubleKey_List
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Lists records from TableWithDoubleKey table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithDoubleKey_List AS

    SELECT *
    FROM TableWithDoubleKey
    -- WHERE 
    -- ORDER BY

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithDoubleKey_List', @value=N'Lists records from TableWithDoubleKey table'
GO

DROP TABLE IF EXISTS TableWithGUIDKey
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Stores typical lookup table data
  Created     : 2022-09-27 Bob
------------------------------------------------------------------------------------------------------ */
CREATE TABLE TableWithGUIDKey (
    Id          uniqueidentifier NOT NULL,
    Name        varchar(50),
    Description varchar(50),
    CONSTRAINT  PK_TableWithGUIDKey PRIMARY KEY (Id ASC)
)
GO

-- Table Comments ---------------------------------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithGUIDKey', @value=N'Stores typical lookup table data'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithGUIDKey', @level2type=N'COLUMN', @level2name=N'Id', @value=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithGUIDKey', @level2type=N'COLUMN', @level2name=N'Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithGUIDKey', @level2type=N'COLUMN', @level2name=N'Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS TableWithGUIDKey_Add
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Adds a record to TableWithGUIDKey table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithGUIDKey_Add (
    @Id          uniqueidentifier,
    @Name        varchar(50),
    @Description varchar(50)
) AS

    INSERT INTO TableWithGUIDKey (
        Id, Name, Description
    ) VALUES (
        @Id, @Name, @Description
    )

    RETURN SCOPE_IDENTITY()

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Add', @value=N'Adds a record to TableWithGUIDKey table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Add', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Add', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Add', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS TableWithGUIDKey_Delete
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Deletes a record from TableWithGUIDKey table. Optionally a trigger will move 
                record to History table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithGUIDKey_Delete (
    @Id uniqueidentifier
) AS

    DELETE FROM
        TableWithGUIDKey
    WHERE
        Id = @Id

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Delete', @value=N'Deletes a record from TableWithGUIDKey table. Optionally a trigger will move record to History table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Delete', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
GO

DROP PROCEDURE IF EXISTS TableWithGUIDKey_Edit
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Edits a record in TableWithGUIDKey table. Optionally a trigger will copy old 
                record to History table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithGUIDKey_Edit (
    @Id          uniqueidentifier,
    @Name        varchar(50),
    @Description varchar(50)
) AS

    UPDATE TableWithGUIDKey SET
        Name        = @Name,
        Description = @Description
    WHERE
        Id = @Id

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Edit', @value=N'Edits a record in TableWithGUIDKey table. Optionally a trigger will copy old record to History table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Edit', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Edit', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Edit', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS TableWithGUIDKey_Get
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Gets a record from TableWithGUIDKey table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithGUIDKey_Get (
    @Id uniqueidentifier
) AS

    SELECT * FROM TableWithGUIDKey WHERE Id = @Id 

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Get', @value=N'Gets a record from TableWithGUIDKey table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_Get', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
GO

DROP PROCEDURE IF EXISTS TableWithGUIDKey_List
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Lists records from TableWithGUIDKey table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithGUIDKey_List AS

    SELECT *
    FROM TableWithGUIDKey
    -- WHERE 
    -- ORDER BY

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithGUIDKey_List', @value=N'Lists records from TableWithGUIDKey table'
GO

DROP TABLE IF EXISTS TableWithKey
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Stores typical lookup table data
  Created     : 2022-09-27 Bob
------------------------------------------------------------------------------------------------------ */
CREATE TABLE TableWithKey (
    Id          int IDENTITY(1,1) NOT NULL,
    Name        varchar(50),
    Description varchar(50),
    CONSTRAINT  PK_TableWithKey PRIMARY KEY (Id ASC)
)
GO

-- Table Comments ---------------------------------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithKey', @value=N'Stores typical lookup table data'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithKey', @level2type=N'COLUMN', @level2name=N'Id', @value=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithKey', @level2type=N'COLUMN', @level2name=N'Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithKey', @level2type=N'COLUMN', @level2name=N'Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS TableWithKey_AddWithProc
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Adds a record to TableWithKey table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithKey_AddWithProc (
    @Name        varchar(50),
    @Description varchar(50)
) AS

    INSERT INTO TableWithKey (
        Name, Description
    ) VALUES (
        @Name, @Description
    )

    RETURN SCOPE_IDENTITY()

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_AddWithProc', @value=N'Adds a record to TableWithKey table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_AddWithProc', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_AddWithProc', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS TableWithKey_DeleteWithProc
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Deletes a record from TableWithKey table. Optionally a trigger will move record to 
                History table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithKey_DeleteWithProc (
    @Id int
) AS

    DELETE FROM
        TableWithKey
    WHERE
        Id = @Id

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_DeleteWithProc', @value=N'Deletes a record from TableWithKey table. Optionally a trigger will move record to History table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_DeleteWithProc', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
GO

DROP PROCEDURE IF EXISTS TableWithKey_EditWithProc
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Edits a record in TableWithKey table. Optionally a trigger will copy old record to 
                History table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithKey_EditWithProc (
    @Id          int,
    @Name        varchar(50),
    @Description varchar(50)
) AS

    UPDATE TableWithKey SET
        Name        = @Name,
        Description = @Description
    WHERE
        Id = @Id

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_EditWithProc', @value=N'Edits a record in TableWithKey table. Optionally a trigger will copy old record to History table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_EditWithProc', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_EditWithProc', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_EditWithProc', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS TableWithKey_GetWithProc
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Gets a record from TableWithKey table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithKey_GetWithProc (
    @Id int
) AS

    SELECT * FROM TableWithKey WHERE Id = @Id 

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_GetWithProc', @value=N'Gets a record from TableWithKey table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_GetWithProc', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
GO

DROP PROCEDURE IF EXISTS TableWithKey_List
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Lists records from TableWithKey table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithKey_List AS

    SELECT *
    FROM TableWithKey
    -- WHERE 
    -- ORDER BY

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithKey_List', @value=N'Lists records from TableWithKey table'
GO

DROP TABLE IF EXISTS TableWithoutKey
GO
/* ------------------------------------------------------------------------------------------------------
  Description : This is representing a table that does not have key even though it has identity and unique 
                index
  Created     : 2022-09-27 Bob
------------------------------------------------------------------------------------------------------ */
CREATE TABLE TableWithoutKey (
    Id          int IDENTITY(1,1) NOT NULL,
    Name        varchar(50),
    Description varchar(50)
)
GO

-- Indexes ----------------------------------------------------------------------------------------------
CREATE UNIQUE INDEX IX_TableWithoutKey ON TableWithoutKey (Id ASC)
GO

-- Table Comments ---------------------------------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithoutKey', @value=N'This is representing a table that does not have key even though it has identity and unique index'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithoutKey', @level2type=N'COLUMN', @level2name=N'Id', @value=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithoutKey', @level2type=N'COLUMN', @level2name=N'Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'TableWithoutKey', @level2type=N'COLUMN', @level2name=N'Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS TableWithoutKey_Add
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Adds a record to TableWithoutKey table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithoutKey_Add (
    @Name        varchar(50),
    @Description varchar(50)
) AS

    INSERT INTO TableWithoutKey (
        Name, Description
    ) VALUES (
        @Name, @Description
    )

    RETURN SCOPE_IDENTITY()

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithoutKey_Add', @value=N'Adds a record to TableWithoutKey table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithoutKey_Add', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithoutKey_Add', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS TableWithoutKey_List
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Lists records from TableWithoutKey table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE TableWithoutKey_List AS

    SELECT *
    FROM TableWithoutKey
    -- WHERE 
    -- ORDER BY

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TableWithoutKey_List', @value=N'Lists records from TableWithoutKey table'
GO

/* ------------------------------------------------------------------------------------------------------
  Description   : Create Lookups Schema
  Created       : 2022-09-27 Bob
------------------------------------------------------------------------------------------------------ */
IF NOT EXISTS ( SELECT * FROM sys.schemas WHERE name='Lookups' )
    EXEC sp_executesql N'CREATE SCHEMA Lookups'
GO

DROP TABLE IF EXISTS Lookups.TableWithSchema
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Stores typical lookup table data
  Created     : 2022-09-27 Bob
------------------------------------------------------------------------------------------------------ */
CREATE TABLE Lookups.TableWithSchema (
    Id          int IDENTITY(1,1) NOT NULL,
    Name        varchar(50),
    Description varchar(50),
    CONSTRAINT  PK_TableWithSchema PRIMARY KEY (Id ASC)
)
GO

-- Table Comments ---------------------------------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'TABLE', @level1name=N'TableWithSchema', @value=N'Stores typical lookup table data'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'TABLE', @level1name=N'TableWithSchema', @level2type=N'COLUMN', @level2name=N'Id', @value=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'TABLE', @level1name=N'TableWithSchema', @level2type=N'COLUMN', @level2name=N'Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'TABLE', @level1name=N'TableWithSchema', @level2type=N'COLUMN', @level2name=N'Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS Lookups.TableWithSchema_Add
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Adds a record to Lookups.TableWithSchema table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE Lookups.TableWithSchema_Add (
    @Name        varchar(50),
    @Description varchar(50)
) AS

    INSERT INTO Lookups.TableWithSchema (
        Name, Description
    ) VALUES (
        @Name, @Description
    )

    RETURN SCOPE_IDENTITY()

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'PROCEDURE', @level1name=N'TableWithSchema_Add', @value=N'Adds a record to Lookups.TableWithSchema table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'PROCEDURE', @level1name=N'TableWithSchema_Add', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'PROCEDURE', @level1name=N'TableWithSchema_Add', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS Lookups.TableWithSchema_Delete
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Deletes a record from Lookups.TableWithSchema table. Optionally a trigger will move 
                record to History table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE Lookups.TableWithSchema_Delete (
    @Id  int
) AS

    DELETE FROM
        Lookups.TableWithSchema
    WHERE
        Id = @Id

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'PROCEDURE', @level1name=N'TableWithSchema_Delete', @value=N'Deletes a record from Lookups.TableWithSchema table. Optionally a trigger will move record to History table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'PROCEDURE', @level1name=N'TableWithSchema_Delete', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
GO

DROP PROCEDURE IF EXISTS Lookups.TableWithSchema_Edit
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Edits a record in Lookups.TableWithSchema table. Optionally a trigger will copy old 
                record to History table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE Lookups.TableWithSchema_Edit (
    @Id          int,
    @Name        varchar(50),
    @Description varchar(50)
) AS

    UPDATE Lookups.TableWithSchema SET
        Name        = @Name,
        Description = @Description
    WHERE
        Id = @Id

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'PROCEDURE', @level1name=N'TableWithSchema_Edit', @value=N'Edits a record in Lookups.TableWithSchema table. Optionally a trigger will copy old record to History table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'PROCEDURE', @level1name=N'TableWithSchema_Edit', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'PROCEDURE', @level1name=N'TableWithSchema_Edit', @level2type=N'PARAMETER', @level2name=N'@Name', @value=N'Name'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'PROCEDURE', @level1name=N'TableWithSchema_Edit', @level2type=N'PARAMETER', @level2name=N'@Description', @value=N'Description'
GO

DROP PROCEDURE IF EXISTS Lookups.TableWithSchema_Get
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Gets a record from Lookups.TableWithSchema table
  Created     : 2022-09-26 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE Lookups.TableWithSchema_Get (
    @Id int
) AS

    SELECT * FROM Lookups.TableWithSchema WHERE Id = @Id 

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'PROCEDURE', @level1name=N'TableWithSchema_Get', @value=N'Gets a record from Lookups.TableWithSchema table'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'Lookups', @level1type=N'PROCEDURE', @level1name=N'TableWithSchema_Get', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N'Id'
GO

DROP PROCEDURE IF EXISTS Method_Without_Table
GO
/* ------------------------------------------------------------------------------------------------------
  Description : Returns a List of child and parent record
  Created     : 2022-08-19 Bob
------------------------------------------------------------------------------------------------------ */
CREATE PROCEDURE Method_Without_Table (
    @Id varchar(50)
) AS
    SELECT * FROM GenericTreeTable

    RETURN @@ROWCOUNT

GO

-- Stored Procedure and Parameter Description ---------------------------------------------------------------
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'Method_Without_Table', @value=N'Returns a List of child and parent record'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'Method_Without_Table', @level2type=N'PARAMETER', @level2name=N'@Id', @value=N''
GO

