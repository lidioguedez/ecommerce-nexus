import express from 'express';
import cors from 'cors';
import helmet from 'helmet';
import compression from 'compression';
import rateLimit from 'express-rate-limit';
import dotenv from 'dotenv';

// Infrastructure
import { DatabaseConfig } from './infrastructure/config/Database';
import { RabbitMQEventPublisher } from './infrastructure/messaging/RabbitMQEventPublisher';
import { MetricsCollector } from './infrastructure/monitoring/MetricsCollector';

// Repository
import { PostgreSQLProductRepository } from './infrastructure/database/PostgreSQLProductRepository';

// Use Cases
import { CreateProductUseCase } from './application/use-cases/CreateProductUseCase';
import { GetProductUseCase } from './application/use-cases/GetProductUseCase';
import { UpdateProductUseCase } from './application/use-cases/UpdateProductUseCase';
import { UpdateStockUseCase } from './application/use-cases/UpdateStockUseCase';
import { ListProductsUseCase } from './application/use-cases/ListProductsUseCase';

// Controllers and Routes
import { ProductController } from './infrastructure/web/controllers/ProductController';
import { createProductRoutes } from './infrastructure/web/routes/productRoutes';

// Load environment variables
dotenv.config();

async function startServer(): Promise<void> {
  try {
    // Initialize database
    const databaseConfig = DatabaseConfig.getInstance();
    await databaseConfig.testConnection();
    await databaseConfig.runMigrations();

    // Initialize messaging
    const eventPublisher = new RabbitMQEventPublisher(
      process.env.RABBITMQ_URL || 'amqp://localhost:5672'
    );
    await eventPublisher.connect();

    // Initialize metrics
    const metricsCollector = MetricsCollector.getInstance();

    // Initialize repository
    const productRepository = new PostgreSQLProductRepository(databaseConfig.getPool());

    // Initialize use cases
    const createProductUseCase = new CreateProductUseCase(productRepository, eventPublisher);
    const getProductUseCase = new GetProductUseCase(productRepository);
    const updateProductUseCase = new UpdateProductUseCase(productRepository, eventPublisher);
    const updateStockUseCase = new UpdateStockUseCase(productRepository, eventPublisher);
    const listProductsUseCase = new ListProductsUseCase(productRepository);

    // Initialize controller
    const productController = new ProductController(
      createProductUseCase,
      getProductUseCase,
      updateProductUseCase,
      updateStockUseCase,
      listProductsUseCase
    );

    // Initialize Express app
    const app = express();
    const port = process.env.PORT || 3001;

    // Security middleware
    app.use(helmet());
    app.use(cors());
    app.use(compression());

    // Rate limiting
    const limiter = rateLimit({
      windowMs: 15 * 60 * 1000, // 15 minutes
      max: 100, // limit each IP to 100 requests per windowMs
      message: 'Too many requests from this IP, please try again later.'
    });
    app.use('/api/', limiter);

    // Body parsing middleware
    app.use(express.json({ limit: '10mb' }));
    app.use(express.urlencoded({ extended: true }));

    // Metrics middleware
    app.use((req, res, next) => {
      const startTime = Date.now();
      
      res.on('finish', () => {
        const duration = (Date.now() - startTime) / 1000;
        metricsCollector.recordHttpRequest(
          req.method,
          req.route?.path || req.path,
          res.statusCode,
          duration
        );
      });
      
      next();
    });

    // Health check endpoint
    app.get('/health', (req, res) => {
      res.status(200).json({
        status: 'healthy',
        service: 'product-service',
        timestamp: new Date().toISOString(),
        version: process.env.npm_package_version || '1.0.0'
      });
    });

    // Metrics endpoint
    app.get('/metrics', async (req, res) => {
      try {
        const metrics = await metricsCollector.getMetrics();
        res.set('Content-Type', 'text/plain');
        res.send(metrics);
      } catch (error) {
        res.status(500).json({ error: 'Unable to collect metrics' });
      }
    });

    // API routes
    app.use('/api/v1/products', createProductRoutes(productController));

    // 404 handler
    app.use('*', (req, res) => {
      res.status(404).json({
        success: false,
        message: 'Endpoint not found'
      });
    });

    // Error handler
    app.use((error: Error, req: express.Request, res: express.Response, next: express.NextFunction) => {
      console.error('Unhandled error:', error);
      res.status(500).json({
        success: false,
        message: 'Internal server error'
      });
    });

    // Start server
    app.listen(port, () => {
      console.log(`Product Service running on port ${port}`);
      console.log(`Health check: http://localhost:${port}/health`);
      console.log(`Metrics: http://localhost:${port}/metrics`);
      console.log(`API: http://localhost:${port}/api/v1/products`);
    });

    // Graceful shutdown
    process.on('SIGTERM', async () => {
      console.log('SIGTERM received, shutting down gracefully...');
      await eventPublisher.disconnect();
      await databaseConfig.close();
      process.exit(0);
    });

    process.on('SIGINT', async () => {
      console.log('SIGINT received, shutting down gracefully...');
      await eventPublisher.disconnect();
      await databaseConfig.close();
      process.exit(0);
    });

  } catch (error) {
    console.error('Failed to start server:', error);
    process.exit(1);
  }
}

// Start the server
startServer().catch(console.error);