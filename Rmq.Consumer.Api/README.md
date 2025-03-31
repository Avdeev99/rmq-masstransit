# RabbitMQ MassTransit Consumer API

This is a .NET API application that consumes messages from a RabbitMQ queue using MassTransit and sends responses using the JSON-RPC 2.0 format.

## Features

- Consumes JSON-RPC messages from RabbitMQ queue
- Processes messages based on the method name
- Type-safe generic message handling
- Responds with JSON-RPC formatted responses
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

## How It Works

1. The consumer registers handlers for specific JSON-RPC method types (e.g., `user.get`)
2. When a message is received, it's consumed by the appropriate handler (e.g., `GetUserConsumer`)
3. The consumer processes the strongly-typed request based on the method:
   - `user.get`: Retrieves user information based on the ID in the request
   - Any other method: Returns a "Method not found" error
4. A strongly-typed JSON-RPC response is sent back to the producer

## Supported Methods

Currently, the consumer handles the following JSON-RPC methods:

| Method | Description | Handler |
|--------|-------------|---------|
| `user.get` | Retrieves user information by ID | `GetUserConsumer` |

To add new methods:
1. Create a new request/response contract in the Core project
2. Implement a new consumer class that implements `IConsumer<JsonRpcRequest<YourRequestType>>`
3. Register the consumer in the MassTransit configuration

## Running with Docker Compose

The easiest way to run this application with the Producer and RabbitMQ:

```bash
# From the solution root directory
docker-compose up -d
```

## JSON-RPC Format

The consumer handles and responds with JSON-RPC 2.0 formatted messages:

### Request Format
```json
{
  "jsonrpc": "2.0",
  "id": "unique-id-string",
  "method": "user.get",
  "params": {
    "id": "user-123"
  }
}
```

### Response Format (Success)
```json
{
  "jsonrpc": "2.0",
  "id": "unique-id-string",
  "result": {
    "id": "generated-guid",
    "name": "John Doe",
    "email": "john.doe@example.com"
  }
}
```

### Response Format (Error)
```json
{
  "jsonrpc": "2.0",
  "id": "unique-id-string",
  "error": {
    "code": -32601,
    "message": "Method not found",
    "data": "Additional error details"
  }
}
``` 