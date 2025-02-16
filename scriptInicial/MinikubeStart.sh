#!bin/sh
echo "Iniciando o Minikube - Kubernertes versão 1.30.0"
# minikube delete
minikube start --driver docker --ports=8080:80,31000:31000,31001:31001,31002:31002,31003:31003,31004:31004 --extra-config=kubelet.housekeeping-interval=10s --kubernetes-version=v1.30.0