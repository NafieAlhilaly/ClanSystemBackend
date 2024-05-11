FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Install dependencies
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

ENTRYPOINT ["dotnet", "run"]