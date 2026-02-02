import { Component, input, effect, signal, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrnSheetImports } from '@spartan-ng/brain/sheet';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmLabelImports } from '@spartan-ng/helm/label';
import { HlmSheetImports } from '@spartan-ng/helm/sheet';
import { HlmSpinnerImports } from '@spartan-ng/helm/spinner';
import { TransactionService } from '@core/services/transaction.service';
import { ProductService } from '@core/services/product.service';
import { CreateTransactionDto, Transaction } from '@core/services/interfaces/transaction.model';
import { Product } from '@core/services/interfaces/product.model';
import { toast } from 'ngx-sonner';
import { finalize } from 'rxjs/operators';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [
    CommonModule,
    BrnSheetImports,
    HlmSpinnerImports,
    HlmSheetImports,
    HlmButtonImports,
    HlmInputImports,
    HlmLabelImports,
    FormsModule
  ],
  template: `
    <hlm-sheet-content *brnSheetContent="let ctx" class="w-full sm:max-w-[500px]">
      @if (isLoading()) {
        <div class="w-full flex flex-row items-center justify-center h-full gap-2">
          <hlm-spinner />
          <div>Cargando datos...</div>
        </div>
      }

      @if (!isLoading()) {
        <hlm-sheet-header>
          <h3 hlmSheetTitle>{{ isViewMode() ? 'Detalle de' : 'Nueva' }} Transacción</h3>
          <p hlmSheetDescription>
            {{ isViewMode() ? 'Información completa de la transacción' : 'Registra una compra o venta de inventario' }}
          </p>
        </hlm-sheet-header>

        <div class="grid gap-4 p-4">
          <div class="grid gap-2">
            <label hlmLabel for="product">Producto</label>
            @if (isViewMode()) {
              <input hlmInput disabled [value]="viewData()?.productName" />
            } @else {
              <select hlmInput id="product" [(ngModel)]="form.productId" (change)="onProductChange()">
                <option value="">Selecciona un producto</option>
                @for (product of products(); track product.id) {
                  <option [value]="product.id">{{ product.name }} (Stock: {{ product.stock }})</option>
                }
              </select>
            }
          </div>

          <div class="grid gap-2">
            <label hlmLabel for="type">Tipo</label>
            @if (isViewMode()) {
              <span [class]="'px-2 py-1 rounded text-xs w-fit ' + 
                (viewData()?.type === 2 ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800')">
                {{ viewData()?.typeDescription }}
              </span>
            } @else {
              <select hlmInput id="type" [(ngModel)]="form.type">
                <option [ngValue]="2">Compra</option>
                <option [ngValue]="1">Venta</option>
              </select>
            }
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div class="grid gap-2">
              <label hlmLabel for="unitPrice">Precio Unit.</label>
              <input hlmInput id="unitPrice" type="number" 
                     [disabled]="isViewMode()"
                     [(ngModel)]="form.unitPrice" 
                     (input)="calculateTotal()" min="0" step="0.01" />
            </div>
            <div class="grid gap-2">
              <label hlmLabel for="quantity">Cantidad</label>
              <input hlmInput id="quantity" type="number" 
                     [disabled]="isViewMode()"
                     [(ngModel)]="form.quantity" 
                     (input)="calculateTotal()" min="1" />
            </div>
          </div>

          <div class="grid gap-2">
            <label hlmLabel for="total">Total</label>
            <input hlmInput id="total" type="number" 
                   [value]="isViewMode() ? viewData()?.totalPrice : form.totalPrice" disabled />
          </div>

          @if (isViewMode()) {
            <div class="grid grid-cols-2 gap-4">
              <div class="grid gap-2">
                <label class="text-xs font-medium text-gray-500">Stock Actual</label>
                <p class="font-medium">{{ viewData()?.currentStock }} unidades</p>
              </div>
              <div class="grid gap-2">
                <label class="text-xs font-medium text-gray-500">Stock Post-Transacción</label>
                <p class="font-medium">{{ viewData()?.stockAfterTransaction }} unidades</p>
              </div>
            </div>
          }

          <div class="grid gap-2">
            <label hlmLabel for="details">Detalles {{ isViewMode() ? '' : '(opcional)' }}</label>
            <input hlmInput id="details" 
                   [disabled]="isViewMode()"
                   [(ngModel)]="form.details" 
                   placeholder="Ej. Proveedor XYZ" />
          </div>

          @if (isViewMode()) {
            <div class="grid gap-2">
              <label class="text-xs font-medium text-gray-500">Fecha de Transacción</label>
              <p class="text-sm">{{ viewData()?.transactionDate | date:'dd/MM/yyyy HH:mm:ss' }}</p>
            </div>
          }

          @if (!isViewMode() && selectedProduct()) {
            <div class="p-3 bg-blue-50 rounded text-sm">
              <p class="font-medium">{{ selectedProduct()?.name }}</p>
              <p class="text-gray-600">Stock actual: {{ selectedProduct()?.stock }} unidades</p>
            </div>
          }
        </div>

        <hlm-sheet-footer>
          @if (isViewMode()) {
            <button hlmBtn variant="outline" (click)="ctx.close()">Cerrar</button>
          } @else {
            <button hlmBtn variant="outline" (click)="ctx.close()">Cancelar</button>
            <button hlmBtn (click)="save(ctx)" [disabled]="isSaving() || !isValid()">
              {{ isSaving() ? 'Guardando...' : 'Guardar' }}
            </button>
          }
        </hlm-sheet-footer>
      }
    </hlm-sheet-content>
  `
})
export class TransactionFormComponent {
  private _transactionService = inject(TransactionService);
  private _productService = inject(ProductService);

