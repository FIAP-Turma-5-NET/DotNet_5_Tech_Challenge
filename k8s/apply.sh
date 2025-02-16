#!bin/sh
echo "Aplicando namespace"
kubectl apply -f namespace.yaml
sleep 1

echo "Aplicando secrets"
kubectl apply -f secrets.yaml
sleep 1

echo "Aplicando configmap"
kubectl apply -f configmap.yaml
sleep 2

echo "Aplicando Database statefulset"
kubectl apply -f DataBase//statefulset.yaml
sleep 1

echo "Aplicando Database service"
kubectl apply -f DataBase//service.yaml
sleep 1

echo "Aplicando RabbitMQ deployment"
kubectl apply -f Application//rabbitMQ-deployment.yaml
sleep 1

echo "Aplicando RabbitMQ service"
kubectl apply -f Application//rabbitMQ-service.yaml
sleep 4

echo "Aplicando API deployment"
kubectl apply -f Application//api-deployment.yaml
sleep 1

echo "Aplicando API service"
kubectl apply -f Application//api-service.yaml
sleep 1

echo "Aplicando Worker"
kubectl apply -f Application//worker-deployment.yaml
sleep 1

echo "Aplicando Prometheus configmap"
kubectl apply -f Monitoring//prometheus-configmap.yaml
sleep 1

echo "Aplicando Prometheus deployment"
kubectl apply -f Monitoring//prometheus-deployment.yaml
sleep 1

echo "Aplicando Prometheus service"
kubectl apply -f Monitoring//prometheus-service.yaml
sleep 1

echo "Aplicando Grafana configmap"
kubectl apply -f Monitoring//grafana-configmap.yaml
sleep 1

echo "Aplicando Grafana deployment"
kubectl apply -f Monitoring//grafana-deployment.yaml
sleep 1

echo "Aplicando Grafana service"
kubectl apply -f Monitoring//grafana-service.yaml
sleep 1