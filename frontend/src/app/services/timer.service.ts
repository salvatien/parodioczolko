import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, timer, Subscription } from 'rxjs';
import { map, takeWhile } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TimerService {
  private timeLeftSubject = new BehaviorSubject<number>(60);
  private timerSubscription?: Subscription;
  private gameStartTime = 0;
  private gameDuration = 60; // seconds

  timeLeft$: Observable<number> = this.timeLeftSubject.asObservable();
  
  startTimer(): void {
    this.gameStartTime = Date.now();
    this.timeLeftSubject.next(this.gameDuration);
    
    this.timerSubscription = timer(0, 1000).pipe(
      map(elapsed => this.gameDuration - elapsed),
      takeWhile(timeLeft => timeLeft >= 0)
    ).subscribe({
      next: (timeLeft) => {
        this.timeLeftSubject.next(timeLeft);
      },
      complete: () => {
        this.timeLeftSubject.next(0);
      }
    });
  }

  stopTimer(): void {
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
      this.timerSubscription = undefined;
    }
  }

  resetTimer(): void {
    this.stopTimer();
    this.timeLeftSubject.next(this.gameDuration);
    this.gameStartTime = 0;
  }

  getElapsedTime(): number {
    if (this.gameStartTime === 0) return 0;
    return Math.round((Date.now() - this.gameStartTime) / 1000);
  }

  isTimeUp(): boolean {
    return this.timeLeftSubject.value <= 0;
  }

  formatTime(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
  }
}