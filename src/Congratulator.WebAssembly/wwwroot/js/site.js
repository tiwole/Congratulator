window.fallingConfetti = (element) => {
    element._confettiStopped = false;

    const fire = () => {
        if (element._confettiStopped) return;

        const rect = element.getBoundingClientRect();
        if (rect.width === 0) { setTimeout(fire, 200); return; }

        confetti({
            origin: {
                x: (rect.left + Math.random() * rect.width) / window.innerWidth,
                y: rect.top / window.innerHeight,
            },
            particleCount: 5,
            spread: 45,
            startVelocity: 9,
            gravity: 0.7,
            ticks: 90,
            drift: (Math.random() - 0.5) * 0.4,
        });

        setTimeout(fire, 450);
    };

    setTimeout(fire, 150);
};

window.stopFallingConfetti = (element) => {
    element._confettiStopped = true;
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