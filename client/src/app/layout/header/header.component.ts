import { Component, inject } from '@angular/core';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatBadgeModule} from '@angular/material/badge'; 
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { AccountService } from '../../core/services/account.service';
import {MatMenuModule} from '@angular/material/menu';
import { MatDivider } from '@angular/material/divider';
import { BusyService } from '../../core/services/busy.service';
import {MatProgressBarModule} from '@angular/material/progress-bar';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    MatIconModule,
    MatButtonModule,
    MatBadgeModule,
    RouterLink,
    RouterLinkActive,
    MatMenuModule,
    MatDivider,
    MatProgressBarModule
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  cartService = inject(CartService)
  accountService = inject(AccountService)
  busyService = inject(BusyService)
  private router = inject(Router)

  logout(){
    this.accountService.logout().subscribe({
      next: () => {
        this.accountService.currentUser.set(null)
        this.router.navigateByUrl('/')
      }
    })
  }
}
