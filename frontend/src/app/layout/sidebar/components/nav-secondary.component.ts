import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgIcon } from '@ng-icons/core';
import { HlmSidebarImports } from '@spartan-ng/helm/sidebar';

@Component({
    selector: 'spartan-nav-secondary',
    standalone: true,
    imports: [CommonModule, RouterModule, HlmSidebarImports, NgIcon],
    template: `
    <ul hlmSidebarMenu>
      @for (item of items; track item.title) {
        <li hlmSidebarMenuItem>
           <a hlmSidebarMenuButton [href]="item.url" size="sm">
             @if (item.icon) {
               <ng-icon [name]="item.icon" />
             }
             <span>{{ item.title }}</span>
           </a>
        </li>
      }
    </ul>
  `,
})
export class NavSecondaryComponent {
    @Input() items: any[] = [];
}
