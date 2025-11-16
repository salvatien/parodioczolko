import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { Song } from '../../models/song.model';
import { SongService } from '../../services/song.service';
import { TimerService } from '../../services/timer.service';
import { GameStateService } from '../../services/game-state.service';
import { SwipeGestureService, SwipeState } from '../../services/swipe-gesture.service';

@Component({
  selector: 'app-game',
  imports: [CommonModule],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss',
})
export class GameComponent implements OnInit, OnDestroy {
  currentSong: Song | null = null;
  isLoading = false;
  error: string | null = null;
  score = 0;
  totalSongs = 0;
  timeLeft = 60;
  
  // Swipe gesture state from service
  swipeState: SwipeState = { 
    offset: 0, 
    direction: 'none', 
    isActive: false, 
    isSlideOutActive: false, 
    slideDirection: 'none', 
    isNewCard: false 
  };
  
  private timerSubscription?: Subscription;
  private swipeSubscription?: Subscription;

  constructor(
    private songService: SongService,
    private timerService: TimerService,
    private gameStateService: GameStateService,
    private router: Router,
    private swipeGestureService: SwipeGestureService
  ) {}

  ngOnInit(): void {
    // Subscribe to swipe state
    this.swipeSubscription = this.swipeGestureService.swipeState$.subscribe(
      state => this.swipeState = state
    );
    
    this.startGame();
  }

  ngOnDestroy(): void {
    this.timerSubscription?.unsubscribe();
    this.swipeSubscription?.unsubscribe();
    this.timerService.stopTimer();
  }

  startGame(): void {
    // Reset game state
    this.score = 0;
    this.totalSongs = 0;
    
    // Set the timer duration (should already be set by welcome component)
    // But ensure we use the correct duration for display
    this.timeLeft = this.timerService.getGameDuration();
    
    // Start timer
    this.timerService.startTimer();
    
    // Subscribe to timer updates
    this.timerSubscription = this.timerService.timeLeft$.subscribe(timeLeft => {
      this.timeLeft = timeLeft;
      if (timeLeft <= 0) {
        this.endGame();
      }
    });

    // Load first song
    this.loadNextSong();
  }

  endGame(): void {
    // Stop timer
    this.timerService.stopTimer();
    
    // Save game results
    const timeElapsed = this.timerService.getElapsedTime();
    this.gameStateService.setGameResults({
      score: this.score,
      totalSongs: this.totalSongs,
      timeElapsed: timeElapsed
    });

    // Navigate to summary
    this.router.navigate(['/summary']);
  }

  loadNextSong(): void {
    // Check if time is up before loading next song
    if (this.timerService.isTimeUp()) {
      this.endGame();
      return;
    }

    this.isLoading = true;
    this.error = null;

    this.songService.getRandomSong().subscribe({
      next: (song: Song) => {
        this.currentSong = song;
        this.isLoading = false;
        
        // Trigger new card animation
        this.swipeGestureService.triggerNewCardAnimation();
      },
      error: (error: any) => {
        console.error('Error loading song:', error);
        this.error = 'Failed to load song. Please try again.';
        this.isLoading = false;
      }
    });
  }

  onCorrect(): void {
    this.score++;
    this.totalSongs++;
    this.gameStateService.updateScore(this.score, this.totalSongs);
    this.loadNextSong();
  }

  onSkip(): void {
    this.totalSongs++;
    this.gameStateService.updateScore(this.score, this.totalSongs);
    this.loadNextSong();
  }

  resetGame(): void {
    // Stop current timer
    this.timerService.stopTimer();
    
    // Reset timer to 60 seconds
    this.timerService.resetTimer();
    
    // Reset game state service
    this.gameStateService.resetGame();
    
    // Navigate to welcome screen
    this.router.navigate(['/welcome']);
  }

  formatTime(seconds: number): string {
    return this.timerService.formatTime(seconds);
  }

  get successRate(): number {
    return this.totalSongs > 0 ? Math.round((this.score / this.totalSongs) * 100) : 0;
  }

  get isTimeRunningOut(): boolean {
    return this.timeLeft <= 10 && this.timeLeft > 0;
  }

  // Touch gesture handlers using the service
  onTouchStart(event: TouchEvent): void {
    this.swipeGestureService.onTouchStart(event);
  }

  onTouchEnd(event: TouchEvent): void {
    if (!this.currentSong || this.isLoading) return;
    
    const result = this.swipeGestureService.onTouchEnd(event);
    
    if (result.completed && result.direction) {
      // Start slide out animation
      this.swipeGestureService.startSlideOutAnimation(result.direction);
      
      // Wait for slide out animation to complete, then update game state
      setTimeout(() => {
        if (result.direction === 'right') {
          this.onCorrect();
        } else {
          this.onSkip();
        }
      }, 200); // Match the CSS animation duration (faster)
    }
  }

  onTouchMove(event: TouchEvent): void {
    if (!this.currentSong || this.isLoading) return;
    
    const shouldPreventDefault = this.swipeGestureService.onTouchMove(event);
    if (shouldPreventDefault) {
      event.preventDefault();
    }
  }
}
