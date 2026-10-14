// AudioManager.js - Synthesized Web Audio API sound engine for Bihar Surfers

export class AudioManager {
  constructor(saveData) {
    this.saveData = saveData;
    this.ctx = null;
    this.bgMusicTimer = null;
    this.isBgmPlaying = false;
    this.tempo = 128;
  }

  init() {
    if (!this.ctx) {
      const AudioCtx = window.AudioContext || window.webkitAudioContext;
      if (AudioCtx) {
        this.ctx = new AudioCtx();
      }
    }
    if (this.ctx && this.ctx.state === 'suspended') {
      this.ctx.resume();
    }
  }

  playFootstep() {
    if (!this.saveData.soundEnabled || !this.ctx) return;
    this.init();

    const now = this.ctx.currentTime;
    const osc = this.ctx.createOscillator();
    const gain = this.ctx.createGain();

    osc.type = 'triangle';
    osc.frequency.setValueAtTime(120, now);
    osc.frequency.exponentialRampToValueAtTime(40, now + 0.04);

    gain.gain.setValueAtTime(0.08, now);
    gain.gain.exponentialRampToValueAtTime(0.001, now + 0.05);

    osc.connect(gain);
    gain.connect(this.ctx.destination);

    osc.start(now);
    osc.stop(now + 0.05);
  }

  playCoin(comboCount = 1) {
    if (!this.saveData.soundEnabled || !this.ctx) return;
    this.init();

    const now = this.ctx.currentTime;
    const osc = this.ctx.createOscillator();
    const gain = this.ctx.createGain();

    const baseFreq = 987 + Math.min(comboCount, 8) * 120;
    osc.type = 'sine';
    osc.frequency.setValueAtTime(baseFreq, now);
    osc.frequency.exponentialRampToValueAtTime(baseFreq * 1.33, now + 0.08);

    gain.gain.setValueAtTime(0.3, now);
    gain.gain.exponentialRampToValueAtTime(0.001, now + 0.2);

    osc.connect(gain);
    gain.connect(this.ctx.destination);

    osc.start(now);
    osc.stop(now + 0.2);
  }

  playNearMiss() {
    if (!this.saveData.soundEnabled || !this.ctx) return;
    this.init();

    const now = this.ctx.currentTime;
    const osc = this.ctx.createOscillator();
    const gain = this.ctx.createGain();

    osc.type = 'sine';
    osc.frequency.setValueAtTime(600, now);
    osc.frequency.exponentialRampToValueAtTime(1200, now + 0.12);

    gain.gain.setValueAtTime(0.25, now);
    gain.gain.exponentialRampToValueAtTime(0.001, now + 0.15);

    osc.connect(gain);
    gain.connect(this.ctx.destination);

    osc.start(now);
    osc.stop(now + 0.15);
  }

  playJump() {
    if (!this.saveData.soundEnabled || !this.ctx) return;
    this.init();

    const now = this.ctx.currentTime;
    const osc = this.ctx.createOscillator();
    const gain = this.ctx.createGain();

    osc.type = 'sine';
    osc.frequency.setValueAtTime(150, now);
    osc.frequency.exponentialRampToValueAtTime(500, now + 0.15);

    gain.gain.setValueAtTime(0.35, now);
    gain.gain.exponentialRampToValueAtTime(0.001, now + 0.18);

    osc.connect(gain);
    gain.connect(this.ctx.destination);

    osc.start(now);
    osc.stop(now + 0.18);
  }

  playSlide() {
    if (!this.saveData.soundEnabled || !this.ctx) return;
    this.init();

    const now = this.ctx.currentTime;
    const osc = this.ctx.createOscillator();
    const gain = this.ctx.createGain();

    osc.type = 'triangle';
    osc.frequency.setValueAtTime(350, now);
    osc.frequency.exponentialRampToValueAtTime(100, now + 0.2);

    gain.gain.setValueAtTime(0.3, now);
    gain.gain.exponentialRampToValueAtTime(0.001, now + 0.22);

    osc.connect(gain);
    gain.connect(this.ctx.destination);

    osc.start(now);
    osc.stop(now + 0.22);
  }

