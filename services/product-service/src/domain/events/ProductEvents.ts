export interface DomainEvent {
  eventType: string;
  aggregateId: string;
  occurredOn: Date;
  eventData: Record<string, any>;
}

export class ProductCreatedEvent implements DomainEvent {
  public readonly eventType = 'product.created';
  public readonly occurredOn: Date;

  constructor(
    public readonly aggregateId: string,
    public readonly eventData: {
      name: string;
      sku: string;
      category: string;
      price: number;
      stock: number;
    }
  ) {
    this.occurredOn = new Date();
  }
}

export class ProductUpdatedEvent implements DomainEvent {
  public readonly eventType = 'product.updated';
  public readonly occurredOn: Date;

  constructor(
    public readonly aggregateId: string,
    public readonly eventData: {
      name?: string;
      price?: number;
      stock?: number;
      isActive?: boolean;
    }
  ) {
    this.occurredOn = new Date();
  }
}

export class ProductDeletedEvent implements DomainEvent {
  public readonly eventType = 'product.deleted';
  public readonly occurredOn: Date;

  constructor(
    public readonly aggregateId: string,
    public readonly eventData: {
      sku: string;
      name: string;
    }
  ) {
    this.occurredOn = new Date();
  }
}

export class ProductStockUpdatedEvent implements DomainEvent {
  public readonly eventType = 'product.inventory.updated';
  public readonly occurredOn: Date;

  constructor(
    public readonly aggregateId: string,
    public readonly eventData: {
      sku: string;
      previousStock: number;
      currentStock: number;
      operation: 'increase' | 'decrease' | 'update';
    }
  ) {
    this.occurredOn = new Date();
  }
}

export class ProductActivatedEvent implements DomainEvent {
  public readonly eventType = 'product.activated';
  public readonly occurredOn: Date;

  constructor(
    public readonly aggregateId: string,
    public readonly eventData: {
      sku: string;
      name: string;
    }
  ) {
    this.occurredOn = new Date();
  }
}

export class ProductDeactivatedEvent implements DomainEvent {
  public readonly eventType = 'product.deactivated';
  public readonly occurredOn: Date;

  constructor(
    public readonly aggregateId: string,
    public readonly eventData: {
      sku: string;
      name: string;
    }
  ) {
    this.occurredOn = new Date();
  }
}