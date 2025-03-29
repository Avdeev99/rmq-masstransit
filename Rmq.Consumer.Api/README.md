# RabbitMQ MassTransit Consumer API

This is a .NET API application that consumes messages from a RabbitMQ queue using MassTransit and sends responses using the JSON-RPC 2.0 format.

## Features

- Consumes JSON-RPC messages from RabbitMQ queue
- Processes messages based on the method name
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

1. The consumer listens to the `jsonrpc-queue` RabbitMQ queue
2. When a message is received, it's parsed as a JSON-RPC request
3. The consumer processes the request based on the `Method` property:
   - `ProcessMessage`: Processes the content specified in the params and returns a result
   - Any other method: Returns a "Method not found" error
4. A JSON-RPC response is sent back to the producer

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
  "method": "MethodName",
  "params": {
    "content": "Your message here"
  }
}
```

### Response Format (Success)
```json
{
  "jsonrpc": "2.0",
  "id": "unique-id-string",
  "result": {
    "content": "Processed: Your message here",
    "success": true,
    "timestamp": "2023-03-29T12:34:56.789Z"
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