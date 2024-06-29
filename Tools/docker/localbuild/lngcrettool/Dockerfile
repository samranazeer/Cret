FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt-get update && apt-get install -y libgdiplus
RUN mkdir -p /shared/database

WORKDIR /app

EXPOSE 443
EXPOSE 80 

# Publish source folder is created by pipeline
COPY ./publish .

VOLUME /shared/database

ENTRYPOINT ["dotnet", "CRET.Web.dll"]
