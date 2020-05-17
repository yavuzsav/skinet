import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminComponent } from './admin.component';
import { SharedModule } from '../shared/shared.module';
import { AdminRoutingModule } from './admin-routing.module';
import { EditProductComponent } from './edit-product/edit-product.component';
import { ProductFormComponent } from './product-form/product-form.component';

@NgModule({
  declarations: [AdminComponent, EditProductComponent, ProductFormComponent],
  imports: [
    CommonModule,
    SharedModule,
    AdminRoutingModule
  ]
})
export class AdminModule { }
