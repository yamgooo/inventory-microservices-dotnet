create table Products
(
    Id          uniqueidentifier default newid()      not null
        primary key,
    Name        nvarchar(200)                         not null,
    Description nvarchar(1000),
    Category    nvarchar(100)                         not null,
    ImageUrl    nvarchar(500),
    Price       decimal(18, 2)                        not null
        check ([Price] >= 0),
    Stock       int              default 0            not null
        check ([Stock] >= 0),
    CreatedAt   datetime2        default getutcdate() not null,
    UpdatedAt   datetime2        default getutcdate() not null,
    IsDeleted   bit              default 0            not null
)
go

create index IX_Products_IsDeleted
    on Products (IsDeleted)
go

create index IX_Products_Category
    on Products (Category)
go

CREATE TRIGGER TR_Products_UpdatedAt
    ON Products
    AFTER UPDATE
    AS
BEGIN
    UPDATE Products
    SET UpdatedAt = GETUTCDATE()
    WHERE Id IN (SELECT Id FROM inserted);
END;
go

create table Transactions
(
    Id                    uniqueidentifier default newid()      not null
        primary key,
    ProductId             uniqueidentifier                      not null
        constraint FK_Transactions_Products
            references Products
            on delete cascade,
    Type                  int              default 1            not null,
    Quantity              int                                   not null
        check ([Quantity] > 0),
    UnitPrice             decimal(18, 2)                        not null
        check ([UnitPrice] >= 0),
    TotalPrice            as [Quantity] * [UnitPrice],
    Details               nvarchar(500),
    TransactionDate       datetime2        default getutcdate() not null,
    CreatedAt             datetime2        default getutcdate() not null,
    StockAfterTransaction int              default 0            not null
)
go

create index IX_Transactions_Date
    on Transactions (TransactionDate desc)
go

create index IX_Transactions_ProductId
    on Transactions (ProductId)
go

