# gRPC Streaming with Dotnet and Docker

In this workshop we will create a grpc streaming server, test it with a gRPC GUI client and then write a console app that can connect to a gRPC streaming service.

This will be helpful for anyone who is just getting started with gRPC in a new project.

### Prerequisites

- **Shell** 

  All commands are written for bash on MacOS, but you can use them in a **GitBash** or **Powershell**. We have used CLI instead of Visual Studio as it gives better view of what is going on underneath.

- **DotNet Core 3.1**

  We have used 3.1 as DotNet 5 is still in beta. Though everything should work the same with 5. If it does not, please help us all by reporting it.

- **BloomRPC**

  Download from here : https://appimage.github.io/BloomRPC/. We use it to tests our gRPC server, before we start writting a client ourselves.

- **Docker**

  

# Part 1  - Create a service

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



**Create a file `Service/Dockerfile`**

```dockerfile
# Service/Dockerfile

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

COPY ./*.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app

COPY --from=build /app .

EXPOSE 80  
ENTRYPOINT ["dotnet", "Service.dll"]
```

Create another file `Service/.dockerignore` to tell docker to ignore files that we do not want to pack: 

```bash
# Service/.dockerignore

**/.dockerignore
**/.project
**/.vs
**/.idea
**/.vscode
**/*.*proj.user
**/bin
**/Dockerfile*
**/obj
```



**Running service as docker container**: 

```bash
# Build docker image
docker build -t server .

# Run service as container
docker run -p 5000:80 server
```

Before we commit, open and check http://localhost:5000/weatherforecast to make sure our docker configurations are working as expected.

```
git add --all
git commit -m "Added dockerfile for service"
```



# Part 2 - Adding gRPC to a webservice

Now that we have a web service running as docker container, we will add grpc capability to it.



Add the nuget to support gRPC in our project

```bash
dotnet add package Grpc.AspNetCore 
```



### Define our gRPC interface in a protobuf file. 

This file is a language agnostic definition of a gRPC interface. For e.g. if we were to create a client in NodeJS, Java, Ruby or Python, we will still use the same proto files as we create here. For now just think of it as special way of defining class and interface.

**So lets create a proto file for our interface `Service/Protos/hello.proto` :** 

```protobuf
// Service/Proto/hello.proto

syntax = "proto3";

// Classes will be generated in this namespace
option csharp_namespace = "Service.Grpc";

// This will generate class MyService.MyServiceBase for us
service MyService {
  rpc SayHello (HelloRequest) returns (HelloResponse) {}
}

// Request will have a string field called name
message HelloRequest {
  string name = 1;
}

// HelloMessage will have a string field called message
message HelloResponse {
  string message = 1;
}
```

 Now add the proto file to our project in `Service/Service.csproj`

```xml
<ItemGroup>
 <Protobuf Include="Protos\hello.proto" GrpcServices="Server" />
</ItemGroup>
```

Now configure gprc in our app in `Service/Startup.cs`

```diff
 public void ConfigureServices(IServiceCollection services)
 {
 	 services.AddControllers();
+  services.AddGrpc();
 }
```



Create our gRPC service. Think of this as controller for gRPC endpoint.

```csharp
using System.Threading.Tasks;
using Grpc.Core;
using Service.Grpc;

namespace Service
{
  public class HelloService : Grpc.MyService.MyServiceBase
  {
    public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
    {
      var response = new HelloResponse();
      response.Message = "Hello " + request.Name;
      return Task.FromResult(response);
    }
  }
}
```



**Where did the class `Service.Grpc.MyService.MyServiceBase` come from ?**

This class was autogenrated by the nuget `Grpc.AspNetCore` we added earlier. It was created in namespace `Service.Grpc` as in hour `hello.proto` we mentioned : 

```protobuf
// Classes are created in namespace Service.Grpc because of this: 
option csharp_namespace = "Service.Grpc";
```

The name of the class is `MyService.MyServiceBase`  as we have named our service in `hello.proto` as : 

```protobuf
// MyService.MyServiceBase is created because of this
service MyService {
  rpc SayHello (HelloRequest) returns (HelloResponse) {}
}
```



Now final touch, we will add an endpoint for our grpc service in `Service/Startup.cs`

```diff
  app.UseEndpoints(endpoints =>
  {
    endpoints.MapControllers();
+   endpoints.MapGrpcService<HelloService>();
  });
```



Lets build our docker image and run app to check if everything is working fine

```bash
# Build docker image
docker build -t server .

# Run service as container
docker run -p 5000:80 server
```

