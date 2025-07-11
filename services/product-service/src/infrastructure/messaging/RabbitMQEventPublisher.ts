import amqp, { Connection, Channel } from 'amqplib';
import { EventPublisher } from '../../application/use-cases/CreateProductUseCase';

export class RabbitMQEventPublisher implements EventPublisher {
  private connection: Connection | null = null;
  private channel: Channel | null = null;

  constructor(private rabbitMQUrl: string) {}

  async connect(): Promise<void> {
    try {
      this.connection = await amqp.connect(this.rabbitMQUrl);
      this.channel = await this.connection.createChannel();

      // Declare exchanges
      await this.channel.assertExchange('product.events', 'topic', { durable: true });
      await this.channel.assertExchange('inventory.events', 'topic', { durable: true });
      
      console.log('RabbitMQ connected and exchanges declared');
    } catch (error) {
      console.error('Error connecting to RabbitMQ:', error);
      throw error;
    }
  }

  async publish(event: any): Promise<void> {
    if (!this.channel) {
      throw new Error('RabbitMQ channel not initialized');
    }

    try {
      const exchange = this.getExchangeForEvent(event.eventType);
      const routingKey = event.eventType;
      const message = JSON.stringify({
        eventType: event.eventType,
        aggregateId: event.aggregateId,
        occurredOn: event.occurredOn,
        eventData: event.eventData
      });

      await this.channel.publish(
        exchange,
        routingKey,
        Buffer.from(message),
        {
          persistent: true,
          messageId: `${event.aggregateId}-${Date.now()}`,
          timestamp: Date.now(),
          contentType: 'application/json'
        }
      );

      console.log(`Event published: ${event.eventType} to exchange: ${exchange}`);
    } catch (error) {
      console.error('Error publishing event:', error);
      throw error;
    }
  }

  private getExchangeForEvent(eventType: string): string {
    if (eventType.includes('inventory') || eventType.includes('stock')) {
      return 'inventory.events';
    }
    return 'product.events';
  }

  async disconnect(): Promise<void> {
    try {
      if (this.channel) {
        await this.channel.close();
        this.channel = null;
      }
      if (this.connection) {
        await this.connection.close();
        this.connection = null;
      }
      console.log('RabbitMQ disconnected');
    } catch (error) {
      console.error('Error disconnecting from RabbitMQ:', error);
      throw error;
    }
  }
}