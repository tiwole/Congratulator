window.observeElement = (element, dotNetRef, rootMargin) => {
    const observer = new IntersectionObserver((entries) => {
        if (entries[0].isIntersecting) {
            dotNetRef.invokeMethodAsync('OnVisible');
            observer.disconnect();
        }
    }, { rootMargin: rootMargin ?? '300px' });
    observer.observe(element);
};