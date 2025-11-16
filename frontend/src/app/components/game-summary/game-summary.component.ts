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
  gameDuration = 60;
  
  constructor(
    private router: Router,
    private gameState: GameStateService
  ) {}

  ngOnInit(): void {
    const gameData = this.gameState.getGameResults();
    this.score = gameData.score;
    this.totalSongs = gameData.totalSongs;
    this.accuracy = this.totalSongs > 0 ? Math.round((this.score / this.totalSongs) * 100) : 0;
    this.performance = this.getPerformanceLevel();
  }

  getPerformanceLevel(): string {
    if (this.accuracy >= 80) return 'Uuuuu jiiiis! 🏆';
    if (this.accuracy >= 60) return 'Slay! 🎵';
    if (this.accuracy >= 40) return 'Najs! 👍';
    if (this.accuracy >= 20) return 'Dobrze jak na Morsa! 💪';
    return 'Oj byczku! 🎯';
  }

  playAgain(): void {
    this.gameState.resetGame();
    this.router.navigate(['/welcome']);
  }
}