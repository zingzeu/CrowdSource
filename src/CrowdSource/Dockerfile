# Dockerfile for production

FROM microsoft/dotnet:1.1.0-runtime
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
VOLUME /segments

ENTRYPOINT ["dotnet", "CrowdSource.dll"]
