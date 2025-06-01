FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:c149fe7e2be3baccf3cc91e9e6cdcca0ce70f7ca30d5f90796d983ff4f27bd9a
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "fake-dotnet-api.dll"]