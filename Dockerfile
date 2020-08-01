FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-stage

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
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
COPY --from=build-stage /artifacts /tasker

WORKDIR /tasker

ENTRYPOINT ["dotnet", "Tasker.dll"]