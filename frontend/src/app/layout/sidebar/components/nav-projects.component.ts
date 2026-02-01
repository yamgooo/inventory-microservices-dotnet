import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgIcon } from '@ng-icons/core';
import { HlmSidebarImports } from '@spartan-ng/helm/sidebar';
import { HlmDropdownMenuImports } from '@spartan-ng/helm/dropdown-menu';

@Component({
  selector: 'spartan-nav-projects',
  standalone: true,
  imports: [CommonModule, RouterModule, HlmSidebarImports, HlmDropdownMenuImports, NgIcon],
  template: `
    <ul hlmSidebarMenu class="gap-2">
      <li hlmSidebarMenuItem>
         <div class="text-xs font-medium text-muted-foreground px-2 py-1.5">Projects</div>
      </li>
      @for (item of projects; track item.name) {
        <li hlmSidebarMenuItem>
           <a hlmSidebarMenuButton [href]="item.url">
             @if (item.icon) {
                <ng-icon [name]="item.icon" class="text-muted-foreground" />
             }
             <span>{{ item.name }}</span>
           </a>
           
           <button hlmSidebarMenuAction [hlmDropdownMenuTrigger]="menu">
             <ng-icon name="lucideMoreHorizontal" />
             <span class="sr-only">More</span>
           </button>
           <ng-template #menu>
             <div hlmDropdownMenuContent class="w-48" align="end">
                <button hlmDropdownMenuItem>
                    <span>View Project</span>
                </button>
                <button hlmDropdownMenuItem>
                    <span>Share Project</span>
                </button>
                <div hlmDropdownMenuSeparator></div>
                <button hlmDropdownMenuItem>
                    <span>Delete Project</span>
                </button>
             </div>
           </ng-template>
        </li>
      }
    </ul>
  `,
})
export class NavProjectsComponent {
  @Input() projects: any[] = [];
}
