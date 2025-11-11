import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TimerService, TIMER_OPTIONS, TimerDuration } from '../../services/timer.service';

@Component({
  selector: 'app-welcome',
  imports: [CommonModule],
  templateUrl: './welcome.component.html',
  styleUrl: './welcome.component.scss',
})
export class WelcomeComponent {
  readonly timerOptions = TIMER_OPTIONS;
  selectedDuration: TimerDuration = 60;

  constructor(
    private router: Router,
    private timerService: TimerService
  ) {
    // Use the last selected duration as default
    this.selectedDuration = this.timerService.getPreferredDuration();
  }

  selectDuration(duration: TimerDuration): void {
    this.selectedDuration = duration;
  }

  startGame(): void {
    this.timerService.setGameDuration(this.selectedDuration);
    this.router.navigate(['/game']);
  }
}