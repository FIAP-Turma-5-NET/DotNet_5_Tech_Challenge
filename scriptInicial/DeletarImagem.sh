#!bin/sh

# Configura o ambiente do Docker para utilizar o minikube 
eval $(minikube -p minikube docker-env)

echo "Deletando a imagem da API"
docker rmi fiap/contato-api:1.0.0

echo "Deletando a imagem do Worker"
docker rmi fiap/fiap_contato_worker:1.0.0