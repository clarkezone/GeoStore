FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG TARGETARCH

# in the image
WORKDIR /source

# copy csproj and restore as distinct layers
COPY GeoStore.sln .
# dest folders aren't created automatically
RUN mkdir /source/GeoStore.Core
RUN mkdir /source/GeoStore.Service
RUN mkdir /source/GeoStore.Tests
RUN mkdir /source/GeoStore.CosmosDB
COPY /src/GeoStore.Core/*.csproj GeoStore.Core/.
COPY /src/GeoStore.Service/*.csproj GeoStore.Service/. 
COPY /src/GeoStore.Tests/*.csproj GeoStore.Tests/.
COPY /src/GeoStore.CosmosDB/*.csproj GeoStore.CosmosDB/.
# verify things are where expected
#RUN ls -al
#RUN ls -al pocketnow.lib
RUN dotnet restore -a $TARGETARCH

# copy and publish app and libraries
COPY . ./
RUN dotnet publish -a $TARGETARCH --no-restore -o /app

# final stage/image
#FROM mcr.microsoft.com/dotnet/aspnet:8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled

WORKDIR /app
COPY --from=build /app .
USER $APP_UID

ENV ASPNETCORE_URLS=http://0.0.0.0:5000;
EXPOSE 5000
ENTRYPOINT ["dotnet", "GeoStore.dll"]