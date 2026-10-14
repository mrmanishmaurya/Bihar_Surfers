// InputManager.js - Desktop Keyboard & Mobile Touch Swipe Gesture Parser
export class InputManager {
  constructor(onAction) {
    this.onAction = onAction; // Callback: 'left', 'right', 'jump', 'slide', 'pause'

    this.touchStartX = 0;
    this.touchStartY = 0;
    this.minSwipeDist = 30; // Min px for swipe gesture

    this.bindKeyboard();
    this.bindTouch();
  }

  bindKeyboard() {
    window.addEventListener('keydown', (e) => {
      switch (e.key) {
        case 'a':
        case 'A':
        case 'ArrowLeft':
          this.onAction('left');
          break;

        case 'd':
        case 'D':
        case 'ArrowRight':
          this.onAction('right');
          break;

        case 'w':
        case 'W':
        case 'ArrowUp':
        case ' ':
          this.onAction('jump');
          break;

        case 's':
        case 'S':
        case 'ArrowDown':
          this.onAction('slide');
          break;

        case 'Escape':
        case 'p':
        case 'P':
          this.onAction('pause');
          break;
      }
    });
  }

  bindTouch() {
    window.addEventListener('touchstart', (e) => {
      if (e.touches.length > 0) {
        this.touchStartX = e.touches[0].clientX;
        this.touchStartY = e.touches[0].clientY;
      }
    }, { passive: true });

    window.addEventListener('touchend', (e) => {
      if (e.changedTouches.length > 0) {
        const touchEndX = e.changedTouches[0].clientX;
        const touchEndY = e.changedTouches[0].clientY;

        const diffX = touchEndX - this.touchStartX;
        const diffY = touchEndY - this.touchStartY;

        if (Math.abs(diffX) > Math.abs(diffY)) {
          // Horizontal Swipe
          if (Math.abs(diffX) > this.minSwipeDist) {
            if (diffX > 0) this.onAction('right');
            else this.onAction('left');
          }
        } else {
          // Vertical Swipe
          if (Math.abs(diffY) > this.minSwipeDist) {
            if (diffY < 0) this.onAction('jump');
            else this.onAction('slide');
          }
        }
      }
    }, { passive: true });
  }
}
