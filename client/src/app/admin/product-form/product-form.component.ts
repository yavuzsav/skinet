import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { IBrand } from '../../shared/models/brand';
import { IType } from '../../shared/models/productType';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
  @Input() productForm: FormGroup;
  @Input() brands: IBrand[];
  @Input() types: IType[];

  constructor() { }

  ngOnInit(): void {
  }

}
