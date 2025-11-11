import { Routes } from '@angular/router';
import { WelcomeComponent } from './components/welcome/welcome.component';
import { GameComponent } from './components/game/game.component';
import { GameSummaryComponent } from './components/game-summary/game-summary.component';

export const routes: Routes = [
  { path: '', redirectTo: '/welcome', pathMatch: 'full' },
  { path: 'welcome', component: WelcomeComponent },
  { path: 'game', component: GameComponent },
  { path: 'summary', component: GameSummaryComponent },
  { path: '**', redirectTo: '/welcome' }
];
