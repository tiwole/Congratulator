window.stopFallingConfetti = (element) => {
    if (element._confettiCanvas) {
        element._confettiCanvas.remove();
        element._confettiCanvas = null;
    }
};

window.fallingConfetti = (element) => {
    window.stopFallingConfetti(element);

    const canvas = document.createElement('canvas');
    canvas.style.cssText = 'position:absolute;top:0;left:0;width:100%;height:100%;pointer-events:none;';
    element.style.position = 'relative';
    element.appendChild(canvas);
    element._confettiCanvas = canvas;
    const myConfetti = confetti.create(canvas, { resize: true });

    const palette = ['#ff4d6d', '#ffd166', '#06d6a0', '#a78bfa', '#38bdf8', '#fb923c', '#f472b6'];

    const fire = () => {
        if (element._confettiCanvas !== canvas) { canvas.remove(); return; }

        const color = palette[Math.floor(Math.random() * palette.length)];
        myConfetti({
            origin: { x: Math.random(), y: 0 },
            particleCount: 1,
            angle: 270,
            spread: 120,
            startVelocity: 1,
            gravity: 0.75,
            ticks: 60,
            drift: (Math.random() - 0.5) * 0.3,
            colors: [color],
        });

        setTimeout(fire, 50);
    };

    setTimeout(fire, 50);
};

window.launchConfetti = (element) => {
    const rect = element.getBoundingClientRect();
    const x = (rect.left + rect.width / 2) / window.innerWidth;
    const y = (rect.bottom) / window.innerHeight;
    confetti({
        origin: { x, y },
        particleCount: 180,
        spread: 360,
        startVelocity: 22,
        gravity: 1,
        ticks: 150,
    });
};

window.observeElement = (element, dotNetRef, rootMargin) => {
    const observer = new IntersectionObserver((entries) => {
        if (entries[0].isIntersecting) {
            dotNetRef.invokeMethodAsync('OnVisible');
            observer.disconnect();
        }
    }, { rootMargin: rootMargin ?? '300px' });
    observer.observe(element);
};