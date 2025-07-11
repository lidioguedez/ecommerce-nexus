import { describe, it, expect, beforeEach, jest } from '@jest/globals';
import { CreateProductUseCase, CreateProductRequest, EventPublisher } from '../../../application/use-cases/CreateProductUseCase';
import { ProductRepository } from '../../../domain/repositories/ProductRepository';
import { Product } from '../../../domain/entities/Product';
import { ProductCreatedEvent } from '../../../domain/events/ProductEvents';

// Mock implementations
const mockProductRepository: jest.Mocked<ProductRepository> = {
  findById: jest.fn(),
  findBySku: jest.fn(),
  findAll: jest.fn(),
  findByCategory: jest.fn(),
  save: jest.fn(),
  update: jest.fn(),
  delete: jest.fn(),
  exists: jest.fn(),
  count: jest.fn()
};

const mockEventPublisher: jest.Mocked<EventPublisher> = {
  publish: jest.fn()
};

describe('CreateProductUseCase', () => {
  let createProductUseCase: CreateProductUseCase;
  
  const validRequest: CreateProductRequest = {
    name: 'Test Product',
    description: 'Test Description',
    price: 99.99,
    sku: 'TEST-001',
    category: 'Electronics',
    stock: 10,
    isActive: true
  };

  beforeEach(() => {
    jest.clearAllMocks();
    createProductUseCase = new CreateProductUseCase(mockProductRepository, mockEventPublisher);
  });

  describe('Successful Product Creation', () => {
    it('should create a new product when valid data is provided', async () => {
      // Arrange
      mockProductRepository.findBySku.mockResolvedValue(null);
      const savedProduct = new Product(validRequest);
      mockProductRepository.save.mockResolvedValue(savedProduct);
      mockEventPublisher.publish.mockResolvedValue(undefined);

      // Act
      const result = await createProductUseCase.execute(validRequest);

      // Assert
      expect(result.success).toBe(true);
      expect(result.product).toBeDefined();
      expect(result.product?.name).toBe(validRequest.name);
      expect(result.product?.sku).toBe(validRequest.sku);
      expect(result.error).toBeUndefined();

      expect(mockProductRepository.findBySku).toHaveBeenCalledWith(validRequest.sku);
      expect(mockProductRepository.save).toHaveBeenCalledWith(expect.any(Product));
      expect(mockEventPublisher.publish).toHaveBeenCalledWith(expect.any(ProductCreatedEvent));
    });

    it('should set isActive to true by default when not specified', async () => {
      // Arrange
      const requestWithoutIsActive = { ...validRequest };
      delete requestWithoutIsActive.isActive;
      
      mockProductRepository.findBySku.mockResolvedValue(null);
      mockProductRepository.save.mockImplementation((product) => Promise.resolve(product));
      mockEventPublisher.publish.mockResolvedValue(undefined);

      // Act
      const result = await createProductUseCase.execute(requestWithoutIsActive);

      // Assert
      expect(result.success).toBe(true);
      expect(result.product?.isActive).toBe(true);
    });

    it('should publish ProductCreatedEvent after successful creation', async () => {
      // Arrange
      mockProductRepository.findBySku.mockResolvedValue(null);
      const savedProduct = new Product(validRequest);
      mockProductRepository.save.mockResolvedValue(savedProduct);
      mockEventPublisher.publish.mockResolvedValue(undefined);

      // Act
      await createProductUseCase.execute(validRequest);

      // Assert
      expect(mockEventPublisher.publish).toHaveBeenCalledWith(
        expect.objectContaining({
          eventType: 'product.created',
          aggregateId: savedProduct.id,
          eventData: expect.objectContaining({
            name: validRequest.name,
            sku: validRequest.sku,
            category: validRequest.category,
            price: validRequest.price,
            stock: validRequest.stock
          })
        })
      );
    });
  });

  describe('SKU Uniqueness Validation', () => {
    it('should fail when SKU already exists', async () => {
      // Arrange
      const existingProduct = new Product(validRequest);
      mockProductRepository.findBySku.mockResolvedValue(existingProduct);

      // Act
      const result = await createProductUseCase.execute(validRequest);

      // Assert
      expect(result.success).toBe(false);
      expect(result.error).toBe(`Product with SKU ${validRequest.sku} already exists`);
      expect(result.product).toBeUndefined();

      expect(mockProductRepository.findBySku).toHaveBeenCalledWith(validRequest.sku);
      expect(mockProductRepository.save).not.toHaveBeenCalled();
      expect(mockEventPublisher.publish).not.toHaveBeenCalled();
    });
  });

  describe('Error Handling', () => {
    it('should handle domain validation errors', async () => {
      // Arrange
      mockProductRepository.findBySku.mockResolvedValue(null);
      const invalidRequest = { ...validRequest, name: '' }; // Invalid name

      // Act
      const result = await createProductUseCase.execute(invalidRequest);

      // Assert
      expect(result.success).toBe(false);
      expect(result.error).toBe('Product name is required');
      expect(result.product).toBeUndefined();

      expect(mockProductRepository.save).not.toHaveBeenCalled();
      expect(mockEventPublisher.publish).not.toHaveBeenCalled();
    });

    it('should handle repository errors', async () => {
      // Arrange
      mockProductRepository.findBySku.mockResolvedValue(null);
      mockProductRepository.save.mockRejectedValue(new Error('Database connection failed'));

      // Act
      const result = await createProductUseCase.execute(validRequest);

      // Assert
      expect(result.success).toBe(false);
      expect(result.error).toBe('Database connection failed');
      expect(result.product).toBeUndefined();

      expect(mockEventPublisher.publish).not.toHaveBeenCalled();
    });

    it('should handle event publisher errors', async () => {
      // Arrange
      mockProductRepository.findBySku.mockResolvedValue(null);
      const savedProduct = new Product(validRequest);
      mockProductRepository.save.mockResolvedValue(savedProduct);
      mockEventPublisher.publish.mockRejectedValue(new Error('Event publishing failed'));

      // Act
      const result = await createProductUseCase.execute(validRequest);

      // Assert
      expect(result.success).toBe(false);
      expect(result.error).toBe('Event publishing failed');
      expect(result.product).toBeUndefined();
    });

    it('should handle unknown errors', async () => {
      // Arrange
      mockProductRepository.findBySku.mockResolvedValue(null);
      mockProductRepository.save.mockRejectedValue('Unknown error');

      // Act
      const result = await createProductUseCase.execute(validRequest);

      // Assert
      expect(result.success).toBe(false);
      expect(result.error).toBe('Unknown error occurred');
      expect(result.product).toBeUndefined();
    });
  });
});