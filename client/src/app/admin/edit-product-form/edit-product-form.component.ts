import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { IBrand } from '../../shared/models/brand';
import { IType } from '../../shared/models/productType';
import { ActivatedRoute } from '@angular/router';
import { AdminService } from '../admin.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-edit-product-form',
  templateUrl: './edit-product-form.component.html',
  styleUrls: ['./edit-product-form.component.scss']
})
export class EditProductFormComponent implements OnInit {
  @Input() productForm: FormGroup;
  @Input() brands: IBrand[];
  @Input() types: IType[];
  loading = false;
  nextPage = false;

  constructor(private route: ActivatedRoute, private adminService: AdminService, private toastrService: ToastrService) {
  }

  ngOnInit(): void {
    if (this.route.snapshot.url[0].path === 'edit') {
      this.nextPage = true;
    }
  }

  onSubmit() {
    this.loading = true;
    if (this.route.snapshot.url[0].path === 'edit') {
      const productId = +this.route.snapshot.paramMap.get('id');
      const productPrice: number = +this.productForm.get('price').value;
      this.adminService.updateProduct(productId, {
        ...this.productForm.value,
        price: productPrice
      }).subscribe((response: any) => {
        this.toastrService.info('Updated Product');
        this.nextPage = true;
      }, error => {
        this.toastrService.error(error);
      });
    } else {
      const productPrice: number = +this.productForm.get('price').value;
      this.adminService.createProduct({
        ...this.productForm.value,
        price: productPrice
      }).subscribe((response: any) => {
        this.toastrService.info('Created Product');
        this.nextPage = true;
      }, error => {
        this.toastrService.error(error);
      });

      this.loading = false;
    }
  }
}
