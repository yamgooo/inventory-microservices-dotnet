import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HlmSidebarImports } from '@spartan-ng/helm/sidebar';
import { AppSidebarComponent } from '../sidebar/app-sidebar.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HlmSidebarImports, AppSidebarComponent],
  template: `
    <app-sidebar>
        <main hlmSidebarInset>
             <div class="flex flex-1 flex-col gap-4 p-4">
                 <router-outlet />
             </div>
        </main>
    </app-sidebar>
  `,
})
export class MainLayoutComponent {
}
