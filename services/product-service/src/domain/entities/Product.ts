import { v4 as uuidv4 } from 'uuid';

export interface ProductProps {
  id?: string;
  name: string;
  description: string;
  price: number;
  sku: string;
  category: string;
  stock: number;
  isActive: boolean;
  createdAt?: Date;
  updatedAt?: Date;
}

export class Product {
  private readonly _id: string;
  private _name: string;
  private _description: string;
  private _price: number;
  private _sku: string;
  private _category: string;
  private _stock: number;
  private _isActive: boolean;
  private _createdAt: Date;
  private _updatedAt: Date;

  constructor(props: ProductProps) {
    this._id = props.id || uuidv4();
    this._name = props.name;
    this._description = props.description;
    this._price = props.price;
    this._sku = props.sku;
    this._category = props.category;
    this._stock = props.stock;
    this._isActive = props.isActive;
    this._createdAt = props.createdAt || new Date();
    this._updatedAt = props.updatedAt || new Date();

    this.validate();
  }

  private validate(): void {
    if (!this._name || this._name.trim().length === 0) {
      throw new Error('Product name is required');
    }
    
    if (!this._description || this._description.trim().length === 0) {
      throw new Error('Product description is required');
    }
    
    if (this._price < 0) {
      throw new Error('Product price cannot be negative');
    }
    
    if (!this._sku || this._sku.trim().length === 0) {
      throw new Error('Product SKU is required');
    }
    
    if (!this._category || this._category.trim().length === 0) {
      throw new Error('Product category is required');
    }
    
    if (this._stock < 0) {
      throw new Error('Product stock cannot be negative');
    }
  }

  // Getters
  get id(): string {
    return this._id;
  }

  get name(): string {
    return this._name;
  }

  get description(): string {
    return this._description;
  }

  get price(): number {
    return this._price;
  }

  get sku(): string {
    return this._sku;
  }

  get category(): string {
    return this._category;
  }

  get stock(): number {
    return this._stock;
  }

  get isActive(): boolean {
    return this._isActive;
  }

  get createdAt(): Date {
    return this._createdAt;
  }

  get updatedAt(): Date {
    return this._updatedAt;
  }

  // Business Methods
  public updatePrice(newPrice: number): void {
    if (newPrice < 0) {
      throw new Error('Product price cannot be negative');
    }
    this._price = newPrice;
    this._updatedAt = new Date();
  }

  public updateStock(newStock: number): void {
    if (newStock < 0) {
      throw new Error('Product stock cannot be negative');
    }
    this._stock = newStock;
    this._updatedAt = new Date();
  }

  public reduceStock(quantity: number): void {
    if (quantity <= 0) {
      throw new Error('Quantity must be positive');
    }
    if (this._stock < quantity) {
      throw new Error('Insufficient stock');
    }
    this._stock -= quantity;
    this._updatedAt = new Date();
  }

  public increaseStock(quantity: number): void {
    if (quantity <= 0) {
      throw new Error('Quantity must be positive');
    }
    this._stock += quantity;
    this._updatedAt = new Date();
  }

  public deactivate(): void {
    this._isActive = false;
    this._updatedAt = new Date();
  }

  public activate(): void {
    this._isActive = true;
    this._updatedAt = new Date();
  }

  public isInStock(): boolean {
    return this._stock > 0;
  }

  public canFulfillOrder(quantity: number): boolean {
    return this._stock >= quantity;
  }

  public toJSON(): Record<string, any> {
    return {
      id: this._id,
      name: this._name,
      description: this._description,
      price: this._price,
      sku: this._sku,
      category: this._category,
      stock: this._stock,
      isActive: this._isActive,
      createdAt: this._createdAt.toISOString(),
      updatedAt: this._updatedAt.toISOString()
    };
  }

  public static fromJSON(json: Record<string, any>): Product {
    return new Product({
      id: json.id,
      name: json.name,
      description: json.description,
      price: json.price,
      sku: json.sku,
      category: json.category,
      stock: json.stock,
      isActive: json.isActive,
      createdAt: json.createdAt ? new Date(json.createdAt) : undefined,
      updatedAt: json.updatedAt ? new Date(json.updatedAt) : undefined
    });
  }
}