CREATE TABLE [dbo].[Owner] (
    [IdOwner]  UNIQUEIDENTIFIER NOT NULL,
    [Name]     VARCHAR (100)    NOT NULL,
    [Address]   VARCHAR(100)  NOT NULL,
    [Photo]    IMAGE            NULL,
    [Birthday] DATE             NOT NULL,
    CONSTRAINT [PK_Owner] PRIMARY KEY CLUSTERED ([IdOwner] ASC)
);

