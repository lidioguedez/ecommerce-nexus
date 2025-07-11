import { Pool, PoolClient } from 'pg';
import { Product } from '../../domain/entities/Product';
import { ProductRepository, ProductFilters } from '../../domain/repositories/ProductRepository';

interface ProductRow {
  id: string;
  name: string;
  description: string;
  price: number;
  sku: string;
  category: string;
  stock: number;
  is_active: boolean;
  created_at: Date;
  updated_at: Date;
}

export class PostgreSQLProductRepository implements ProductRepository {
  constructor(private pool: Pool) {}

  async findById(id: string): Promise<Product | null> {
    const client = await this.pool.connect();
    try {
      const query = 'SELECT * FROM products WHERE id = $1';
      const result = await client.query(query, [id]);
      
      if (result.rows.length === 0) {
        return null;
      }

      return this.mapRowToProduct(result.rows[0]);
    } finally {
      client.release();
    }
  }

  async findBySku(sku: string): Promise<Product | null> {
    const client = await this.pool.connect();
    try {
      const query = 'SELECT * FROM products WHERE sku = $1';
      const result = await client.query(query, [sku]);
      
      if (result.rows.length === 0) {
        return null;
      }

      return this.mapRowToProduct(result.rows[0]);
    } finally {
      client.release();
    }
  }

  async findAll(filters?: ProductFilters): Promise<Product[]> {
    const client = await this.pool.connect();
    try {
      const { query, params } = this.buildFilterQuery(filters);
      const result = await client.query(query, params);
      
      return result.rows.map(row => this.mapRowToProduct(row));
    } finally {
      client.release();
    }
  }

  async findByCategory(category: string): Promise<Product[]> {
    const client = await this.pool.connect();
    try {
      const query = 'SELECT * FROM products WHERE category = $1 ORDER BY name';
      const result = await client.query(query, [category]);
      
      return result.rows.map(row => this.mapRowToProduct(row));
    } finally {
      client.release();
    }
  }

  async save(product: Product): Promise<Product> {
    const client = await this.pool.connect();
    try {
      const query = `
        INSERT INTO products (id, name, description, price, sku, category, stock, is_active, created_at, updated_at)
        VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10)
        RETURNING *
      `;
      
      const params = [
        product.id,
        product.name,
        product.description,
        product.price,
        product.sku,
        product.category,
        product.stock,
        product.isActive,
        product.createdAt,
        product.updatedAt
      ];

      const result = await client.query(query, params);
      return this.mapRowToProduct(result.rows[0]);
    } finally {
      client.release();
    }
  }

  async update(product: Product): Promise<Product> {
    const client = await this.pool.connect();
    try {
      const query = `
        UPDATE products 
        SET name = $2, description = $3, price = $4, category = $5, stock = $6, is_active = $7, updated_at = $8
        WHERE id = $1
        RETURNING *
      `;
      
      const params = [
        product.id,
        product.name,
        product.description,
        product.price,
        product.category,
        product.stock,
        product.isActive,
        product.updatedAt
      ];

      const result = await client.query(query, params);
      
      if (result.rows.length === 0) {
        throw new Error(`Product with ID ${product.id} not found`);
      }

      return this.mapRowToProduct(result.rows[0]);
    } finally {
      client.release();
    }
  }

  async delete(id: string): Promise<void> {
    const client = await this.pool.connect();
    try {
      const query = 'DELETE FROM products WHERE id = $1';
      const result = await client.query(query, [id]);
      
      if (result.rowCount === 0) {
        throw new Error(`Product with ID ${id} not found`);
      }
    } finally {
      client.release();
    }
  }

  async exists(id: string): Promise<boolean> {
    const client = await this.pool.connect();
    try {
      const query = 'SELECT 1 FROM products WHERE id = $1';
      const result = await client.query(query, [id]);
      
      return result.rows.length > 0;
    } finally {
      client.release();
    }
  }

  async count(filters?: ProductFilters): Promise<number> {
    const client = await this.pool.connect();
    try {
      const { query, params } = this.buildFilterQuery(filters, true);
      const result = await client.query(query, params);
      
      return parseInt(result.rows[0].count, 10);
    } finally {
      client.release();
    }
  }

  private mapRowToProduct(row: ProductRow): Product {
    return new Product({
      id: row.id,
      name: row.name,
      description: row.description,
      price: row.price,
      sku: row.sku,
      category: row.category,
      stock: row.stock,
      isActive: row.is_active,
      createdAt: row.created_at,
      updatedAt: row.updated_at
    });
  }

  private buildFilterQuery(filters?: ProductFilters, isCount: boolean = false): { query: string; params: any[] } {
    const baseQuery = isCount ? 'SELECT COUNT(*) FROM products' : 'SELECT * FROM products';
    const conditions: string[] = [];
    const params: any[] = [];
    let paramIndex = 1;

    if (filters?.category) {
      conditions.push(`category = $${paramIndex}`);
      params.push(filters.category);
      paramIndex++;
    }

    if (filters?.name) {
      conditions.push(`name ILIKE $${paramIndex}`);
      params.push(`%${filters.name}%`);
      paramIndex++;
    }

    if (filters?.priceMin !== undefined) {
      conditions.push(`price >= $${paramIndex}`);
      params.push(filters.priceMin);
      paramIndex++;
    }

    if (filters?.priceMax !== undefined) {
      conditions.push(`price <= $${paramIndex}`);
      params.push(filters.priceMax);
      paramIndex++;
    }

    if (filters?.inStock !== undefined) {
      if (filters.inStock) {
        conditions.push('stock > 0');
      } else {
        conditions.push('stock = 0');
      }
    }

    if (filters?.isActive !== undefined) {
      conditions.push(`is_active = $${paramIndex}`);
      params.push(filters.isActive);
      paramIndex++;
    }

    let query = baseQuery;
    if (conditions.length > 0) {
      query += ' WHERE ' + conditions.join(' AND ');
    }

    if (!isCount) {
      query += ' ORDER BY created_at DESC';
    }

    return { query, params };
  }
}