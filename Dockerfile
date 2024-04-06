# Publish application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
USER 0
COPY --from=mcr.microsoft.com/dotnet/aspnet:8.0 /etc/passwd /etc/group /scratch/etc/
RUN grep ":0:" /scratch/etc/group || echo "app:x:0:" >>/scratch/etc/group && mkdir -p /rootfs/etc && cp /scratch/etc/group /rootfs/etc && \
    grep ":x:1001:" /scratch/etc/passwd || echo "app:x:1001:0::/home/app:/usr/sbin/nologin" >>/scratch/etc/passwd && mkdir -p /rootfs/etc && cp /scratch/etc/passwd /rootfs/etc
RUN mkdir -m 770 -p /rootfs/home/app
ENV HOME=/home/build
WORKDIR /src
COPY . ./
RUN dotnet restore ./src/GeoStore.Service/GeoStore.Service.csproj
RUN dotnet publish --no-restore -c Release -o /rootfs/app ./src/GeoStore.Service/GeoStore.Service.csproj
RUN chown -R 1001:0 /rootfs/app /rootfs/home/app && chmod -R g=u /rootfs/app /rootfs/home/app

# Build application image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY --from=build-env /rootfs /
USER 1001:0
ENV ASPNETCORE_URLS=http://*:8080 \
    HOME=/home/app
WORKDIR /app
ENTRYPOINT ["dotnet", "/app/GeoStore.Service.dll"]
