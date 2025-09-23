import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { Shop } from './features/shop/shop';
import { ProductDetailsComponent } from './features/shop/product-details/product-details.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'shop',
    component: Shop
  },
  {
    path: 'shop/:id',
    component: ProductDetailsComponent
  },{
    path: '**',
    redirectTo: '',
    pathMatch: 'full'
  }
];
