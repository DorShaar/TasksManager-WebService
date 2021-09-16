FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build-stage

WORKDIR /build

#Copy sources.
COPY ./Tasker ./Tasker
COPY ./Tasker.Tests ./Tasker.Tests
COPY ./nuget.config ./nuget.config

#Restore nugets
RUN dotnet restore ./Tasker/Tasker.csproj

#Build.
RUN dotnet build ./Tasker/Tasker.csproj \
	-c Release \
	--no-restore

#Test.
RUN dotnet test ./Tasker.Tests/Tasker.Tests.csproj \
    -c Release \
    -r /TestResults

#Publish.
RUN dotnet publish ./Tasker/Tasker.csproj \
	-c Release \
	-o /artifacts

#Create image from compiled project.
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
COPY --from=build-stage /artifacts /app

WORKDIR /app

ENTRYPOINT ["dotnet", "Tasker.dll"]