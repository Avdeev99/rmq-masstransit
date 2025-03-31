# RabbitMQ MassTransit Producer API with JSON-RPC

This is a .NET API application that publishes messages to a RabbitMQ queue using MassTransit and waits for responses using the JSON-RPC 2.0 format.

## Features

- JSON-RPC 2.0 endpoint for publishing messages to RabbitMQ queue
- Messages published in JSON-RPC format following the specification
- Generic message handling architecture for type-safe request/response
- Dynamic service resolution for different message types
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
curl -X POST http://localhost:8080/api/json-rpc \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "method": "user.get",
    "params": {
      "id": "user-123"
    }
  }'
```

The response will also be in JSON-RPC 2.0 format:

```json
{
  "jsonrpc": "2.0",
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "result": {
    "id": "generated-guid",
    "name": "John Doe",
    "email": "john.doe@example.com"
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

## Architecture Details

The application uses a dynamic and generic approach to handle different types of JSON-RPC requests:

1. The controller receives requests as `JsonRpcRequestBase` objects
2. Based on the `method` field, it routes to the appropriate handler (e.g., `user.get`)
3. The handler converts the base request to a strongly-typed request with proper params
4. A generic `IMessageService<TRequest, TResponse>` is resolved from the service provider
5. The message is sent to RabbitMQ and waits for a response
6. The typed response is returned to the client

This architecture allows for:
- Type safety throughout the request/response pipeline
- Easy addition of new message types and handlers
- Decoupled components that can be independently tested

## Supported Methods

Currently, the API supports the following JSON-RPC methods:

| Method | Description | Parameters | Response |
|--------|-------------|------------|----------|
| `user.get` | Retrieves user information by ID | `id`: String - User identifier | User object with id, name, and email |

To add new methods:
1. Create a new request/response contract in the Core project
2. Add a new handler method in JsonRpcController
3. Register the message consumer in MassTransit configuration

## Docker Environment Variables

Environment variables are configured in the `docker-compose.yml` file. You can modify them there or override them when running Docker Compose:

```bash
RabbitMQ__Host=custom-rabbitmq-host \
RabbitMQ__Username=custom-user \
RabbitMQ__Password=custom-password \
docker-compose up -d
```