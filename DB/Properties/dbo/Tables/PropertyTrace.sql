CREATE TABLE [dbo].[PropertyTrace] (
    [IdPropertyTrace] UNIQUEIDENTIFIER NOT NULL,
    [DateSale]        DATETIME         NOT NULL,
    [Name]            VARCHAR (100)    NOT NULL,
    [Value]           DECIMAL (12, 2)  NOT NULL,
    [Tax]             DECIMAL (12, 2)  NOT NULL,
    [IdProperty]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_PropertyTrace] PRIMARY KEY CLUSTERED ([IdPropertyTrace] ASC),
    CONSTRAINT [FK_PropertyTrace_Property] FOREIGN KEY ([IdProperty]) REFERENCES [dbo].[Property] ([IdProperty])
);

