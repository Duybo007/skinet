import { APP_INITIALIZER, ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { InitService } from './core/services/init.service';
import { lastValueFrom } from 'rxjs';
import { authInterceptor } from './core/interceptors/auth.interceptor';

/**
 * Function to initialize the app by calling InitService before the application starts.
 * It ensures that initialization logic is executed before the app becomes usable.
 */
function initializeApp(initService: InitService) {
  return () =>
    lastValueFrom(initService.init()) // Converts the Observable returned by `init()` into a Promise
      .finally(() => {
        // Remove the initial splash screen once initialization is complete
        const splash = document.getElementById('initial-splash');
        if (splash) {
          splash.remove();
        }
      });
}

/**
 * Application configuration using the `ApplicationConfig` interface.
 * This is a modern approach in Angular for configuring providers in a standalone app.
 */
export const appConfig: ApplicationConfig = {
  providers: [
    // Improves change detection performance by coalescing events
    provideZoneChangeDetection({ eventCoalescing: true }),

    // Provides the routing configuration for the app
    provideRouter(routes),

    // Enables Angular animations asynchronously for improved performance
    provideAnimationsAsync(),

    // Configures the HTTP client with optional interceptors (currently empty array)
    provideHttpClient(withInterceptors([
      authInterceptor
    ])),

    // Ensures that `initializeApp` runs before the app starts
    {
      provide: APP_INITIALIZER,  // Token for running initialization logic
      useFactory: initializeApp, // Factory function to initialize the app
      multi: true,               // Allows multiple initializers to run in parallel
      deps: [InitService]        // Specifies the dependencies needed (InitService)
    }
  ]
};
