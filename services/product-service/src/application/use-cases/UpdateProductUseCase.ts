import { Product } from '../../domain/entities/Product';
import { ProductRepository } from '../../domain/repositories/ProductRepository';
import { ProductUpdatedEvent } from '../../domain/events/ProductEvents';
import { EventPublisher } from './CreateProductUseCase';

export interface UpdateProductRequest {
  id: string;
  name?: string;
  description?: string;
  price?: number;
  category?: string;
  stock?: number;
  isActive?: boolean;
}

export interface UpdateProductResponse {
  success: boolean;
  product?: Product;
  error?: string;
}

export class UpdateProductUseCase {
  constructor(
    private productRepository: ProductRepository,
    private eventPublisher: EventPublisher
  ) {}

  async execute(request: UpdateProductRequest): Promise<UpdateProductResponse> {
    try {
      if (!request.id || request.id.trim().length === 0) {
        return {
          success: false,
          error: 'Product ID is required'
        };
      }

      // Find existing product
      const existingProduct = await this.productRepository.findById(request.id);
      if (!existingProduct) {
        return {
          success: false,
          error: `Product with ID ${request.id} not found`
        };
      }

      // Create updated product with new values
      const updatedProduct = new Product({
        id: existingProduct.id,
        name: request.name ?? existingProduct.name,
        description: request.description ?? existingProduct.description,
        price: request.price ?? existingProduct.price,
        sku: existingProduct.sku, // SKU cannot be updated
        category: request.category ?? existingProduct.category,
        stock: request.stock ?? existingProduct.stock,
        isActive: request.isActive ?? existingProduct.isActive,
        createdAt: existingProduct.createdAt,
        updatedAt: new Date()
      });

      // Save updated product
      const savedProduct = await this.productRepository.update(updatedProduct);

      // Publish event
      const eventData: any = {};
      if (request.name !== undefined) eventData.name = request.name;
      if (request.price !== undefined) eventData.price = request.price;
      if (request.stock !== undefined) eventData.stock = request.stock;
      if (request.isActive !== undefined) eventData.isActive = request.isActive;

      if (Object.keys(eventData).length > 0) {
        const event = new ProductUpdatedEvent(savedProduct.id, eventData);
        await this.eventPublisher.publish(event);
      }

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