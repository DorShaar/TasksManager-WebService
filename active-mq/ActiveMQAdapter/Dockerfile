FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-stage

WORKDIR /build

#Copy sources.
COPY ./ ./

#Restore nugets
RUN dotnet restore ./ActiveMQAdapter/ActiveMQAdapter.csproj
RUN dotnet restore ./ActiveMqAdapterTestConsumer/ActiveMqAdapterTestConsumer.csproj

#Build.
RUN dotnet build ./ActiveMQAdapter/ActiveMQAdapter.csproj \
	-c Release \
	--no-restore
	
RUN dotnet build ./ActiveMqAdapterTestConsumer/ActiveMqAdapterTestConsumer.csproj \
	-c Release \
	--no-restore

#Publish.
RUN dotnet publish ./ActiveMqAdapterTestConsumer/ActiveMqAdapterTestConsumer.csproj \
	-c Release \
	-o /artifacts

#Create image from compiled project.
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
COPY --from=build-stage /artifacts /tasker

WORKDIR /tasker

ENTRYPOINT ["dotnet", "ActiveMqAdapterTestConsumer.dll"]