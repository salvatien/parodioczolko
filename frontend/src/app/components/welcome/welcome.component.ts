import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TimerService, TIMER_OPTIONS, TimerDuration } from '../../services/timer.service';
import { SongService } from '../../services/song.service';

@Component({
  selector: 'app-welcome',
  imports: [CommonModule],
  templateUrl: './welcome.component.html',
  styleUrl: './welcome.component.scss',
})
export class WelcomeComponent {
  readonly timerOptions = TIMER_OPTIONS;
  selectedDuration: TimerDuration = 60;
  totalSongs = 0;

  constructor(
    private router: Router,
    private timerService: TimerService,
    private songService: SongService
  ) {
    // Use the last selected duration as default
    this.selectedDuration = this.timerService.getPreferredDuration();
    // Get actual song count
    this.totalSongs = this.songService.getTotalSongs();
  }

  selectDuration(duration: TimerDuration): void {
    this.selectedDuration = duration;
  }

  startGame(): void {
    this.timerService.setGameDuration(this.selectedDuration);
    this.router.navigate(['/game']);
  }
}