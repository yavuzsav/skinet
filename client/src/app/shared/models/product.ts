export interface IProduct {
    id: number;
    name: string;
    description: string;
    price: number;
    pictureUrl: string;
    productType: string;
    productBrand: string;
    photos: IProductPhotos[];
}

export interface IProductToCreate {
  name: string;
  description: string;
  price: number;
  // pictureUrl?: string;
  productTypeId: number;
  productBrandId: number;
}

export interface IProductPhotos {
  id: number;
  url: string;
  isMain: boolean;
}
