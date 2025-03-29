# RabbitMQ MassTransit with JSON-RPC

.NET applications that demonstrate how to use MassTransit with RabbitMQ for message publishing and consuming using the JSON-RPC 2.0 protocol format.

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
2. It publishes these requests to a RabbitMQ queue named `jsonrpc-queue`
3. The Consumer API picks up the messages from the queue
4. The Consumer processes the messages based on the method name
5. The Consumer sends back JSON-RPC responses
6. The Producer receives the responses and returns them to the original caller

## Testing the API

Send a message via the Producer API using JSON-RPC 2.0 format:

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

For detailed documentation, see:
- [Producer API README](./Rmq.Producer.Api/README.md)
- [Consumer API README](./Rmq.Consumer.Api/README.md)