  playTrainHorn() {
    if (!this.saveData.soundEnabled || !this.ctx) return;
    this.init();

    const now = this.ctx.currentTime;
    [311, 370].forEach(freq => {
      const osc = this.ctx.createOscillator();
      const gain = this.ctx.createGain();
      osc.type = 'sawtooth';
      osc.frequency.setValueAtTime(freq, now);

      gain.gain.setValueAtTime(0, now);
      gain.gain.linearRampToValueAtTime(0.2, now + 0.05);
      gain.gain.setValueAtTime(0.2, now + 0.5);
      gain.gain.linearRampToValueAtTime(0.001, now + 0.7);

      osc.connect(gain);
      gain.connect(this.ctx.destination);
      osc.start(now);
      osc.stop(now + 0.7);
    });
  }

  playPowerUp() {
    if (!this.saveData.soundEnabled || !this.ctx) return;
    this.init();

    const now = this.ctx.currentTime;
    const freqs = [440, 554, 659, 880, 1108];
    freqs.forEach((f, idx) => {
      const osc = this.ctx.createOscillator();
      const gain = this.ctx.createGain();
      osc.type = 'triangle';
      osc.frequency.setValueAtTime(f, now + idx * 0.05);

      gain.gain.setValueAtTime(0.3, now + idx * 0.05);
      gain.gain.exponentialRampToValueAtTime(0.001, now + idx * 0.05 + 0.3);

      osc.connect(gain);
      gain.connect(this.ctx.destination);
      osc.start(now + idx * 0.05);
      osc.stop(now + idx * 0.05 + 0.3);
    });
  }

  playCrash() {
    if (!this.saveData.soundEnabled || !this.ctx) return;
    this.init();

    const now = this.ctx.currentTime;
    const osc = this.ctx.createOscillator();
    const gain = this.ctx.createGain();

    osc.type = 'sawtooth';
    osc.frequency.setValueAtTime(160, now);
    osc.frequency.exponentialRampToValueAtTime(40, now + 0.4);

    gain.gain.setValueAtTime(0.6, now);
    gain.gain.exponentialRampToValueAtTime(0.001, now + 0.45);

    osc.connect(gain);
    gain.connect(this.ctx.destination);

    osc.start(now);
    osc.stop(now + 0.45);
  }

  startMusic() {
    if (!this.saveData.musicEnabled || this.isBgmPlaying) return;
    this.init();
    this.isBgmPlaying = true;
    let step = 0;

    const bassNotes = [110, 110, 146, 130, 110, 110, 164, 146];
    const melodyNotes = [440, 0, 523, 587, 440, 659, 587, 523];

    const playStep = () => {
      if (!this.isBgmPlaying || !this.saveData.musicEnabled || !this.ctx) return;
      const now = this.ctx.currentTime;

      const bassFreq = bassNotes[step % bassNotes.length];
      if (bassFreq > 0) {
        const osc = this.ctx.createOscillator();
        const gain = this.ctx.createGain();
        osc.type = 'triangle';
        osc.frequency.setValueAtTime(bassFreq, now);
        gain.gain.setValueAtTime(0.12, now);
        gain.gain.exponentialRampToValueAtTime(0.001, now + 0.18);
        osc.connect(gain);
        gain.connect(this.ctx.destination);
        osc.start(now);
        osc.stop(now + 0.18);
      }

      if (step % 2 === 0) {
        const noise = this.ctx.createOscillator();
        const nGain = this.ctx.createGain();
        noise.type = 'square';
        noise.frequency.setValueAtTime(800, now);
        nGain.gain.setValueAtTime(0.04, now);
        nGain.gain.exponentialRampToValueAtTime(0.001, now + 0.05);
        noise.connect(nGain);
        nGain.connect(this.ctx.destination);
        noise.start(now);
        noise.stop(now + 0.05);
      }

      const melFreq = melodyNotes[step % melodyNotes.length];
      if (melFreq > 0 && Math.random() > 0.3) {
        const mOsc = this.ctx.createOscillator();
        const mGain = this.ctx.createGain();
        mOsc.type = 'sine';
        mOsc.frequency.setValueAtTime(melFreq, now);
        mGain.gain.setValueAtTime(0.08, now);
        mGain.gain.exponentialRampToValueAtTime(0.001, now + 0.15);
        mOsc.connect(mGain);
        mGain.connect(this.ctx.destination);
        mOsc.start(now);
        mOsc.stop(now + 0.15);
      }

      step++;
      this.bgMusicTimer = setTimeout(playStep, (60 / this.tempo / 2) * 1000);
    };

    playStep();
  }

  stopMusic() {
    this.isBgmPlaying = false;
    if (this.bgMusicTimer) {
      clearTimeout(this.bgMusicTimer);
      this.bgMusicTimer = null;
    }
  }
}
