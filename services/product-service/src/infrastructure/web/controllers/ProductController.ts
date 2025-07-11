import { Request, Response } from 'express';
import { validationResult } from 'express-validator';
import { CreateProductUseCase } from '../../../application/use-cases/CreateProductUseCase';
import { GetProductUseCase } from '../../../application/use-cases/GetProductUseCase';
import { UpdateProductUseCase } from '../../../application/use-cases/UpdateProductUseCase';
import { UpdateStockUseCase } from '../../../application/use-cases/UpdateStockUseCase';
import { ListProductsUseCase } from '../../../application/use-cases/ListProductsUseCase';

export class ProductController {
  constructor(
    private createProductUseCase: CreateProductUseCase,
    private getProductUseCase: GetProductUseCase,
    private updateProductUseCase: UpdateProductUseCase,
    private updateStockUseCase: UpdateStockUseCase,
    private listProductsUseCase: ListProductsUseCase
  ) {}

  async createProduct(req: Request, res: Response): Promise<void> {
    try {
      const errors = validationResult(req);
      if (!errors.isEmpty()) {
        res.status(400).json({
          success: false,
          message: 'Validation failed',
          errors: errors.array()
        });
        return;
      }

      const result = await this.createProductUseCase.execute(req.body);

      if (result.success) {
        res.status(201).json({
          success: true,
          message: 'Product created successfully',
          data: result.product?.toJSON()
        });
      } else {
        res.status(400).json({
          success: false,
          message: result.error
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        message: 'Internal server error'
      });
    }
  }

  async getProduct(req: Request, res: Response): Promise<void> {
    try {
      const { id } = req.params;
      const result = await this.getProductUseCase.execute({ id });

      if (result.success) {
        res.status(200).json({
          success: true,
          data: result.product?.toJSON()
        });
      } else {
        res.status(404).json({
          success: false,
          message: result.error
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        message: 'Internal server error'
      });
    }
  }

  async updateProduct(req: Request, res: Response): Promise<void> {
    try {
      const errors = validationResult(req);
      if (!errors.isEmpty()) {
        res.status(400).json({
          success: false,
          message: 'Validation failed',
          errors: errors.array()
        });
        return;
      }

      const { id } = req.params;
      const result = await this.updateProductUseCase.execute({
        id,
        ...req.body
      });

      if (result.success) {
        res.status(200).json({
          success: true,
          message: 'Product updated successfully',
          data: result.product?.toJSON()
        });
      } else {
        res.status(404).json({
          success: false,
          message: result.error
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        message: 'Internal server error'
      });
    }
  }

  async updateStock(req: Request, res: Response): Promise<void> {
    try {
      const errors = validationResult(req);
      if (!errors.isEmpty()) {
        res.status(400).json({
          success: false,
          message: 'Validation failed',
          errors: errors.array()
        });
        return;
      }

      const { id } = req.params;
      const { operation, quantity } = req.body;

      const result = await this.updateStockUseCase.execute({
        productId: id,
        operation,
        quantity
      });

      if (result.success) {
        res.status(200).json({
          success: true,
          message: 'Stock updated successfully',
          data: result.product?.toJSON()
        });
      } else {
        res.status(400).json({
          success: false,
          message: result.error
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        message: 'Internal server error'
      });
    }
  }

  async listProducts(req: Request, res: Response): Promise<void> {
    try {
      const {
        page = 1,
        limit = 10,
        category,
        name,
        priceMin,
        priceMax,
        inStock,
        isActive
      } = req.query;

      const filters: any = {};
      if (category) filters.category = category as string;
      if (name) filters.name = name as string;
      if (priceMin) filters.priceMin = parseFloat(priceMin as string);
      if (priceMax) filters.priceMax = parseFloat(priceMax as string);
      if (inStock !== undefined) filters.inStock = inStock === 'true';
      if (isActive !== undefined) filters.isActive = isActive === 'true';

      const result = await this.listProductsUseCase.execute({
        filters,
        page: parseInt(page as string),
        limit: parseInt(limit as string)
      });

      if (result.success) {
        res.status(200).json({
          success: true,
          data: result.products?.map(product => product.toJSON()),
          pagination: {
            page: result.page,
            limit: result.limit,
            total: result.total,
            totalPages: Math.ceil((result.total || 0) / (result.limit || 10))
          }
        });
      } else {
        res.status(400).json({
          success: false,
          message: result.error
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        message: 'Internal server error'
      });
    }
  }
}