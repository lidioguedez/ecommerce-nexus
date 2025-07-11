import { Product } from '../../domain/entities/Product';
import { ProductRepository } from '../../domain/repositories/ProductRepository';
import { ProductStockUpdatedEvent } from '../../domain/events/ProductEvents';
import { EventPublisher } from './CreateProductUseCase';

export interface UpdateStockRequest {
  productId: string;
  operation: 'increase' | 'decrease' | 'set';
  quantity: number;
}

export interface UpdateStockResponse {
  success: boolean;
  product?: Product;
  error?: string;
}

export class UpdateStockUseCase {
  constructor(
    private productRepository: ProductRepository,
    private eventPublisher: EventPublisher
  ) {}

  async execute(request: UpdateStockRequest): Promise<UpdateStockResponse> {
    try {
      if (!request.productId || request.productId.trim().length === 0) {
        return {
          success: false,
          error: 'Product ID is required'
        };
      }

      if (request.quantity < 0) {
        return {
          success: false,
          error: 'Quantity cannot be negative'
        };
      }

      if (request.operation !== 'increase' && request.operation !== 'decrease' && request.operation !== 'set') {
        return {
          success: false,
          error: 'Invalid operation. Must be increase, decrease, or set'
        };
      }

      // Find existing product
      const existingProduct = await this.productRepository.findById(request.productId);
      if (!existingProduct) {
        return {
          success: false,
          error: `Product with ID ${request.productId} not found`
        };
      }

      const previousStock = existingProduct.stock;

      // Update stock based on operation
      switch (request.operation) {
        case 'increase':
          existingProduct.increaseStock(request.quantity);
          break;
        case 'decrease':
          existingProduct.reduceStock(request.quantity);
          break;
        case 'set':
          existingProduct.updateStock(request.quantity);
          break;
      }

      // Save updated product
      const savedProduct = await this.productRepository.update(existingProduct);

      // Publish event
      const event = new ProductStockUpdatedEvent(savedProduct.id, {
        sku: savedProduct.sku,
        previousStock,
        currentStock: savedProduct.stock,
        operation: request.operation
      });

      await this.eventPublisher.publish(event);

      return {
        success: true,
        product: savedProduct
      };
    } catch (error) {
      return {
        success: false,
        error: error instanceof Error ? error.message : 'Unknown error occurred'
      };
    }
  }
}