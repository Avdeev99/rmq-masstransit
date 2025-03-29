# RabbitMQ MassTransit Producer API with JSON-RPC

This is a .NET API application that publishes messages to a RabbitMQ queue using MassTransit and waits for responses using the JSON-RPC 2.0 format.

## Features

- JSON-RPC 2.0 endpoint for publishing messages to RabbitMQ queue
- Messages published in JSON-RPC format following the specification
- Waits for responses from consumers
- Dockerized application

## Requirements

- .NET 8.0 SDK
- Docker and Docker Compose (for containerized setup)
- RabbitMQ server (automatically included in Docker Compose)

## Configuration

RabbitMQ connection settings can be found in `appsettings.json`:

```json
"RabbitMQ": {
  "Host": "localhost",
  "VirtualHost": "/",
  "Username": "guest",
  "Password": "guest"
}
```

## Running the Application

### With .NET CLI

```bash
dotnet run
```

### With Docker

Build and run using just Docker:

```bash
# Build the Docker image
docker build -t rmq-producer-api -f ../Dockerfile .

# Run the container
docker run -p 8080:8080 -p 8081:8081 --name rmq-producer-api rmq-producer-api
```

### With Docker Compose (Recommended)

The easiest way to run the application with all its dependencies:

```bash
# Navigate to the solution root directory
cd ..

# Start the services (RabbitMQ and API)
docker-compose up -d

# Check the logs
docker-compose logs -f
```

This will start:
- RabbitMQ server with management UI (http://localhost:15672)
- The Producer API application

## Usage

Send a message using JSON-RPC 2.0 format:

```bash
curl -X POST http://localhost:8080/api/jsonrpc \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "method": "ProcessMessage",
    "params": {
      "content": "Your message here"
    }
  }'
```

The response will also be in JSON-RPC 2.0 format:

```json
{
  "jsonrpc": "2.0",
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "result": {
    "content": "Processed: Your message here",
    "success": true,
    "timestamp": "2023-03-29T12:34:56.789Z"
  }
}
```

## JSON-RPC 2.0 Format

The API follows the JSON-RPC 2.0 specification:

### Request Format

```json
{
  "jsonrpc": "2.0",          // Required, must be exactly "2.0"
  "id": "unique-id-string",  // Request identifier, echoed in the response
  "method": "MethodName",    // Required, name of the method to invoke
  "params": {}               // Optional, parameters for the method
}
```

### Response Format (Success)

```json
{
  "jsonrpc": "2.0",          // Required, must be exactly "2.0"
  "id": "unique-id-string",  // Same as in the request
  "result": {}               // Result object, only present on success
}
```

### Response Format (Error)

```json
{
  "jsonrpc": "2.0",          // Required, must be exactly "2.0"
  "id": "unique-id-string",  // Same as in the request
  "error": {                 // Error object, only present on error
    "code": -32000,          // Error code
    "message": "Error message", // Error message
    "data": {}               // Optional additional error data
  }
}
```

## Docker Environment Variables

Environment variables are configured in the `docker-compose.yml` file. You can modify them there or override them when running Docker Compose:

```bash
RabbitMQ__Host=custom-rabbitmq-host \
RabbitMQ__Username=custom-user \
RabbitMQ__Password=custom-password \
docker-compose up -d
``` 