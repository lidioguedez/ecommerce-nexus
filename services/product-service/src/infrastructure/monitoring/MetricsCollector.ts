import client from 'prom-client';

export class MetricsCollector {
  private static instance: MetricsCollector;
  
  // HTTP Metrics
  private httpRequestDuration = new client.Histogram({
    name: 'http_request_duration_seconds',
    help: 'Duration of HTTP requests in seconds',
    labelNames: ['method', 'route', 'status_code'],
    buckets: [0.1, 0.5, 1, 2, 5]
  });

  private httpRequestTotal = new client.Counter({
    name: 'http_requests_total',
    help: 'Total number of HTTP requests',
    labelNames: ['method', 'route', 'status_code']
  });

  // Business Metrics
  private productCreated = new client.Counter({
    name: 'products_created_total',
    help: 'Total number of products created',
    labelNames: ['category']
  });

  private productUpdated = new client.Counter({
    name: 'products_updated_total',
    help: 'Total number of products updated',
    labelNames: ['category']
  });

  private stockUpdated = new client.Counter({
    name: 'stock_updates_total',
    help: 'Total number of stock updates',
    labelNames: ['operation']
  });

  private currentStock = new client.Gauge({
    name: 'current_stock_level',
    help: 'Current stock level by product',
    labelNames: ['product_id', 'sku', 'category']
  });

  // Database Metrics
  private databaseQueries = new client.Counter({
    name: 'database_queries_total',
    help: 'Total number of database queries',
    labelNames: ['operation', 'table']
  });

  private databaseQueryDuration = new client.Histogram({
    name: 'database_query_duration_seconds',
    help: 'Duration of database queries in seconds',
    labelNames: ['operation', 'table'],
    buckets: [0.01, 0.05, 0.1, 0.5, 1, 2]
  });

  private constructor() {
    // Register default metrics
    client.collectDefaultMetrics({
      prefix: 'product_service_',
      timeout: 10000,
      gcDurationBuckets: [0.001, 0.01, 0.1, 1, 2, 5],
    });
  }

  static getInstance(): MetricsCollector {
    if (!MetricsCollector.instance) {
      MetricsCollector.instance = new MetricsCollector();
    }
    return MetricsCollector.instance;
  }

  // HTTP Metrics Methods
  recordHttpRequest(method: string, route: string, statusCode: number, duration: number): void {
    this.httpRequestTotal.inc({ method, route, status_code: statusCode });
    this.httpRequestDuration.observe({ method, route, status_code: statusCode }, duration);
  }

  // Business Metrics Methods
  recordProductCreated(category: string): void {
    this.productCreated.inc({ category });
  }

  recordProductUpdated(category: string): void {
    this.productUpdated.inc({ category });
  }

  recordStockUpdate(operation: string): void {
    this.stockUpdated.inc({ operation });
  }

  updateCurrentStock(productId: string, sku: string, category: string, stock: number): void {
    this.currentStock.set({ product_id: productId, sku, category }, stock);
  }

  // Database Metrics Methods
  recordDatabaseQuery(operation: string, table: string, duration: number): void {
    this.databaseQueries.inc({ operation, table });
    this.databaseQueryDuration.observe({ operation, table }, duration);
  }

  // Get all metrics
  getMetrics(): Promise<string> {
    return client.register.metrics();
  }

  // Clear all metrics (useful for testing)
  clearMetrics(): void {
    client.register.clear();
  }
}