  transactionId = input<string | null>(null);
  onSaved = output();

  isViewMode = signal(false);
  viewData = signal<Transaction | null>(null);

  form = {
    productId: '',
    type: 1,
    quantity: 1,
    unitPrice: 0,
    totalPrice: 0,
    details: ''
  };

  products = signal<Product[]>([]);
  selectedProduct = signal<Product | null>(null);
  isLoading = signal(false);
  isSaving = signal(false);

  constructor() {
    this.loadProducts();

    effect(() => {
      const id = this.transactionId();
      if (id) {
        this.isViewMode.set(true);
        this.loadTransaction(id);
      } else {
        this.isViewMode.set(false);
        this.resetForm();
      }
    });
  }

  loadProducts(): void {
    this._productService.getProducts({ page: 1, pageSize: 100 }).subscribe({
      next: res => {
        if (res.success) {
          this.products.set(res.data.items);
        }
      }
    });
  }

  loadTransaction(id: string): void {
    this.isLoading.set(true);
    this._transactionService
      .getTransactionById(id)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: res => {
          if (res.success) {
            this.viewData.set(res.data);
            this.form = {
              productId: res.data.productId,
              type: res.data.type,
              quantity: res.data.quantity,
              unitPrice: res.data.unitPrice,
              totalPrice: res.data.totalPrice,
              details: res.data.details || ''
            };
          }
        },
        error: () => toast.error('Error al cargar la transacción')
      });
  }

  onProductChange(): void {
    const product = this.products().find(p => p.id === this.form.productId);
    this.selectedProduct.set(product || null);
    if (product) {
      this.form.unitPrice = product.price;
      this.calculateTotal();
    }
  }

  calculateTotal(): void {
    this.form.totalPrice = this.form.quantity * this.form.unitPrice;
  }

  isValid(): boolean {
    return !!this.form.productId && this.form.quantity > 0 && this.form.unitPrice >= 0;
  }

  save(ctx: any): void {
    if (!this.isValid()) {
      toast.error('Completa todos los campos');
      return;
    }

    const product = this.selectedProduct();
    if (this.form.type === 1 && product && product.stock < this.form.quantity) {
      toast.error(`Stock insuficiente. Disponible: ${product.stock}`);
      return;
    }

    this.isSaving.set(true);

    const dto: CreateTransactionDto = {
      productId: this.form.productId,
      type: this.form.type,
      quantity: this.form.quantity,
      unitPrice: this.form.unitPrice,
      totalPrice: this.form.totalPrice,
      details: this.form.details.trim() || ''
    };

    this._transactionService
      .createTransaction(dto)
      .pipe(finalize(() => this.isSaving.set(false)))
      .subscribe({
        next: res => {
          if (res.success) {
            this.onSaved.emit();
            this.reloadProductsAfterSave();
            
            setTimeout(() => {
              toast.success('Transacción creada correctamente');
            }, 100);
            
            ctx.close();
            this.resetForm();
          } else {
            toast.error(res.message || 'Error al crear transacción');
          }
        },
        error: () => toast.error('Error al crear transacción')
      });
  }

  reloadProductsAfterSave(): void {
    this._productService.getProducts({ page: 1, pageSize: 100 }).subscribe({
      next: res => {
        if (res.success) {
          this.products.set(res.data.items);
        }
      }
    });
  }

  resetForm(): void {
    this.form = {
      productId: '',
      type: 1,
      quantity: 1,
      unitPrice: 0,
      totalPrice: 0,
      details: ''
    };
    this.selectedProduct.set(null);
  }
}