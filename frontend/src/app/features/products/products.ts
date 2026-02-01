import { Component, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideClipboardList, lucideFilter, lucidePlus } from '@ng-icons/lucide';
import { HlmInputImports } from '@spartan-ng/helm/input'
import { HlmButtonImports } from '@spartan-ng/helm/button'
import { HlmIconImports } from '@spartan-ng/helm/icon';
import { HlmTableImports } from '@spartan-ng/helm/table';
import { ProductService } from '@core/services/product.service'
import { Product, ProductFilters  } from '@core/services/interfaces/product.model'
import { Subscription } from 'rxjs'
import { HlmItemImports } from '@spartan-ng/helm/item';
import { HlmSpinnerImports } from '@spartan-ng/helm/spinner';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { ProductFormComponent } from '@features/products/product-form';
import { HlmSheetImports } from '@spartan-ng/helm/sheet';
import { BrnSheetImports } from '@spartan-ng/brain/sheet';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [ 
    NgIcon, 
    HlmInputImports, 
    HlmButtonImports, 
    HlmIconImports, 
    HlmItemImports, 
    ProductFormComponent,
    HlmSpinnerImports,
    HlmSheetImports,
    HlmTableImports,
    BrnSheetImports,
    FormsModule],
  templateUrl: './products.html',
  styleUrl: './products.css',
  providers: [
    provideIcons({ lucideClipboardList, lucideFilter, lucidePlus })
  ],
})
export class Products implements OnInit, OnDestroy {

  isLoading = signal(false);
  products = signal<Product[]>([]);
  totalCount = signal(0);
  totalPages = signal(0);
  currentPage = signal(1);

  totalActivo = computed(() => {
    return this.products().reduce((acc, product) => acc + (product.price * product.stock), 0);
  });
  
  nameSearchTerm = '';
  pageSize = 10;

  selectedId = signal<string | null>(null);

  private subscriptions = new Subscription();

  constructor(private productService: ProductService) {}

  loadProducts(): void {
    this.isLoading.set(true);

    const filters: ProductFilters = {
      name: this.nameSearchTerm || undefined,
      page: this.currentPage(),
      pageSize: this.pageSize
    };

    const sub = this.productService.getProducts(filters)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (response) => {
          if (response?.success && response?.data) {
            this.products.set(response.data.items);
            this.totalCount.set(response.data.totalCount);
            this.totalPages.set(response.data.totalPages);
            this.currentPage.set(response.data.page);
          } else {
            this.products.set([]);
          }
        },
        error: (error) => {
          console.error(error);
          this.products.set([]);
        }
      });

    this.subscriptions.add(sub);
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  onSearch(): void {
    this.currentPage.set(1);
    this.loadProducts();
  }

  clearFilters(): void {
    this.nameSearchTerm = '';
    this.currentPage.set(1);
    this.loadProducts();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadProducts();
    }
  }

  previousPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.update(current => current - 1);
      this.loadProducts();
    }
  }

  nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.update(current => current + 1);
      this.loadProducts();
    }
  }

  createProduct(): void {
    this.selectedId.set(null)
  }

  deleteProduct(): void {
    alert('open modal Product')
  }

  editProduct(): void {
    alert('open modal Product')
  }
}