import { Component, Input, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { ToastrService } from 'ngx-toastr';
import { IProduct } from '../../shared/models/product';
import { HttpEvent, HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-edit-product-photos',
  templateUrl: './edit-product-photos.component.html',
  styleUrls: ['./edit-product-photos.component.scss']
})
export class EditProductPhotosComponent implements OnInit {
  @Input() product: IProduct;
  progress = 0;
  addPhotoMode = false;

  constructor(private adminService: AdminService, private toastrService: ToastrService) { }

  ngOnInit(): void {
  }

  addPhotoModeToggle() {
    this.addPhotoMode = !this.addPhotoMode;
  }

  uploadFile(file: File) {
    this.adminService.AddProductPhoto(this.product.id, file).subscribe((event: HttpEvent<any>) => {
      switch (event.type) {
        case HttpEventType.UploadProgress:
          this.progress = Math.round(event.loaded / event.total * 100);
          break;
        case HttpEventType.Response:
          this.product = event.body;
          setTimeout(() => {
            this.progress = 0;
            this.addPhotoMode = false;
            this.toastrService.success('Photo Added!');
          }, 1500);
      }
    }, error => {
      if (error.errors) {
        this.toastrService.error(error.errors[0]);
      } else {
        this.toastrService.error('Problem uploading image');
      }
      this.progress = 0;
    });
  }

  deletePhoto(photoId: number) {
    this.adminService.deleteProductPhoto(this.product.id, photoId).subscribe(() => {
      const photoIndex = this.product.photos.findIndex(x => x.id === photoId);
      this.product.photos.splice(photoIndex, 1);
      this.toastrService.success('Deleted photo');
    }, error => {
      this.toastrService.error('Problem deleting photo');
    });
  }

  setMainPhoto(photoId: number) {
    this.adminService.setMainPhoto(this.product.id, photoId).subscribe((product: IProduct) => {
      this.product = product;
    }, error => {
      this.toastrService.error(error);
    });
  }

}
