import { Product } from '../../domain/entities/Product';
import { ProductRepository } from '../../domain/repositories/ProductRepository';

export interface GetProductRequest {
  id: string;
}

export interface GetProductResponse {
  success: boolean;
  product?: Product;
  error?: string;
}

export class GetProductUseCase {
  constructor(private productRepository: ProductRepository) {}

  async execute(request: GetProductRequest): Promise<GetProductResponse> {
    try {
      if (!request.id || request.id.trim().length === 0) {
        return {
          success: false,
          error: 'Product ID is required'
        };
      }

      const product = await this.productRepository.findById(request.id);

      if (!product) {
        return {
          success: false,
          error: `Product with ID ${request.id} not found`
        };
      }

      return {
        success: true,
        product
      };
    } catch (error) {
      return {
        success: false,
        error: error instanceof Error ? error.message : 'Unknown error occurred'
      };
    }
  }
}