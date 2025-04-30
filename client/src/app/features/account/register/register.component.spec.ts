import { ComponentFixture, TestBed, fakeAsync, tick, flush } from '@angular/core/testing';
import { RegisterComponent } from './register.component';
import { AccountService } from '../../../core/services/account.service';
import { Router } from '@angular/router';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { of, throwError } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let mockAccountService: any;
  let mockRouter: any;
  let mockSnackbar: any;

  beforeEach(() => {
    mockAccountService = jasmine.createSpyObj('AccountService', ['register']);
    mockRouter = jasmine.createSpyObj('Router', ['navigateByUrl']);
    mockSnackbar = jasmine.createSpyObj('SnackbarService', ['success']);

    TestBed.configureTestingModule({
      imports: [RegisterComponent, ReactiveFormsModule, NoopAnimationsModule],
      providers: [
        { provide: AccountService, useValue: mockAccountService },
        { provide: Router, useValue: mockRouter },
        { provide: SnackbarService, useValue: mockSnackbar }
      ]
    });

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should display password validation errors when password does not qualify', fakeAsync(() => {
    component.registerForm.setValue({
      firstName: 'Duy',
      lastName: 'Ngo',
      email: 'duybo95@gmail.com',
      password: 'abc'
    });

    const errorResponse = {
      error: {
        errors: {
          Password: [
            "Passwords must be at least 6 characters.",
            "Passwords must have at least one lowercase ('a'-'z').",
            "Passwords must have at least one uppercase ('A'-'Z').",
            "Passwords must have at least one non alphanumeric character."
          ]
        }
      }
    };
    mockAccountService.register.and.returnValue(throwError(() => errorResponse));

    component.onSubmit();
    tick();
    fixture.detectChanges();

    expect(component.validationErrors).toContain("Passwords must be at least 6 characters.");
    expect(component.validationErrors).toContain("Passwords must have at least one lowercase ('a'-'z').");
    expect(component.validationErrors).toContain("Passwords must have at least one uppercase ('A'-'Z').");
    expect(component.validationErrors).toContain("Passwords must have at least one non alphanumeric character.");

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain("Passwords must be at least 6 characters.");
    expect(compiled.textContent).toContain("Passwords must have at least one lowercase ('a'-'z').");
    expect(compiled.textContent).toContain("Passwords must have at least one uppercase ('A'-'Z').");
    expect(compiled.textContent).toContain("Passwords must have at least one non alphanumeric character.");
    flush();
  }));

  it('should navigate to login page on successful registration', fakeAsync(() => {
    component.registerForm.setValue({
      firstName: 'Duy',
      lastName: 'Ngo',
      email: 'duybo95@gmail.com',
      password: 'Valid1!'
    });

    mockAccountService.register.and.returnValue(of({}));

    component.onSubmit();
    tick();

    expect(mockSnackbar.success).toHaveBeenCalledWith("Registration successful - you can now login");
    expect(mockRouter.navigateByUrl).toHaveBeenCalledWith('/account/login');
    flush();
  }));
});