import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '@env/environment';
import { ApiResponse, PagedResult } from '@core/interfaces/api-response.interface';
import { Transaction, CreateTransactionDto, TransactionFilters } from '@core/services/interfaces/transaction.model';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private readonly apiUrl = `${environment.apiUrls.transactions}/Transaction`;

  constructor(private http: HttpClient) {}

  getTransactions(filters: TransactionFilters): Observable<ApiResponse<PagedResult<Transaction>>> {
    return this.http.post<ApiResponse<PagedResult<Transaction>>>(`${this.apiUrl}/filtered`, filters);
  }

  getTransactionById(id: string): Observable<ApiResponse<Transaction>> {
    return this.http.get<ApiResponse<Transaction>>(`${this.apiUrl}/${id}`);
  }

  createTransaction(transaction: CreateTransactionDto): Observable<ApiResponse<Transaction>> {
    return this.http.post<ApiResponse<Transaction>>(`${this.apiUrl}/create`, transaction);
  }
}