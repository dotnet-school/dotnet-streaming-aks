# gRPC Streaming with Dotnet and Docker

In this tutorial we will create a grpc streaming server, test it with a gRPC GUI client and then write a console app that can connect to a gRPC streaming service.

This will be helpful for anyone who is just getting started with gRPC in a new project.

### Prerequisites

- **Shell** 

  All commands are written for bash on MacOS, but you can use them in a **GitBash** or **Powershell**. We have used CLI instead of Visual Studio as it gives better view of what is going on underneath.

- **DotNet Core 3.1**

  We have used 3.1 as DotNet 5 is still in beta. Though everything should work the same with 5. If it does not, please help us all by reporting it.

- **BloomRPC**

  Download from here : https://appimage.github.io/BloomRPC/. We use it to tests our gRPC server, before we start writting a client ourselves.

- **Docker**

  

# Part 1  - GRPC Server

To begin with, we will create a simple web service and then add grpc to it. We could straightaway create a grpc service with command `dotnet new grpc`, but we want to start from a web service so that we can better understand how to add grpc to an existing web service.



**Creating a webservice**

```bash
# Create a folder for our project
mkdir dotnet-docker-grpc-stream

# Create a new project names as Service
dotnet new webapi -o Service

# Generate a gitignore file
dotnet new gitignore

# Start our server
cd Service
dotnet run
```



Before commiting our code, check the url https://localhost:5001/weatherforecast to make sure our service is working fine.

```
git add --all
git commit -m "Created a web service"
```

