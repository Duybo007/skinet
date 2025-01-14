import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./layout/header/header.component";
import { HttpClient } from '@angular/common/http';
import { Product } from './shared/models/product';
import { Pagination } from './shared/models/pagination';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit{
  private http = inject(HttpClient);
  
  baseUrl = "https://localhost:5001/api/";
  products: Product[] = [];


  ngOnInit(): void {
    this.getProducts();
  }

  getProducts(){
    this.http.get<Pagination<Product>>(this.baseUrl + 'products').subscribe({
      next: (response) => this.products = response.items,
      error: (error) => console.error(error)
    })
  }
}
