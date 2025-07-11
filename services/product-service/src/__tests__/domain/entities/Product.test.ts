import { describe, it, expect } from '@jest/globals';
import { Product } from '../../../domain/entities/Product';

describe('Product Entity', () => {
  const validProductProps = {
    name: 'Test Product',
    description: 'Test Description',
    price: 99.99,
    sku: 'TEST-001',
    category: 'Electronics',
    stock: 10,
    isActive: true
  };

  describe('Constructor', () => {
    it('should create a valid product with all properties', () => {
      const product = new Product(validProductProps);

      expect(product.id).toBeDefined();
      expect(product.name).toBe(validProductProps.name);
      expect(product.description).toBe(validProductProps.description);
      expect(product.price).toBe(validProductProps.price);
      expect(product.sku).toBe(validProductProps.sku);
      expect(product.category).toBe(validProductProps.category);
      expect(product.stock).toBe(validProductProps.stock);
      expect(product.isActive).toBe(validProductProps.isActive);
      expect(product.createdAt).toBeInstanceOf(Date);
      expect(product.updatedAt).toBeInstanceOf(Date);
    });

    it('should generate a unique ID when not provided', () => {
      const product1 = new Product(validProductProps);
      const product2 = new Product(validProductProps);

      expect(product1.id).not.toBe(product2.id);
    });

    it('should use provided ID when given', () => {
      const customId = 'custom-id-123';
      const product = new Product({ ...validProductProps, id: customId });

      expect(product.id).toBe(customId);
    });
  });

  describe('Validation', () => {
    it('should throw error when name is empty', () => {
      expect(() => {
        new Product({ ...validProductProps, name: '' });
      }).toThrow('Product name is required');
    });

    it('should throw error when description is empty', () => {
      expect(() => {
        new Product({ ...validProductProps, description: '' });
      }).toThrow('Product description is required');
    });

    it('should throw error when price is negative', () => {
      expect(() => {
        new Product({ ...validProductProps, price: -1 });
      }).toThrow('Product price cannot be negative');
    });

    it('should throw error when SKU is empty', () => {
      expect(() => {
        new Product({ ...validProductProps, sku: '' });
      }).toThrow('Product SKU is required');
    });

    it('should throw error when category is empty', () => {
      expect(() => {
        new Product({ ...validProductProps, category: '' });
      }).toThrow('Product category is required');
    });

    it('should throw error when stock is negative', () => {
      expect(() => {
        new Product({ ...validProductProps, stock: -1 });
      }).toThrow('Product stock cannot be negative');
    });
  });

  describe('Business Methods', () => {
    let product: Product;

    beforeEach(() => {
      product = new Product(validProductProps);
    });

    describe('updatePrice', () => {
      it('should update price when valid', () => {
        const newPrice = 199.99;
        product.updatePrice(newPrice);

        expect(product.price).toBe(newPrice);
        expect(product.updatedAt).toBeInstanceOf(Date);
      });

      it('should throw error when price is negative', () => {
        expect(() => {
          product.updatePrice(-1);
        }).toThrow('Product price cannot be negative');
      });
    });

    describe('updateStock', () => {
      it('should update stock when valid', () => {
        const newStock = 20;
        product.updateStock(newStock);

        expect(product.stock).toBe(newStock);
        expect(product.updatedAt).toBeInstanceOf(Date);
      });

      it('should throw error when stock is negative', () => {
        expect(() => {
          product.updateStock(-1);
        }).toThrow('Product stock cannot be negative');
      });
    });

    describe('reduceStock', () => {
      it('should reduce stock when sufficient quantity available', () => {
        const initialStock = product.stock;
        const reduction = 5;
        product.reduceStock(reduction);

        expect(product.stock).toBe(initialStock - reduction);
        expect(product.updatedAt).toBeInstanceOf(Date);
      });

      it('should throw error when quantity is zero or negative', () => {
        expect(() => {
          product.reduceStock(0);
        }).toThrow('Quantity must be positive');

        expect(() => {
          product.reduceStock(-1);
        }).toThrow('Quantity must be positive');
      });

      it('should throw error when insufficient stock', () => {
        expect(() => {
          product.reduceStock(product.stock + 1);
        }).toThrow('Insufficient stock');
      });
    });

    describe('increaseStock', () => {
      it('should increase stock when valid quantity', () => {
        const initialStock = product.stock;
        const increase = 5;
        product.increaseStock(increase);

        expect(product.stock).toBe(initialStock + increase);
        expect(product.updatedAt).toBeInstanceOf(Date);
      });

      it('should throw error when quantity is zero or negative', () => {
        expect(() => {
          product.increaseStock(0);
        }).toThrow('Quantity must be positive');

        expect(() => {
          product.increaseStock(-1);
        }).toThrow('Quantity must be positive');
      });
    });

    describe('activate/deactivate', () => {
      it('should deactivate product', () => {
        product.deactivate();

        expect(product.isActive).toBe(false);
        expect(product.updatedAt).toBeInstanceOf(Date);
      });

      it('should activate product', () => {
        product.deactivate();
        product.activate();

        expect(product.isActive).toBe(true);
        expect(product.updatedAt).toBeInstanceOf(Date);
      });
    });

    describe('isInStock', () => {
      it('should return true when stock is greater than 0', () => {
        expect(product.isInStock()).toBe(true);
      });

      it('should return false when stock is 0', () => {
        product.updateStock(0);
        expect(product.isInStock()).toBe(false);
      });
    });

    describe('canFulfillOrder', () => {
      it('should return true when stock is sufficient', () => {
        expect(product.canFulfillOrder(5)).toBe(true);
      });

      it('should return false when stock is insufficient', () => {
        expect(product.canFulfillOrder(product.stock + 1)).toBe(false);
      });

      it('should return true when quantity equals stock', () => {
        expect(product.canFulfillOrder(product.stock)).toBe(true);
      });
    });
  });

  describe('Serialization', () => {
    it('should serialize to JSON correctly', () => {
      const product = new Product(validProductProps);
      const json = product.toJSON();

      expect(json).toEqual({
        id: product.id,
        name: product.name,
        description: product.description,
        price: product.price,
        sku: product.sku,
        category: product.category,
        stock: product.stock,
        isActive: product.isActive,
        createdAt: product.createdAt.toISOString(),
        updatedAt: product.updatedAt.toISOString()
      });
    });

    it('should deserialize from JSON correctly', () => {
      const originalProduct = new Product(validProductProps);
      const json = originalProduct.toJSON();
      const deserializedProduct = Product.fromJSON(json);

      expect(deserializedProduct.id).toBe(originalProduct.id);
      expect(deserializedProduct.name).toBe(originalProduct.name);
      expect(deserializedProduct.description).toBe(originalProduct.description);
      expect(deserializedProduct.price).toBe(originalProduct.price);
      expect(deserializedProduct.sku).toBe(originalProduct.sku);
      expect(deserializedProduct.category).toBe(originalProduct.category);
      expect(deserializedProduct.stock).toBe(originalProduct.stock);
      expect(deserializedProduct.isActive).toBe(originalProduct.isActive);
      expect(deserializedProduct.createdAt.getTime()).toBe(originalProduct.createdAt.getTime());
      expect(deserializedProduct.updatedAt.getTime()).toBe(originalProduct.updatedAt.getTime());
    });
  });
});