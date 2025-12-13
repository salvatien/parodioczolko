import { Injectable } from '@angular/core';

export interface GameResults {
  score: number;
  totalSongs: number;
  timeElapsed: number;
}

@Injectable({
  providedIn: 'root'
})
export class GameStateService {
  private gameResults: GameResults = {
    score: 0,
    totalSongs: 0,
    timeElapsed: 0
  };

  setGameResults(results: GameResults): void {
    this.gameResults = { ...results };
  }

  getGameResults(): GameResults {
    return { ...this.gameResults };
  }

  resetGame(): void {
    this.gameResults = {
      score: 0,
      totalSongs: 0,
      timeElapsed: 0
    };
  }

  updateScore(score: number, totalSongs: number): void {
    this.gameResults.score = score;
    this.gameResults.totalSongs = totalSongs;
  }

  setTimeElapsed(timeElapsed: number): void {
    this.gameResults.timeElapsed = timeElapsed;
  }
}