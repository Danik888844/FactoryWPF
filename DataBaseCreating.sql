USE FactoryDB;

CREATE TABLE City
(
    CityID INT IDENTITY(1,1) PRIMARY KEY,
    CityName NVARCHAR(50) NOT NULL
);

CREATE TABLE Gild
(
    GildID INT IDENTITY(1,1) PRIMARY KEY,
    GildName NVARCHAR(50) NOT NULL
);

CREATE TABLE Employee
(
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(50) NOT NULL
);

CREATE TABLE CityGildEmployee
(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    CityID INT,
    GildID INT,
    EmployeeID INT
);

-- Добавление внешних ключей
ALTER TABLE CityGildEmployee
ADD CONSTRAINT FK_CityGildEmployee_City
FOREIGN KEY (CityID)
REFERENCES City(CityID);

ALTER TABLE CityGildEmployee
ADD CONSTRAINT FK_CityGildEmployee_Gild
FOREIGN KEY (GildID)
REFERENCES Gild(GildID);

ALTER TABLE CityGildEmployee
ADD CONSTRAINT FK_CityGildEmployee_Employee
FOREIGN KEY (EmployeeID)
REFERENCES Employee(EmployeeID);

CREATE TABLE BrigadeShift
(
    BrigadeShiftID INT IDENTITY(1,1) PRIMARY KEY,
    ShiftName NVARCHAR(255) NOT NULL
);

INSERT INTO BrigadeShift (ShiftName) VALUES ('Дневная');
INSERT INTO BrigadeShift (ShiftName) VALUES ('Ночная');

CREATE TABLE Brigade
(
    BrigadeID INT IDENTITY(1,1) PRIMARY KEY,
    BrigadeName NVARCHAR(255) NOT NULL
);

INSERT INTO Brigade (BrigadeName) VALUES ('Первая');
INSERT INTO Brigade (BrigadeName) VALUES ('Вторая');

CREATE TABLE Factory
(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    GildID INT,
    EmployeeID INT,
    CityID INT,
    BrigadeShiftID INT,
    BrigadeID INT,
    FOREIGN KEY (GildID) REFERENCES Gild(GildID) ON DELETE CASCADE,
    FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID) ON DELETE CASCADE,
    FOREIGN KEY (CityID) REFERENCES City(CityID) ON DELETE CASCADE,
	FOREIGN KEY (BrigadeShiftID) REFERENCES BrigadeShift(BrigadeShiftID),
    FOREIGN KEY (BrigadeID) REFERENCES Brigade(BrigadeID)
);
