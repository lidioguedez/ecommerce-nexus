import { Product } from '../../domain/entities/Product';
import { ProductRepository, ProductFilters } from '../../domain/repositories/ProductRepository';

export interface ListProductsRequest {
  filters?: ProductFilters;
  page?: number;
  limit?: number;
}

export interface ListProductsResponse {
  success: boolean;
  products?: Product[];
  total?: number;
  page?: number;
  limit?: number;
  error?: string;
}

export class ListProductsUseCase {
  constructor(private productRepository: ProductRepository) {}

  async execute(request: ListProductsRequest = {}): Promise<ListProductsResponse> {
    try {
      const page = request.page || 1;
      const limit = request.limit || 10;

      if (page < 1) {
        return {
          success: false,
          error: 'Page must be greater than 0'
        };
      }

      if (limit < 1 || limit > 100) {
        return {
          success: false,
          error: 'Limit must be between 1 and 100'
        };
      }

      // Get products with filters
      const products = await this.productRepository.findAll(request.filters);
      
      // Get total count
      const total = await this.productRepository.count(request.filters);

      // Apply pagination
      const startIndex = (page - 1) * limit;
      const endIndex = startIndex + limit;
      const paginatedProducts = products.slice(startIndex, endIndex);

      return {
        success: true,
        products: paginatedProducts,
        total,
        page,
        limit
      };
    } catch (error) {
      return {
        success: false,
        error: error instanceof Error ? error.message : 'Unknown error occurred'
      };
    }
  }
}