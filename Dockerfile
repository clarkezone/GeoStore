FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG TARGETARCH

# in the image
WORKDIR /source

# copy csproj and restore as distinct layers
COPY /src/GeoStore.sln .
# dest folders aren't created automatically
RUN mkdir /src/GeoStore.Core
RUN mkdir GeoStore.Service
RUN mkdir GeoStore.Tests
COPY GeoStore.Core/*.csproj GeoStore.Core/.
COPY GeoStore.Service/*.csproj GeoStore.Service/. 
COPY GeoStore.Tests/*.csproj GeoStore.Tests/.
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