import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Song } from '../models/song.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SongService {
  private readonly apiUrl = environment.apiUrl + '/api/songs';

  constructor(private http: HttpClient) {}

  /**
   * Get a random song for the game
   */
  getRandomSong(): Observable<Song> {
    return this.http.get<Song>(`${this.apiUrl}/random`);
  }

  /**
   * Get all songs (for administration)
   */
  getAllSongs(): Observable<Song[]> {
    return this.http.get<Song[]>(this.apiUrl);
  }

  /**
   * Get a specific song by ID
   */
  getSongById(id: string): Observable<Song> {
    return this.http.get<Song>(`${this.apiUrl}/${id}`);
  }
}
