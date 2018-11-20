# Builder

FROM microsoft/dotnet:2.1-sdk AS builder
RUN mkdir /build
RUN mkdir /publish
WORKDIR /build
COPY ./src /build/src
COPY ./test /build/test
COPY ./WebApplication1.sln /build/
RUN dotnet restore ./WebApplication1.sln
RUN dotnet test ./test/CrowdSource.Test/CrowdSource.Test.csproj && \
    dotnet publish ./src/CrowdSource/CrowdSource.csproj -c Release -o /publish
# Dockerfile for production

FROM microsoft/dotnet:2.1.0-runtime
WORKDIR /app
COPY --from=builder /publish .
VOLUME /segments

ENTRYPOINT ["dotnet", "CrowdSource.dll"]
