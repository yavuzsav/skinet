import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IProductToCreate } from '../shared/models/product';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createProduct(product: IProductToCreate) {
    return this.http.post(this.baseUrl + 'products', product);
  }

  updateProduct(id: number, product: IProductToCreate) {
    return this.http.put(this.baseUrl + 'products/' + id, product);
  }

  deleteProduct(id: number) {
    return this.http.delete(this.baseUrl + 'products/' + id );
  }
}
