import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { GameStateService } from '../../services/game-state.service';

@Component({
  selector: 'app-game-summary',
  imports: [CommonModule],
  templateUrl: './game-summary.component.html',
  styleUrl: './game-summary.component.scss',
})
export class GameSummaryComponent implements OnInit {
  score = 0;
  totalSongs = 0;
  timeElapsed = 0;
  accuracy = 0;
  performance = '';
  
  constructor(
    private router: Router,
    private gameState: GameStateService
  ) {}

  ngOnInit(): void {
    const gameData = this.gameState.getGameResults();
    this.score = gameData.score;
    this.totalSongs = gameData.totalSongs;
    this.timeElapsed = gameData.timeElapsed;
    this.accuracy = this.totalSongs > 0 ? Math.round((this.score / this.totalSongs) * 100) : 0;
    this.performance = this.getPerformanceLevel();
  }

  getPerformanceLevel(): string {
    if (this.accuracy >= 80) return 'Excellent! 🏆';
    if (this.accuracy >= 60) return 'Great! 🎵';
    if (this.accuracy >= 40) return 'Good! 👍';
    if (this.accuracy >= 20) return 'Keep Trying! 💪';
    return 'Practice Makes Perfect! 🎯';
  }

  playAgain(): void {
    this.gameState.resetGame();
    this.router.navigate(['/welcome']);
  }

  shareResults(): void {
    const text = `I scored ${this.score}/${this.totalSongs} (${this.accuracy}%) in Parodioczołko! 🎵 Can you beat my score?`;
    if (navigator.share) {
      navigator.share({
        title: 'Parodioczołko Score',
        text: text
      });
    } else {
      navigator.clipboard.writeText(text).then(() => {
        alert('Score copied to clipboard!');
      });
    }
  }
}