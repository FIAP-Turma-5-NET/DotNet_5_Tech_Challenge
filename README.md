# FIAP_Contato

<h1  align="center">Turma - 5  .NET - Tech Challenge </h1>

<h3  align="center">Contato</h3>

# Sumário

<!--ts-->

- [Projeto no Docker (Windows)](#projeto-no-docker-windows)

- [Configurar DBeaver](#configurar-dbeaver)

- [Acessar API, Prometheus e Grafana](#acessar-api-prometheus-e-grafana)

- [Projeto Kubernetes (Windows)](#projeto-kubernetes-windows)

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

👉 No browser acesse a API pela a url: http://localhost:7109/swagger/index.html

👉 No browser acesse a Prometheus pela a url: http://localhost:9090/

👉 No browser acesse a Grafana pela a url: http://localhost:3000/

---

# Projeto Kubernetes (Windows)

> **⚠️ Importante:** 
> - Substitua `seu-usuario` pelo seu nome de usuário do Windows
> - Substitua `seu-usuario-dockerhub` pelo seu nome de usuário do Docker Hub
> - Ajuste os caminhos de diretório conforme sua estrutura de pastas

## Pré-requisitos

### 1. Instalação do Chocolatey
O Chocolatey é um gerenciador de pacotes para Windows que facilita a instalação de ferramentas.

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

# Verificar instalação
choco -v
```

### 2. Instalação do Kind
O Kind permite criar clusters Kubernetes localmente usando containers Docker.

```powershell
# Instalar Kind
choco install kind -y

# Verificar instalação
kind version
```

### 3. Criar Cluster Kubernetes
```powershell
# Criar cluster
kind create cluster --name fiap-net

# Verificar cluster
kubectl cluster-info
kubectl get nodes
```

## Preparação das Imagens Docker

### 1. Construir as Imagens
```powershell
# Navegue até o diretório do projeto
cd "C:\Caminho\Para\Seu\Projeto"  # ⚠️ AJUSTE ESTE CAMINHO

# Build das imagens (substitua 'seu-usuario-dockerhub' pelo seu usuário do Docker Hub)
docker build -t seu-usuario-dockerhub/fiap-contato-api:latest -f Dockerfile.api .
docker build -t seu-usuario-dockerhub/fiap-contato-consumer:latest -f Dockerfile.consumer .
```

### 2. Publicar as Imagens
```powershell
# Login no Docker Hub
docker login

# Push das imagens (substitua 'seu-usuario-dockerhub' pelo seu usuário do Docker Hub)
docker push seu-usuario-dockerhub/fiap-contato-api:latest
docker push seu-usuario-dockerhub/fiap-contato-consumer:latest
```

## Implantação no Kubernetes

### 1. Aplicar Configurações
```powershell
# Navegue até o diretório k8s do projeto
cd "C:\Caminho\Para\Seu\Projeto\k8s"  # ⚠️ AJUSTE ESTE CAMINHO

# ⚠️ IMPORTANTE: Antes de aplicar, verifique se as imagens nos arquivos yaml 
# estão com o nome correto do seu usuário do Docker Hub
kubectl apply -f .
```

### 2. Verificar Status
```powershell
# Verificar pods
kubectl get pods

# Verificar serviços
kubectl get services

# Monitorar logs
kubectl logs -f deployment/fiap-contato-api
```

## Acessar Serviços

Para acessar os serviços externamente, execute os seguintes comandos em terminais separados:

| Serviço | Comando | URL de Acesso |
|---------|---------|---------------|
| Prometheus | `kubectl port-forward service/prometheus-service 9090:9090` | http://localhost:9090 |
| Grafana | `kubectl port-forward service/grafana-service 3000:3000` | http://localhost:3000 |
| cAdvisor | `kubectl port-forward service/cadvisor-service 8080:8080` | http://localhost:8080 |
| API | `kubectl port-forward service/fiap-contato-api-service 7109:7109` | http://localhost:7109 |
| MySQL | `kubectl port-forward service/mysql-service 3306:3360` | Via Workbench |
| RabbitMQ | `kubectl port-forward service/rabbitmq-service 15672:15672` | http://localhost:15672 |

## Reiniciar Ambiente

Para recriar todos os recursos do zero:

```powershell
# Remover recursos existentes
kubectl delete deployment --all
kubectl delete service --all
kubectl delete pod --all
kubectl delete pvc --all
kubectl delete pv --all

# Verificar se tudo foi removido
kubectl get all

# Recriar recursos (ajuste o caminho conforme sua estrutura)
cd "C:\Caminho\Para\Seu\Projeto\k8s"  # ⚠️ AJUSTE ESTE CAMINHO
kubectl apply -f .

# Verificar status
kubectl get pods

# Acessar API
kubectl port-forward service/fiap-contato-api-service 7109:7109

# Monitorar logs do consumer
kubectl logs -f deployment/fiap-contato-consumer
```

> **📝 Notas:**
> 1. Certifique-se de ajustar todos os caminhos de diretório conforme sua estrutura de pastas
> 2. Substitua 'seu-usuario-dockerhub' pelo seu nome de usuário do Docker Hub em todos os comandos
> 3. Verifique se os arquivos yaml na pasta k8s estão usando o nome correto das suas imagens Docker

