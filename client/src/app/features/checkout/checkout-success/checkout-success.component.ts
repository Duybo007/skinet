import { Component, inject, OnDestroy } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { SignalrService } from '../../../core/services/signalr.service';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { CurrencyPipe, DatePipe, NgIf } from '@angular/common';
import { PaymentCardPipe } from '../../../shared/pipes/payment-card.pipe';
import { AddressPipe } from '../../../shared/pipes/address.pipe';
import { OrderService } from '../../../core/services/order.service';

@Component({
  selector: 'app-checkout-success',
  standalone: true,
  imports: [
    MatButton,
    RouterLink,
    MatProgressSpinner,
    DatePipe,
    CurrencyPipe,
    PaymentCardPipe,
    AddressPipe,
    NgIf
  ],
  templateUrl: './checkout-success.component.html',
  styleUrl: './checkout-success.component.scss'
})
export class CheckoutSuccessComponent implements OnDestroy{
  signalRService = inject(SignalrService)
  orderService = inject(OrderService)
  
  ngOnDestroy(): void {
    this.orderService.orderComplete = false;
    this.signalRService.orderSignal.set(null);
  }
}
