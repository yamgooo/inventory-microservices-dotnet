import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout';

export const routes: Routes = [
    {
        path: '',
        component: MainLayoutComponent,  
        children: [
            {
                path: '',
                redirectTo: 'dashboard',
                pathMatch: 'full'
            },
            {
                path: 'dashboard',  
                loadComponent: () => import('@features/dashboard/dashboard')
                    .then(m => m.Dashboard)
            },
            {
                path: 'products', 
                loadComponent: () => import('@features/products/products')
                    .then(m => m.Products)
            },
            {
                path: 'transactions', 
                loadComponent: () => import('@features/transactions/transactions')
                    .then(m => m.Transactions)
            }
        ]
    },
    {
        path: '**',
        redirectTo: 'dashboard' 
    }
];