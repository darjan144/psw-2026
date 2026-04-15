import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Router } from '@angular/router';

import { errorInterceptor } from './error.interceptor';
import { ToastService } from '../services/toast.service';

describe('errorInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let toast: ToastService;
  let router: { navigate: ReturnType<typeof vi.fn> };
  let removeItemSpy: ReturnType<typeof vi.spyOn>;

  beforeEach(() => {
    router = { navigate: vi.fn() };
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
        { provide: Router, useValue: router },
      ],
    });
    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    toast = TestBed.inject(ToastService);
    removeItemSpy = vi.spyOn(Storage.prototype, 'removeItem');
  });

  afterEach(() => {
    httpMock.verify();
    removeItemSpy.mockRestore();
  });

  it('shows error toast on 400 with body message', () => {
    const spy = vi.spyOn(toast, 'error');
    http.get('/api/test').subscribe({ next: () => {}, error: () => {} });
    httpMock.expectOne('/api/test').flush({ message: 'Bad request body' }, { status: 400, statusText: 'Bad Request' });
    expect(spy).toHaveBeenCalledWith('Bad request body');
  });

  it('shows error toast on 500', () => {
    const spy = vi.spyOn(toast, 'error');
    http.get('/api/test').subscribe({ next: () => {}, error: () => {} });
    httpMock.expectOne('/api/test').flush('boom', { status: 500, statusText: 'Server Error' });
    expect(spy).toHaveBeenCalled();
  });

  it('clears token and redirects to /login on 401', () => {
    http.get('/api/test').subscribe({ next: () => {}, error: () => {} });
    httpMock.expectOne('/api/test').flush('unauthorized', { status: 401, statusText: 'Unauthorized' });
    expect(removeItemSpy).toHaveBeenCalledWith('token');
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });
});
