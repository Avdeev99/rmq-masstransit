# RabbitMQ MassTransit with JSON-RPC

.NET applications that demonstrate how to use MassTransit with RabbitMQ for message publishing and consuming using the JSON-RPC 2.0 protocol format with a generic, type-safe approach.

## Quick Start

The easiest way to run this application is using Docker Compose:

```bash
# Start all services (RabbitMQ, Producer API, and Consumer API)
docker-compose up -d

# Check logs
docker-compose logs -f
```

## Components

- **RabbitMQ**: Message broker running on port 5672 (15672 for management UI)
- **Producer API**: .NET API for publishing messages in JSON-RPC format and receiving responses
- **Consumer API**: .NET API for consuming JSON-RPC messages from RabbitMQ and responding to them

## Architecture

1. The Producer API receives JSON-RPC requests through its HTTP endpoint
2. It converts the request to a strongly-typed message using generics
3. It publishes these requests to a RabbitMQ queue based on the message type (e.g., `queue:q.user.get`)
4. The Consumer API picks up the messages from the queue with the appropriate consumer
5. The Consumer processes the strongly-typed messages based on the method
6. The Consumer sends back strongly-typed JSON-RPC responses
7. The Producer receives the responses and returns them to the original caller

## Testing the API

Send a message via the Producer API using JSON-RPC 2.0 format:

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

## Supported Methods

Currently, the application supports the following JSON-RPC methods:

| Method | Description | Parameters | Response |
|--------|-------------|------------|----------|
| `user.get` | Retrieves user information by ID | `id`: String - User identifier | User object with id, name, and email |

## More Information

For detailed documentation, see:
- [Producer API README](./Rmq.Producer.Api/README.md)
- [Consumer API README](./Rmq.Consumer.Api/README.md)