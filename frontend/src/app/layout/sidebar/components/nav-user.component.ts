import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgIcon } from '@ng-icons/core';
import { HlmSidebarImports } from '@spartan-ng/helm/sidebar';
import { HlmDropdownMenuImports } from '@spartan-ng/helm/dropdown-menu';
import { HlmAvatarImports } from '@spartan-ng/helm/avatar';

@Component({
   selector: 'spartan-nav-user',
   standalone: true,
   imports: [CommonModule, HlmSidebarImports, HlmDropdownMenuImports, HlmAvatarImports, NgIcon],
   template: `
    <ul hlmSidebarMenu>
      <li hlmSidebarMenuItem>
         <button hlmSidebarMenuButton size="lg" class="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground" [hlmDropdownMenuTrigger]="menu">
             <hlm-avatar hlmSidebarMenuButtonIcon class="h-8 w-8 rounded-lg">
                <img [src]="user.avatar" [alt]="user.name" />
                <span fallback class="rounded-lg">CN</span>
             </hlm-avatar>
             <div class="grid flex-1 text-left text-sm leading-tight">
                <span class="truncate font-semibold">{{ user.name }}</span>
                <span class="truncate text-xs">{{ user.email }}</span>
             </div>
             <ng-icon name="lucideChevronsUpDown" class="ml-auto size-4" />
         </button>
         
         <ng-template #menu>
            <div hlmDropdownMenu class="w-[--radix-dropdown-menu-trigger-width] min-w-56 rounded-lg" side="bottom" align="end" [sideOffset]="4">
               <div hlmDropdownMenuLabel class="p-0 font-normal">
                  <div class="flex items-center gap-2 px-1 py-1.5 text-left text-sm">
                     <hlm-avatar class="h-8 w-8 rounded-lg">
                        <img [src]="user.avatar" [alt]="user.name" />
                        <span fallback class="rounded-lg">CN</span>
                     </hlm-avatar>
                     <div class="grid flex-1 text-left text-sm leading-tight">
                        <span class="truncate font-semibold">{{ user.name }}</span>
                        <span class="truncate text-xs">{{ user.email }}</span>
                     </div>
                  </div>
               </div>
               <div hlmDropdownMenuSeparator></div>
               <div hlmDropdownMenuGroup>
                  <button hlmDropdownMenuItem>
                     <ng-icon name="lucideSparkles" class="mr-2 h-4 w-4" />
                     <span>Upgrade to Pro</span>
                  </button>
               </div>
               <div hlmDropdownMenuSeparator></div>
               <div hlmDropdownMenuGroup>
                  <button hlmDropdownMenuItem>
                     <ng-icon name="lucideBadgeCheck" class="mr-2 h-4 w-4" />
                     <span>Account</span>
                  </button>
                  <button hlmDropdownMenuItem>
                     <ng-icon name="lucideCreditCard" class="mr-2 h-4 w-4" />
                     <span>Billing</span>
                  </button>
                  <button hlmDropdownMenuItem>
                     <ng-icon name="lucideBell" class="mr-2 h-4 w-4" />
                     <span>Notifications</span>
                  </button>
               </div>
               <div hlmDropdownMenuSeparator></div>
               <button hlmDropdownMenuItem>
                  <ng-icon name="lucideLogOut" class="mr-2 h-4 w-4" />
                  <span>Log out</span>
               </button>
            </div>
         </ng-template>
      </li>
    </ul>
  `,
})
export class NavUserComponent {
   @Input() user: any = {};
}
