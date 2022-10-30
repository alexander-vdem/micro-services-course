# Micro service course
A follow-along implementation of Les Jackson's course on microservices found here: https://youtu.be/DgVjEo3OGBI

### Debugging tools
- An edtior of choice. While implementing the project [Visual Studio Code](https://code.visualstudio.com/) is being used.
- [Dotnet 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) is used to build/debug the .NET projects within the solution

## Installation

1. Install [Docker Desktop](https://www.docker.com/products/docker-desktop/)
2. [Enable Kubernetes](https://docs.docker.com/desktop/kubernetes/#enable-kubernetes) on your freshly installed Docker Desktop

3. Clone this repository and open a cmd/PowerShell terminal

4. Navigate to micro-services-course\PlatformService direcory
  
  - Build PlatformService docker image:
  ```sh
  docker build -t <docker user id>/platformservice:latest .
  ```
  - Push the docker image to dockerhub:
  ```sh
  docker push <docker user id>/platformservice:latest
  ```
  
5. Navigate to micro-services-course\CommandsService direcory
  - Build CommandsService docker image:
  ```sh
  docker build -t <docker user id>/commandservice:latest .
  ```
  - Push the docker image to dockerhub:
  ```sh
  docker push <docker user id>/commandservice:latest
  ```
6.  Navigate to micro-services-course\K8S direcory
  Revisit all .yaml files and substitute alexvdem with your docker docker user id
  - Apply PlatformService deployment
  ```sh
  kubectl apply -f .\platforms-depl.yaml
  ```
  
  - Apply NodePort service for PlatformService pod
  ```sh
  kubectl apply -f .\platforms-np-srv.yaml
  ```
  
  - Apply CommandsService deployment
  ```sh
  kubectl apply -f .\commands-depl.yaml
  ```
  
  - Apply deployment of Ingress nginx gateway 
  ```sh
  kubectl apply -f .\https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.4.0/deploy/static/provider/cloud/deploy.yaml
  ```
  
  - Insert entry in your hosts file 
  ```sh
  127.0.0.1 alexvdem-test
  ```
  
  - Apply gateway configuration deployment
  ```sh
  kubectl apply -f .\ingress-srv.yaml
  ```
  
  - Apply Persistent Volume Claim for MSSQL deployment
  ```sh
  kubectl apply -f local-pvc.yaml
  ```
  - Create Kubernetes secret for storing passowrd key-value pair to serve MSSQL
  ```sh
  kubectl create secret generic mssql --from-literal=SA_PASSWORD="pa55w0rd!"
  ```
  
  - Apply MSSQL container deployment alongside with it's cluster IP service and Loadbalancer service
  ```sh
  kubectl apply -f mssql-plat-depl.yamll
  ```
