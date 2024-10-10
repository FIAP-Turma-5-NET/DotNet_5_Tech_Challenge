CREATE DATABASE IF NOT EXISTS FIAPContato;

USE FIAPContato;

CREATE TABLE IF NOT EXISTS Contato (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    DDD VARCHAR(2) NOT NULL,
    Telefone VARCHAR(15) NOT NULL,
    Email VARCHAR(100) NOT NULL
);

INSERT INTO Contato (Nome, DDD, Telefone, Email)
VALUES ('Fulano Fiap de Souza', '11', '91234-5678', 'fulanofifi@email.com');