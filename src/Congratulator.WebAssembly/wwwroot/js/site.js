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