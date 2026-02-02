export enum TransactionType {
  Purchase = 1,
  Sale = 2
}

export interface Transaction {
  id: string;
  productId: string;
  productName: string;
  currentStock: number;
  type: TransactionType;
  typeDescription: string;
  quantity: number;
  stockAfterTransaction: number;
  unitPrice: number;
  totalPrice: number;
  details: string;
  transactionDate: string;
  createdAt: string;
}

export interface CreateTransactionDto {
  productId: string;
  type: TransactionType;
  quantity: number;
  unitPrice: number;
  totalPrice?: number;
  details?: string;
}

export interface UpdateTransactionDto {
  id: string;
  quantity: number;
  unitPrice: number;
  details?: string;
}

export interface TransactionFilters {
  productId?: string;
  transactionType?: TransactionType;
  startDate?: string;
  endDate?: string;
  minAmount?: number;
  maxAmount?: number;
  page: number;
  pageSize: number;
}