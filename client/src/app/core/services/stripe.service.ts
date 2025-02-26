import { inject, Injectable } from '@angular/core';
import {ConfirmationToken, loadStripe, Stripe, StripeAddressElement, StripeAddressElementOptions, StripeElements, StripePaymentElement} from '@stripe/stripe-js'
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CartService } from './cart.service';
import { Cart } from '../../shared/models/cart';
import { firstValueFrom, map } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class StripeService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient);
  private cartService = inject(CartService);
  private accountService = inject(AccountService);
  private stripePromise?: Promise<Stripe | null>;
  private elements?: StripeElements;
  private addressElement?: StripeAddressElement;
  private paymentElement?: StripePaymentElement;

  constructor() {
    // Load Stripe instance asynchronously with the public key from environment variables
    this.stripePromise = loadStripe(environment.stripePublicKey);
  }

  // Returns the Stripe instance promise
  getStripeInstance() {
    return this.stripePromise;
  }

  // Initializes Stripe Elements and retrieves the client secret for the payment intent
  async initializeElements() {
    if (!this.elements) {
      const stripe = await this.getStripeInstance();
      if (stripe) {
        const cart = await firstValueFrom(this.createOrUpdatePaymentIntent());
        this.elements = stripe.elements({ clientSecret: cart.clientSecret, appearance: { labels: 'floating' } });
      } else {
        throw new Error('Stripe has not been loaded');
      }
    }
    return this.elements;
  }

  async createConfirmationToken(){
    // steps before create confirmation token
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();

    if(result.error) throw new Error(result.error.message)
    if(stripe){
      return await stripe.createConfirmationToken({elements});
    } else {
      throw new Error('Stripe not available');
    }
  }

  async confirmPayment(confirmationToken: ConfirmationToken){
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();

    if(result.error) throw new Error(result.error.message)
    
    const clientSecret = this.cartService.cart()?.clientSecret

    if(stripe && clientSecret){
      return await stripe.confirmPayment({
        clientSecret: clientSecret,
        confirmParams: {
          confirmation_token: confirmationToken.id
        },
        redirect: 'if_required'
      })
    } else {
      throw new Error('Unable to load Stripe');
    }
  }

  async createPaymentElement(){
    if(!this.paymentElement){
      const elements = await this.initializeElements();
      if(elements){
        this.paymentElement = elements.create('payment')
      } else {
        throw new Error('Stripe Elements have not been initialized');
      }
    }
    return this.paymentElement
  }

  // Creates an Address Element for Stripe with default user information
  async createAddressElement() {
    if (!this.addressElement) {
      const elements = await this.initializeElements();
      if (elements) {
        const user = this.accountService.currentUser();
        let defaultValues: StripeAddressElementOptions['defaultValues'] = {};

        if (user) {
          defaultValues.name = user.firstName + ' ' + user.lastName;
        }

        if (user?.address) {
          defaultValues.address = {
            line1: user.address.line1,
            line2: user.address.line2,
            city: user.address.city,
            state: user.address.state,
            country: user.address.country,
            postal_code: user.address.postalCode
          };
        }

        const options: StripeAddressElementOptions = {
          mode: 'shipping', // Set mode to shipping for address collection
          defaultValues
        };
        this.addressElement = elements.create('address', options);
      } else {
        throw new Error('Stripe Elements instance has not been loaded');
      }
    }
    return this.addressElement;
  }

  // Creates or updates a payment intent by sending a request to the backend
  createOrUpdatePaymentIntent() {
    const cart = this.cartService.cart();
    if (!cart) throw new Error('Problem with cart');
    return this.http.post<Cart>(this.baseUrl + 'payments/' + cart.id, {}).pipe(
      map(cart => {
        this.cartService.setCart(cart); // Update cart in service with the latest data
        return cart;
      })
    );
  }

  // Disposes of Stripe Elements to ensure a fresh instance can be created
  disposeElements() {
    this.elements = undefined;
    this.addressElement = undefined;
    this.paymentElement = undefined;
  }
}
