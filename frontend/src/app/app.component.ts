import { Component, OnInit, Inject, DOCUMENT } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { GameComponent } from './components/game/game.component';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, GameComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'Parodioczolko';

  constructor(@Inject(DOCUMENT) private document: Document) {}

  ngOnInit() {
    // Add preconnect to API domain for better performance
    this.addApiPreconnect();
  }

  private addApiPreconnect() {
    if (environment.apiUrl) {
      try {
        const apiUrl = new URL(environment.apiUrl);
        const preconnectLink = this.document.createElement('link');
        preconnectLink.rel = 'preconnect';
        preconnectLink.href = apiUrl.origin;
        this.document.head.appendChild(preconnectLink);
      } catch (error) {
        console.warn('Could not add API preconnect link:', error);
      }
    }
  }
}
