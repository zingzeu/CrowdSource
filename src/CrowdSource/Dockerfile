# Dockerfile for production

FROM microsoft/dotnet:1.1.0-runtime

WORKDIR /app

COPY ./ .

VOLUME /app/wwwroot/segments

ENTRYPOINT ["dotnet","CrowdSource.dll"]
