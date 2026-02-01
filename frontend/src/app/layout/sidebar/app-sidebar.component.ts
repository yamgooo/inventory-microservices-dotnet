import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { HlmSidebarImports } from '@spartan-ng/helm/sidebar';
import { data, sidebarIcons } from './sidebar-data';
import { NavMainComponent } from './components/nav-main.component';

@Component({
   selector: 'app-sidebar',
   standalone: true,
   imports: [
      CommonModule,
      HlmSidebarImports,
      NgIcon,
      NavMainComponent,
   ],
   providers: [provideIcons(sidebarIcons)],
   template: `
    <div hlmSidebarWrapper class="flex-col">
       <div class="flex flex-1">
         <hlm-sidebar side="left" variant="sidebar" collapsible="offcanvas" sidebarContainerClass=" h-100svh">
            <hlm-sidebar-header>
               <ul hlmSidebarMenu>
                  <li hlmSidebarMenuItem>
                     <a hlmSidebarMenuButton size="lg" href="#">
                        <div class="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
                           <ng-icon name="lucideCommand" class="size-4" />
                        </div>
                        <div class="grid flex-1 text-left text-sm leading-tight">
                           <span class="truncate font-semibold">{{data.team.name}}</span>
                           <span class="truncate text-xs">{{data.team.plan}}</span>
                        </div>
                     </a>
                  </li>
               </ul>
            </hlm-sidebar-header>

            <hlm-sidebar-content>
               <spartan-nav-main [items]="data.navMain" />
            </hlm-sidebar-content>

         </hlm-sidebar>
         <ng-content />
       </div>
    </div>
  `,
   host: {
      class: 'block [--header-height:3.5rem]',
   }
})
export class AppSidebarComponent {
   public data = data;
}
