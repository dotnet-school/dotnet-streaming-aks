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

  

### Step1 - Creating a gRPC service

```bash
# Create a folder for our project
mkdir dotnet-docker-grpc-stream

# Create a new project names as Service
dotnet new grpc -o Service

# Generate a gitignore file
dotnet new gitignore

# Start our server
cd Service
dotnet run

# Commit your code
git add --all
git commit -m "Created a web service"
```

>  If you are using MacOS, you will get error. Kestrel doesn't support HTTP/2 with TLS on macOS and older Windows versions such as Windows 7.
>
> You will be able to run the server after next step in which we will run service inside docker container.
>
> If you still want to fix this, read  https://docs.microsoft.com/en-us/aspnet/core/grpc/troubleshoot?view=aspnetcore-3.1#unable-to-start-aspnet-core-grpc-app-on-macos



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

Create another file `Service/.dockerignore` to tell docker which files to ignore

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

Now run the server as docker container: 

```bash
# Build docker image
docker build -t server .

# Run service as container
docker run -p 5000:80 server
```

Checkout a protobug created at `dotnet-docker-grpc-stream/Service/Protos/greep.proto`. We will use it in next step to test our server.

### Step 2 - Testing a gRPC endpoint 

Just like we can use postman to test an HTTP endpoint, we will use [BloomRPC](https://appimage.github.io/BloomRPC/.) to test our gRPC endpoint.

Download and open the app from [here](https://appimage.github.io/BloomRPC). 

![image-20201106215735673](/Users/dawn/projects/dotnet-school/dotnet-docker-grpc-stream/docs/images/open-bloom.png)



Now select the file from our project :  ``dotnet-docker-grpc-stream/Service/Protos/greep.proto`` 

![image-20201106220446319](/Users/dawn/projects/dotnet-school/dotnet-docker-grpc-stream/docs/images/load-proto-file.png)



Now enter the address for our gRPC server in address bar as `localhost:5000`

![image-20201106220446319](/Users/dawn/projects/dotnet-school/dotnet-docker-grpc-stream/docs/images/set-grpc-server-address.png)



Now click on the play button to test our endpoint.

![image-20201106220446319](/Users/dawn/projects/dotnet-school/dotnet-docker-grpc-stream/docs/images/run-first-test.png)

