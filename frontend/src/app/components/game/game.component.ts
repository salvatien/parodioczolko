import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Song } from '../../models/song.model';
import { SongService } from '../../services/song.service';

@Component({
  selector: 'app-game',
  imports: [CommonModule],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss',
})
export class GameComponent implements OnInit {
  currentSong: Song | null = null;
  isLoading = false;
  error: string | null = null;
  score = 0;
  totalSongs = 0;

  constructor(private songService: SongService) {}

  ngOnInit(): void {
    this.loadNextSong();
  }

  loadNextSong(): void {
    this.isLoading = true;
    this.error = null;

    this.songService.getRandomSong().subscribe({
      next: (song: Song) => {
        this.currentSong = song;
        this.isLoading = false;
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
    this.loadNextSong();
  }

  onSkip(): void {
    this.totalSongs++;
    this.loadNextSong();
  }

  resetGame(): void {
    this.score = 0;
    this.totalSongs = 0;
    this.loadNextSong();
  }

  get successRate(): number {
    return this.totalSongs > 0 ? Math.round((this.score / this.totalSongs) * 100) : 0;
  }
}
