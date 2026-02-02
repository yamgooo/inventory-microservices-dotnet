import { Component, input, effect, signal, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrnSheetImports } from '@spartan-ng/brain/sheet';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmLabelImports } from '@spartan-ng/helm/label';
import { HlmSheetImports } from '@spartan-ng/helm/sheet';
import { HlmSpinnerImports } from '@spartan-ng/helm/spinner';
import { ProductService } from '@core/services/product.service';
import { Product, CreateProductDto, UpdateProductDto } from '@core/services/interfaces/product.model';
import { toast } from 'ngx-sonner';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
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
          <h3 hlmSheetTitle>{{ productId() ? 'Editar' : 'Nuevo' }} Producto</h3>
          <p hlmSheetDescription>Completa la información del producto.</p>
        </hlm-sheet-header>

        <div class="grid gap-4 p-4">
          <div class="grid gap-2">
            <label hlmLabel for="name">Nombre</label>
            <input [disabled]="!isEdit()" hlmInput id="name" [(ngModel)]="form().name" 
                   placeholder="Ej. Monitor Pro" />
          </div>
          <div class="grid gap-2">
            <label hlmLabel for="description">Descripción</label>
            <input [disabled]="!isEdit()" hlmInput id="description" [(ngModel)]="form().description" />
          </div>
          <div class="grid gap-2">
            <label hlmLabel for="cat">Categoría</label>
            <input [disabled]="!isEdit()" hlmInput id="cat" [(ngModel)]="form().category" />
          </div>
          <div class="grid gap-2">
            <label hlmLabel for="image">Imagen URL</label>
            <input [disabled]="!isEdit()" hlmInput id="image" [(ngModel)]="form().imageUrl" />
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div class="grid gap-2">
              <label hlmLabel for="stock">Stock</label>
              <input disabled="true" hlmInput id="stock" type="number" [(ngModel)]="form().stock" />
            </div>
            <div class="grid gap-2">
              <label hlmLabel for="price">Precio</label>
              <input [disabled]="!isEdit()" hlmInput id="price" type="number" [(ngModel)]="form().price" />
            </div>
          </div>
        </div>

        <hlm-sheet-footer>
          @if (isEdit()) {
            <button hlmBtn variant="outline" (click)="cancel()">Cancelar</button>
            <button hlmBtn (click)="save()" [disabled]="isSaving()">
              {{ isSaving() ? 'Guardando...' : 'Guardar' }}
            </button>
          } @else {
            <button hlmBtn variant="outline" (click)="ctx.close()">Cerrar</button>
            @if (productId()) {
              <button hlmBtn variant="destructive" (click)="deleteProduct()">
                Eliminar
              </button>
            }
            <button hlmBtn (click)="enableEdit()">Editar</button>
          }
        </hlm-sheet-footer>
      }
    </hlm-sheet-content>
  `
})
export class ProductFormComponent {
  private _service = inject(ProductService);

  productId = input<string | null>(null);
  onSaved = output();
  onDeleted = output();

  form = signal<Partial<Product>>({
    name: '',
    description: '',
    category: '',
    imageUrl: '',
    stock: 0,
    price: 0
  });

  isEdit = signal(false);
  isLoading = signal(false);
  isSaving = signal(false);
  originalData: Partial<Product> = {};

  constructor() {
    effect(() => {
      const id = this.productId();
      if (id) {
        this.loadProduct(id);
      } else {
        this.resetForm();
      }
    });
  }

  loadProduct(id: string): void {
    this.isLoading.set(true);
    this._service
      .getProductById(id)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: res => {
          if (res.success) {
            this.form.set(res.data);
            this.originalData = { ...res.data };
            this.isEdit.set(false);
          }
        },
        error: () => toast.error('Error al cargar el producto')
      });
  }

  resetForm(): void {
    this.form.set({
      name: '',
      description: '',
      category: '',
      imageUrl: '',
      stock: 0,
      price: 0
    });
    this.isEdit.set(true);
  }

  cancel(): void {
    if (this.productId()) {
      this.form.set({ ...this.originalData });
      this.isEdit.set(false);
    } else {
      this.resetForm();
    }
  }

  enableEdit(): void {
    this.isEdit.set(true);
  }

  save(): void {
    const data = this.form();

    if (!data.name || !data.category) {
      toast.error('Completa los campos obligatorios');
      return;
    }

    this.isSaving.set(true);

    const id = this.productId();
    const request = id
      ? this._service.updateProduct(id, data as UpdateProductDto)
      : this._service.createProduct(data as CreateProductDto);

    request.pipe(finalize(() => this.isSaving.set(false))).subscribe({
      next: res => {
        if (res.success) {
          toast.success(id ? 'Producto actualizado' : 'Producto creado');
          this.onSaved.emit();
        } else {
          toast.error(res.message || 'Error al guardar');
        }
      },
      error: err => toast.error(err.message || 'Error al guardar')
    });
  }

  deleteProduct(): void {
    const id = this.productId();
    if (!id) return;

    if (!confirm('¿Seguro que deseas eliminar este producto?')) return;

    this._service.deleteProduct(id).subscribe({
      next: () => {
        toast.success('Producto eliminado');
        this.onDeleted.emit();
      },
      error: () => toast.error('Error al eliminar')
    });
  }
}