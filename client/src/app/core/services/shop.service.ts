import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Product } from '../../shared/models/product';
import { Pagination } from '../../shared/models/pagination';
import { ShopParams } from '../../shared/models/shopParams';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  private http = inject(HttpClient);
  
  baseUrl = "https://localhost:5001/api/";

  types: string[] = [];
  brands: string[] = [];

  getProducts(shopParams: ShopParams){
    let params = new HttpParams()

    params = params.append('pageIndex', shopParams.pageNumber)
    params = params.append('pageSize', shopParams.pageSize)


    if(shopParams.brands.length > 0){
      params = params.append('brands', shopParams.brands.join(','))
    }

    if(shopParams.types.length > 0){
      params = params.append('types', shopParams.types.join(','))
    }

    if(shopParams.sort){
      params = params.append('sort', shopParams.sort)
    }

    if(shopParams.search){
      params = params.append('search', shopParams.search)
    }

    return this.http.get<Pagination<Product>>(this.baseUrl + 'products', {params})
  }

  getProduct(id: number){
    return this.http.get<Product>(this.baseUrl + 'products/' + id)
  }

  getBrands(){
    if(this.brands.length > 0) return;
    return this.http.get<string[]>(this.baseUrl + 'products/brands').subscribe({
      next: (data) => this.brands = data,
      error: (error) => console.error('Error:', error)
    })
  }
  
  getTypes(){
    if(this.types.length > 0) return;
    return this.http.get<string[]>(this.baseUrl + 'products/types').subscribe({
      next: (data) => this.types = data,
      error: (error) => console.error('Error:', error)
    })
  }
}
