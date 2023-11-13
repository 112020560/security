#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Presentation/Akros.Authorizer.WebApi/Akros.Authorizer.WebApi.csproj", "src/Presentation/Akros.Authorizer.WebApi/"]
RUN dotnet restore "src/Presentation/Akros.Authorizer.WebApi/Akros.Authorizer.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Presentation/Akros.Authorizer.WebApi"
RUN dotnet build "Akros.Authorizer.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Akros.Authorizer.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Akros.Authorizer.WebApi.dll"]