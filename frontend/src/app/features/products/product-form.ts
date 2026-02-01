import { Component, input, effect, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { lucideX } from '@ng-icons/lucide';
import { provideIcons } from '@ng-icons/core';
import { BrnSheetImports } from '@spartan-ng/brain/sheet';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmLabelImports } from '@spartan-ng/helm/label';
import { HlmSheetImports } from '@spartan-ng/helm/sheet';
import { ProductService } from '@core/services/product.service';
import { Product } from '@core/services/interfaces/product.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [BrnSheetImports, HlmSheetImports, HlmButtonImports, HlmInputImports, HlmLabelImports, FormsModule],
  providers: [provideIcons({ lucideX })],
  template: `
    <hlm-sheet-content *brnSheetContent="let ctx" class="w-full sm:max-w-[500px]">
      <hlm-sheet-header>
        <h3 hlmSheetTitle>{{ productId() ? 'Editar' : 'Nuevo' }} Producto</h3>
        <p hlmSheetDescription>Completa la información del producto.</p>
      </hlm-sheet-header>
      
      <div class="grid gap-4 p-4">
        <div class="grid gap-2">
          <label hlmLabel for="name">Nombre</label>
          <input hlmInput id="name" [(ngModel)]="form().name" placeholder="Ej. Monitor Pro" />
        </div>
        <div class="grid gap-2">
          <label hlmLabel for="cat">Categoría</label>
          <input hlmInput id="cat" [(ngModel)]="form().category" />
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div class="grid gap-2">
            <label hlmLabel for="stock">Stock</label>
            <input hlmInput id="stock" type="number" [(ngModel)]="form().stock" />
          </div>
          <div class="grid gap-2">
            <label hlmLabel for="price">Precio</label>
            <input hlmInput id="price" type="number" [(ngModel)]="form().price" />
          </div>
        </div>
      </div>

      <hlm-sheet-footer>
        <button hlmBtn variant="outline" (click)="ctx.close()">Cancelar</button>
        <button hlmBtn (click)="save()">Guardar</button>
      </hlm-sheet-footer>
    </hlm-sheet-content>
  `
})

export class ProductFormComponent {
  private _service = inject(ProductService);
  
  productId = input<string | null>(null);
  form = signal<Partial<Product>>({ name: '', category: '', stock: 0, price: 0 });

  constructor() {
    effect(() => {
      const id = this.productId();
      if (id) {
        this._service.getProductById(id).subscribe(res => {
          if (res.success) this.form.set(res.data);
        });
      } else {
        this.form.set({ name: '', category: '', stock: 0, price: 0 });
      }
    });
  }

  save() {
    console.log('Guardando:', this.form());
    alert('Guardado con éxito (simulado)');
  }
}