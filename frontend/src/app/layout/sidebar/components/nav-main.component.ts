import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgIcon } from '@ng-icons/core';
import { HlmCollapsibleImports } from '@spartan-ng/helm/collapsible';
import { HlmSidebarImports } from '@spartan-ng/helm/sidebar';

@Component({
  selector: 'spartan-nav-main',
  standalone: true,
  imports: [CommonModule, RouterModule, HlmCollapsibleImports, HlmSidebarImports, NgIcon],
  template: `
    <ul hlmSidebarMenu>
      @for (item of items; track item.title) {
        <hlm-collapsible [expanded]="item.isActive" asChild class="group/collapsible">
           <li hlmSidebarMenuItem>
             <button hlmCollapsibleTrigger hlmSidebarMenuButton [tooltip]="item.title">
               @if (item.icon) {
                 <ng-icon [name]="item.icon" />
               }
               <span>{{ item.title }}</span>
               <ng-icon name="lucideChevronRight" class="ml-auto transition-transform duration-200 group-data-[state=open]/collapsible:rotate-90" />
             </button>
             <hlm-collapsible-content>
               <ul hlmSidebarMenuSub>
                 @for (subItem of item.items; track subItem.title) {
                   <li hlmSidebarMenuSubItem>
                     <a hlmSidebarMenuSubButton [href]="subItem.url">
                       <span>{{ subItem.title }}</span>
                     </a>
                   </li>
                 }
               </ul>
             </hlm-collapsible-content>
           </li>
        </hlm-collapsible>
      }
    </ul>
  `,
})
export class NavMainComponent {
  @Input() items: any[] = [];
}
