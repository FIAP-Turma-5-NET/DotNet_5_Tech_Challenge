# FIAP_Contato

<h1  align="center">Turma - 5  .NET - Tech Challenge </h1>

<h3  align="center">Contato</h3>

# Sumário

<!--ts-->

- [Projeto no Docker (Windows)](#projeto-no-docker-windows)

- [Configurar DBeaver](#configurar-dbeaver)

- [Acessar API, Prometheus e Grafana](#acessar-api-prometheus-e-grafana)

<!--te-->

---

# Projeto no Docker (Windows)

⚠️Abra :

👉 Abra o Docker desktop e o terminal

- No terminar entre no caminho que está o projeto. Exemplo:

```console
  cd C:\Workspaces\FIAP\DotNet_5_Tech_Challenge
```

- Verifique se o arquivo docker-compose.yml está no diretório

```console
 ls
```

- Execute o docker-compose

```console
 docker-compose up -d
```

---

## Configurar DBeaver

👉 No DBeaver acesso o banco Mysql opção url

```
Url: jdbc:mysql://host.docker.internal:3360/mysql?allowPublicKeyRetrieval=true&useSSL=false

Username: root
Password: 202406
```

## Acessar API, Prometheus e Grafana

👉 No browser acesse a API pela a url: https://localhost:7109/swagger/index.html

👉 No browser acesse a Prometheus pela a url: http://localhost:9090/

👉 No browser acesse a Grafana pela a url: http://localhost:3000/

---
