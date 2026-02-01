import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http'
import { environment } from '@env/environment';
import { ApiResponse, PagedResult, } from '@core/interfaces/api-response.interface'
import { ProductFilters, Product, CreateProductDto, UpdateProductDto, } from '@core/services/interfaces/product.model'

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private readonly apiUrl = `${environment.apiUrls.products}/Products`;

  constructor(private http: HttpClient) { }

  GetApiUrl() : string
  {
    return this.apiUrl;
  }

  getProducts(filters: ProductFilters): Observable<ApiResponse<PagedResult<Product>>> {
    return this.http.post<ApiResponse<PagedResult<Product>>>(
      `${this.apiUrl}/filter`,
      filters
    );
  }

  getProductById(id: string): Observable<ApiResponse<Product>> {
    return this.http.get<ApiResponse<Product>>(
      `${this.apiUrl}/${id}`
    )
  }

  createProduct(product: CreateProductDto): Observable<ApiResponse<Product>> {
    return this.http.post<ApiResponse<Product>>(
      `${this.apiUrl}`,
      product
    )
  }

  updateProduct(id: string, product: UpdateProductDto): Observable<ApiResponse<Product>> {
    return this.http.put<ApiResponse<Product>>(
      `${this.apiUrl}/${id}`,
      product
    )
  }

  
  deleteProduct(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    )
  }

}

