export interface Song {
  id: string;
  partitionKey: string;
  artist: string;
  name: string;
  year: number;
  etag?: string;
}