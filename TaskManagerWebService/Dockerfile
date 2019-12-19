FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["MyFirstWebApp/MyFirstWebApp.csproj", "MyFirstWebApp/"]
RUN dotnet restore "MyFirstWebApp/MyFirstWebApp.csproj"
COPY . .
WORKDIR "/src/MyFirstWebApp"
RUN dotnet build "MyFirstWebApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MyFirstWebApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MyFirstWebApp.dll"]