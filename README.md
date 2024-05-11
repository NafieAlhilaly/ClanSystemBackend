# Clan System Backend

## Development Setup

### Prerequisites
Using Docker:
- Docker
- Docker Compose

Running locally on your machine:
you'll need to make sure you have
 - .NET sdk8.0
 - Mongodb v7 instance running.

### Setup
Setup Database URL and other options from `/Properties/launchSettings.json`

### Build and Run
- Build and start the Docker containers using Docker Compose:
    ```shell
    docker-compose up --build
    ```

- Locally using dotnet command directly
make sure database is running.
    ```shell
    dotnet run
    ```


The development server should now be running at `http://localhost:5224`.
or what you  specified in the `/Properties/launchSettings.json`.