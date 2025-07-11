import { Router } from 'express';
import { body, param, query } from 'express-validator';
import { ProductController } from '../controllers/ProductController';

export function createProductRoutes(productController: ProductController): Router {
  const router = Router();

  // Create product
  router.post(
    '/',
    [
      body('name')
        .isString()
        .isLength({ min: 1, max: 255 })
        .withMessage('Name must be between 1 and 255 characters'),
      body('description')
        .isString()
        .isLength({ min: 1, max: 1000 })
        .withMessage('Description must be between 1 and 1000 characters'),
      body('price')
        .isFloat({ min: 0 })
        .withMessage('Price must be a positive number'),
      body('sku')
        .isString()
        .isLength({ min: 1, max: 100 })
        .withMessage('SKU must be between 1 and 100 characters'),
      body('category')
        .isString()
        .isLength({ min: 1, max: 100 })
        .withMessage('Category must be between 1 and 100 characters'),
      body('stock')
        .isInt({ min: 0 })
        .withMessage('Stock must be a non-negative integer'),
      body('isActive')
        .optional()
        .isBoolean()
        .withMessage('isActive must be a boolean')
    ],
    productController.createProduct.bind(productController)
  );

  // Get product by ID
  router.get(
    '/:id',
    [
      param('id')
        .isUUID()
        .withMessage('Product ID must be a valid UUID')
    ],
    productController.getProduct.bind(productController)
  );

  // Update product
  router.put(
    '/:id',
    [
      param('id')
        .isUUID()
        .withMessage('Product ID must be a valid UUID'),
      body('name')
        .optional()
        .isString()
        .isLength({ min: 1, max: 255 })
        .withMessage('Name must be between 1 and 255 characters'),
      body('description')
        .optional()
        .isString()
        .isLength({ min: 1, max: 1000 })
        .withMessage('Description must be between 1 and 1000 characters'),
      body('price')
        .optional()
        .isFloat({ min: 0 })
        .withMessage('Price must be a positive number'),
      body('category')
        .optional()
        .isString()
        .isLength({ min: 1, max: 100 })
        .withMessage('Category must be between 1 and 100 characters'),
      body('stock')
        .optional()
        .isInt({ min: 0 })
        .withMessage('Stock must be a non-negative integer'),
      body('isActive')
        .optional()
        .isBoolean()
        .withMessage('isActive must be a boolean')
    ],
    productController.updateProduct.bind(productController)
  );

  // Update stock
  router.patch(
    '/:id/stock',
    [
      param('id')
        .isUUID()
        .withMessage('Product ID must be a valid UUID'),
      body('operation')
        .isIn(['increase', 'decrease', 'set'])
        .withMessage('Operation must be increase, decrease, or set'),
      body('quantity')
        .isInt({ min: 0 })
        .withMessage('Quantity must be a non-negative integer')
    ],
    productController.updateStock.bind(productController)
  );

  // List products
  router.get(
    '/',
    [
      query('page')
        .optional()
        .isInt({ min: 1 })
        .withMessage('Page must be a positive integer'),
      query('limit')
        .optional()
        .isInt({ min: 1, max: 100 })
        .withMessage('Limit must be between 1 and 100'),
      query('category')
        .optional()
        .isString()
        .isLength({ min: 1, max: 100 })
        .withMessage('Category must be between 1 and 100 characters'),
      query('name')
        .optional()
        .isString()
        .isLength({ min: 1, max: 255 })
        .withMessage('Name must be between 1 and 255 characters'),
      query('priceMin')
        .optional()
        .isFloat({ min: 0 })
        .withMessage('PriceMin must be a positive number'),
      query('priceMax')
        .optional()
        .isFloat({ min: 0 })
        .withMessage('PriceMax must be a positive number'),
      query('inStock')
        .optional()
        .isBoolean()
        .withMessage('inStock must be a boolean'),
      query('isActive')
        .optional()
        .isBoolean()
        .withMessage('isActive must be a boolean')
    ],
    productController.listProducts.bind(productController)
  );

  return router;
}