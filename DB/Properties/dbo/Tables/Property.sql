CREATE TABLE [dbo].[Property] (
    [IdProperty]   UNIQUEIDENTIFIER NOT NULL,
    [Name]         NVARCHAR (100)   NOT NULL,
    [Address]      NVARCHAR (100)   NOT NULL,
    [Price]        DECIMAL (12, 2)  NOT NULL,
    [CodeInternal] INT              NOT NULL,
    [Year]         SMALLINT         NOT NULL,
    [IdOwner]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Property] PRIMARY KEY CLUSTERED ([IdProperty] ASC),
    CONSTRAINT [FK_Property_Owner] FOREIGN KEY ([IdOwner]) REFERENCES [dbo].[Owner] ([IdOwner])
);

