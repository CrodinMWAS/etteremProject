CREATE DATABASE restaurant DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_hungarian_ci;

CREATE TABLE Users (
    ID int AUTO_INCREMENT,
    name varchar(255),
    email varchar(255),
    password varchar(255),
    PRIMARY KEY(ID)
);

CREATE TABLE Dishes (
    dishID int AUTO_INCREMENT,
    dishName varchar(255),
    PRIMARY KEY(dishID)
);

CREATE TABLE Orders (
    orderID int AUTO_INCREMENT,
    userID int,
    dishID int,
    PRIMARY KEY (orderID),
    FOREIGN KEY (userID) REFERENCES Users(ID),
    FOREIGN KEY (dishID) REFERENCES Dishes(dishID)
);

CREATE TABLE Stock (
    stockID int AUTO_INCREMENT,
    name varchar(255),
    amount int,
    PRIMARY KEY (StockID)
);

CREATE TABLE Ingridients (
    dishID int,
    stockID int
)