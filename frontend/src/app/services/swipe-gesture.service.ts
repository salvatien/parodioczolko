import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface SwipeState {
  offset: number;
  direction: 'left' | 'right' | 'none';
  isActive: boolean;
  isSlideOutActive: boolean;
  slideDirection: 'left' | 'right' | 'none';
  isNewCard: boolean;
}

export interface SwipeConfig {
  minSwipeDistance: number;
  maxVerticalDistance: number;
  maxOffset: number;
  sensitivity: number;
}

@Injectable({
  providedIn: 'root'
})
export class SwipeGestureService {
  private touchStartX = 0;
  private touchStartY = 0;
  
  private swipeStateSubject = new BehaviorSubject<SwipeState>({
    offset: 0,
    direction: 'none',
    isActive: false,
    isSlideOutActive: false,
    slideDirection: 'none',
    isNewCard: false
  });
  
  private config: SwipeConfig = {
    minSwipeDistance: 100,
    maxVerticalDistance: 50,
    maxOffset: 150,
    sensitivity: 0.8
  };

  swipeState$: Observable<SwipeState> = this.swipeStateSubject.asObservable();

  onTouchStart(event: TouchEvent): void {
    if (event.touches.length === 1) {
      this.touchStartX = event.touches[0].clientX;
      this.touchStartY = event.touches[0].clientY;
      
      this.updateSwipeState({
        offset: 0,
        direction: 'none',
        isActive: true
      });
    }
  }

  onTouchMove(event: TouchEvent): boolean {
    const currentState = this.swipeStateSubject.value;
    
    if (!currentState.isActive) return false;

    const touch = event.touches[0];
    const deltaX = touch.clientX - this.touchStartX;
    const deltaY = Math.abs(touch.clientY - this.touchStartY);
    
    // Check if this is a horizontal swipe
    if (Math.abs(deltaX) > deltaY && Math.abs(deltaX) > 20) {
      this.updateSwipeAnimation(deltaX);
      return true; // Indicate that we should prevent default
    }
    
    return false;
  }

  onTouchEnd(event: TouchEvent): { completed: boolean; direction?: 'left' | 'right' } {
    const currentState = this.swipeStateSubject.value;
    
    if (!currentState.isActive) {
      return { completed: false };
    }

    if (event.changedTouches.length === 1) {
      const touchEndX = event.changedTouches[0].clientX;
      const touchEndY = event.changedTouches[0].clientY;
      
      const deltaX = touchEndX - this.touchStartX;
      const deltaY = Math.abs(touchEndY - this.touchStartY);
      
      // Check if it's a valid horizontal swipe
      if (Math.abs(deltaX) >= this.config.minSwipeDistance && deltaY <= this.config.maxVerticalDistance) {
        const direction = deltaX > 0 ? 'right' : 'left';
        this.animateSwipeComplete(direction);
        return { completed: true, direction };
      }
    }
    
    // Reset if swipe wasn't completed
    this.resetSwipeAnimation();
    return { completed: false };
  }

  private updateSwipeAnimation(deltaX: number): void {
    const offset = Math.max(
      -this.config.maxOffset,
      Math.min(this.config.maxOffset, deltaX * this.config.sensitivity)
    );
    
    const direction = Math.abs(deltaX) > 30 
      ? (deltaX > 0 ? 'right' : 'left')
      : 'none';
    
    this.updateSwipeState({
      offset,
      direction,
      isActive: true
    });
  }

  private animateSwipeComplete(direction: 'left' | 'right'): void {
    this.updateSwipeState({
      offset: direction === 'right' ? 300 : -300,
      direction,
      isActive: false
    });
  }

  resetSwipeAnimation(): void {
    this.updateSwipeState({
      offset: 0,
      direction: 'none',
      isActive: false
    });
  }

  startSlideOutAnimation(direction: 'left' | 'right'): void {
    this.updateSwipeState({
      isSlideOutActive: true,
      slideDirection: direction,
      isActive: false,
      offset: 0,
      direction: 'none'
    });
  }

  triggerNewCardAnimation(): void {
    this.updateSwipeState({
      isNewCard: true,
      isSlideOutActive: false,
      slideDirection: 'none',
      offset: 0,
      direction: 'none',
      isActive: false
    });

    // Reset new card animation after it completes
    setTimeout(() => {
      this.updateSwipeState({
        isNewCard: false
      });
    }, 500);
  }

  private updateSwipeState(newState: Partial<SwipeState>): void {
    const currentState = this.swipeStateSubject.value;
    this.swipeStateSubject.next({ ...currentState, ...newState });
  }

  updateConfig(config: Partial<SwipeConfig>): void {
    this.config = { ...this.config, ...config };
  }

  getConfig(): SwipeConfig {
    return { ...this.config };
  }
}