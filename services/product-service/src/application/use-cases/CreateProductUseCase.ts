import { Product } from '../../domain/entities/Product';
import { ProductRepository } from '../../domain/repositories/ProductRepository';
import { ProductCreatedEvent } from '../../domain/events/ProductEvents';

export interface CreateProductRequest {
  name: string;
  description: string;
  price: number;
  sku: string;
  category: string;
  stock: number;
  isActive?: boolean;
}

export interface CreateProductResponse {
  success: boolean;
  product?: Product;
  error?: string;
}

export interface EventPublisher {
  publish(event: any): Promise<void>;
}

export class CreateProductUseCase {
  constructor(
    private productRepository: ProductRepository,
    private eventPublisher: EventPublisher
  ) {}

  async execute(request: CreateProductRequest): Promise<CreateProductResponse> {
    try {
      // Validate SKU uniqueness
      const existingProduct = await this.productRepository.findBySku(request.sku);
      if (existingProduct) {
        return {
          success: false,
          error: `Product with SKU ${request.sku} already exists`
        };
      }

      // Create new product
      const product = new Product({
        name: request.name,
        description: request.description,
        price: request.price,
        sku: request.sku,
        category: request.category,
        stock: request.stock,
        isActive: request.isActive ?? true
      });

      // Save product
      const savedProduct = await this.productRepository.save(product);

      // Publish event
      const event = new ProductCreatedEvent(savedProduct.id, {
        name: savedProduct.name,
        sku: savedProduct.sku,
        category: savedProduct.category,
        price: savedProduct.price,
        stock: savedProduct.stock
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