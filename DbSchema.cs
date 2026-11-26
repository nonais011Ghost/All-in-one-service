namespace AIOSICT011.DataAccess
{
    // The database structure and starter data. Db.EnsureDatabaseCreated() runs this once, on the first launch.
    public static class DbSchema
    {
        public const string Script = @"
CREATE TABLE Regions (
    RegionID INT IDENTITY(1,1) PRIMARY KEY,
    Unit  NVARCHAR(50) NOT NULL,
    Ward  NVARCHAR(50) NOT NULL,
    Block NVARCHAR(50) NOT NULL
);
CREATE TABLE ServiceTypes (
    ServiceTypeID INT IDENTITY(1,1) PRIMARY KEY,
    TypeName NVARCHAR(100) NOT NULL UNIQUE
);
CREATE TABLE Customers (
    CustomerID   INT IDENTITY(1,1) PRIMARY KEY,
    MembershipID NVARCHAR(30)  NOT NULL UNIQUE,
    FullName     NVARCHAR(100) NOT NULL,
    Phone        NVARCHAR(20)  NOT NULL,
    Email        NVARCHAR(100) NOT NULL,
    Address      NVARCHAR(200) NULL,
    RegionID     INT NOT NULL REFERENCES Regions(RegionID)
);
CREATE TABLE ServiceProviders (
    ProviderID    INT IDENTITY(1,1) PRIMARY KEY,
    FullName      NVARCHAR(100) NOT NULL,
    Phone         NVARCHAR(20)  NOT NULL,
    Address       NVARCHAR(200) NULL,
    BusinessRegNo NVARCHAR(20)  NOT NULL UNIQUE,
    RegionID      INT NOT NULL REFERENCES Regions(RegionID),
    ServiceTypeID INT NOT NULL REFERENCES ServiceTypes(ServiceTypeID),
    HourlyRate    DECIMAL(18,2) NOT NULL DEFAULT 0,
    IsAvailable   BIT NOT NULL DEFAULT 1,
    IsBlacklisted BIT NOT NULL DEFAULT 0,
    RedFlags      INT NOT NULL DEFAULT 0,
    AvgRating     DECIMAL(3,2) NOT NULL DEFAULT 0
);
CREATE TABLE Users (
    UserID       INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(64)  NOT NULL,
    Role         NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin','Customer','Provider')),
    PersonID     INT NULL
);
CREATE TABLE Transactions (
    TransactionID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID    INT NOT NULL REFERENCES Customers(CustomerID),
    ProviderID    INT NOT NULL REFERENCES ServiceProviders(ProviderID),
    ServiceDate   DATETIME NOT NULL,
    HoursWorked   DECIMAL(18,2) NULL,
    Status        NVARCHAR(30) NOT NULL DEFAULT 'Requested'
                  CHECK (Status IN ('Requested','In Progress','Completed','Paid','Closed')),
    Amount        DECIMAL(18,2) NULL,
    InvoiceNo     NVARCHAR(30) NULL,
    CustomerPaid  BIT NOT NULL DEFAULT 0,
    ProviderPaid  BIT NOT NULL DEFAULT 0,
    Rating        INT NULL CHECK (Rating BETWEEN 1 AND 5),
    Feedback      NVARCHAR(300) NULL,
    RedFlag       BIT NOT NULL DEFAULT 0
);

INSERT INTO Regions (Unit, Ward, Block) VALUES
 ('Gaborone Central','Ward 1','Block 3'), ('Gaborone West','Ward 2','Block 8'), ('Gaborone North','Ward 3','Block 9');
INSERT INTO ServiceTypes (TypeName) VALUES
 ('Plumbing'), ('Electrical'), ('House Cleaning'), ('Gardening'), ('Babysitting');
INSERT INTO Customers (MembershipID, FullName, Phone, Email, Address, RegionID)
 VALUES ('AIOS-000001','Neo Customer','71234567','neo@example.com','Plot 100, Gaborone',1);
INSERT INTO ServiceProviders (FullName, Phone, Address, BusinessRegNo, RegionID, ServiceTypeID, HourlyRate, AvgRating)
 VALUES ('Mike Plumber','72345678','Plot 404, Block 3','BW-REG-0001',1,1,150.00,4.8),
        ('Kagiso Electrical','73456789','Plot 300, Block 8','BW-REG-0002',2,2,180.00,0);
-- Demo logins (passwords are stored as SHA-256 hashes): admin / admin123 ; neo and mike / password1
INSERT INTO Users (Username, PasswordHash, Role, PersonID) VALUES
 ('admin','240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9','Admin',NULL),
 ('neo','0b14d501a594442a01c6859541bcb3e8164d183d32937b851835442f69d5c94e','Customer',1),
 ('mike','0b14d501a594442a01c6859541bcb3e8164d183d32937b851835442f69d5c94e','Provider',1);
";
    }
}
