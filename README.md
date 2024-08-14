# FIAP_Contato

<h1  align="center">Tech Challenge - Fase 1</h1>

<h3  align="center">Contato</h3>

# Sumário

<!--ts-->

-  [Imagens do docker](#imagens-docker)

-  [Executar Projeto](#executar-projeto)

<!--te-->

---

# Imagens Docker

⚠️Abra :

👉 Com o Docker desktop e o terminal aberto

```bash

# Digite o comando abaixo
	docker run -d --name mysql-fiap-contato -e MYSQL_ROOT_PASSWORD=202406 -p 3360:3306 mysql:8.0.32

```

---

## Executar Projeto

👉 Na pasta do projeto {CaminhoDoSeuProjeto}\FIAP.Contato\FIAP_Contato.Data\Scripts\CriacaoBase.sql:
```bash
    1 - Copie o script
    2 - Execute o script no DBeaver
    3 - Execute o Visual Studio

```
👉 No DBeaver acesso o banco Mysql opção url
	
 Url: jdbc:mysql://host.docker.internal:3360/mysql?allowPublicKeyRetrieval=true&useSSL=false
 Username: root
 Password: 202406 

👉 No browser acesse a API pela a url: https://localhost:7109/swagger/index.html

---