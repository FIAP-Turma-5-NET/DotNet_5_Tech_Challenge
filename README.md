# FIAP_Contato

<h1  align="center">Turma - 5 .NET - Tech Challenge </h1>

<h3  align="center">Contato</h3>

# Sumário

<!--ts-->

- [Projeto no Docker (Windows)](#projeto-no-docker-windows)
- [Configurar DBeaver](#configurar-dbeaver)
- [Acessar API, Prometheus e Grafana Docker](#acessar-api-prometheus-e-grafana-docker)
- [Projeto no Kubernetes (Windows)](#projeto-no-kubernetes-windows)
- [Acessar API, Prometheus e Grafana Kubernetes (Minikube)](#acessar-api-prometheus-e-grafana-kubernetes-minikube)

## <!--te-->

# Projeto no Docker (Windows)

⚠️Abra :

👉 Abra o Docker desktop e o terminal

- No terminar entre no caminho que está o projeto. Exemplo:

```console
cd  C:\Workspaces\FIAP\DotNet_5_Tech_Challenge
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

- Docker:

Url: jdbc:mysql://host.docker.internal:3360/mysql?allowPublicKeyRetrieval=true&useSSL=false

- Kubernetes:

Server host: localhost Port: 31000

-------
Username: root
Password: 202406

```

---

## Acessar API, Prometheus e Grafana Docker

👉 No browser acesse a API pela a url: http://localhost:7109/swagger/index.html

👉 No browser acesse a Prometheus pela a url: http://localhost:9090/

👉 No browser acesse a Grafana pela a url: http://localhost:3000/

---

# Projeto no Kubernetes (Windows)

⚠️Abra :

👉 Abra o Docker desktop e o Visual Code com o código-font.

- No terminar Git Bash entre no caminho que estão o scripts

```console
cd scriptInicial
```

- Após entrar no caminho execute os scripts:

```console
sh MinikubeStart.sh
```

```console
sh CriarImagem.sh
```

- Minikube por Dasboard

```console
minikube dashboard
```

⚠️ Após terminar de subir as imagens

- Em um novo terminar Git Bash entre no caminho que está o K8S.

```console
cd  k8s
```

- Execute o script

```console
sh apply.sh
```

---

## Acessar API, Prometheus e Grafana Kubernetes (Minikube)

👉 No browser acesse a API pela a url: http://localhost:31001/swagger/index.html

👉 No browser acesse a RabbitMQ pela a url: http://localhost:31002/

👉 No browser acesse a Prometheus pela a url: http://localhost:31003/

👉 No browser acesse a Grafana pela a url: http://localhost:31004/

---
