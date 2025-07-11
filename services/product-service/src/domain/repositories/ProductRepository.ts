import { Product } from '../entities/Product';

export interface ProductFilters {
  category?: string;
  name?: string;
  priceMin?: number;
  priceMax?: number;
  inStock?: boolean;
  isActive?: boolean;
}

export interface ProductRepository {
  findById(id: string): Promise<Product | null>;
  findBySku(sku: string): Promise<Product | null>;
  findAll(filters?: ProductFilters): Promise<Product[]>;
  findByCategory(category: string): Promise<Product[]>;
  save(product: Product): Promise<Product>;
  update(product: Product): Promise<Product>;
  delete(id: string): Promise<void>;
  exists(id: string): Promise<boolean>;
  count(filters?: ProductFilters): Promise<number>;
}