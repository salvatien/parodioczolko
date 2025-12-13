import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Song } from '../models/song.model';
import songsData from '../../assets/songs.json';

@Injectable({
  providedIn: 'root',
})
export class SongService {
  private readonly songs: Song[] = songsData;

  /**
   * Get a random song for the game
   */
  getRandomSong(): Observable<Song> {
    const randomIndex = Math.floor(Math.random() * this.songs.length);
    const randomSong = this.songs[randomIndex];
    return of(randomSong);
  }

  /**
   * Get all songs (for administration)
   */
  getAllSongs(): Observable<Song[]> {
    return of(this.songs);
  }

  /**
   * Get a specific song by ID
   */
  getSongById(id: string): Observable<Song | null> {
    const song = this.songs.find(s => s.id === id);
    return of(song || null);
  }

  /**
   * Get total number of songs available
   */
  getTotalSongs(): number {
    return this.songs.length;
  }
}
