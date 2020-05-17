import { Component, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { ShopService } from '../../shop/shop.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { IProduct } from '../../shared/models/product';
import { IBrand } from '../../shared/models/brand';
import { IType } from '../../shared/models/productType';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-edit-product',
  templateUrl: './edit-product.component.html',
  styleUrls: ['./edit-product.component.scss']
})
export class EditProductComponent implements OnInit {
  productForm: FormGroup;
  product: IProduct;
  brands: IBrand[];
  types: IType[];

  constructor(private adminService: AdminService,
              private shopService: ShopService,
              private activatedRoute: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder) {
  }

  ngOnInit(): void {
    this.createEditProductForm();

    const brands = this.getBrands();
    const types = this.getTypes();

    forkJoin([types, brands]).subscribe(results => {
      this.types = results[0];
      this.brands = results[1];
    }, error => {
      console.log(error);
    }, () => {
      if (this.activatedRoute.snapshot.url[0].path === 'edit') {
        this.loadProduct();
      }
    });
  }

  createEditProductForm() {
    this.productForm = this.fb.group({
      name: [null, Validators.required],
      description: [null, Validators.required],
      price: [
        null,
        [Validators.required, Validators.min(0.01), Validators.pattern('^\\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(\\.[0-9][0-9])?$')]
      ],
      productTypeId: [null, Validators.required],
      productBrandId: [null, Validators.required]
    });
  }

  loadProduct() {
    const productId = +this.activatedRoute.snapshot.paramMap.get('id');
    this.shopService.getProduct(productId).subscribe((response: any) => {
      const productBrandId = this.brands && this.brands.find(x => x.name === response.productBrand).id;
      const productTypeId = this.types && this.types.find(x => x.name === response.productType).id;
      this.product = response;

      this.productForm.patchValue({...response, productBrandId, productTypeId});
    });
  }

  getBrands() {
    return this.shopService.getBrands();
  }

  getTypes() {
    return this.shopService.getTypes();
  }

}
