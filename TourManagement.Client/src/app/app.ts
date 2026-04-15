import { Component, inject } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

import { AuthService } from './core/services/auth.service';
import { UserRole } from './core/models/user.model';
import { ToastContainerComponent } from './shared/components/toast/toast-container.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ToastContainerComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  readonly title = 'Tour Management';
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly Role = UserRole;

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
