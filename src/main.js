// main.js - Entry point for BIHAR SURFERS 3D Endless Railway Runner
import { Game } from './game/Game.js';

window.addEventListener('DOMContentLoaded', () => {
  const gameContainer = document.getElementById('game-container');
  const uiContainer = document.getElementById('ui-layer');

  if (gameContainer && uiContainer) {
    const game = new Game(gameContainer, uiContainer);
    window.biharSurfersGame = game;
  }
});
