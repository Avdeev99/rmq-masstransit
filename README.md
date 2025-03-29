# RabbitMQ MassTransit Producer with JSON-RPC

.NET application that demonstrates how to use MassTransit with RabbitMQ for message publishing and receiving responses using the JSON-RPC 2.0 protocol format.

## Quick Start

The easiest way to run this application is using Docker Compose:

```bash
# Start all services (RabbitMQ and API)
docker-compose up -d

# Check logs
docker-compose logs -f
```

## Components

- **RabbitMQ**: Message broker running on port 5672 (15672 for management UI)
- **Producer API**: .NET API for publishing messages in JSON-RPC format and receiving responses

## Testing the API

Send a message via the API using JSON-RPC 2.0 format:

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

## More Information

For detailed documentation, see the [API README](./Rmq.Producer.Api/README.md)