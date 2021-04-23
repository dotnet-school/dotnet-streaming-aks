FROM artifacts.cf.saxo/docker/base/saxo-dotnet-sdk:3.1-ubuntu-ci-1.0.23 AS build
WORKDIR /source

COPY ./*.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c release -o /app --no-restore

FROM artifacts.cf.saxo/docker/base/saxo-dotnet-aspnet:3.1-ubuntu-ci-1.0.23
WORKDIR /app

COPY --from=build /app .

EXPOSE 80  
ENTRYPOINT ["dotnet", "Service.dll"]
